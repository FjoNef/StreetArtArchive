namespace StreetArtArchive.Models;

public class StreetArtApplicationSettings
{
    private string _picturesFolderPath = null!;
    public string PicturesFolderPath
    {
        get => _picturesFolderPath;
        set => _picturesFolderPath = Environment.ExpandEnvironmentVariables(value);
    }

    public int ThumbnailMaxSize { get; set; }
}