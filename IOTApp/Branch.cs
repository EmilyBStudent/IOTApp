using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace IOTApp
{
    /// <summary>
    /// A branch of the company.
    /// </summary>
    class Branch
    {
        /// <summary>
        /// The unique ID number of the branch.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the branch.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initialise the branch with the given data.
        /// </summary>
        /// <param name="id">The unique ID number of the branch.</param>
        /// <param name="name">The name of the branch.</param>
        public Branch(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Represent the branch as a string by simply returning its name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
