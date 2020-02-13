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
        //@Yu Shaohang
        public Employee FindCurrentRepAndCPByHeadId(int idHead)
        {
            Employee head = context.Employees.Where(e => e.IdEmployee == idHead).FirstOrDefault();
            string codeDepartment = head.CodeDepartment;
            Employee currentRep = context.Employees.Where(e => e.CodeDepartment.Equals(codeDepartment) && e.Role.Label.Equals("Representative")).Include(x=>x.Department.CollectionPt).FirstOrDefault();
            return currentRep;
        }

        //@Yu Shaohang
        public void DelegateEmployeeToActingRole(string name)
        {
            // Find the emp by empName--> change emp role to idRole=4
            Employee e = FindEmployeeByNameAndRole(name);
            //e.IdRole = 4;// set to idRole to 4
            e.Role = context.Roles.OfType<Role>().Where(x => x.Label.Equals("ActingHead")).FirstOrDefault();
            
            context.SaveChanges();
        }
        //@Yu Shaohang
        public Employee FindEmployeeByNameAndRole(string name)
        {
            return context.Employees.Where(e => e.Name == name).Where(e=>e.IdRole==1).Include(x => x.Role).FirstOrDefault();
        }
        //@Yu Shaohang
        public List<Employee> FindEmployeeListByDepartmentAndRole(string codeDepartment)
        {
            return context.Employees.Where(e => e.CodeDepartment.Equals(codeDepartment)).Where(e => e.IdRole == 1).ToList();
        }
        //@Yu Shaohang
        public void ChangeNewRepCP(string name,string location)
        {
            Employee e = FindEmployeeByName(name);
            //change to rep
            e.Role = context.Roles.OfType<Role>().Where(x => x.Label.Equals("Representative")).FirstOrDefault();
            //e.IdRole = 3;
            context.SaveChanges();
            //change cp
            Department dep = context.Departments.Where(d => d.CodeDepartment.Equals(e.CodeDepartment)).Include(x=>x.CollectionPt).FirstOrDefault();
            dep.CollectionPt = context.CollectionPoints.OfType<CollectionPoint>().Where(c => c.Location.Equals(location)).FirstOrDefault();
            context.SaveChanges();
        }
        //@Yu Shaohang
        public void PutOldRepBack(string name)
        {
            Employee e = FindEmployeeByName(name);
            //change to employee
            //e.Role = context.Roles.OfType<Role>().Where(x => x.Label.Equals("Employee")).FirstOrDefault();
            e.IdRole = 1;
            context.SaveChanges();
        }
        //@Yu Shaohang
        public Employee FindEmployeeByName(string name)
        {
            //return context.Employees.Include("Role").Where(e => e.Name == name).Include(x=>x.Role).FirstOrDefault();
            return context.Employees.Where(e => e.Name == name).Include(x => x.Role).FirstOrDefault();
        }
        //@Yu Shaohang
        public Employee FindDepartmentRep(String codeDepartment)
        {
            return context.Employees.Where(e => e.CodeDepartment.Equals(codeDepartment) && e.IdRole == 3).FirstOrDefault();
            //return context.Employees.Where(e => e.CodeDepartment.Equals(codeDepartment)).Where(e => e.IdRole == 3).FirstOrDefault();
            //return context.Employees.Where(e => e.IdRole==3).FirstOrDefault();
        }
        //@Yu Shaohang
        public List<Employee> FindEmployeeListByDepartment(string codeDepartment)
        {
            // so dep rep will also appear in the collection point list for head to change cp.
            return context.Employees.OfType<Employee>().Where(x => (x.Role.Label.Equals("Employee") || x.Role.Label.Equals("Representative")) && x.CodeDepartment.Equals(codeDepartment)).ToList();
            //return context.Employees.OfType<Employee>().Where(x =>x.Role.Label.Equals("Employee") && x.CodeDepartment.Equals(codeDepartment)).ToList();
        }
        //@Yu Shaohang
        public List<Requisition> RaisesRequisitions(string codeDepartment)
        {
            List<Employee> empList = FindEmployeeListByDepartment(codeDepartment);
            List<Requisition> reqList = context.Requisitions.ToList(); // to find the list of requisitions
            List<Requisition> empReqList = new List<Requisition>(); // find req list of employee belong to specific department 
            foreach(Employee e in empList)
            {
                foreach(Requisition r in reqList)
                {
                    if (r.IdEmployee==e.IdEmployee && r.IdStatusCurrent==2)
                    {
                        empReqList.Add(r);
                    }
                }
            }
            return empReqList;
        }

        //@Shutong
        public Employee UpdateRoleToActingHead(int idEmployee)
        {
            Employee employee = context.Employees.OfType<Employee>().Where(x => x.IdEmployee == idEmployee).Include(x => x.Role).FirstOrDefault();
            employee.Role = context.Roles.OfType<Role>().Where(x => x.Label.Contains("ActingHead")).FirstOrDefault();
            
            context.SaveChanges();
            return employee;

        }
        //@Shutong
        internal Employee UpdateRoleToEmployee(int idEmployee)
        {
            Employee employee = context.Employees.OfType<Employee>().Where(x => x.IdEmployee == idEmployee).Include(x => x.Role).FirstOrDefault();
            employee.Role= context.Roles.OfType<Role>().Where(x => x.Label.Contains("Employee")).FirstOrDefault();
            
            context.SaveChanges();
            return employee;
        }
       //@Shutong
        public Employee FindEmployeeByUsername(string username)
        {
            return context.Employees.OfType<Employee>().Where(x => x.UserName.Equals(username)).Include(x=>x.Role).FirstOrDefault();

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
        //@Shutong
        internal List<string> FindEmailsByRole(string role)
        {
            List<string> emails = new List<string>();
            foreach(Employee e in context.Employees.OfType<Employee>().Where(x => x.Role.Label.Contains(role)).ToList())
            {
                emails.Add(e.Email);
            }
            return emails;
        }
        //@Shutong
        internal List<int> FindIdByRole(string role)
        {

            List<int> ids = new List<int>();
            foreach (Employee e in context.Employees.OfType<Employee>().Where(x => x.Role.Label.Contains(role)).ToList())
            {
                ids.Add(e.IdEmployee);
            }
            return ids;
         
        }

        //@Shutong
        internal int FindHeadIdByIdEmployee(int idEmployee)
        {
            Employee e = context.Employees.OfType<Employee>().Where(x => x.IdEmployee == idEmployee).Include(x => x.Department).FirstOrDefault();
            string codeDepartment = e.Department.CodeDepartment;
            return context.Employees.OfType<Employee>().Where(x => x.CodeDepartment == codeDepartment && x.Role.Label.Equals("Head")).FirstOrDefault().IdEmployee;
        }
        //@Shutong
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

        //@Shutong
        internal List<Employee> FindAllClerk()
        {
           
            
          return context.Employees.OfType<Employee>().Where(x => x.Role.Label.Contains("Clerk")).ToList();

            
        }
    }
}