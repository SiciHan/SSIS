using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class CategoryDAO
    {
        private readonly SSISContext context;

        public CategoryDAO()
        {
            this.context=new SSISContext();
        }

/*        public CategoryDAO(SSISContext context)
        {
            this.context = context;
        }*/

        public Category Create(Category category)
        {
            context.Categories.Add(category);
            context.SaveChanges();
            return category;
            
        }

        public Category Delete(int id)
        {
            Category category = (Category)context.Categories.Find(id);
            if (category != null)
            {
                context.Categories.Remove(category);
            }
            context.SaveChanges();
            return category;

        }

        public IEnumerable<Category> FindAllCategories()
        {
            throw new NotImplementedException();
        }

        public Category Update(Category category)
        {
            throw new NotImplementedException();
        }
    }
}