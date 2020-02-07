using System;
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
            this.context = new SSISContext();
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
        public List<Delegation> FindDelegationlist()
        {
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