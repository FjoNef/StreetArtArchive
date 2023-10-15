namespace StreetArtArchive.Models;

public class StreetArtApplicationSettings
{
    public string PicturesFolderPath { get; set; } = null!;

    public int ThumbnailMaxSize { get; set; }
}