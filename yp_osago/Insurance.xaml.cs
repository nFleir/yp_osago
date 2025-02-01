using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для Insurance.xaml
    /// </summary>
    public partial class Insurance : Page
    {
        private Entities db = new Entities();
        public Insurance()
        {
            InitializeComponent();

            var app = Application.Current as App;
            int currentUserId = app?.CurrentUserID ?? 0;

            if (currentUserId > 0)
            {
                var driver = db.drivers.FirstOrDefault(d => d.user_id == currentUserId);

                if (driver != null)
                {
                    var policy = db.policies
                                   .FirstOrDefault(p => p.driver_id == driver.driver_id);

                    if (policy != null)
                    {

                        PolicyNumber.Text = policy.policy_number;
                        IssueDate.Text = policy.issue_date.HasValue ? policy.issue_date.Value.ToString("dd/MM/yyyy") : "Не указана";
                        ExpirationDate.Text = policy.expiration_date.HasValue ? policy.expiration_date.Value.ToString("dd/MM/yyyy") : "Не указана";

                        DriverFullName.Text = $"{driver.first_name} {driver.last_name} {driver.second_name}";

                        var car = db.cars.FirstOrDefault(c => c.car_id == policy.car_id);
                        if (car != null)
                        {
                            var carBrand = db.car_brands.FirstOrDefault(b => b.brand_id == car.brand_id);
                            var carCategory = db.car_categories.FirstOrDefault(c => c.category_id == car.category_id);

                            CarInfo.Text = $"{carBrand?.brand_name} {car.model} ({carCategory?.category_name})";
                        }

                        DrivingLicenseSeries.Text = policy.driving_license_series;
                        DrivingLicenseNumber.Text = policy.driving_license_number;

                        PolicyCost.Text = policy.cost.ToString("C");
                    }
                    else
                    {
                        MessageBox.Show("Полис не найден.");
                    }
                }
                else
                {
                    MessageBox.Show("Водитель не найден.");
                }
            }
            else
            {
                MessageBox.Show("Ошибка: ID пользователя не задан.");
            }
        }
    }
}
