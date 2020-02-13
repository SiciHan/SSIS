using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class PurchaseOrderDAO
    {
        private readonly SSISContext context;

        public PurchaseOrderDAO()
        {
            this.context = new SSISContext();
        }

        //@Shutong
        public List<PurchaseOrder> FindIncompletePO()
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>().
                Where(x => x.Status.Label.Equals("Incomplete")).
                Include(x=>x.Supplier).
                Include(x=>x.PurchaseOrderDetails).
                Include(x=>x.PurchaseOrderDetails.Select(c=>c.Item)).
                ToList<PurchaseOrder>();
        }

        //@Shutong
        public List<int> FindIdOfAllIncompletePO()
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>().
                Where(x => x.Status.Label.Equals("Incomplete")).
                Select(x=>x.IdPurchaseOrder).
                ToList<int>();
        }
        //@Shutong
        public List<PurchaseOrder> FindPendingPO()
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>().
                Where(x => x.Status.Label.Equals("Pending")).
                Include(x => x.Supplier).
                Include(x => x.PurchaseOrderDetails).
                Include(x => x.PurchaseOrderDetails.Select(p => p.Item)).
                Include(x => x.StoreClerk).
                ToList<PurchaseOrder>();
        }
        //@Shutong
        public List<PurchaseOrder> FindRejectedPO()
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>().
    Where(x => x.Status.Label.Equals("Rejected")).
    Include(x => x.Supplier).
    Include(x => x.PurchaseOrderDetails).
    Include(x => x.PurchaseOrderDetails.Select(p => p.Item)).
    Include(x => x.StoreClerk).
    ToList<PurchaseOrder>();
        }
        //@Shutong
        public List<PurchaseOrder> FindApprovedPO()
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>().
        Where(x => x.Status.Label.Equals("Approved")).
        Include(x => x.Supplier).
        Include(x => x.PurchaseOrderDetails).
        Include(x => x.PurchaseOrderDetails.Select(p => p.Item)).
        Include(x => x.StoreClerk).
        ToList<PurchaseOrder>();
        }
        //@Shutong
        public List<PurchaseOrder> FindDeliveredPO()
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>().
     Where(x => x.Status.Label.Equals("Delivered")).
     Include(x => x.Supplier).
     Include(x => x.PurchaseOrderDetails).
     Include(x => x.PurchaseOrderDetails.Select(p => p.Item)).
     Include(x => x.StoreClerk).
     ToList<PurchaseOrder>();
        }
        //@Shutong
        public List<PurchaseOrder> FindCancelledPO()
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>().
                Where(x => x.Status.Label.Equals("Cancelled")).
                Include(x => x.Supplier).
                Include(x => x.PurchaseOrderDetails).
                Include(x => x.PurchaseOrderDetails.Select(p => p.Item)).
                Include(x => x.StoreClerk).
                ToList<PurchaseOrder>();
        }

        // Willis
        public List<PurchaseOrder> FindAllPO()
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>()
                                        .Include(po => po.Supplier)
                                        .Include(po => po.Status)
                                        .ToList();
        }
        //@Shutong
        public PurchaseOrder Create(string codeSupplier, int idStoreClerk)
        {
            SSISContext context = new SSISContext();
            PurchaseOrder purchaseOrder = new PurchaseOrder {
                StoreClerk = context.Employees.OfType<Employee>().Where(x => x.IdEmployee == idStoreClerk).FirstOrDefault(),
                Supplier = context.Suppliers.OfType<Supplier>().Where(x => x.CodeSupplier.Equals(codeSupplier)).FirstOrDefault(),
                Status = context.Status.OfType<Status>().Where(x => x.Label.Equals("Incomplete")).FirstOrDefault(),
                ApprovedDate = DateTime.Parse("01/01/1900"),
                DeliverDate = DateTime.Parse("01/01/1900"),
                OrderDate = DateTime.Parse("01/01/1900")
            };
            context.PurchaseOrders.Add(purchaseOrder);
            context.SaveChanges();
            context.Dispose();
            return purchaseOrder;
        }
        //@Shutong
        public bool IsIncompletePOExist(string codeSupplier)
        {
           
            if (FindIncompletePOWithSupplier(codeSupplier) == null)
            {
                return false;
            }
            return true;
        }
        //@Shutong
        public PurchaseOrder FindIncompletePOWithSupplier(string codeSupplier)
        {
            SSISContext context = new SSISContext();
            PurchaseOrder po=context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.Status.Label.Equals("Incomplete") && x.IdSupplier.Equals(codeSupplier)).Include(b => b.Supplier).Include(b=>b.StoreClerk).FirstOrDefault();
            context.Dispose();
            return po;
        }
        //@Shutong
        public List<Supplier> FindSuppliersFromIncompletePOCart()
        {
            List<Supplier> suppliers = new List<Supplier>();
            suppliers = context.PurchaseOrders.OfType<PurchaseOrder>().
                Where(x => x.Status.Label.Equals("Incomplete")).
                Select(x =>x.Supplier).Distinct().Include(x => x.PurchaseOrders)
                .Include(x => x.PurchaseOrders.Select(c => c.PurchaseOrderDetails))
                .Include(x => x.PurchaseOrders.Select(c => c.PurchaseOrderDetails.Select(p=>p.Item)))
                .ToList<Supplier>();

            foreach(Supplier s in suppliers)
            {
                List<PurchaseOrder> incompletePOs = s.PurchaseOrders.Where(x => x.IdStatus == 1).ToList();

                if (incompletePOs.Count() > 1)
                {
                    PurchaseOrder keptPO = incompletePOs.FirstOrDefault();
                    for(int i=1;i<incompletePOs.Count();i++)
                    {
                        foreach(PurchaseOrderDetail pod in incompletePOs[i].PurchaseOrderDetails)
                        {
                            pod.PurchaseOrder = keptPO;
                        }
                    }
                }
            }
            return suppliers;
        }
        //@Shutong
        public PurchaseOrder UpdateStatusToPending(int purchaseOrderID)
        {
            PurchaseOrder po=context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.IdPurchaseOrder == purchaseOrderID).Include(c=>c.Status).FirstOrDefault();
            po.Status = context.Status.OfType<Status>().Where(x => x.Label.Equals("Pending")).FirstOrDefault();
            po.OrderDate = DateTime.Now;
            context.SaveChanges();
            return po;
        }
        //@Shutong
        public PurchaseOrder UpdateStatusToIncomplete(int purchaseOrderID)
        {
            PurchaseOrder po = context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.IdPurchaseOrder == purchaseOrderID).Include(c => c.Status).FirstOrDefault();
            po.Status = context.Status.OfType<Status>().Where(x => x.Label.Equals("Incomplete")).FirstOrDefault();
            po.OrderDate = DateTime.Parse("01/01/1900");
            context.SaveChanges();
            return po;
        }
        //@Shutong
        public PurchaseOrder UpdateStatusToCancelled(int purchaseOrderID)
        {
            PurchaseOrder po = context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.IdPurchaseOrder == purchaseOrderID).Include(c => c.Status).FirstOrDefault();
            po.Status = context.Status.OfType<Status>().Where(x => x.Label.Equals("Cancelled")).FirstOrDefault();            
            context.SaveChanges();
            return po;
        }
        //@Shutong
        public PurchaseOrder UpdateStatusToDelivered(int purchaseOrderID)
        {
            //need to ask the user to input delivered amount and delivery Remarks
            PurchaseOrder po = context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.IdPurchaseOrder == purchaseOrderID).
                Include(c => c.Status).
                Include(c => c.Supplier).
                Include(c => c.StoreClerk).
                Include(c => c.PurchaseOrderDetails).
                Include(x => x.PurchaseOrderDetails.Select(c => c.Item)).
                FirstOrDefault();
            po.Status = context.Status.OfType<Status>().Where(x => x.Label.Equals("Delivered")).FirstOrDefault();
            po.DeliverDate = DateTime.Now;

            foreach(PurchaseOrderDetail pod in po.PurchaseOrderDetails)
            {
                //also need to create a stock record
                StockRecord stockRecord = new StockRecord();
                stockRecord.Supplier = po.Supplier;
                stockRecord.StoreClerk = po.StoreClerk;
                stockRecord.Operation = context.Operations.OfType<Operation>().Where(x => x.Label.Equals("Delivery")).FirstOrDefault();
                stockRecord.Date = DateTime.Now;
                stockRecord.Item = pod.Item;
                //add stockunit and available unit
                stockRecord.Item.StockUnit += pod.DeliveredUnit;
                stockRecord.Item.AvailableUnit += pod.DeliveredUnit;
                stockRecord.Unit = pod.DeliveredUnit;
                //add the stock record
                context.StockRecords.Add(stockRecord);
            }
            context.SaveChanges();
            return po;
        }
        //@Shutong
        public PurchaseOrder UpdateRejectedToIncomplete(int purchaseOrderID)
        {
            PurchaseOrder po = context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.IdPurchaseOrder == purchaseOrderID).Include(c => c.Status).FirstOrDefault();
            po.Status = context.Status.OfType<Status>().Where(x => x.Label.Equals("Incomplete")).FirstOrDefault();
            po.OrderDate = DateTime.Parse("01/01/1900");
            po.PurchaseRemarks = null;
            context.SaveChanges();
            return po;
        }
        //@SHutong
        public PurchaseOrder FindPOById(int id)
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.IdPurchaseOrder == id).Include(x => x.StoreClerk).Include(x => x.Supplier).FirstOrDefault();
        }
        //@Shutong
        public PurchaseOrder UpdateSchedule(int idPO, string deliverDate)
        {
            PurchaseOrder po = context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.IdPurchaseOrder == idPO).FirstOrDefault();
            po.DeliverDate = DateTime.Parse(deliverDate);
            context.SaveChanges();
            return po;
        }

        //Huang Yuzhe
        public List<PurchaseOrder> FindHandledPO()
        {
            List<PurchaseOrder> handledPO = context.PurchaseOrders
                .Where(x => x.Status.Label.Equals("Approved")
                    || x.Status.Label.Equals("Rejected")
                    || x.Status.Label.Equals("Delivered")
                )
                .Include(x => x.Supplier)
                .Include(x => x.PurchaseOrderDetails)
                .Include(x => x.PurchaseOrderDetails.Select(p => p.Item))
                .Include(x => x.StoreClerk)
                .Include(x => x.Status)
                .ToList();

            return handledPO;
        }

        //Huang Yuzhe
        public void UpdatePOToApproved(List<PurchaseOrder> POs)
        {
            foreach (PurchaseOrder po in POs)
            {
                PurchaseOrder temp = context.PurchaseOrders
                    .Where(x => x.IdPurchaseOrder == po.IdPurchaseOrder)
                    .FirstOrDefault();
                temp.IdStatus = 3;
                temp.ApprovedDate = DateTime.Now;
                context.SaveChanges();
            }
        }

        //Huang Yuzhe
        public void UpdatePOToRejected(List<PurchaseOrder> POs, string remarks)
        {
            foreach (PurchaseOrder po in POs)
            {
                PurchaseOrder temp = context.PurchaseOrders
                    .Where(x => x.IdPurchaseOrder == po.IdPurchaseOrder)
                    .FirstOrDefault();
                temp.IdStatus = 4;
                temp.PurchaseRemarks = remarks;
                context.SaveChanges();
            }
        }
    }
}