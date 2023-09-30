using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StreetArtArchive.Models;

namespace StreetArtArchive.Services;

public class PictureService
{
    private readonly IMongoCollection<PicturesMetadata> _pictureMetadataCollection;

    public PictureService(
        IOptions<StreetArtDbSettings> picturesDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            picturesDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            picturesDatabaseSettings.Value.DatabaseName);

        _pictureMetadataCollection = mongoDatabase.GetCollection<PicturesMetadata>(
            picturesDatabaseSettings.Value.PicturesMetadataCollectionName);
    }

    public async Task<List<PicturesMetadata>> GetAsync(int page) =>
        await _pictureMetadataCollection.Find(_ => true).Skip(page*9).Limit(9).ToListAsync();

    public async Task<PicturesMetadata?> GetAsync(string id) =>
        await _pictureMetadataCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(PicturesMetadata newBook) =>
        await _pictureMetadataCollection.InsertOneAsync(newBook);

    public async Task UpdateAsync(string id, PicturesMetadata updatedPictures) =>
        await _pictureMetadataCollection.ReplaceOneAsync(x => x.Id == id, updatedPictures);

    public async Task RemoveAsync(string id) =>
        await _pictureMetadataCollection.DeleteOneAsync(x => x.Id == id);
}