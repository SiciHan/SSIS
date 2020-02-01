using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class StockRecordDAO
    {
        private readonly SSISContext context;

        public StockRecordDAO()
        {
            this.context = new SSISContext();
        }

        internal void StockAdjustmentDuringDisbursement(int qtyDisbursed, DisbursementItem di, int idStoreClerk)
        {
            // 1. Raise SA for damaged good for approval
            StockRecord raiseSA = new StockRecord
            {
                Date = DateTime.Now,
                IdOperation = 5,
                IdDepartment = di.Disbursement.CodeDepartment,
                IdStoreClerk = idStoreClerk,
                IdItem = di.IdItem,
                Unit = (di.UnitIssued - qtyDisbursed) * -1
            };
            context.StockRecords.Add(raiseSA);

            // 2. Create reversal StockRecord transaction for unit issued to the department
            StockRecord reverseSr = new StockRecord
            {
                Date = DateTime.Now,
                IdOperation = 1,
                IdDepartment = di.Disbursement.CodeDepartment,
                IdStoreClerk = idStoreClerk,
                IdItem = di.IdItem,
                Unit = di.UnitIssued - qtyDisbursed
            };
            context.StockRecords.Add(reverseSr);

            context.SaveChanges();
        }
    }
}