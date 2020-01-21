using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class CategoryDAO
    {
        public void Update(Category c)
        {
            using (SSISContext context = new SSISContext())
            {
                 Category cat=context.Categories.OfType<Category>().Where(x=> x.IdCategory==c.IdCategory).FirstOrDefault<Category>();
                 cat.Label = c.Label;
                 context.SaveChanges();
            }
        }
        public void Create(Category cp)
        {
            using (SSISContext context = new SSISContext())
            {
                context.Categories.Add(cp);
                context.SaveChanges();
            }
        }

        public List< Category> FindAll()
        {
            using (SSISContext context = new SSISContext())
            {
                return context.Categories.OfType<Category>().ToList<Category>();
            }
        }

        public void Delete(Category cp)
        {
            using (SSISContext context = new SSISContext())
            {
                Category  Category = context.Categories.OfType<Category>().Where(x => x.IdCategory == cp.IdCategory).FirstOrDefault();
                context. Categories.Remove(Category);
                context.SaveChanges();
            }
        }

    }
}