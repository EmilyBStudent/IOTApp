using MySqlConnector;
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
using System.Windows.Shapes;

namespace IOTApp
{
    /// <summary>
    /// Interaction logic for ViewEmployeesWindow.xaml. This window allows the user to
    /// view, search and manage employees.
    /// </summary>
    public partial class ViewEmployeesWindow : Window
    {
        private Database _db;

        /// <summary>
        /// Initialise the window and save a reference to the database connection.
        /// </summary>
        /// <param name="db"></param>
        public ViewEmployeesWindow(Database db)
        {
            InitializeComponent();
            _db = db;
            FillEmployeesDataGrid();
        }

        /// <summary>
        /// Fill the employee list data grid with the employees' data.
        /// </summary>
        private void FillEmployeesDataGrid()
        {
            DataGridEmployeeList.DataContext = null;
            string sql = "SELECT e.id, e.given_name, e.family_name, e.date_of_birth, " +
                "e.gender_identity, e.gross_salary, b.branch_name, CONCAT(s.given_name," +
                " ' ', s.family_name) AS supervisor_name FROM employees AS e " +
                "LEFT JOIN branches AS b ON e.branch_id = b.id " +
                "LEFT JOIN employees AS s ON e.supervisor_id = s.id " +
                "ORDER BY e.family_name, e.given_name, e.id;";
            List<Employee> employees = _db.QueryEmployees(sql);
            DataGridEmployeeList.DataContext = employees;
        }

        /// <summary>
        /// Clicking the Close button closes the View Employees dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
