namespace StreetArtArchive.Models;

public class SavePictureRequest
{
    public IFormFile Image { get; set; }
    
    public CategoryRequest[] Categories { get; set; }
    
    public string Id { get; set; }
}