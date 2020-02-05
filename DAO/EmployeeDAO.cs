using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            return context.Employees.OfType<Employee>().Where(x => x.UserName.Equals(username)).FirstOrDefault();

        }

        public int FindClerkIdByCPId(int CPId)
        {
            int ClerkId = context.CPClerks
                .Where(x => x.IdCollectionPt == CPId)
                .Select(x => x.IdStoreClerk)
                .FirstOrDefault();
            return ClerkId;
        }

        public Employee FindEmployeeById(int idEmployee)
        {
            return context.Employees.OfType<Employee>().Where(x => x.IdEmployee == idEmployee).Include(x=>x.Department).FirstOrDefault();

        }

        public List<Employee> FindByRole (int IdRole)
        {
            return context.Employees.Where(x => x.IdRole == IdRole).ToList();
        }
    }
}