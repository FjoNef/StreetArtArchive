using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace StreetArtArchive.Models;

public class PicturesMetadata
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public string? ImagePath { get; set; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ThumbnailId { get; set; }
    
    [BsonIgnore]
    public Thumbnail? Thumbnail { get; set; }
    
    public ICollection<Category>? Categories { get; set; }
}