using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public interface ICategoryDAO
    {
        IEnumerable<Category> FindAllCategories();
        Category Create(Category category);
        Category Update(Category category);
        Category Delete (int id);
    }
}