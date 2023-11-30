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
            List<Branch> branches = _db.QueryBranches();
            ComboBoxBranch.ItemsSource = branches;
        }

        /// <summary>
        /// Fill the employee list data grid with the employees' data, filtering by the
        /// selected search/filter options if relevant.
        /// </summary>
        private void FillEmployeesDataGrid()
        {
            DataGridEmployeeList.DataContext = null;
            string whereClause = BuildEmployeesWhereClause();
            List<Employee> employees = _db.QueryEmployees(whereClause);
            DataGridEmployeeList.DataContext = employees;
        }

        /// <summary>
        /// Build a SQL WHERE clause to query the employees table, based on the
        /// search/filter options in use.
        /// </summary>
        /// <returns>The completed SQL WHERE clause.</returns>
        private string BuildEmployeesWhereClause()
        {
            string sql = String.Empty;

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
        /// Get the selected employee, if any, and return them. If no employee is
        /// selected, show the user an error message and return null.
        /// </summary>
        /// <returns>The selected employee, or null if no employee is
        /// selected.</returns>
        private Employee? GetSelectedEmployee()
        {
            // Get the selected employee, if any.
            Employee? emp = (Employee)DataGridEmployeeList.SelectedItem;
            if (emp == null)
            {
                MessageBox.Show("Please select an employee.", "No employee selected",
                    MessageBoxButton.OK);
            }
            return emp;
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

        /// <summary>
        /// Clicking the Delete Employee button removes the employee from the employees
        /// table. It also removes any references to that employee from the rest of the
        /// database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            // Get selected employee from the data grid, if any.
            Employee? emp = GetSelectedEmployee();
            if (emp == null)
                return;

            // Confirm the user is certain they want to delete the selected employee.
            string msg = "Deleting the employee will permanently remove them from the " +
                "database. This cannot be undone. Are you sure you want to delete " +
                $"{emp}?";
            string caption = "Confirm delete employee";
            MessageBoxResult result = MessageBox.Show(msg, caption, 
                MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                int id = emp.Id;
                // Delete the employee from the employees table, and remove references to
                // them as employee supervisor, branch manager, and working with clients.
                string[] sqlQueries = {
                    $"DELETE FROM employees WHERE id = {id};",
                    $"UPDATE employees SET supervisor_id = null, updated_at = NOW() " +
                        $"WHERE supervisor_id = {id};",
                    $"DELETE FROM working_with WHERE employee_id = {id};",
                    // Branches table is set up to automatically set updated_at field
                    // to current timestamp when a record is updated.
                    $"UPDATE branches SET manager_id = null, manager_started_at = null " +
                        $"WHERE manager_id = {id};",
                };
                foreach (string sql in sqlQueries)
                {
                    _db.ExecuteNonQuery(sql);
                }
                MessageBox.Show($"Employee {emp} deleted.", "Employee deleted",
                    MessageBoxButton.OK);
                FillEmployeesDataGrid();
            }
        }

        /// <summary>
        /// Clicking the "View Sales" button brings up a simple sales report for the
        /// selected employee.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonViewSales_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected employee, if any.
            Employee? emp = GetSelectedEmployee();
            if (emp == null)
                return;

            // Open the employee sales window with the selected employee.
            EmployeeSalesWindow win = new(_db, emp);
            win.Owner = this;
            win.ShowDialog();
        }

        /// <summary>
        /// Open the Edit Employee to edit the given employee, or the currently selected
        /// employee if no employee is passed in.
        /// </summary>
        /// <param name="emp">The employee to edit - optional.</param>
        private void EditEmployee(Employee? emp = null)
        {
            if (emp == null)
                // Get the selected employee, if any.
                emp = GetSelectedEmployee();
            if (emp == null)
                return;

            // Open the Edit Employee window with the selected employee.
            AddEditEmployeeWindow win = new(_db, emp);
            win.Owner = this;
            win.ShowDialog();

            // Refresh the employee list after the Edit Employee dialog is closed.
            FillEmployeesDataGrid();
        }

        /// <summary>
        /// Clicking the "Edit" button opens the Add/Edit Employee window in Edit mode,
        /// using the details of the employee passed in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonEditEmployee_Click(object sender, RoutedEventArgs e)
        {
            EditEmployee();
        }

        /// <summary>
        /// Double-clicking an employee in the employee list data grid opens the Edit
        /// Employee window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridEmployeeList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditEmployee();
        }
    }
}
