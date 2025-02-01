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
    /// Логика взаимодействия для ClientPolicy.xaml
    /// </summary>
    public partial class ClientPolicy : Page
    {
        public ClientPolicy()
        {
            InitializeComponent();
            LoadRegions();
        }

        private static readonly Regex FioRegex = new Regex(@"^[А-Яа-яA-Za-z\s\-]+$");
        private static readonly Regex DriverSerialRegex = new Regex(@"^[A-Za-z]{2}$");
        private static readonly Regex DriverNumberRegex = new Regex(@"^\d{6}$");
        private static readonly Regex PassportSerialRegex = new Regex(@"^\d{2}$");
        private static readonly Regex PassportNumberRegex = new Regex(@"^\d{10}$");

        private readonly Entities _db = new Entities();

        private const string RequiredFieldsError = "Заполните все обязательные поля!";
        private const string AgeValidationError = "Пользователю должно быть не менее 18 лет!";
        private const string ExistingDriverError = "Пользователь уже зарегистрирован как водитель";
        private const string UserNotFoundError = "Не найден текущий пользователь!";
        private const string FioFormatError = "ФИО должно содержать минимум 2 компонента (Фамилия Имя)";


        private void LoadRegions()
        {
            try
            {
                RegionBox.ItemsSource = _db.regions.ToList();
                RegionBox.DisplayMemberPath = "region_name";
                RegionBox.SelectedValuePath = "region_id";
            }
            catch (Exception ex)
            {
                ShowError("Ошибка загрузки регионов", ex.Message);
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateForm()) return;

                var currentUserId = ((App)Application.Current).CurrentUserID;
                if (currentUserId == 0)
                {
                    ShowError("Ошибка авторизации", UserNotFoundError);
                    return;
                }

                if (IsDriverRegistered(currentUserId))
                {
                    ShowError("Ошибка регистрации", ExistingDriverError);
                    return;
                }

                var driver = CreateDriver(currentUserId);
                SaveDriver(driver);

                NavigateToNextPage();
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

        private bool ValidateForm()
        {
            if (!CheckRequiredFields()) return false;
            if (!ValidateFieldFormats()) return false;
            if (!ValidateAge()) return false;
            return ValidateFioStructure();
        }

        private bool CheckRequiredFields()
        {
            var requiredFields = new List<bool>
    {
        !string.IsNullOrWhiteSpace(FullName.Text),
        DateBirth.SelectedDate != null,
        RegionBox.SelectedValue != null,
        !string.IsNullOrWhiteSpace(SerialDriverNumber.Text),
        !string.IsNullOrWhiteSpace(NumberDriver.Text),
        InspireDateLicense.SelectedDate != null,
        !string.IsNullOrWhiteSpace(SerialPassport.Text),
        !string.IsNullOrWhiteSpace(NumberPassport.Text),
        InspireDatePassport.SelectedDate != null
    };

            if (requiredFields.All(f => f)) return true;

            ShowWarning("Проверка данных", RequiredFieldsError);
            return false;
        }

        private bool ValidateFieldFormats()
        {
            var validations = new Dictionary<Func<bool>, string>
    {
        {() => FioRegex.IsMatch(FullName.Text), "ФИО содержит недопустимые символы"},
        {() => DriverSerialRegex.IsMatch(SerialDriverNumber.Text), "Неверный формат серии водительского удостоверения"},
        {() => DriverNumberRegex.IsMatch(NumberDriver.Text), "Неверный формат номера водительского удостоверения"},
        {() => PassportSerialRegex.IsMatch(SerialPassport.Text), "Неверный формат серии паспорта"},
        {() => PassportNumberRegex.IsMatch(NumberPassport.Text), "Неверный формат номера паспорта"}
    };

            foreach (var validation in validations)
            {
                if (!validation.Key())
                {
                    ShowWarning("Ошибка формата", validation.Value);
                    return false;
                }
            }
            return true;
        }

        private bool ValidateAge()
        {
            var birthDate = DateBirth.SelectedDate.Value;
            var age = DateTime.Today.Year - birthDate.Year;
            if (birthDate > DateTime.Today.AddYears(-age)) age--;

            if (age >= 18) return true;

            ShowWarning("Проверка возраста", AgeValidationError);
            return false;
        }

        private bool ValidateFioStructure()
        {
            var fioParts = FullName.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (fioParts.Length >= 2 && fioParts.Length <= 3) return true;

            ShowWarning("Ошибка формата", FioFormatError);
            return false;
        }

        private bool IsDriverRegistered(int userId)
        {
            return _db.drivers.Any(d => d.user_id == userId);
        }

        private drivers CreateDriver(int userId)
        {
            var fioParts = FullName.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            return new drivers
            {
                user_id = userId,
                last_name = fioParts[0],
                first_name = fioParts[1],
                second_name = fioParts.Length > 2 ? fioParts[2] : "",
                birth_date = DateBirth.SelectedDate.Value,
                region_id = (int)RegionBox.SelectedValue,
                driving_license_series = SerialDriverNumber.Text,
                driving_license_number = NumberDriver.Text,
                license_issue_date = InspireDateLicense.SelectedDate.Value,
                passport_series = SerialPassport.Text,
                passport_number = NumberPassport.Text,
                passport_issue_date = InspireDatePassport.SelectedDate.Value
            };
        }

        private void SaveDriver(drivers driver)
        {
            _db.drivers.Add(driver);
            _db.SaveChanges();
        }

        private void NavigateToNextPage()
        {
            FrameManager.MainFrame.Navigate(new ClientCar());
        }

        private void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowWarning(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
