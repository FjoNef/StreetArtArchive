using Aspose.Imaging;

using Microsoft.AspNetCore.Mvc;
using StreetArtArchive.Models;
using StreetArtArchive.Services;

namespace StreetArtArchive.Controllers;

[ApiController]
[Route("[controller]")]
public class PicturesController : ControllerBase
{
    private readonly PictureService _pictureService;

    private readonly ILogger<PicturesController> _logger;

    public PicturesController(ILogger<PicturesController> logger, PictureService pictureService)
    {
        _logger = logger;
        _pictureService = pictureService;
    }

    [HttpGet("{page:length(24)}")]
    public async Task<object> Get(int page)
    {
        var pictures = await _pictureService.GetAsync(page);
        return new { pictures, hasMore = pictures.Count == 9 };
    }
    
    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<PicturesMetadata>> Get(string id)
    {
        var book = await _pictureService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        return book;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromForm] SavePictureRequest request)
    {
        var filePath = @"C:\Users\fjodo\Pictures\" + request.Image.FileName;
        using (var stream = System.IO.File.Create(filePath))
        {
            request.Image.CopyTo(stream);
        }
        
        
        using (Image image = Image.Load(filePath))
        {
            // Invoke the Resize method with the type of LanczosResample. 
            image.Resize(100, 100, ResizeType.Bell); 
            // Call the Save method to save the thumbnail image.       
            image.Save(filePath+".thumbnail");    
        }
        
        var newPicture = new PicturesMetadata
        {
            ImagePath = filePath,
            Categories = request.Categories.Select(c => new Category(){ Name = c.Name, Values = new[]{c.Values}}).ToArray()
        };

        await _pictureService.CreateAsync(newPicture);

        return CreatedAtAction(nameof(Get), new { id = newPicture.Id }, newPicture);
    }
}