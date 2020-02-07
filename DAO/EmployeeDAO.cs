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
        public void ChangeNewRepCP(string name,string location)
        {
            Employee e = FindEmployeeByName(name);
            e.IdRole = 3;

            string codeDepartment = e.CodeDepartment;
            // find employee department and use if else to set idlocation
            Department dep = context.Departments.Where(d => d.CodeDepartment.Equals(codeDepartment)).FirstOrDefault();
            if(location.Equals("Management School"))
            {
                dep.IdCollectionPt = 1;
            }
            else if(location.Equals("Stationery Store"))
            {
                dep.IdCollectionPt = 2;
            }
            else if (location.Equals("Medical School"))
            {
                dep.IdCollectionPt = 3;
            }
            else if (location.Equals("Engineering School"))
            {
                dep.IdCollectionPt = 4;
            }
            else if (location.Equals("Science School"))
            {
                dep.IdCollectionPt = 5;
            }
            else if (location.Equals("University Hospital"))
            {
                dep.IdCollectionPt = 6;
            }
            else
            {

            }
            context.SaveChanges();
        }
        //SH
        public void PutOldRepBack(string name)
        {
            Employee e = FindEmployeeByName(name);
            e.IdRole = 1;
            context.SaveChanges();
        }
        //SH
        public Employee FindEmployeeByName(string name)
        {
            return context.Employees.Where(e => e.Name == name).FirstOrDefault();
        }
        //SH
        public Employee FindDepartmentRep(String codeDepartment)
        {
            
            return context.Employees.Where(e => e.CodeDepartment.Equals(codeDepartment)).Where(e => e.IdRole == 3).FirstOrDefault();
            //return context.Employees.Where(e => e.IdRole==3).FirstOrDefault();
        }
        // SH
        public List<Employee> FindEmployeeListByDepartment(string codeDepartment)
        {
            return context.Employees.OfType<Employee>().Where(x => x.CodeDepartment.Equals(codeDepartment)).ToList();
        }
        //SH
        public List<Requisition> RaisesRequisitions(string codeDepartment)
        {
            List<Employee> empList = FindEmployeeListByDepartment(codeDepartment);
            List<Requisition> reqList = context.Requisitions.ToList(); // to find the list of requisitions
            List<Requisition> empReqList = new List<Requisition>(); // find req list of employee belong to specific department 
            foreach(Employee e in empList)
            {
                foreach(Requisition r in reqList)
                {
                    if (r.IdEmployee==e.IdEmployee && r.IdStatusCurrent==1)
                    {
                        empReqList.Add(r);
                    }
                }
            }
            return empReqList;
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

        internal List<string> FindEmailsByRole(string role)
        {
            List<string> emails = new List<string>();
            foreach(Employee e in context.Employees.OfType<Employee>().Where(x => x.Role.Label.Contains(role)).ToList())
            {
                emails.Add(e.Email);
            }
            return emails;
        }

        internal List<int> FindIdByRole(string role)
        {

            List<int> ids = new List<int>();
            foreach (Employee e in context.Employees.OfType<Employee>().Where(x => x.Role.Label.Contains(role)).ToList())
            {
                ids.Add(e.IdEmployee);
            }
            return ids;
         
        }


        internal int FindHeadIdByIdEmployee(int idEmployee)
        {
            Employee e = context.Employees.OfType<Employee>().Where(x => x.IdEmployee == idEmployee).Include(x => x.Department).FirstOrDefault();
            string codeDepartment = e.Department.CodeDepartment;
            return context.Employees.OfType<Employee>().Where(x => x.CodeDepartment == codeDepartment && x.Role.Label.Equals("Head")).FirstOrDefault().IdEmployee;
        }

        internal int FindActingHeadIdByIdEmployee(int idEmployee)
        {
            Employee e = context.Employees.OfType<Employee>().Where(x => x.IdEmployee == idEmployee).Include(x => x.Department).FirstOrDefault();
            string codeDepartment = e.Department.CodeDepartment;
            Employee ah=context.Employees.OfType<Employee>().Where(x => x.CodeDepartment == codeDepartment && x.Role.Label.Equals("ActingHead")).FirstOrDefault();
            if (ah != null)
            {
                return ah.IdEmployee;
            }
            return 0; 
            
        }
    }
}