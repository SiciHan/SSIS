using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class DisbursementDAO
    {
        private readonly SSISContext context;

        public DisbursementDAO()
        {
            this.context = new SSISContext();
        }

        //James
        public List<Disbursement> FindByStatus(String status)
        {
            var list = context.Disbursements.Where(x => x.Status.Label == status)
                .ToList();

            return list;
        }

        //James
        public void UpdateStatus(IEnumerable<int> disbIdsToSchedule)
        {
            try
            {
                context.Disbursements.Where(x => disbIdsToSchedule.Contains(x.IdDisbursement))
                    .ToList()
                    .ForEach(x => x.IdStatus = 10);
                context.SaveChanges();
            } catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
        }

        public Disbursement FindById(int disbId)
        {
            return context.Disbursements.Where(x => x.IdDisbursement == disbId).FirstOrDefault();
        }
    }
}