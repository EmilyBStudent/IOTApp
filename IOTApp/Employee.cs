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
    class Employee
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
        public char GenderIdentity { get; set; }
        /// <summary>
        /// The employee's annual salary before tax.
        /// </summary>
        public int GrossSalary { get; set; }
        /// <summary>
        /// The full name of the employee's supervisor.
        /// </summary>
        public string SupervisorName { get; set; }
        /// <summary>
        /// The name of the branch the employee works at.
        /// </summary>
        public string BranchName { get; set; }

        /// <summary>
        /// Return the employee's full name formatted as a string.
        /// </summary>
        /// <returns></returns>
        public string GetFullName()
        {
            return $"{GivenName} {FamilyName}";
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
