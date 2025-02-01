using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace yp_osago
{
    /// <summary>
    /// Логика взаимодействия для ClientCar.xaml
    /// </summary>
    public partial class ClientCar : Page
    {
        public ClientCar()
        {
            InitializeComponent();
            LoadData();
        }
        private static readonly Regex TextRegex = new Regex(@"^[a-zA-Zа-яА-Я]+$");
        private static readonly Regex YearRegex = new Regex(@"^\d{4}$");
        private static readonly Regex RegNumberRegex = new Regex(@"^[a-zA-Z0-9]{8,}$");
        private static readonly Regex NumberRegex = new Regex(@"^\d{10}$");

        private readonly Entities _db = new Entities();
        private bool _isNavigating;
        private static readonly Random _random = new Random();

        private const string ValidationErrorTitle = "Ошибка валидации";
        private const string SuccessMessage = "Заявка успешно отправлена! Стоимость полиса: {0} рублей.";
        private const string PolicyNumberPrefix = "PL";



        private void LoadData()
        {
            try
            {
                LoadCarCategories();
                LoadCarBrands();
            }
            catch (Exception ex)
            {
                ShowError("Ошибка загрузки данных", ex.Message);
            }
        }

        private void LoadCarCategories()
        {
            CategoryBox.ItemsSource = _db.car_categories.ToList();
            CategoryBox.DisplayMemberPath = "category_name";
            CategoryBox.SelectedValuePath = "category_id";
        }

        private void LoadCarBrands()
        {
            BrandCar.ItemsSource = _db.car_brands.ToList();
            BrandCar.DisplayMemberPath = "brand_name";
            BrandCar.SelectedValuePath = "brand_id";
        }

        private void BtnDone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInputs()) return;

                var driver = GetCurrentDriver();
                if (driver == null) return;

                var newCar = CreateCar(driver.driver_id);
                var carId = SaveCarAndGetId(newCar);

                if (carId == 0) return;

                CreateInsurancePolicy(driver, carId);
                ShowSuccessMessage();
            }
            catch (SqlException ex)
            {
                ShowError("Ошибка базы данных", ex.Message);
            }
            catch (Exception ex)
            {
                ShowError("Непредвиденная ошибка", ex.Message);
            }
        }

        private bool ValidateInputs()
        {
            return ValidateSelectedBrand() &&
                   ValidateSelectedCategory() &&
                   ValidateModel() &&
                   ValidateYear() &&
                   ValidateRegNumber() &&
                   ValidateSTSNumber() &&
                   ValidatePTSNumber();
        }

        private bool ValidateSelectedBrand()
        {
            if (BrandCar.SelectedValue != null) return true;

            ShowWarning("Выберите марку автомобиля");
            return false;
        }

        private bool ValidateSelectedCategory()
        {
            if (CategoryBox.SelectedValue != null) return true;

            ShowWarning("Выберите категорию автомобиля");
            return false;
        }

        private bool ValidateModel()
        {
            if (TextRegex.IsMatch(ModelCar.Text)) return true;

            ShowWarning("Модель должна содержать только буквы");
            return false;
        }

        private bool ValidateYear()
        {
            if (YearRegex.IsMatch(YearManifacture.Text) &&
                int.TryParse(YearManifacture.Text, out int year) &&
                year >= 1900 && year <= DateTime.Now.Year + 1)
                return true;

            ShowWarning("Год производства должен быть четырехзначным числом между 1900 и " + (DateTime.Now.Year + 1));
            return false;
        }

        private bool ValidateRegNumber()
        {
            if (RegNumberRegex.IsMatch(RegNumber.Text)) return true;

            ShowWarning("Рег. номер должен содержать минимум 8 букв/цифр");
            return false;
        }

        private bool ValidateSTSNumber()
        {
            if (NumberRegex.IsMatch(STSNumber.Text)) return true;

            ShowWarning("СТС номер должен содержать 10 цифр");
            return false;
        }

        private bool ValidatePTSNumber()
        {
            if (NumberRegex.IsMatch(PTSnumber.Text)) return true;

            ShowWarning("ПТС номер должен содержать 10 цифр");
            return false;
        }

        private drivers GetCurrentDriver()
        {
            var driver = _db.drivers.FirstOrDefault(d => d.user_id == ((App)Application.Current).CurrentUserID);
            if (driver != null) return driver;

            ShowError("Водитель не найден", "ошибка");
            return null;
        }

        private cars CreateCar(int driverId)
        {
            return new cars
            {
                brand_id = (int)BrandCar.SelectedValue,
                category_id = (int)CategoryBox.SelectedValue,
                driver_id = driverId,
                model = ModelCar.Text,
                year_manufacture = int.Parse(YearManifacture.Text),
                reg_number = RegNumber.Text,
                stc_number = STSNumber.Text,
                pts_number = PTSnumber.Text
            };
        }

        private int SaveCarAndGetId(cars car)
        {
            _db.cars.Add(car);
            _db.SaveChanges();
            return car.car_id;
        }

        private void CreateInsurancePolicy(drivers driver, int carId)
        {
            var policy = new policies
            {
                policy_number = GeneratePolicyNumber(),
                driver_id = driver.driver_id,
                car_id = carId,
                driving_license_series = driver.driving_license_series,
                driving_license_number = driver.driving_license_number,
                cost = GeneratePolicyCost()
            };

            _db.policies.Add(policy);
            _db.SaveChanges();
        }

        private decimal GeneratePolicyCost()
        {
            return _random.Next(2000, 5001);
        }

        private string GeneratePolicyNumber()
        {
            return $"{PolicyNumberPrefix}-{DateTime.Now:yyyyMMddHHmmss}-{_random.Next(1000, 9999):D4}";
        }

        private void ShowSuccessMessage()
        {
            MessageBox.Show(string.Format(SuccessMessage, GeneratePolicyCost()),
                          "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, ValidationErrorTitle,
                          MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ShowError(string title, string message)
        {
            MessageBox.Show(message, title,
                          MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
