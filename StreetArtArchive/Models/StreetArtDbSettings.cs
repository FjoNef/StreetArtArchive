namespace StreetArtArchive.Models;

public class StreetArtDbSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string PicturesMetadataCollectionName { get; set; } = null!;
    
    public string ThumbnailsCollectionName { get; set; } = null!;
}