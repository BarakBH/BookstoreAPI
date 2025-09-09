using System.Xml.Serialization;

namespace BookstoreApi.Models;

// list for all books.
[XmlRoot("bookstore")]
public class Bookstore
{
    [XmlElement("book")]
    public List<Book> Books { get; set; } = new();
}
