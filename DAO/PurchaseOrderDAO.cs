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
        public List<PurchaseOrder> FindIncompletePO()
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>().
                Where(x => x.Status.Label.Equals("Incomplete")).
                Include(x=>x.Supplier).
                Include(x=>x.PurchaseOrderDetails).
                Include(x=>x.PurchaseOrderDetails.Select(c=>c.Item)).
                ToList<PurchaseOrder>();
        }

        public List<int> FindIdOfAllIncompletePO()
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>().
                Where(x => x.Status.Label.Equals("Incomplete")).
                Select(x=>x.IdPurchaseOrder).
                ToList<int>();
        }

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

        public List<PurchaseOrder> FindAllPO()
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>().ToList<PurchaseOrder>();
        }

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

        public bool IsIncompletePOExist(string codeSupplier)
        {
           
            if (FindIncompletePOWithSupplier(codeSupplier) == null)
            {
                return false;
            }
            return true;
        }

        public PurchaseOrder FindIncompletePOWithSupplier(string codeSupplier)
        {
            SSISContext context = new SSISContext();
            PurchaseOrder po=context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.Status.Label.Equals("Incomplete") && x.IdSupplier.Equals(codeSupplier)).Include(b => b.Supplier).Include(b=>b.StoreClerk).FirstOrDefault();
            context.Dispose();
            return po;
        }

        public List<Supplier> FindSuppliersFromIncompletePOCart()
        {
            List<Supplier> suppliers = new List<Supplier>();
            suppliers = context.PurchaseOrders.OfType<PurchaseOrder>().
                Where(x => x.Status.Label.Equals("Incomplete")).
                Select(x =>x.Supplier).Distinct().Include(x => x.PurchaseOrders)
                .Include(x => x.PurchaseOrders.Select(c => c.PurchaseOrderDetails))
                .Include(x => x.PurchaseOrders.Select(c => c.PurchaseOrderDetails.Select(p=>p.Item)))
                .ToList<Supplier>();
            return suppliers;
        }

        public PurchaseOrder UpdateStatusToPending(int purchaseOrderID)
        {
            PurchaseOrder po=context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.IdPurchaseOrder == purchaseOrderID).Include(c=>c.Status).FirstOrDefault();
            po.Status = context.Status.OfType<Status>().Where(x => x.Label.Equals("Pending")).FirstOrDefault();
            po.OrderDate = DateTime.Now;
            context.SaveChanges();
            return po;
        }

        public PurchaseOrder UpdateStatusToIncomplete(int purchaseOrderID)
        {
            PurchaseOrder po = context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.IdPurchaseOrder == purchaseOrderID).Include(c => c.Status).FirstOrDefault();
            po.Status = context.Status.OfType<Status>().Where(x => x.Label.Equals("Incomplete")).FirstOrDefault();
            po.OrderDate = DateTime.Parse("01/01/1900");
            context.SaveChanges();
            return po;
        }

        public PurchaseOrder UpdateStatusToCancelled(int purchaseOrderID)
        {
            PurchaseOrder po = context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.IdPurchaseOrder == purchaseOrderID).Include(c => c.Status).FirstOrDefault();
            po.Status = context.Status.OfType<Status>().Where(x => x.Label.Equals("Cancelled")).FirstOrDefault();            
            context.SaveChanges();
            return po;
        }
    }
}