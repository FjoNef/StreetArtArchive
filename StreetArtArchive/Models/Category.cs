using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StreetArtArchive.Models;

public class Category
{
    public string? Name { get; set; }
    
    [BsonIgnoreIfNull]
    public ICollection<Category>? SubCategories { get; set; }
    
    [BsonIgnoreIfNull]
    public ICollection<string>? Values { get; set; }
}