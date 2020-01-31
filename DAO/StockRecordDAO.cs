using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team8ADProjectSSIS.DAO
{
    public class StockRecordDAO
    {
        private readonly SSISContext context;

        public StockRecordDAO()
        {
            this.context = new SSISContext();
        }
    }
}