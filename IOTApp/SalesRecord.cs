using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTApp
{
    /// <summary>
    /// Record of sales made by a specific employee to a specific client.
    /// </summary>
    public class SalesRecord
    {
        /// <summary>
        /// The client's unique ID.
        /// </summary>
        public int ClientId { get; set; }
        /// <summary>
        /// The client's name.
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// The employee's unique ID.
        /// </summary>
        public int EmployeeId { get; set; }
        /// <summary>
        /// The employee's name.
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// Total sales made by this employee to this client.
        /// </summary>
        public int TotalSales { get; set; }

        /// <summary>
        /// Construct an empty sales record.
        /// </summary>
        public SalesRecord() {
            ClientName = string.Empty;
            EmployeeName = string.Empty;
        }

        /// <summary>
        /// Construct a sales record using the provided data.
        /// </summary>
        /// <param name="clientId">The client's unique ID.</param>
        /// <param name="clientName">The client's name.</param>
        /// <param name="employeeId">The employee's unique ID.</param>
        /// <param name="employeeName">The employee's name.</param>
        /// <param name="totalSales">Total sales made by this employee to this
        /// client.</param>
        public SalesRecord(int clientId, string clientName, int employeeId, 
            string employeeName, int totalSales)
        {
            ClientId = clientId;
            ClientName = clientName;
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            TotalSales = totalSales;
        }
    }
}
