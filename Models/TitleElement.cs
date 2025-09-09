using System.Xml.Serialization;

namespace BookstoreApi.Models;

public class TitleElement
{
    [XmlAttribute("lang")] // keeping lang attribuite for cases we want to change the lang for other readers
    public string? Lang { get; set; } = "en";

    [XmlText]
    public string Value { get; set; } = string.Empty;
}
