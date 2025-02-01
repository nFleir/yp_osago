using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using yp_osago.User;

namespace yp_osago
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isNav = false;
        public MainWindow()
        {
            InitializeComponent();
            FrameManager.MainFrame = MainFrame;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isNav)
            {
                return;
            }

            if (MessageBox.Show("Выйти из приложения?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }


        private void CloseWindow()
        {
            isNav = true;
            Close();
        }

        private void RegBtn_Click(object sender, RoutedEventArgs e)
        {
            new Registration().Show();
            CloseWindow();
        }

        private void AutorizBtn_Click(object sender, RoutedEventArgs e)
        {
            new Autorization().Show();
            CloseWindow();
        }
    }
}
