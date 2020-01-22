using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{

    public class SSISInitializer<T> :DropCreateDatabaseAlways<SSISContext>
    //public class SSISInitializer<T>: CreateDatabaseIfNotExists<SSISContext>
    {
        protected override void Seed(SSISContext context)
        {
            //more seeding here
            Category category = new Category();
            category.Label = "Ruler"; 
            context.Categories.Add(category);
            context.SaveChanges();
            base.Seed(context);
        }
    }
}
