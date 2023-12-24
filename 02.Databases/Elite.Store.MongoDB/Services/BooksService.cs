namespace DB.Mongo.Services;
public class BooksService
{
    readonly IMongoCollection<Book> _book;
    public BooksService(IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
    {
        MongoClient client = new MongoClient(new MongoClientSettings
        {
            Server = new MongoServerAddress("localhost", 27017)
        });

        // var client = new MongoClient(bookStoreDatabaseSettings.Value.ConnectionString);
        var database = client.GetDatabase(bookStoreDatabaseSettings.Value.DatabaseName);
        _book = database.GetCollection<Book>(bookStoreDatabaseSettings.Value.BooksCollectionName);
    }
    public async Task<List<Book>> GetAsync() => await _book.Find(_ => true).ToListAsync();
    public async Task<Book?> GetAsync(string id) => await _book.Find(item => item.Id == id).FirstOrDefaultAsync();
    public async Task CreateAsync(Book newBook) => await _book.InsertOneAsync(newBook);
    public async Task UpdateAsync(string id, Book updatedBook) => await _book.ReplaceOneAsync(item => item.Id == id, updatedBook);
    public async Task RemoveAsync(string id) => await _book.DeleteOneAsync(item => item.Id == id);
}