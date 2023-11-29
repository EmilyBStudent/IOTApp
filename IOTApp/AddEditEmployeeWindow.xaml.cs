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
    /// Interaction logic for AddEditEmployeeWindow.xaml
    /// </summary>
    public partial class AddEditEmployeeWindow : Window
    {
        private Database _db;
        private WindowMode _windowMode;

        /// <summary>
        /// Initialise the window in Add Mode.
        /// </summary>
        /// <param name="db">The database connection to use.</param>
        public AddEditEmployeeWindow(Database db)
        {
            InitializeComponent();
            _db = db;
            _windowMode = WindowMode.AddMode;
            Title = "Add Employee";
            InitialiseAllModes();
        }

        /// <summary>
        /// Window initialisation tasks that are common to both Add Mode and Edit Mode.
        /// </summary>
        private void InitialiseAllModes()
        {
            // Fill the branch combo box with the list of branches.
            string sql = "SELECT id, branch_name FROM branches;";
            List<Branch> branches = _db.QueryBranches(sql);
            ComboBoxBranch.ItemsSource = branches;

            // Fill the supervisor combo box with a list of employees.
            sql = "SELECT e.id, e.given_name, e.family_name, e.date_of_birth, " +
                "e.gender_identity, e.gross_salary, b.branch_name, CONCAT(s.given_name," +
                " ' ', s.family_name) AS supervisor_name FROM employees AS e " +
                "LEFT JOIN branches AS b ON e.branch_id = b.id " +
                "LEFT JOIN employees AS s ON e.supervisor_id = s.id " +
                "ORDER BY family_name, given_name;";
            List<Employee> supervisors = _db.QueryEmployees(sql);
            ComboBoxSupervisor.ItemsSource = supervisors;
        }
    }

    public enum WindowMode
    {
        AddMode,
        EditMode
    }
}
