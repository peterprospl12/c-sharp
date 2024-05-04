using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace PTLab09___LINQ_XML_XPATH_XHTML;

internal class Program
{
    private static void CreateXmlFromLinq(List<Car> myCars)
    {
        var nodes = myCars.Select(car => new XElement("car",
            new XElement("Model", car.Model),
            new XElement("Year", car.Year),
            new XElement("engine",
                new XAttribute("model", car.Motor.Model),
                new XElement("Displacement", car.Motor.Displacement),
                new XElement("HorsePower", car.Motor.HorsePower))
        )); // zapytanie LINQ
        var rootNode = new XElement("cars", nodes); // stwórz węzel zawierający wyniki zapytania
        rootNode.Save("CarsFromLinq.xml");
    }

    private static void CreateXHtml(List<Car> myCars)
    {
        var doc = new XDocument(
            new XDocumentType("html", null, null, null),
            new XElement(XNamespace.Get("http://www.w3.org/1999/xhtml") + "html",
                new XElement("body",
                    new XElement("table",
                        new XElement("thead",
                            new XElement("tr",
                                new XElement("th", "Model"),
                                new XElement("th", "Year"),
                                new XElement("th", "Engine Model"),
                                new XElement("th", "Displacement"),
                                new XElement("th", "HorsePower")
                            )
                        ),
                        new XElement("tbody",
                            from car in myCars
                            select new XElement("tr",
                                new XElement("td", car.Model),
                                new XElement("td", car.Year),
                                new XElement("td", car.Motor.Model),
                                new XElement("td", car.Motor.Displacement),
                                new XElement("td", car.Motor.HorsePower)
                            )
                        )
                    )
                )
            )
        );

        doc.Save("myCars.html");
    }

    private static void LinqExpressions(List<Car> myCars)
    {
        var carA6 = myCars
            .Where(car => car.Model == "A6")
            .Select(car => new
            {
                engineType = car.Motor.Model == "TDI" ? "diesel" : "petrol",
                hppl = car.Motor.hppl()
            });

        var carA6Group = carA6
            .GroupBy(car => car.engineType)
            .Select(group => new
            {
                EngineType = group.Key,
                AvgHppl = group.Average(car => car.hppl)
            });

        foreach (var group in carA6Group)
            Console.WriteLine($"Engine Type: {group.EngineType}, Average Hppl: {group.AvgHppl}");
    }

    private static void XPathTask()
    {
        var rootNode = XElement.Load("cars.xml");
        var xPathExpression = "sum(//engine[@model!='TDI']/HorsePower) div count(//engine[@model!='TDI']/HorsePower)";
        var avgHp = (double)rootNode.XPathEvaluate(xPathExpression);
        Console.WriteLine("Avg HP: " + avgHp);

        var xPathExpression1 = "//Model[not(. = following::Model)]";
        var models = rootNode.XPathSelectElements(xPathExpression1);

        foreach (var model in models) Console.WriteLine(model);
    }

    private static void CarSerDeializer(List<Car> myCars)
    {
        var ser = new XmlSerializer(typeof(List<Car>), new XmlRootAttribute("cars"));
        using var writer = new StreamWriter("cars.xml");
        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add(string.Empty, string.Empty);
        ser.Serialize(writer, myCars, namespaces);

        List<Car> deserializedCars;
        var xmlReader = XmlReader.Create("cars.xml");
        deserializedCars = (List<Car>)ser.Deserialize(xmlReader);

        foreach (var car in deserializedCars)
            Console.WriteLine(
                $"Model: {car.Model}, Year: {car.Year}, Engine: {car.Motor.Model}, Displacement: {car.Motor.Displacement}, HorsePower: {car.Motor.HorsePower}");
    }

    private static void XmlConverter()
    {
        var doc = XDocument.Load("cars.xml");

        var nodesToChange = doc.Descendants("HorsePower").ToList();
        var yearNodes = doc.Descendants("Year").ToList();
        var modelNodes = doc.Descendants("Model").ToList();


        foreach (var node in nodesToChange)
        {
            var newNode = new XElement("hp", node.Nodes());
            node.ReplaceWith(newNode);
        }

        foreach (var (modelNode, yearNode) in modelNodes.Zip(yearNodes, (m, y) => (m, y)))
        {
            modelNode.SetAttributeValue("year", yearNode.Value);
            yearNode.Remove();
        }

        doc.Save("cars_converted.xml");
    }

    private static void Main(string[] args)
    {
        var myCars = new List<Car>
        {
            new("E250", new Engine(1.8, 204, "CGI"), 2009),
            new("E350", new Engine(3.5, 292, "CGI"), 2009),
            new("A6", new Engine(2.5, 187, "FSI"), 2012),
            new("A6", new Engine(2.8, 220, "FSI"), 2012),
            new("A6", new Engine(3.0, 295, "TFSI"), 2012),
            new("A6", new Engine(2.0, 175, "TDI"), 2011),
            new("A6", new Engine(3.0, 309, "TDI"), 2011),
            new("S6", new Engine(4.0, 414, "TFSI"), 2012),
            new("S8", new Engine(4.0, 513, "TFSI"), 2012)
        };


        LinqExpressions(myCars);
        CarSerDeializer(myCars);
        XPathTask();
        CreateXmlFromLinq(myCars);
        CreateXHtml(myCars);
        XmlConverter();
    }
}