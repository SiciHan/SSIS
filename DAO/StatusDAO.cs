//@SHutong
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
 
    public class StatusDAO
    {
        private readonly SSISContext context;

        public StatusDAO()
        {
            this.context = new SSISContext();
        }

        public Status FindIncompleteStatus()
        {
            
            Status status= context.Status.OfType<Status>().Where(x => x.Label.Equals("Incomplete")).FirstOrDefault();
            return status;
        }
    }
}