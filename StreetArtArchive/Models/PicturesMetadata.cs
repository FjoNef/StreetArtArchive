using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StreetArtArchive.Models;

public class PicturesMetadata
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public string? ImagePath { get; set; }
    
    public ICollection<Category>? Categories { get; set; }
}