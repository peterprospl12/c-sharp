using System.Xml.Serialization;

namespace PTLab09___LINQ_XML_XPATH_XHTML;

[XmlType("car")]
public class Car
{
    public Car()
    {
    }

    public Car(string model, Engine motor, int year)
    {
        Model = model;
        Motor = motor;
        Year = year;
    }

    public string Model { get; set; }
    public int Year { get; set; }

    [XmlElement("engine")] public Engine Motor { get; set; }
}