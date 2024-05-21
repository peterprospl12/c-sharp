using System.Xml.Serialization;

namespace PTLab10___WPF_LINQ;

[XmlType("car")]
public class Car : IComparable<Car>
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

    public int CompareTo(Car? other)
    {
        if (other == null) return 1;

        int modelComparison = Model.CompareTo(other.Model);
        if (modelComparison != 0) return modelComparison;

        int yearComparison = Year.CompareTo(other.Year);
        if (yearComparison != 0) return yearComparison;

        return Motor.CompareTo(other.Motor);
    }
    public override string ToString()
    {
        return $"Model: {Model}, Year: {Year}, Engine: {Motor.ToString()}";
    }

}