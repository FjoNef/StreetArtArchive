using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StreetArtArchive.Models;

namespace StreetArtArchive.Services;

public class PictureService
{
    private readonly IMongoCollection<PicturesMetadata> _pictureMetadataCollection;
    
    private readonly IMongoCollection<Thumbnail> _thumbnailsCollection;

    public PictureService(
        IOptions<StreetArtDbSettings> picturesDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            picturesDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            picturesDatabaseSettings.Value.DatabaseName);

        _pictureMetadataCollection = mongoDatabase.GetCollection<PicturesMetadata>(
            picturesDatabaseSettings.Value.PicturesMetadataCollectionName);
        
        _thumbnailsCollection = mongoDatabase.GetCollection<Thumbnail>(
            picturesDatabaseSettings.Value.ThumbnailsCollectionName);
    }

    public async Task<List<PicturesMetadata>> GetAsync(int page) =>
        await _pictureMetadataCollection.Find(_ => true).Skip(page*9).Limit(9).ToListAsync();

    public async Task<PicturesMetadata?> GetAsync(string id) =>
        await _pictureMetadataCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(PicturesMetadata newPicture) =>
        await _pictureMetadataCollection.InsertOneAsync(newPicture);

    public async Task UpdateAsync(string id, PicturesMetadata updatedPictures) =>
        await _pictureMetadataCollection.ReplaceOneAsync(x => x.Id == id, updatedPictures);

    public async Task<PicturesMetadata?> RemoveAsync(string id)
    {
        var deletedPicture = await _pictureMetadataCollection.FindOneAndDeleteAsync(x => x.Id == id);
        await RemoveThumbnailAsync(deletedPicture?.ThumbnailId);

        return deletedPicture;
    }
    
    public async Task SaveThumbnailAsync(Thumbnail newThumbnail) =>
        await _thumbnailsCollection.InsertOneAsync(newThumbnail);
    
    public async Task<Thumbnail?> GetThumbnailAsync(string? id) =>
        await _thumbnailsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    
    public async Task RemoveThumbnailAsync(string? id) =>
         await _thumbnailsCollection.DeleteOneAsync(x => x.Id == id);
}