﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class DelegationDAO
    {
        private readonly SSISContext context;
        public DelegationDAO()
        {
            context = new SSISContext();
        }
        //private readonly SSISContext context1;

        //public DelegationDAO()
        //{
        //    this.context = new SSISContext();
        //}
        public void DeactivateDelegationByDelegationId(int idDelegation)
        {
            Delegation d = context.Delegations.Where(x => x.IdDelegation == idDelegation).FirstOrDefault();
            // set d. end date
            DateTime today = DateTime.Today;
            d.StartDate = today;
            d.EndDate = today;
            // change idRole as well
            Employee e = context.Employees.Where(x => x.Role.Label.Equals("ActingHead")).FirstOrDefault();
            if (e != null)
            {
                int id = e.Role.IdRole;
                e.IdRole = 1; // if idrole set to 1==> can find in the employee list for delegation acting head
            }

            context.SaveChanges();
        }
        public void DeactivateDelegationById(int idEmployee)
        {
           Delegation d= FindDelegationById(idEmployee);
            // set d. end date
            DateTime today = DateTime.Today;
            d.StartDate = today;
            d.EndDate = today;
            // change idRole as well
            Employee e = context.Employees.Where(x => x.Role.Label.Equals("ActingHead")).FirstOrDefault();
            if (e != null)
            {
                int id = e.Role.IdRole;
                e.IdRole = 1; // if idrole set to 1==> can find in the employee list for delegation acting head
            }
           
            context.SaveChanges();
        }
        public Delegation FindDelegationById(int idEmployee)
        {
            return context.Delegations.Where(d => d.IdEmployee == idEmployee).FirstOrDefault();
        }
        //SH
        public void RemoveDelegate(int idEmployee)
        {
            Delegation deleg = context.Delegations.Where(d => d.IdEmployee == idEmployee).FirstOrDefault();
            context.Delegations.Remove(deleg);
            context.SaveChanges();
        }
        //SH
        public List<Delegation> FindDelegationListByDepartment(string codeDepartment)
        {
           List<Delegation> delegationAllDepartment= context.Delegations.Include("Employee").ToList();
            List<Employee> employeeListByDepartment = context.Employees.Where(e => e.CodeDepartment.Equals(codeDepartment)).ToList();
            List<Delegation> delegationDepartmentList = new List<Delegation>();
            foreach(Delegation d in delegationAllDepartment)
            {
                // put each delegation id  to test with specific 
                foreach(Employee e in employeeListByDepartment)
                {
                    if (d.IdEmployee == e.IdEmployee)
                    {
                        delegationDepartmentList.Add(d);
                    }
                }
            }
            delegationDepartmentList=delegationDepartmentList.OrderBy(c => c.StartDate).OrderBy(d=>d.EndDate).ToList();
            return delegationDepartmentList;
        }
        //SH
        public List<Delegation> FindDelegationlist()
        {
            //
            return context.Delegations.Include("Employee").ToList();
        }
        //SH
        public void UpdateDelegation(string name, DateTime startDate,DateTime endDate)
        {
            Employee ActingHead = context.Employees.Where(e => e.Name.Equals(name)).FirstOrDefault();
            int idEmployee = ActingHead.IdEmployee;
            Delegation deleg= new Delegation();
            deleg.IdEmployee = idEmployee;
            deleg.StartDate = startDate;
            deleg.EndDate = endDate;
            context.Delegations.Add(deleg);
            context.SaveChanges();
        }

        
        public void Update(Delegation d)
        {
            using (SSISContext context = new SSISContext())
            {
                Delegation del= context.Delegations.OfType<Delegation>().Where(x => x.IdDelegation==d.IdDelegation).FirstOrDefault();
                del.IdEmployee = d.IdEmployee;
                del.StartDate = d.StartDate;
                del.EndDate = d.EndDate;
                context.SaveChanges();
            }
        }
        public void Create(Delegation d)
        {
            using (SSISContext context = new SSISContext())
            {
                context.Delegations.Add(d);
                context.SaveChanges();
            }
        }

        public List<Delegation> FindAll()
        {
            using (SSISContext context = new SSISContext())
            {
                return context.Delegations.OfType<Delegation>().ToList<Delegation>();
            }
        }

        internal bool CheckIfInDelegationPeriod(int idActingHead)
        {
            
            List<Delegation> delegations=context.Delegations.OfType<Delegation>().Where(x => x.IdEmployee == idActingHead).ToList();

            if (delegations.Count == 0)
            {
                return false;
            }
            foreach(Delegation d in delegations)
            {
                //if start date is ealier than current tiem, and current time is earlier than end date
                if(DateTime.Compare(d.StartDate, DateTime.Now)<=0 && DateTime.Compare(DateTime.Now, d.EndDate) <= 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void Delete(Delegation d)
        {
            using (SSISContext context = new SSISContext())
            {
                Delegation del = context.Delegations.OfType<Delegation>().Where(x => x.IdDelegation == d.IdDelegation).FirstOrDefault();
                context.Delegations.Remove(del);
                context.SaveChanges();
            }
        }
    }
}