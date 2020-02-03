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

        public void UpdateStockRecord(int IdStoreClerk, List<int> IdDisbursementItem)
        {
            foreach (int x in IdDisbursementItem)
            {
                DisbursementItem disbursementItem = context.DisbursementItems
                                                    .Where(di => di.IdDisbursementItem == x)
                                                    .FirstOrDefault();

                Operation operation = context.Operations.Where(o => o.IdOperation == 1).FirstOrDefault();
                Item item = context.Items.Where(i => i.IdItem == disbursementItem.IdItem).FirstOrDefault();
                Employee storeclerk = context.Employees.Where(e => e.IdEmployee == IdStoreClerk).FirstOrDefault();
                Disbursement disbursement = context.Disbursements
                                                    .Where(d => d.IdDisbursement == disbursementItem.IdDisbursement).FirstOrDefault();
                Department department = context.Departments
                                                .Where(dpt => dpt.CodeDepartment.Equals(disbursement.CodeDepartment)).FirstOrDefault();
                Supplier supplier = context.Suppliers.Where(s => s.CodeSupplier == item.CodeSupplier1).FirstOrDefault();

                StockRecord stockrecord = new StockRecord();
                stockrecord.Date = DateTime.Now;
                stockrecord.IdDepartment = disbursement.CodeDepartment;
                stockrecord.Department = department;
                stockrecord.IdOperation = 1;
                stockrecord.Operation = operation;
                stockrecord.IdItem = disbursementItem.IdItem;
                stockrecord.Item = item;
                stockrecord.IdSupplier = item.CodeSupplier1;
                stockrecord.Supplier = supplier;
                stockrecord.IdStoreClerk = IdStoreClerk;
                stockrecord.StoreClerk = storeclerk;
                stockrecord.Unit = disbursementItem.UnitIssued;
                context.StockRecords.Add(stockrecord);
                context.SaveChanges();

            }
        }
    }
}