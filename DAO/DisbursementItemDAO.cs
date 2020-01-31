using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team8ADProjectSSIS.DAO
{
    public class DisbursementItemDAO
    {
        private readonly SSISContext context;

        public DisbursementItemDAO()
        {
            this.context = new SSISContext();
        }

        public void CreateDisbursementItem()
        { 
        }
    }
}