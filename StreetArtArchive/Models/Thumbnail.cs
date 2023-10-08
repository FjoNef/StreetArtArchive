using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StreetArtArchive.Models;

public class Thumbnail
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public byte[]? Data { get; set; }
}