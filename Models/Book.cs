using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace BookstoreApi.Models;

// Matches a <book> node in the XML, including attributes and elements.
[XmlType("book")]
public class Book
{
    // Attributes on <book>
    [XmlAttribute("category")]
    public string Category { get; set; } = string.Empty;

    // Elements inside <book>
    [XmlElement("isbn")]
    [Required]
    public string Isbn { get; set; } = string.Empty;

    [XmlElement("title")]
    public TitleElement Title { get; set; } = new();

    // The XML may have multiple <author> elements so ill keep in a list
    [XmlElement("author")]
    public List<string> Authors { get; set; } = new();

    [XmlElement("year")]
    [Range(1, 9999)] // since when books came out ?
    public int Year { get; set; }

    [XmlElement("price")]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}
