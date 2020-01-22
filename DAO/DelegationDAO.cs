using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class DelegationDAO
    {
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