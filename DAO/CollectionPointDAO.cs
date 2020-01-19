using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class CollectionPointDAO
    {
        public void Update(CollectionPoint cp)
        {
            using (SSISContext context = new SSISContext())
            {
                CollectionPoint collectionPoint = context.CollectionPoints.OfType<CollectionPoint>().Where(x => x.IdCollectionPt == cp.IdCollectionPt).FirstOrDefault();
                collectionPoint = new CollectionPoint(cp);
                context.SaveChanges();

            }
        }
    }
}