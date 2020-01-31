using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class CollectionPointDAO
        
    {
        private readonly SSISContext context;

        public CollectionPointDAO()
        {
            this.context = new SSISContext();
        }

        public void Update(CollectionPoint cp)
        {
           
                CollectionPoint collectionPoint = context.CollectionPoints.OfType<CollectionPoint>().Where(x => x.IdCollectionPt == cp.IdCollectionPt).FirstOrDefault();
                collectionPoint = new CollectionPoint(cp);
                context.SaveChanges();
                
            
        }
        public void Create(CollectionPoint cp)
        {
       
                context.CollectionPoints.Add(cp);
                context.SaveChanges();
            
        }

        public List<CollectionPoint> FindAll()
        {
      
                return context.CollectionPoints.OfType<CollectionPoint>().ToList<CollectionPoint>();
            
        }

        public void Delete(CollectionPoint cp)
        {

                CollectionPoint collectionPoint = context.CollectionPoints.OfType<CollectionPoint>().Where(x => x.IdCollectionPt == cp.IdCollectionPt).FirstOrDefault();
                context.CollectionPoints.Remove(collectionPoint);
                context.SaveChanges();
            
        }

    }
}