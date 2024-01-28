using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using SkiaSharp;
using StreetArtArchive.Models;
using StreetArtArchive.Services;

namespace StreetArtArchive.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class PicturesController : ControllerBase
{
    private readonly PictureService _pictureService;

    private readonly IOptions<StreetArtApplicationSettings> _applicationSettings;

    public PicturesController(PictureService pictureService, IOptions<StreetArtApplicationSettings> applicationSettings)
    {
        _pictureService = pictureService;
        _applicationSettings = applicationSettings;
    }

    [HttpGet]
    public async Task<object> GetList(int page)
    {
        var pictures = await _pictureService.GetAsync(page);
        foreach (var picture in pictures)
        {
            picture.Thumbnail = await _pictureService.GetThumbnailAsync(picture.ThumbnailId);
        }
        return new { pictures, hasMore = pictures.Count == 9 };
    }
    
    [HttpGet]
    public async Task<ActionResult<PicturesMetadata?>> GetById(string id)
    {
        var picture = await _pictureService.GetAsync(id);
        if (picture != null)
        {
            picture.Thumbnail = await _pictureService.GetThumbnailAsync(picture.ThumbnailId);
        }

        return picture;
    }
    
    [HttpGet]
    public IActionResult GetFullPicture(string path)
    {
        var directory = Directory.CreateDirectory(_applicationSettings.Value.PicturesFolderPath);
        var filePath = Path.Combine(directory.ToString(), path);
        return PhysicalFile(filePath, GetContentType(filePath));
    }
    
    [HttpPost]
    public async Task<IActionResult> SavePicture([FromForm] SavePictureRequest request)
    {
        var newPicture = await CreatePicturesMetadata(request);

        await _pictureService.CreateAsync(newPicture);

        return CreatedAtAction(nameof(SavePicture), newPicture);
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdatePicture([FromForm] SavePictureRequest request)
    {
        var oldPicture = await _pictureService.GetAsync(request.Id);
        
        var newPicture = await CreatePicturesMetadata(request);

        newPicture.Id = oldPicture?.Id;

        await _pictureService.UpdateAsync(request.Id, newPicture);

        if (oldPicture != null)
        {
            await _pictureService.RemoveThumbnailAsync(oldPicture.ThumbnailId);
            DeletePictureFromDisc(oldPicture.ImagePath);
        }

        return CreatedAtAction(nameof(SavePicture), newPicture);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteById(string id)
    {
        var deletedPicture = await _pictureService.RemoveAsync(id);

        if (deletedPicture is null)
        {
            return NotFound();
        }

        DeletePictureFromDisc(deletedPicture.ImagePath);

        return Accepted();
    }

    private void DeletePictureFromDisc(string? imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            return;
        }
        var directory = Directory.CreateDirectory(_applicationSettings.Value.PicturesFolderPath);
        var filePath = Path.Combine(directory.ToString(), imagePath);
        System.IO.File.Delete(filePath);
    }

    private async Task<Thumbnail> CreateThumbnailAsync(SavePictureRequest request)
    {
        SKBitmap originalBitmap;
        await using (var stream = request.Image.OpenReadStream())
        {
            originalBitmap = SKBitmap.Decode(stream);
        }

        var (newWidth, newHeight) = CalculateNewSize(originalBitmap, _applicationSettings.Value.ThumbnailMaxSize);
        var thumbnailBitmap = originalBitmap.Resize(new SKSizeI(newWidth, newHeight), SKFilterQuality.Low);
        var thumbnailImage = SKImage.FromBitmap(thumbnailBitmap);
        var thumbnailData = thumbnailImage.Encode();
        var thumbnail = new Thumbnail() { Data = thumbnailData.ToArray() };

        await _pictureService.SaveThumbnailAsync(thumbnail);
        
        return thumbnail;
    }

    private static (int newWidth, int newHeight) CalculateNewSize(SKBitmap originalBitmap, int maxSize)
    {
        var ratio = Math.Max(originalBitmap.Width / maxSize, originalBitmap.Height / maxSize);
        var newWidth = originalBitmap.Width / ratio;
        var newHeight = originalBitmap.Height / ratio;
        return (newWidth, newHeight);
    }

    private static string GetContentType(string fileName)
    {
        new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType);
        return contentType ?? "application/octet-stream";
    }

    private async Task<PicturesMetadata> CreatePicturesMetadata(SavePictureRequest request)
    {
        var directory = Directory.CreateDirectory(_applicationSettings.Value.PicturesFolderPath);
        var fileName = Path.GetRandomFileName() + Path.GetExtension(request.Image.FileName);
        var filePath = Path.Combine(directory.ToString(), fileName);
        await using (var stream = System.IO.File.Create(filePath))
        {
            await request.Image.CopyToAsync(stream);
        }

        var thumbnail = await CreateThumbnailAsync(request);

        var newPicture = new PicturesMetadata
        {
            ImagePath = fileName,
            ThumbnailId = thumbnail.Id,
            Categories = request.Categories.Select(c => new Category() { Name = c.Name, Values = new[] { c.Values } })
                .ToArray()
        };
        return newPicture;
    }
}