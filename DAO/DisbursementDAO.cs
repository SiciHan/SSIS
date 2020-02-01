using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class DisbursementDAO
    {
        public List<Disbursement> GetAllDisbursements()
        {
            List<Disbursement> models = new List<Disbursement>();
            using (SSISContext db = new SSISContext())
            {
                models = db.Disbursements
                    .Include("DisbursementItems.Item")
                    .Include("Department.CollectionPt.CPClerks.StoreClerk")
                    .Include("Status")
                    .ToList();
            }
            return models;
        }

        public Disbursement GetDisbursement(int id)
        {
            Disbursement model = new Disbursement();
            using (SSISContext db = new SSISContext())
            {
                model = db.Disbursements
                    .Include("DisbursementItems.Item")
                    .Include("Department.CollectionPt.CPClerks.StoreClerk")
                    .Include("Status")
                    .Where(x => x.IdDisbursement == id)               
                    .FirstOrDefault();
            }
            return model;
        }

        public List<Disbursement> GetDeptDisbursements(string codeDepartment)
        {
            List<Disbursement> models = new List<Disbursement>();
            using (SSISContext db = new SSISContext())
            {
                models = db.Disbursements
                    .Include("DisbursementItems.Item")
                    .Include("Department.CollectionPt.CPClerks.StoreClerk")
                    .Include("Status")
                    .Where(x=>x.CodeDepartment == codeDepartment)
                    .ToList();
            }
            return models;
        }

        public Disbursement GetScheduledDisbursement(string codeDepartment)
        {
            Disbursement model = new Disbursement();
            using (SSISContext db = new SSISContext())
            {
                model = db.Disbursements
                    .Include("DisbursementItems.Item")
                    .Include("Department.CollectionPt.CPClerks.StoreClerk")
                    .Include("Status")
                    .Where(x => x.IdStatus == 10 && x.CodeDepartment == codeDepartment)
                    .FirstOrDefault();
            }
            return model;
        }

        public List<Disbursement> GetReceivedDisbursements(string codeDepartment)
        {
            List<Disbursement> model = new List<Disbursement>();
            using (SSISContext db = new SSISContext())
            {
                model = db.Disbursements
                    .Include("DisbursementItems.Item")
                    .Include("Department.CollectionPt.CPClerks.StoreClerk")
                    .Include("Status")
                    .Where(x => x.IdStatus == 11 && x.CodeDepartment == codeDepartment)
                    .ToList();
            }
            return model;
        }

        public bool AcknowledgeCollection(int idDisbursement)
        {
            using (SSISContext db = new SSISContext())
            {
                Disbursement disbursement = db.Disbursements.OfType<Disbursement>()
                    .Where(x => x.IdDisbursement == idDisbursement)
                    .FirstOrDefault();
                if (disbursement == null) return false;
                List<DisbursementItem> disbursementItems = db.DisbursementItems.OfType<DisbursementItem>()
                   .Where(x => x.IdDisbursement == idDisbursement)
                   .ToList();
                disbursement.IdStatus = 11;
                foreach (DisbursementItem di in disbursementItems)
                    di.IdStatus = 11;
                db.SaveChanges();
            }
            return true;
        }
    }
}