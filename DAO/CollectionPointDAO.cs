using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        //@Shutong
        public List<CollectionPoint> FindAll()
        {
      
                return context.CollectionPoints.OfType<CollectionPoint>().ToList<CollectionPoint>();
            
        }

        public CollectionPoint Find(int id)
        {
            using (SSISContext context = new SSISContext())
            {
                return context.CollectionPoints.OfType<CollectionPoint>().Where(x=>x.IdCollectionPt == id).FirstOrDefault();
            }
        }

        public void Delete(CollectionPoint cp)
        {

                CollectionPoint collectionPoint = context.CollectionPoints.OfType<CollectionPoint>().Where(x => x.IdCollectionPt == cp.IdCollectionPt).FirstOrDefault();
                context.CollectionPoints.Remove(collectionPoint);
                context.SaveChanges();
            
        }

        public List<int> FindByClerkId(int clerkId)
        {
            List<int> CPs = new List<int>();
            CPs = context.CPClerks.Include("CollectionPoint")
                .Where(x => x.IdStoreClerk == clerkId)
                .Select(x => x.CollectionPoint.IdCollectionPt)
                .ToList();
            return CPs;
        }


        //@Shutong
        internal Employee FindClerkByCollectionPointId(int idCollectionPt)
        {
            return context.CPClerks.Where(x => x.IdCollectionPt == idCollectionPt)
                .Select(x =>x.StoreClerk)
                .FirstOrDefault();//assume only one clerk handle the cp.
        }

        public void ChangeCPTo(int ClerkId, List<int> new_IdCPs)
        {
            List<int> IdTables = context.CPClerks
                .Where(x => x.IdStoreClerk == ClerkId)
                .Select(x => x.IdCA)
                .ToList();

            int id = IdTables[0];
            CollectionPoint cp = context.CPClerks
                .Include("CollectionPoint")
                .Where(x => x.IdStoreClerk == ClerkId)
                .Select(x => x.CollectionPoint)
                .FirstOrDefault();
            foreach (int new_id in new_IdCPs)
            {
                CPClerk cPClerk = context.CPClerks
                    .Where(x => x.IdCA == id)
                    .FirstOrDefault();
                cPClerk.IdCollectionPt = new_id;
                context.SaveChanges();
                id++;
            }
            //context.SaveChanges();

        }

        //@Shutong
        internal string FindByDepartment(string codeDepartment)
        {
            Department d = context.Departments.Where(x => x.CodeDepartment.Equals(codeDepartment)).Include(x=>x.CollectionPt).FirstOrDefault();
            return d.CollectionPt.Location;
        }
    }
}