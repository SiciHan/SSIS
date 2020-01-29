using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class DisbursementItemDAO
    {
        private readonly SSISContext context;

        public DisbursementItemDAO()
        {
            this.context = new SSISContext();
        }

        //James
        internal void GiveAndTake(IList<int> itemId, IList<int> transferQtyNum, IList<int> itemIdDeptFrom)
        {
            throw new NotImplementedException();
        }
        
        //James
        internal List<DisbursementItem> FindCorrespondingDisbursementItems(ICollection<DisbursementItem> disbursementItems)
        {
            List<DisbursementItem> di = new List<DisbursementItem>();
            // for every disbItem, this will send along the list of other departments' items which are only "Prepared"
            // this would be for the dynamic dropdownlist used in selecting which dept to take items from
            foreach (DisbursementItem i in disbursementItems)
            {
                di.AddRange(context.DisbursementItems
                    .Where(x => x.Disbursement.IdStatus == 9 && x.IdDisbursement != i.IdDisbursement && x.IdItem == i.IdItem)
                    .ToList());
            }

            return di;
        }
    }
}