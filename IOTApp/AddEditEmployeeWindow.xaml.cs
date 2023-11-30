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
    /// Interaction logic for AddEditEmployeeWindow.xaml. This window allows adding or
    /// editing an employee depending on which mode it is initialised in.
    /// </summary>
    public partial class AddEditEmployeeWindow : Window
    {
        private Database _db;
        private WindowMode _windowMode;
        private Employee? _employee;

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
        /// Initialise the window in Edit Mode, in order to edit the given employee.
        /// </summary>
        /// <param name="db">The database connection to use.</param>
        /// <param name="employee">The employee to edit.</param>
        public AddEditEmployeeWindow(Database db, Employee employee)
        {
            InitializeComponent();
            _db = db;
            _windowMode = WindowMode.EditMode;
            _employee = employee;
            Title = "Edit Employee";
            InitialiseAllModes();
            FillEmployeeData();
        }

        /// <summary>
        /// Window initialisation tasks that are common to both Add Mode and Edit Mode.
        /// </summary>
        private void InitialiseAllModes()
        {
            // Fill the branch combo box with the list of branches.
            List<Branch> branches = _db.QueryBranches();
            ComboBoxBranch.ItemsSource = branches;

            // Fill the supervisor combo box with a list of employees.
            List<Employee> supervisors = _db.QueryEmployees();
            ComboBoxSupervisor.ItemsSource = supervisors;
        }

        /// <summary>
        /// If in edit mode, initialise the window fields with the details of the
        /// employee to edit.
        /// </summary>
        private void FillEmployeeData()
        {
            TextBoxGivenName.Text = _employee.GivenName;
            TextBoxFamilyName.Text = _employee.FamilyName;
            DatePickerDateOfBirth.Text = _employee.DateOfBirth.ToString();
            ComboBoxGender.Text = _employee.GenderIdentity;
            TextBoxSalary.Text = _employee.GrossSalary.ToString();
            ComboBoxBranch.Text = _employee.BranchName;
            ComboBoxSupervisor.Text = _employee.SupervisorName;
            LabelCreatedDate.Content = LabelCreatedDate.Content + _employee.CreatedDate;
            LabelUpdatedDate.Content = LabelUpdatedDate.Content + _employee.UpdatedDate;
        }

        /// <summary>
        /// Validate the employee data in the form. If not valid, show the user an error
        /// and return false. If valid, return true.
        /// </summary>
        /// <returns>True if employee data is valid, false if invalid.</returns>
        private bool ValidateEmployeeData()
        {
            // Check for missing data.
            if (TextBoxGivenName.Text.Trim().Length <= 0)
            {
                MessageBox.Show("Please enter a given name.", "No given name",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            else if (TextBoxFamilyName.Text.Trim().Length <= 0)
            {
                MessageBox.Show("Please enter a family name.", "No family name",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            else if (DatePickerDateOfBirth.SelectedDate == null)
            {
                MessageBox.Show("Please enter a date of birth.", "No date of birth",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            else if (ComboBoxGender.Text == String.Empty)
            {
                MessageBox.Show("Please select a gender identity.", "No gender selected",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            else if (TextBoxSalary.Text.Trim().Length <= 0)
            {
                MessageBox.Show("Please enter the salary.", "No salary",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            else if (ComboBoxBranch.Text == String.Empty)
            {
                MessageBox.Show("Please select a branch.", "No branch selected",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            // Validate salary as an integer.
            string salaryText = TextBoxSalary.Text.Trim();
            bool parseSalary = int.TryParse(salaryText, out int salary);
            if (parseSalary)
            {
                if (salary < 0)
                {
                    MessageBox.Show("Salary cannot be negative.", "Negative salary",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return false;
                }
                else if (salary > 1000000000)
                {
                    MessageBox.Show("Salary is too large. Please double-check the number.",
                        "Salary too large", MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Please enter salary as a whole dollar amount, without " +
                    "punctuation or cents.", "Salary not valid", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Clicking the Save button validates the input. If the data is valid it saves
        /// the employee data and closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            bool validData = ValidateEmployeeData();
            if (validData)
            {
                string givenName = TextBoxGivenName.Text.Trim();
                string familyName = TextBoxFamilyName.Text.Trim();
                DateTime dob = (DateTime)DatePickerDateOfBirth.SelectedDate;
                string dobIsoFormat = $"{dob.Year}-{dob.Month}-{dob.Day}";
                string gender = ComboBoxGender.Text;
                string salary = TextBoxSalary.Text.Trim();

                string supervisorId;
                if (ComboBoxSupervisor.SelectedItem != null)
                    supervisorId = ((Employee)ComboBoxSupervisor.SelectedItem).Id.ToString();
                else
                    supervisorId = "null";

                string branchId;
                if (ComboBoxBranch.SelectedItem != null)
                    branchId = ((Branch)ComboBoxBranch.SelectedItem).Id.ToString();
                else
                    branchId = "null";

                string sql = "INSERT INTO employees (given_name, family_name, " +
                    "date_of_birth, gender_identity, gross_salary, " +
                    "supervisor_id, branch_id, created_at) VALUES (" +
                    $"'{givenName}', '{familyName}', '{dobIsoFormat}', '{gender}', " +
                    $"{salary}, {supervisorId}, {branchId}, NOW());";
                _db.ExecuteNonQuery(sql);

                MessageBox.Show("New employee added.", "Employee added",
                    MessageBoxButton.OK);
                Close();
            }
        }
    }

    public enum WindowMode
    {
        AddMode,
        EditMode
    }
}
