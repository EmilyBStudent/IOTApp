using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTApp
{
    /// <summary>
    /// An employee record. This does not include all information from the database, and
    /// it summarises some information in a readable form.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// The employee's unique ID.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The employee's first name.
        /// </summary>
        public string GivenName { get; set; }
        /// <summary>
        /// The employee's surname.
        /// </summary>
        public string FamilyName { get; set; }
        /// <summary>
        /// The employee's date of birth.
        /// </summary>
        public DateOnly DateOfBirth { get; set; }
        /// <summary>
        /// The employee's gender identity represented as a single character. Valid
        /// identity codes are M, F, and O (for Other).
        /// </summary>
        public string GenderIdentity { get; set; }
        /// <summary>
        /// The employee's annual salary before tax.
        /// </summary>
        public int GrossSalary { get; set; }
        /// <summary>
        /// The full name of the employee's supervisor. This can be null in the database;
        /// if so, it will be initialised as an empty string.
        /// </summary>
        public string SupervisorName { get; set; }
        /// <summary>
        /// The name of the branch the employee works at. If null, it will be initialised
        /// as an empty string.
        /// </summary>
        public string BranchName { get; set; }

        /// <summary>
        /// Initialise the employee with the given details.
        /// </summary>
        /// <param name="id">The employee's unique ID number.</param>
        /// <param name="givenName">The employee's first name.</param>
        /// <param name="familyName">The employee's surname.</param>
        /// <param name="dob">The employee's date of birth.</param>
        /// <param name="gender">The employee's gender identity, represented by one of
        /// the chars M, F or O (for Other).</param>
        /// <param name="salary">The employee's annual salary before tax.</param>
        /// <param name="supervisor">The employee's supervisor, if known. Can be null or
        /// omitted.</param>
        /// <param name="branch">The employee's branch, if known. Can be null or
        /// omitted.</param>
        public Employee(int id, string givenName, string familyName, DateOnly dob,
            string gender, int salary, string? supervisor="", string? branch="")
        {
            Id = id;
            GivenName = givenName;
            FamilyName = familyName;
            DateOfBirth = dob;
            GenderIdentity = gender;
            GrossSalary = salary;

            // If the supervisor name or the branch name is null, initialise the relevant
            // property as an empty string instead.
            if (supervisor == null)
                SupervisorName = String.Empty;
            else
                SupervisorName = supervisor;
            if (branch == null)
                BranchName = String.Empty;
            else
                BranchName = branch;
        }

        /// <summary>
        /// Return the employee's full name formatted as a string.
        /// </summary>
        /// <returns></returns>
        public string GetFullName()
        {
            return $"{GivenName} {FamilyName}";
        }

        /// <summary>
        /// Format the salary as a currency string, starting with a dollar sign and
        /// separating the thousands with commas.
        /// </summary>
        /// <returns>The employee's salary, formatted as currency.</returns>
        public string GetSalaryAsCurrency()
        {
            string origSalary = GrossSalary.ToString();
            string formattedSalary = String.Empty;

            // Starting from the end of the salary number string, copy one digit at a
            // time to the formatted string, inserting a comma every 3 digits.
            for (int i = origSalary.Length - 1; i < 0; i--)
            {
                formattedSalary = origSalary.Substring(i, 1) + formattedSalary;
                /// Add a comma between every 3 digits.
                if ((origSalary.Length - 1 - i) % 3 == 0)
                    formattedSalary = "," + formattedSalary;
            }

            // Ensure we didn't finish with a comma, then add the dollar sign.
            if (formattedSalary.Substring(0, 1) == ",")
                formattedSalary = formattedSalary.Substring(1);
            formattedSalary = "$" + formattedSalary;
            return formattedSalary;
        }

        /// <summary>
        /// Represent the Employee as a string by returning the employee's full name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetFullName();
        }
    }
}
