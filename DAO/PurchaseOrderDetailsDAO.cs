using System;
using System.Collections.Generic;
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
        public List<PurchaseOrderDetail> FindDetailPO(int IdPurchaseOrder)
        {
            return context.PurchaseOrderDetails.OfType<PurchaseOrderDetail>()
                                               .Where(pod => pod.IdPurchaseOrder == IdPurchaseOrder)
                                               .ToList<PurchaseOrderDetail>();
        }
    }
}