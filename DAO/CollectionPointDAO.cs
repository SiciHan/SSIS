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
        public void Create(CollectionPoint cp)
        {
            using (SSISContext context = new SSISContext())
            {
                context.CollectionPoints.Add(cp);
                context.SaveChanges();
            }
        }

        public List<CollectionPoint> FindAll()
        {
            using (SSISContext context = new SSISContext())
            {
                return context.CollectionPoints.OfType<CollectionPoint>().ToList<CollectionPoint>();
            }
        }

        public void Delete(CollectionPoint cp)
        {
            using (SSISContext context = new SSISContext())
            {
                CollectionPoint collectionPoint = context.CollectionPoints.OfType<CollectionPoint>().Where(x => x.IdCollectionPt == cp.IdCollectionPt).FirstOrDefault();
                context.CollectionPoints.Remove(collectionPoint);
                context.SaveChanges();
            }
        }

    }
}