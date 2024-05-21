using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;

namespace PTLab10___WPF_LINQ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BindingListEx<Car> myCarsBinded { get; set; }

        public MainWindow()
        {
            List<Car> myCars = new List<Car>(){
             new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
             new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
             new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
             new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
             new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
             new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
             new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
             new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
             new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
            };
            LINQ_queries(myCars);
            TaskTwo(myCars);
            myCarsBinded = new BindingListEx<Car>(myCars);
            InitializeComponent();
            dataGrid.ItemsSource = myCarsBinded;
            dataGrid.Sorting += DataGrid_Sorting;
            ComboBox_AddProperties();
        }

        private void TaskTwo(List<Car> myCars)
        {
            Comparison<Car> arg1 = delegate (Car a, Car b)
            {
                return b.Motor.HorsePower.CompareTo(a.Motor.HorsePower);
            };

            Predicate<Car> arg2 = delegate (Car a)
            {
                return a.Motor.Model == "TDI";
            };

            Action<Car> arg3 = delegate (Car a)
            {
                MessageBox.Show(a.ToString());
            };
            myCars.Sort(new Comparison<Car>(arg1));
            myCars.FindAll(arg2).ForEach(arg3);

        }

        private void LINQ_queries(List<Car> myCars)
        {
            // Query expression syntax  
            var elements = from car in myCars
                           where car.Model == "A6"
                           group car by car.Motor.Model == "TDI" into gr
                           orderby gr.Average(car => car.Motor.HorsePower / car.Motor.Displacement) descending
                           select new
                           {
                               engineType = gr.Key ? "diesel" : "petrol",
                               avgHPPL = gr.Average(car => car.Motor.HorsePower / car.Motor.Displacement)
                           };
            StringBuilder sb = new StringBuilder();
            foreach (var e in elements)
            {
                sb.AppendLine(e.engineType + ": " + e.avgHPPL);
            }

            MessageBox.Show(sb.ToString());
            // Method-based query syntax

            elements = myCars
                .Where(car => car.Model == "A6")
                .GroupBy(car => car.Motor.Model == "TDI")
                .Select(gr => new
                {
                    engineType = gr.Key ? "diesel" : "petrol",
                    avgHPPL = gr.Average(car => car.Motor.HorsePower / car.Motor.Displacement)
                }).OrderByDescending(o => o.avgHPPL);

            sb = new StringBuilder();
            foreach (var e in elements)
            {
                sb.AppendLine(e.engineType + ": " + e.avgHPPL);
            }

            MessageBox.Show(sb.ToString());
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Pobierz nazwę kolumny, która została kliknięta
            string columnName = e.Column.Header.ToString();
            // Sprawdź, czy kolumna jest pusta
            if (string.IsNullOrEmpty(columnName))
                return;

            // Posortuj listę myCarsBinded według wybranej kolumny
            switch (columnName)
            {
                case "Model":
                    myCarsBinded = new BindingListEx<Car>(myCarsBinded.OrderBy(c => c.Model).ToList());
                    break;
                case "Engine Displacement":
                    myCarsBinded = new BindingListEx<Car>(myCarsBinded.OrderBy(c => c.Motor.Displacement).ToList());
                    break;
                case "Engine HorsePower":
                    myCarsBinded = new BindingListEx<Car>(myCarsBinded.OrderBy(c => c.Motor.HorsePower).ToList());
                    break;
                case "Year":
                    myCarsBinded = new BindingListEx<Car>(myCarsBinded.OrderBy(c => c.Year).ToList());
                    break;
                case "Engine Model":
                    myCarsBinded = new BindingListEx<Car>(myCarsBinded.OrderBy(c => c.Motor.Model).ToList());
                    break;
                default:
                    return;
            }

            // Zaktualizuj wyświetlane dane w kontrolce DataGrid
            dataGrid.ItemsSource = myCarsBinded;

            // Zatrzymaj domyślne sortowanie
            e.Handled = true;
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Dodawanie nowego samochodu
            var dialog = new AddEditCarWindow();
            if (dialog.ShowDialog() == true)
            {
                var x = dialog.NewCar;
                myCarsBinded.Add(dialog.NewCar);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Edycja istniejącego samochodu
            if (dataGrid.SelectedItem != null)
            {
                var selectedCar = (Car)dataGrid.SelectedItem;
                var dialog = new AddEditCarWindow(selectedCar);
                if (dialog.ShowDialog() == true)
                {
                    // Aktualizacja danych samochodu
                    int index = myCarsBinded.IndexOf(selectedCar);
                    myCarsBinded[index] = dialog.NewCar;
                    dataGrid.ItemsSource = myCarsBinded;
                }
            }
            else
            {
                MessageBox.Show("Please select a car to edit.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Usuwanie samochodu
            if (dataGrid.SelectedItem != null)
            {
                var selectedCar = (Car)dataGrid.SelectedItem;
                myCarsBinded.Remove(selectedCar);
                dataGrid.ItemsSource = myCarsBinded;
            }
            else
            {
                MessageBox.Show("Please select a car to delete.");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchTextBox.Text.ToLower();

            var filteredCars = myCarsBinded.Where(car =>
                car.Model.ToLower().Contains(searchText) ||
                car.Year.ToString().Contains(searchText) ||
                car.Motor.Model.ToLower().Contains(searchText) ||
                car.Motor.Displacement.ToString().Contains(searchText) ||
                car.Motor.HorsePower.ToString().Contains(searchText)
            ).ToList();

            filteredCars.Sort();

            dataGrid.ItemsSource = filteredCars;
        }

        private void ComboBox_AddProperties()
        {
            Type carType = typeof(Car);
            var properties = carType.GetProperties();
            List<string> propertyNames = new List<string>();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string) || property.PropertyType == typeof(int))
                { 
                    propertyNames.Add(property.Name);
                }
            }

            properties = typeof(Engine).GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string) || property.PropertyType == typeof(int))
                { 
                    propertyNames.Add(property.Name);
                }
            }
            propertiesComboBox.ItemsSource = propertyNames;
        }

        private void ComboButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProperty = propertiesComboBox.SelectedItem as string;

            if (selectedProperty == null)
            {
                MessageBox.Show("Please select property");
                return;
            }

            string value = comboSearchTextBox.Text;

            if (string.IsNullOrEmpty(value))
            {
                MessageBox.Show("Please enter value");
                return;
            }

            PropertyDescriptor? prop = TypeDescriptor.GetProperties(typeof(Car))[selectedProperty];

            List<Car> foundCar = [myCarsBinded.Find(prop, value)];
            if (foundCar != null)
            {
                resultComboBox.ItemsSource = foundCar;
            }
            else
            {
                MessageBox.Show("No car found");
            }



        }


    }
}