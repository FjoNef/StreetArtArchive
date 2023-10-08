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

    private readonly ILogger<PicturesController> _logger;

    public PicturesController(ILogger<PicturesController> logger, PictureService pictureService)
    {
        _logger = logger;
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
    
    /*[HttpGet("{id:length(24)}")]
    public async Task<ActionResult<PicturesMetadata>> Get(string id)
    {
        var book = await _pictureService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        return book;
    }*/
    
    [HttpPost]
    public async Task<IActionResult> SavePicture([FromForm] SavePictureRequest request)
    {
        var filePath = @"C:\Users\fjodo\Pictures\" + request.Image.FileName;
        using (var stream = System.IO.File.Create(filePath))
        {
            await request.Image.CopyToAsync(stream);
        }

        SKBitmap originalBitmap;
        using (var stream =  request.Image.OpenReadStream())
        {
            originalBitmap = SKBitmap.Decode(stream);
        }
        
        var (newWidth, newHeight) = CalculateNewSize(originalBitmap);

        var thumbnailBitmap = originalBitmap.Resize(new SKSizeI(newWidth, newHeight), SKFilterQuality.Low);

        var thumbnailImage = SKImage.FromBitmap(thumbnailBitmap);

        var thumbnailData = thumbnailImage.Encode();

        var thumbnail = new Thumbnail() { Data = thumbnailData.ToArray() };

        await _pictureService.SaveThumbnailAsync(thumbnail);
        
        var newPicture = new PicturesMetadata
        {
            ImagePath = filePath,
            ThumbnailId = thumbnail.Id,
            Categories = request.Categories.Select(c => new Category(){ Name = c.Name, Values = new[]{c.Values}}).ToArray()
        };

        await _pictureService.CreateAsync(newPicture);

        return CreatedAtAction(nameof(GetList), new { id = newPicture.Id }, newPicture);
    }

    private static (int newWidth, int newHeight) CalculateNewSize(SKBitmap originalBitmap)
    {
        var ratio = Math.Max(originalBitmap.Width / 500.0, originalBitmap.Height / 500.0);
        var newWidth = (int)(originalBitmap.Width / ratio);
        var newHeight = (int)(originalBitmap.Height / ratio);
        return (newWidth, newHeight);
    }
}