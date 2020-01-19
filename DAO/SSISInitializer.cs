using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Team8ADProjectSSIS.DAO
{
    public class SSISInitializer<T>: CreateDatabaseIfNotExists<SSISContext>
    {
        protected override void Seed(SSISContext context)
        {
            //more seeding here
            base.Seed(context);
        }
    }
}
