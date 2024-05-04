using System.Xml.Serialization;

namespace PTLab09___LINQ_XML_XPATH_XHTML;

public class Engine
{
    public Engine()
    {
    }

    public Engine(double displacement, double horsePower, string model)
    {
        Displacement = displacement;
        HorsePower = horsePower;
        Model = model;
    }

    [XmlAttribute("model")] public string Model { get; set; }

    public double Displacement { get; set; }
    public double HorsePower { get; set; }

    public double hppl()
    {
        return HorsePower / Displacement;
    }
}