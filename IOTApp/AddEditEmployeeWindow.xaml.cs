﻿using System;
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
