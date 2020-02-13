//@SHutong
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{

    //public class SSISInitializer<T> :DropCreateDatabaseIfModelChanges<SSISContext>
    public class SSISInitializer<T>: CreateDatabaseIfNotExists<SSISContext>
    {
        protected override void Seed(SSISContext context)
        {
            base.Seed(context);
        }
    }
}
