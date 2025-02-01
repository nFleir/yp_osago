using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;


namespace yp_osago.User
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }
        private static readonly Regex LoginRegex = new Regex(@"^[a-zA-Z0-9]{6,}$");
        private static readonly Regex PasswordRegex = new Regex(@"^(?=.*\d)[a-zA-Z0-9]{6,}$");
        private readonly Entities db = new Entities();
        private bool _isNavigating;

        private const string ValidationErrorMessage = "Проверьте введенные данные";
        private const string RegistrationSuccessMessage = "Вы зарегистрированы в систему!";
        private const string ExitConfirmationMessage = "Вы действительно хотите выйти из приложения?";
        private const string LoginExistsMessage = "Аккаунт с таким логином уже существует\nПопробуйте другой";


        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            var login = UserLogin.Text;
            var password = UserPassword.Password;

            try
            {
                if (!ValidateCredentials(login, password)) return;

                if (IsLoginExists(login))
                {
                    MessageBox.Show(LoginExistsMessage, "Ошибка регистрации",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                CreateNewUser(login, password);
                ShowRegistrationSuccess();
                NavigateToLogin();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка базы данных",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateCredentials(string login, string password)
        {
            if (LoginRegex.IsMatch(login) && PasswordRegex.IsMatch(password))
                return true;

            var errorMessage = new StringBuilder(ValidationErrorMessage);

            if (!LoginRegex.IsMatch(login))
            {
                errorMessage.AppendLine("\n\nТребования к логину:");
                errorMessage.AppendLine("- Латиница и цифры");
                errorMessage.AppendLine("- Минимум 6 символов");
            }

            if (!PasswordRegex.IsMatch(password))
            {
                errorMessage.AppendLine("\n\nТребования к паролю:");
                errorMessage.AppendLine("- Латиница и цифры");
                errorMessage.AppendLine("- Минимум 6 символов");
                errorMessage.AppendLine("- Хотя бы одна цифра");
            }

            MessageBox.Show(errorMessage.ToString(), "Ошибка валидации",
                          MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        private bool IsLoginExists(string login)
        {
            return db.users.Any(u => u.login == login);
        }

        private void CreateNewUser(string login, string password)
        {
            var newUser = new users
            {
                login = login,
                password = password,
                role_id = 1
            };

            db.users.Add(newUser);
            db.SaveChanges();
        }

        private void ShowRegistrationSuccess()
        {
            MessageBox.Show(RegistrationSuccessMessage, "Успешная регистрация",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NavigateToLogin()
        {
            _isNavigating = true;
            new Autorization().Show();
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_isNavigating) return;

            e.Cancel = MessageBox.Show(ExitConfirmationMessage, "Подтверждение выхода",
                                     MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes;
        }

        private void NavToLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigateToLogin();
        }
    }
}
