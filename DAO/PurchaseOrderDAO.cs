using System;
using System.Collections.Generic;
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
            return context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.Status.Label.Equals("Incomplete")).ToList<PurchaseOrder>();
        }

        public List<PurchaseOrder> FindPendingPO()
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.Status.Label.Equals("Pending")).ToList<PurchaseOrder>();
        }

        public List<PurchaseOrder> FindRejectedPO()
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.Status.Label.Equals("Rejected")).ToList<PurchaseOrder>();
        }

        public List<PurchaseOrder> FindApprovedPO()
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.Status.Label.Equals("Approved")).ToList<PurchaseOrder>();
        }

        public List<PurchaseOrder> FindDeliveredPO()
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.Status.Label.Equals("Delivered")).ToList<PurchaseOrder>();
        }

        public List<PurchaseOrder> FindCancelledPO()
        {
            return context.PurchaseOrders.OfType<PurchaseOrder>().Where(x => x.Status.Label.Equals("Cancelled")).ToList<PurchaseOrder>();
        }
    }
}