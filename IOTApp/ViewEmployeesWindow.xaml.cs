using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Globalization;
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

            // Parse and store search/filter settings.
            string searchName = TextBoxSearchName.Text.Trim();
            Branch? searchBranch = (Branch)ComboBoxBranch.SelectedItem;
            string minSalaryText = TextBoxMinSalary.Text.Trim();
            string maxSalaryText = TextBoxMaxSalary.Text.Trim();
            int minSalary, maxSalary;
            bool validMax = false, validMin = false;
            validMin = int.TryParse(minSalaryText, out minSalary);
            validMax = int.TryParse(maxSalaryText, out maxSalary);

            // Search/filter employees if any of the search/filter fields are in use.
            if ((searchName.Length > 0) || (searchBranch != null) ||
                validMin || validMax)
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

                // Search/filter employees by minimum salary.
                if (validMin)
                {
                    if ((searchName.Length > 0) || (searchBranch != null))
                        sql = sql + "AND ";
                    sql = sql + $"e.gross_salary > {minSalary} ";
                }

                // Search/filter employees by maximum salary.
                if (validMax)
                {
                    if ((searchName.Length > 0) || (searchBranch != null) ||
                        validMin)
                    {
                        sql = sql + "AND ";
                    }
                    sql = sql + $"e.gross_salary < {maxSalary} ";
                }
            }

            // Sort employees by family and given name.
            sql = sql + "ORDER BY e.family_name, e.given_name, e.id;";
            return sql;
        }

        /// <summary>
        /// Validate the data in the salary search fields. If appropriate, show an error
        /// to the user to explain the problem.
        /// </summary>
        /// <param name="textbox">The salary textbox to validate.</param>
        /// <param name="showErrors">If true, will show an error to the user for the
        /// first validation issue found.</param>
        /// <returns>True if the salary search data is valid, false if invalid.</returns>
        private bool ValidateSalaryFields(TextBox textbox, bool showErrors = true)
        {
            // A blank is a valid value as it simply means "no minimum" or "no
            // maximum".
            if (String.IsNullOrWhiteSpace(textbox.Text))
            {
                return true;
            }

            // Parse textbox value as an int if possible and record the context for
            // validation.
            string salaryText = textbox.Text.Trim();
            int salaryVal;
            bool validInt = false;
            validInt = int.TryParse(salaryText, out salaryVal);
            string valType = String.Empty;
            if (textbox == TextBoxMinSalary)
                valType = "minimum";
            else
                valType = "maximum";
            var textInfo = new CultureInfo("en-US", false).TextInfo;

            if (!validInt)
            {
                if (showErrors)
                {
                    string msg = $"The {valType} salary to search for must be a valid " +
                        "integer. Please do not include any punctuation or " +
                        "decimals.";
                    string caption = $"{textInfo.ToTitleCase(valType)} salary not valid";
                    MessageBox.Show(msg, caption, MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                }
                return false;
            }
            else if (salaryVal < 0)
            {
                if (showErrors)
                {
                    string msg = $"The {valType} salary cannot be negative.";
                    string caption = $"{textInfo.ToTitleCase(valType)} salary not valid";
                    MessageBox.Show(msg, caption, MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                }
                return false;
            }

            // Validate min and max as a range (min must be lower than max).
            int minSalary = 0, maxSalary = int.MaxValue;
            bool validMin, validMax;
            validMin = int.TryParse(TextBoxMinSalary.Text.Trim(), out minSalary);
            validMax = int.TryParse(TextBoxMaxSalary.Text.Trim(), out maxSalary);
            if (validMin && validMax && (minSalary >= maxSalary))
            {
                if (showErrors)
                {
                    string msg = "The minimum salary must be lower than the maximum salary.";
                    string caption = "Salary range not valid";
                    MessageBox.Show(msg, caption, MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                }
                return false;
            }

            return true;
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
            FillEmployeesDataGrid();
        }

        /// <summary>
        /// If the text in the minimum salary search box has changed, check whether the
        /// box contents are a valid int. If so, use the data entered to filter the
        /// employee list. Otherwise, ignore.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxMinSalary_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValidateSalaryFields(TextBoxMinSalary, false))
            {
                FillEmployeesDataGrid();
            }
        }

        /// <summary>
        /// If the text in the maximum salary search box has changed, check whether the
        /// box contents are a valid int. If so, use the data entered to filter the
        /// employee list. Otherwise, ignore.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxMaxSalary_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValidateSalaryFields(TextBoxMaxSalary, false))
            {
                FillEmployeesDataGrid();
            }
        }

        /// <summary>
        /// When the user moves focus away from the minimum salary textbox,
        /// validate the data. If the data is invalid, show the user an error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxMinSalary_LostFocus(object sender, RoutedEventArgs e)
        {
            // Skip validation if the textbox losing focus is empty.
            if (!String.IsNullOrWhiteSpace(TextBoxMinSalary.Text))
            {
                ValidateSalaryFields(TextBoxMinSalary, true);
            }
        }

        /// <summary>
        /// When the user moves focus away from the maximum salary textbox,
        /// validate the data. If the data is invalid, show the user an error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxMaxSalary_LostFocus(object sender, RoutedEventArgs e)
        {
            // Skip validation if the textbox losing focus is empty.
            if (!String.IsNullOrWhiteSpace(TextBoxMaxSalary.Text))
            {
                ValidateSalaryFields(TextBoxMaxSalary, true);
            }
        }

        /// <summary>
        /// Clicking the "Add" button opens the Add/Edit Employee window in Add mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            AddEditEmployeeWindow win = new(_db);
            win.Owner = this;
            win.ShowDialog();

            // After the dialog closes, refresh the employee list.
            FillEmployeesDataGrid();
        }
    }
}
