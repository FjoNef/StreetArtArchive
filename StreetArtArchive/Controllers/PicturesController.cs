using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using StreetArtArchive.Models;
using StreetArtArchive.Services;

namespace StreetArtArchive.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class PicturesController : ControllerBase
{
    private readonly PictureService _pictureService;

    public PicturesController(PictureService pictureService)
    {
        _pictureService = pictureService;
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
        var book = await _pictureService.GetAsync(id);

        return book;
    }
    
    [HttpPost]
    public async Task<IActionResult> SavePicture([FromForm] SavePictureRequest request)
    {
        //TODO: move constants
        var directory = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                                  Path.Combine("StreetArtArchive", "Pictures"));
        var filePath = directory + Path.GetRandomFileName() + Path.GetExtension(request.Image.FileName);
        await using (var stream = System.IO.File.Create(filePath))
        {
            await request.Image.CopyToAsync(stream);
        }

        var thumbnail = await CreateThumbnailAsync(request);

        var newPicture = new PicturesMetadata
        {
            ImagePath = filePath,
            ThumbnailId = thumbnail.Id,
            Categories = request.Categories.Select(c => new Category(){ Name = c.Name, Values = new[]{c.Values}}).ToArray()
        };

        await _pictureService.CreateAsync(newPicture);

        return CreatedAtAction(nameof(SavePicture), newPicture);
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteById(string id)
    {
        await _pictureService.RemoveAsync(id);

        return Accepted();
    }

    private async Task<Thumbnail> CreateThumbnailAsync(SavePictureRequest request)
    {
        SKBitmap originalBitmap;
        await using (var stream = request.Image.OpenReadStream())
        {
            originalBitmap = SKBitmap.Decode(stream);
        }

        var (newWidth, newHeight) = CalculateNewSize(originalBitmap);
        var thumbnailBitmap = originalBitmap.Resize(new SKSizeI(newWidth, newHeight), SKFilterQuality.Low);
        var thumbnailImage = SKImage.FromBitmap(thumbnailBitmap);
        var thumbnailData = thumbnailImage.Encode();
        var thumbnail = new Thumbnail() { Data = thumbnailData.ToArray() };

        await _pictureService.SaveThumbnailAsync(thumbnail);
        
        return thumbnail;
    }

    private static (int newWidth, int newHeight) CalculateNewSize(SKBitmap originalBitmap)
    {
        //TODO: move constants
        var ratio = Math.Max(originalBitmap.Width / 400.0, originalBitmap.Height / 400.0);
        var newWidth = (int)(originalBitmap.Width / ratio);
        var newHeight = (int)(originalBitmap.Height / ratio);
        return (newWidth, newHeight);
    }
}