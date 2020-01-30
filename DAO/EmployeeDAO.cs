using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class EmployeeDAO
    {
        private readonly SSISContext context;

        public EmployeeDAO()
        {
            this.context = new SSISContext();
        }

        public Employee FindEmployeeByUsername(string username)
        {
            return context.Employees.OfType<Employee>().Where(x => x.UserName == username).FirstOrDefault();

        }

    }
}