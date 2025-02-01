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
    /// Логика взаимодействия для Autorization.xaml
    /// </summary>
    public partial class Autorization : Window
    {
        public Autorization()
        {
            InitializeComponent();
        }
        private static readonly Regex LoginRegex = new Regex(@"^[a-zA-Z0-9]{6,}$");
        private static readonly Regex PasswordRegex = new Regex(@"^(?=.*\d)[a-zA-Z0-9]{6,}$");
        private readonly Entities db = new Entities();
        private bool _isNavigating;

        private const string LoginErrorTitle = "Ошибка ввода логина";
        private const string PasswordErrorTitle = "Ошибка ввода пароля";
        private const string ValidationErrorMessage = "Проверьте введенные данные";
        private const string AuthSuccessMessage = "Вы авторизовались!";
        private const string ExitConfirmationMessage = "Вы действительно хотите выйти из приложения?";

        private enum UserRole
        {
            Admin = 1,
            Employee = 2
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = UserLogin.Text;
            var password = UserPassword.Password;

            try
            {
                if (!ValidateCredentials(login, password)) return;

                var user = AuthenticateUser(login, password);
                if (user == null)
                {
                    ShowAuthError();
                    return;
                }

                CompleteAuthentication(user);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
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

            MessageBox.Show(errorMessage.ToString(), ValidationErrorMessage,
                          MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        private users AuthenticateUser(string login, string password)
        {
            return db.users
                .AsNoTracking()
                .FirstOrDefault(u => u.login == login && u.password == password);
        }

        private void ShowAuthError()
        {
            MessageBox.Show("Неверные учетные данные или пользователь не найден",
                          "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CompleteAuthentication(users user)
        {
            MessageBox.Show(AuthSuccessMessage, "Успешная авторизация",
                          MessageBoxButton.OK, MessageBoxImage.Information);

            var app = (App)Application.Current;
            app.CurrentPositionID = user.role_id;
            app.CurrentUserID = user.user_id;

            OpenRoleSpecificWindow(user.role_id);
            CloseWindow();
        }

        private void OpenRoleSpecificWindow(int roleId)
        {
            Window nextWindow = null;

            switch (roleId)
            {
                case (int)UserRole.Admin:
                    nextWindow = new UserWindow();
                    break;
                case (int)UserRole.Employee:
                    nextWindow = new EmployeeWindow();
                    break;
                default:
                    throw new ArgumentException("Неизвестная роль пользователя");
            }

            nextWindow.Show();
        }

        private void CloseWindow()
        {
            _isNavigating = true;
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_isNavigating) return;

            e.Cancel = MessageBox.Show(ExitConfirmationMessage, "Подтверждение выхода",
                                     MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes;
        }

        private void NavToReg_Click(object sender, RoutedEventArgs e)
        {
            new Registration().Show();
            CloseWindow();
        }
    }
}
