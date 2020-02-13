using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class PurchaseOrderDetailsDAO
    {
        private readonly SSISContext context;

        public PurchaseOrderDetailsDAO()
        {
            this.context = new SSISContext();
        }


        public PurchaseOrderDetail CreateOrAddAmount(Item i, PurchaseOrder po)
        {
            SSISContext context=new SSISContext();

            //item and po into pod

            PurchaseOrderDetail pod=context.PurchaseOrderDetails.OfType<PurchaseOrderDetail>().
                Where(x => x.Item.IdItem == i.IdItem && x.PurchaseOrder.IdPurchaseOrder == po.IdPurchaseOrder).FirstOrDefault();
            if (pod == null) {
                pod= new PurchaseOrderDetail
                {
                    IdItem=i.IdItem,
                    IdPurchaseOrder=po.IdPurchaseOrder,
                    OrderUnit = i.ReorderUnit,
                    DeliveredUnit = 0,
           
                };
                context.PurchaseOrderDetails.Add(pod);
                context.SaveChanges();
            }
            else
            {
                pod.OrderUnit += i.ReorderUnit;
                context.SaveChanges();
            }
            context.Dispose();
            return pod;
        }
        public List<PurchaseOrderDetail> FindDetailPO(int IdPurchaseOrder)
        {
            return context.PurchaseOrderDetails.OfType<PurchaseOrderDetail>().Include("Item")
                                               .Where(pod => pod.IdPurchaseOrder == IdPurchaseOrder)
                                               .ToList<PurchaseOrderDetail>();
        }

        //@Shutong
        public List<PurchaseOrderDetail> FindAllIncompletePODetails(List<PurchaseOrder> list)
        {
            List<PurchaseOrderDetail> res = new List<PurchaseOrderDetail>();
            foreach(PurchaseOrder po in list)
            {
                res.AddRange(po.PurchaseOrderDetails);
            }
            return res;
        }

        //@Shutong
        public PurchaseOrderDetail DeletePOD(int id)
        {
            PurchaseOrderDetail pod = context.PurchaseOrderDetails.OfType<PurchaseOrderDetail>().
                Where(x => x.IdPOD == id).FirstOrDefault();
            if (pod != null)
            {
                context.PurchaseOrderDetails.Remove(pod);
                context.SaveChanges();
            }
            //check if the purchaseOrder contains 0 POD
            PurchaseOrder purchaseOrder = context.PurchaseOrders.OfType<PurchaseOrder>()
                .Where(x => x.IdPurchaseOrder == pod.IdPurchaseOrder).FirstOrDefault();

            if(purchaseOrder!=null&& (purchaseOrder.PurchaseOrderDetails==null || !purchaseOrder.PurchaseOrderDetails.Any())) context.PurchaseOrders.Remove(purchaseOrder);
            context.SaveChanges();
            return pod;
        }
        //@Shutong
        public PurchaseOrderDetail UpdateOrderUnitById(int orderUnit, int idPOD)
        {
            PurchaseOrderDetail pod = context.PurchaseOrderDetails.OfType<PurchaseOrderDetail>().
                Where(x => x.IdPOD == idPOD).FirstOrDefault();
            pod.OrderUnit = orderUnit;
            context.SaveChanges();
            return pod;
        }

        //@Shutong
        public List<PurchaseOrderDetail> FindPODetailsByPOId(int id)
        {
            return context.PurchaseOrderDetails.OfType<PurchaseOrderDetail>().Where(x => x.PurchaseOrder.IdPurchaseOrder == id)
                .Include(p => p.Item).ToList();
        }

        //@Shutong
        public PurchaseOrderDetail UpdateDeliveredUnitAndRemarksById(int idPOD,int deliveredUnit,string Remarks)
        {
            PurchaseOrderDetail pod = context.PurchaseOrderDetails.OfType<PurchaseOrderDetail>().
                Where(x => x.IdPOD == idPOD).FirstOrDefault();
            pod.DeliveredUnit = deliveredUnit;
            pod.DeliveryRemark = Remarks;
            context.SaveChanges();
            return pod;
        }
    }
}