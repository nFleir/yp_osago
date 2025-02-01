using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Data.SqlClient;

namespace yp_osago
{
    /// <summary>
    /// Логика взаимодействия для RegPolice.xaml
    /// </summary>
    public partial class RegPolice : Page
    {
        public RegPolice(policies selectedPolicy)
        {
            InitializeComponent();
            currentPolicy = db.policies.FirstOrDefault(p => p.policy_id == selectedPolicy.policy_id);

            if (currentPolicy != null)
            {
                LoadPolicyData();
            }
        }

        private readonly Regex insuranceRegex = new Regex(@"^[А-Яа-яA-Za-z\s\-]+$");
        private Entities db = new Entities();
        private policies currentPolicy;



        private void LoadPolicyData()
        {
            PolicyNumber.Text = currentPolicy.policy_number;
            InsuranceCompany.Text = currentPolicy.insurance_company;
            IssueDate.SelectedDate = currentPolicy.issue_date;
            ExpirationDate.SelectedDate = currentPolicy.expiration_date;
            FIODriver.Text = currentPolicy.drivers.FullName;
            CarDriver.Text = currentPolicy.cars.FullCarInfo;
            DrivingLicenseSerial.Text = currentPolicy.driving_license_series;
            DrivingLicenseNumber.Text = currentPolicy.driving_license_number;
            Cost.Text = currentPolicy.cost.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (currentPolicy != null)
            {
                try
                {



                    DateTime? issueDate = IssueDate.SelectedDate;
                    DateTime? expirationDate = ExpirationDate.SelectedDate;

                    if (issueDate == null || expirationDate == null)
                    {
                        MessageBox.Show("Даты начала и окончания действия полиса не могут быть пустыми.",
                            "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }


                    if (expirationDate < issueDate)
                    {
                        MessageBox.Show("Дата окончания полиса не может быть раньше даты начала.",
                            "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }


                    if (issueDate > DateTime.Now)
                    {
                        MessageBox.Show("Дата начала действия полиса не может быть в будущем.",
                            "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }


                    currentPolicy.issue_date = issueDate.Value;
                    currentPolicy.expiration_date = expirationDate.Value;


                    if (decimal.TryParse(Cost.Text, out decimal cost))
                        currentPolicy.cost = cost;
                    else
                    {
                        MessageBox.Show("В поле 'Конечная цена' должно быть число.",
                            "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    db.SaveChanges();
                    MessageBox.Show("Полис обновлён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService?.Navigate(new Policies());
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
