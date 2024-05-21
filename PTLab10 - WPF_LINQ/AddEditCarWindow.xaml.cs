using System.Windows;

namespace PTLab10___WPF_LINQ
{
    public partial class AddEditCarWindow : Window
    {
        public string TempModel { get; set; }
        public int TempYear { get; set; }
        public string TempEngineModel { get; set; }
        public double TempEngineDisplacement { get; set; }
        public double TempEngineHorsePower { get; set; }

        public Car NewCar { get; set; }

        public AddEditCarWindow()
        {
            InitializeComponent();
            NewCar = new Car();
            this.DataContext = this;
        }

        public AddEditCarWindow(Car car) : this()
        {
            TempModel = car.Model;
            TempYear = car.Year;
            TempEngineModel = car.Motor.Model;
            TempEngineDisplacement = car.Motor.Displacement;
            TempEngineHorsePower = car.Motor.HorsePower;

            NewCar = new Car(car.Model, new Engine(car.Motor.Displacement, car.Motor.HorsePower, car.Motor.Model), car.Year);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            NewCar = new Car(TempModel, new Engine(TempEngineDisplacement, TempEngineHorsePower, TempEngineModel), TempYear);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}
