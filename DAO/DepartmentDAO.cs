using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class DepartmentDAO
    {
        private readonly SSISContext context;

        public DepartmentDAO()
        {
            context = new SSISContext();
        }
        //private readonly SSISContext context;

        //public DepartmentDAO()
        //{
        //    this.context = new SSISContext();
        //}
        //SH
        public Department FindDepartmentCollectionPoint(String codeDepartment)
        {
            return context.Departments.Where(d => d.CodeDepartment.Equals(codeDepartment)).FirstOrDefault();
        }
        public bool UpdateCollectionPt(string codeDepartment, int idCollectionPt)
        {
            Department model = null;
            using (SSISContext db = new SSISContext())
            {
                model = db.Departments.OfType<Department>()
                    .Where(x => x.CodeDepartment == codeDepartment)
                    .FirstOrDefault();
                if (model == null) return false;
                CollectionPoint collectionPt = db.CollectionPoints.OfType<CollectionPoint>()
                   .Where(x => x.IdCollectionPt == idCollectionPt)
                   .FirstOrDefault();
                if (collectionPt == null) return false;
                model.IdCollectionPt = idCollectionPt;
                db.SaveChanges();
            }
            return true;
        }

        //@Shutong
        internal string FindCodeDepartmentByIdEmployee(int v)
        {
            Department department = context.Employees.OfType<Employee>().Where(x => x.IdEmployee == v).Select(x => x.Department).FirstOrDefault();
            return department.CodeDepartment;
        }

        public List<Department> FindAllDepartments()
        {
            List<Department> departments = context.Departments.ToList();
            return departments;
        }
    }
}