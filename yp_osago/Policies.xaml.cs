using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Логика взаимодействия для Policies.xaml
    /// </summary>
    public partial class Policies : Page
    {
        private Entities db = new Entities();
        public Policies()
        {
            InitializeComponent();
            DataGridPolicies.ItemsSource = db.policies.ToList();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var policyToDelete = DataGridPolicies.SelectedItems.Cast<policies>().Select(p => p.policy_id).ToList();
            if (MessageBox.Show($"Вы точно хотите удалить {policyToDelete.Count()} полисов?", "Информация", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var PoliciesToDelete = db.policies.Where(p => policyToDelete.Contains(p.policy_id)).ToList();
                    db.policies.RemoveRange(PoliciesToDelete);
                    db.SaveChanges();
                    DataGridPolicies.ItemsSource = db.policies.ToList();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPolicies.SelectedItem is policies selectedPolicy)
            {
                FrameManager.MainFrame.Navigate(new RegPolice(selectedPolicy));
            }
            else
            {
                MessageBox.Show("Выберите полис для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
