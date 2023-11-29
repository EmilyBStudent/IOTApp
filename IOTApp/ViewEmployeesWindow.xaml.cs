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
            FillBranchComboBox();
        }

        /// <summary>
        /// Get a list of company branches and assign it as the Branch Search combo
        /// box's item source.
        /// </summary>
        private void FillBranchComboBox()
        {
            string sql = "SELECT id, branch_name FROM branches;";
            List<Branch> branches = _db.QueryBranches(sql);
            ComboBoxBranch.ItemsSource = branches;
        }

        /// <summary>
        /// Fill the employee list data grid with the employees' data, filtering by the
        /// selected search/filter options if relevant.
        /// </summary>
        private void FillEmployeesDataGrid()
        {
            DataGridEmployeeList.DataContext = null;
            string sql = BuildEmployeesSQLQuery();
            List<Employee> employees = _db.QueryEmployees(sql);
            DataGridEmployeeList.DataContext = employees;
        }

        /// <summary>
        /// Build a SQL string to query the employees table for a list of employees,
        /// dependent on the search/filter options in use.
        /// </summary>
        /// <returns>The completed SQL query.</returns>
        private string BuildEmployeesSQLQuery()
        {
            string sql = "SELECT e.id, e.given_name, e.family_name, e.date_of_birth, " +
                "e.gender_identity, e.gross_salary, b.branch_name, CONCAT(s.given_name," +
                " ' ', s.family_name) AS supervisor_name FROM employees AS e " +
                "LEFT JOIN branches AS b ON e.branch_id = b.id " +
                "LEFT JOIN employees AS s ON e.supervisor_id = s.id ";

            // Search/filter employees if any of the search/filter fields are in use.
            string searchName = TextBoxSearchName.Text.Trim();
            Branch? searchBranch = (Branch)ComboBoxBranch.SelectedItem;
            if ((searchName.Length > 0) || (searchBranch != null))
            {
                sql = sql + "WHERE ";

                // Search/filter employees by name.
                if (searchName.Length > 0)
                {
                    // Allow the user to search both given name and family name fields
                    // simultaneously.
                    string[] names = searchName.Split(' ');
                    for (int i = 0; i < names.Length; i++)
                    {
                        sql = sql + $"(e.given_name LIKE '%{names[i]}%' OR " +
                            $"e.family_name LIKE '%{names[i]}%') ";
                        if (i < names.Length - 1)
                            sql = sql + "AND ";
                    }
                }

                // Search/filter employees by branch.
                if (searchBranch != null)
                {
                    if (searchName.Length > 0)
                        sql = sql + "AND ";
                    int branchId = searchBranch.Id;
                    sql = sql + $"b.id = {branchId} ";
                }
            }

            // Sort employees by family and given name.
            sql = sql + "ORDER BY e.family_name, e.given_name, e.id;";
            return sql;
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

        /// <summary>
        /// Search/filter the employee list by name when the text in the Search Name
        /// textbox changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxSearchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            FillEmployeesDataGrid();
        }

        /// <summary>
        /// Enable the branch search combo box when the checkbox is checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxFilterBranch_Checked(object sender, RoutedEventArgs e)
        {
            ComboBoxBranch.IsEnabled = true;
        }

        /// <summary>
        /// Disable the branch search combo box and remove its current selection when the
        /// checkbox is unchecked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxFilterBranch_Unchecked(object sender, RoutedEventArgs e)
        {
            ComboBoxBranch.IsEnabled = false;
            ComboBoxBranch.SelectedItem = null;
        }

        /// <summary>
        /// When the selected item in the Branch Search combo box changes, refresh the
        /// employee list in the DataGrid with the latest search/filter options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillEmployeesDataGrid();
        }

        /// <summary>
        /// Clear all filters and show the full employee list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClearFilters_Click(object sender, RoutedEventArgs e)
        {
            TextBoxSearchName.Clear();
            TextBoxMinSalary.Clear();
            TextBoxMaxSalary.Clear();
            CheckBoxFilterBranch.IsChecked = false;
        }
    }
}
