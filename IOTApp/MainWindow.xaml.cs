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

namespace IOTApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Database _db;

        /// <summary>
        /// Initialise the window by creating a connection to the database and storing
        /// it.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _db = new Database();
        }

        /// <summary>
        /// Clicking the File -> Exit menu item closes the app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Open the View Employees window as a dialog.
        /// </summary>
        private void ViewEmployees()
        {
            ViewEmployeesWindow win = new(_db);
            win.Owner = this;
            win.ShowDialog();
        }

        /// <summary>
        /// Clicking the Employees button opens the View Employees dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonEmployees_Click(object sender, RoutedEventArgs e)
        {
            ViewEmployees();
        }

        /// <summary>
        /// Clicking the Employees -> View/Manage menu item opens the View Employees
        /// dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemViewEmployees_Click(object sender, RoutedEventArgs e)
        {
            ViewEmployees();
        }
    }
}
