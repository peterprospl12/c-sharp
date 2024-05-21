using System.Xml.Serialization;

namespace PTLab10___WPF_LINQ;
public class Engine : IComparable<Engine>
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

    public int CompareTo(Engine? other)
    {
        if (other == null) return 1;

        int horsePowerComparison = HorsePower.CompareTo(other.HorsePower);
        if (horsePowerComparison != 0) return horsePowerComparison;

        int modelComparison = Model.CompareTo(other.Model);
        if (modelComparison != 0) return modelComparison;

        return Displacement.CompareTo(other.Displacement);
    }

    public override string ToString()
    {
        return $"Displacement: {Displacement}, Horse Power: {HorsePower}, Model: {Model}";
    }

}
