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
using System.Windows.Shapes;

namespace yp_osago
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public bool isNav = false;
        public UserWindow()
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

        private void cpClick_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new ClientPolicy());
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new Insurance());
        }
    }
}
