using System.Xml.Serialization;
using System.Text.RegularExpressions;
using BookstoreApi.Models;

namespace BookstoreApi.Services;

public class XmlBookRepository : IBookRepository
{
    private const string FilePath = "Data/bookstore.xml";
    private static readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.SupportsRecursion);

    public XmlBookRepository() { }

    // ===== IBookRepository =====

    public async Task<List<Book>> GetAllAsync()
    {
        var store = await LoadAsync().ConfigureAwait(false);
        return store.Books.Select(Clone).ToList();
    }

    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        var store = await LoadAsync().ConfigureAwait(false);
        var match = store.Books.FirstOrDefault(b => b.Isbn == isbn);
        return match is null ? null : Clone(match);
    }

    public async Task AddAsync(Book book)
    {
        ValidateBook(book);

        _lock.EnterWriteLock();
        try
        {
            var store = await LoadNoLockAsync().ConfigureAwait(false);

            if (store.Books.Any(b => b.Isbn == book.Isbn))
                throw new InvalidOperationException($"A book with ISBN {book.Isbn} already exists.");

            store.Books.Add(Clone(book));
            await SaveNoLockAsync(store).ConfigureAwait(false);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public async Task UpdateAsync(string isbn, Book updated)
    {
        ValidateBook(updated);

        _lock.EnterWriteLock();
        try
        {
            var store = await LoadNoLockAsync().ConfigureAwait(false);

            var existing = store.Books.FirstOrDefault(b => b.Isbn == isbn)
                ?? throw new KeyNotFoundException($"No book with ISBN {isbn}.");

            // If client changes the ISBN, keep it unique
            if (!string.Equals(isbn, updated.Isbn, StringComparison.Ordinal) &&
                store.Books.Any(b => b.Isbn == updated.Isbn))
            {
                throw new InvalidOperationException($"A book with ISBN {updated.Isbn} already exists.");
            }

            // Update fields
            existing.Isbn = updated.Isbn;
            existing.Title = Clone(updated.Title);
            existing.Authors = updated.Authors.ToList();
            existing.Category = updated.Category;
            existing.Year = updated.Year;
            existing.Price = updated.Price;

            await SaveNoLockAsync(store).ConfigureAwait(false);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public async Task DeleteAsync(string isbn)
    {
        _lock.EnterWriteLock();
        try
        {
            var store = await LoadNoLockAsync().ConfigureAwait(false);
            var removed = store.Books.RemoveAll(b => b.Isbn == isbn);
            if (removed == 0)
                throw new KeyNotFoundException($"No book with ISBN {isbn}.");

            await SaveNoLockAsync(store).ConfigureAwait(false);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    // ===== Internals =====

    private async Task<Bookstore> LoadAsync()
    {
        _lock.EnterReadLock();
        try
        {
            return await LoadNoLockAsync().ConfigureAwait(false);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    private async Task<Bookstore> LoadNoLockAsync()
    {
        if (!File.Exists(FilePath))
            return new Bookstore();

        await using var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var serializer = new XmlSerializer(typeof(Bookstore));
        return (Bookstore?)serializer.Deserialize(fs) ?? new Bookstore();
    }

    private async Task SaveNoLockAsync(Bookstore store)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(FilePath))!);

        var tempPath = FilePath + ".tmp";
        await using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            var serializer = new XmlSerializer(typeof(Bookstore));
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty); 
            serializer.Serialize(fs, store, ns);
        }

        if (File.Exists(FilePath))
            File.Replace(tempPath, FilePath, null);
        else
            File.Move(tempPath, FilePath);
    }

    private static void ValidateBook(Book book)
    {
        if (string.IsNullOrWhiteSpace(book.Isbn) || !Regex.IsMatch(book.Isbn, @"^\d{13}$"))
            throw new ArgumentException("ISBN must be exactly 13 digits.");

        if (book.Title is null || string.IsNullOrWhiteSpace(book.Title.Value))
            throw new ArgumentException("Title is required.");

        if (book.Year < 1 || book.Year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Year is out of range.");

        if (book.Price < 0)
            throw new ArgumentException("Price cannot be negative.");
    }

    // Shallow clones: protect repo state from external mutations
    private static Book Clone(Book b) => new()
    {
        Category = b.Category,
        Isbn = b.Isbn,
        Title = Clone(b.Title),
        Authors = b.Authors.ToList(),
        Year = b.Year,
        Price = b.Price
    };

    private static TitleElement Clone(TitleElement t) => new()
    {
        Lang = t.Lang,
        Value = t.Value
    };
}
