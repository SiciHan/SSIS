using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{

    public class DisbursementDAO
    {
        private readonly SSISContext context;

        public DisbursementDAO()
        {
            this.context = new SSISContext();
        }

        public List<string> ReturnStoreClerkCP(int IdStoreClerk)
        {
            // Check IdStoreClerk selected collection point
            List<int> CPClerk = new List<int>();
            CPClerk = context.CPClerks
                      .Where(x => x.IdStoreClerk == IdStoreClerk)
                      .Select(x => x.IdCollectionPt).ToList();
            Debug.WriteLine(CPClerk);
            // Check Department that have selected the same collection point as the storeclerk
            List<String> DClerk = new List<String>();
            foreach (int CollectionPt in CPClerk)
            {
                var CodeDepartment = context.Departments
                                    .Where(x => x.IdCollectionPt == CollectionPt)
                                    .Select(x => x.CodeDepartment);
                if (CodeDepartment != null)
                {
                    foreach (var cd in CodeDepartment)
                        DClerk.Add(cd);
                }
            }
            return DClerk;
        }
        // Willis
        public Boolean CheckExistDisbursement(List<string> DClerk, DateTime Today, DateTime LastThu) 
        {
            // Check for IdDisbursement and DisbursementItem that have DClerk and status "preparing"
            List<int> IdDisbursementItemClerk = new List<int>();
            foreach (string CodeDpt in DClerk)
            {
                var IdDisbursementItem = context.Disbursements
                                                .Join(context.DisbursementItems,
                                                d => d.IdDisbursement, di => di.IdDisbursement,
                                                (d, di) => new { d, di })
                                                .Where(x => x.d.CodeDepartment.Equals(CodeDpt))
                                                .Where(x => x.d.Date <= Today)
                                                .Where(x => x.d.Date >= LastThu)
                                                .Where(x => x.di.IdStatus == 8)
                                                .Select(x => x.di.IdDisbursementItem);
                if (IdDisbursementItem != null) 
                {
                    foreach (var idDI in IdDisbursementItem)
                        IdDisbursementItemClerk.Add(idDI);
                }
            }

            if (IdDisbursementItemClerk.Any())
                return true;
            return false;
        }

        //Willis
        public List<int> CreateDisbursement(List<Retrieval> RetrievalItem)
        {
            
            List<int> PKDisbursement = new List<int>();
            // Get Department from RetrievalItem
            List<String> SelectedCodeDepartment = new List<String>();
            foreach (var ri in RetrievalItem)
            {
                if (!SelectedCodeDepartment.Any())
                    SelectedCodeDepartment.Add(ri.CodeDepartment);
                else if (!SelectedCodeDepartment.Contains(ri.CodeDepartment))
                    SelectedCodeDepartment.Add(ri.CodeDepartment);
            }

            foreach (var scd in SelectedCodeDepartment)
            {
                Department department = context.Departments
                                        .Where(d => d.CodeDepartment.Equals(scd))
                                        .FirstOrDefault();
                Status status = context.Status.Where(s => s.IdStatus == 8).FirstOrDefault();
                Disbursement NewDisbursement = new Disbursement();
                NewDisbursement.CodeDepartment = scd;
                NewDisbursement.Department = department;
                NewDisbursement.IdStatus = 8;
                NewDisbursement.Status = status;
                NewDisbursement.Date = DateTime.Now;
                context.Disbursements.Add(NewDisbursement);
                context.SaveChanges();

                // Get Id of Created Disbursement
                int pk = NewDisbursement.IdDisbursement;
                PKDisbursement.Add(pk);
            }

            return PKDisbursement;
        }

        //Willis
        public void UpdateDisbursement(List<int> IdDisbursement)
        {
            foreach (int id in IdDisbursement) 
            {
                Disbursement disbursement = context.Disbursements
                                                    .Where(d => d.IdDisbursement == id)
                                                    .FirstOrDefault();

                Department department = context.Departments
                                                .Where(dpt => dpt.CodeDepartment.Equals(disbursement.CodeDepartment))
                                                .FirstOrDefault();

                if (disbursement != null && disbursement.IdDisbursement != 9) 
                {
                    Status status = context.Status.Where(s => s.IdStatus == 9).FirstOrDefault();
                    disbursement.IdStatus = 9;//Prepared
                    disbursement.Status = status;
                    disbursement.IdCollectionPt = department.IdCollectionPt;
                    context.SaveChanges();
                }
            }
            
        }

        //Willis
        public List<Retrieval> RetrievePreparingItem(List<string> DClerk, DateTime Today, DateTime LastThu)
        {
            // Join DisbursementItem and Item Entity return status "preparing"
            List<Retrieval> PItem = context.Items
                                            .Join(context.DisbursementItems,
                                            i => i.IdItem, di => di.IdItem,
                                            (i, di) => new { i, di })
                                            .Join(context.Disbursements,
                                            idi => idi.di.IdDisbursement, d => d.IdDisbursement,
                                            (idi, d) => new {idi,d })
                                            .Where(x => x.d.Date <= Today)
                                            .Where(x => x.d.Date >= LastThu)
                                            .Where(x => x.idi.di.IdStatus == 8)
                                            .Select(x => new Retrieval {
                                                Description = x.idi.i.Description,
                                                IdItem = x.idi.i.IdItem,
                                                StockUnit = x.idi.i.StockUnit,
                                                Unit = x.idi.di.UnitRequested,
                                                CodeDepartment = x.d.CodeDepartment,
                                                IdStatus = x.idi.di.IdStatus,
                                                Location = x.idi.i.Location
                                            }).ToList();
            List<Retrieval> Preparingitem = new List<Retrieval>(); 
            foreach (string CodeDpt in DClerk)
            {
                var pi = PItem.Where(x => x.CodeDepartment.Equals(CodeDpt));
                foreach (var p in pi)
                    Preparingitem.Add(p);
            }
            List<Retrieval> RetrievalForm = Preparingitem.GroupBy(x => x.IdItem)
                                            .Select(y => new Retrieval
                                            {
                                                Description = y.First().Description,
                                                IdItem = y.First().IdItem,
                                                StockUnit = y.First().StockUnit,
                                                Unit = y.Sum(z => z.Unit),
                                                Location = y.First().Location
                                            }).ToList();

            return RetrievalForm;
        }

        internal List<Disbursement> GetReceivedAndDisbursedDisbursements(string codeDepartment, string searchContext)
        {
            List<Disbursement> model = new List<Disbursement>();
            using (SSISContext db = new SSISContext())
            {
                if (string.IsNullOrEmpty(searchContext))
                {
                    model = db.Disbursements
                        .Include("DisbursementItems.Item")
                        .Include("Department")
                        .Include("CollectedBy")
                        .Include("DisbursedBy")
                        .Include("CollectionPoint")
                        .Include("Status")
                        .Where(x => (x.IdStatus == 11||x.IdStatus==7) && x.CodeDepartment == codeDepartment)
                        .ToList();
                }
                else
                {
                    model = db.Disbursements
                        .Include("DisbursementItems.Item")
                        .Include("Department")
                        .Include("CollectedBy")
                        .Include("DisbursedBy")
                        .Include("CollectionPoint")
                        .Include("Status")
                        .Where(x => (x.IdStatus == 11 || x.IdStatus == 7) && x.CodeDepartment == codeDepartment &&
                        (x.Date.ToString().Contains(searchContext) || x.CollectionPoint.Location.ToString().Contains(searchContext) || x.DisbursedBy.Name.ToString().Contains(searchContext) || x.CollectedBy.Name.ToString().Contains(searchContext)))
                        .ToList();
                }

            }
            return model;

        }

        //Willis
        public List<Retrieval> CheckRetrievalFormExist(List<Retrieval> RetrievalItem, DateTime LastThu)
        {
            List<int> IdDisbursementItem = context.DisbursementItems
                                        .Where(di => di.Disbursement.Date > LastThu)
                                        .Select(di => di.IdDisbursementItem).ToList();

            List<int> ExistingIdRequisition = new List<int>();
            foreach (var id in IdDisbursementItem)
            {
                string codeDepartment = context.DisbursementItems
                                                .Where(di => di.IdDisbursementItem == id)
                                                .Select(di => di.Disbursement.CodeDepartment)
                                                .FirstOrDefault();

                var existing = context.RequisitionItems 
                                    .Join(context.Requisitions,
                                    ri => ri.IdRequisiton, r => r.IdRequisition,
                                    (ri, r) => new { ri, r })
                                    .Join(context.Items,
                                    rir => rir.ri.IdItem, i => i.IdItem,
                                    (rir, i) => new { rir, i })
                                    .Join(context.DisbursementItems,
                                    riri => riri.rir.ri.IdItem, di => di.IdItem,
                                    (riri, di) => new { riri, di })
                                    .Where(x => x.di.IdDisbursementItem == id)
                                    .Where(x => x.riri.rir.r.ApprovedDate > LastThu)
                                    .Where(x => x.riri.rir.r.Employee.CodeDepartment.Equals(codeDepartment))
                                    .Select(x => x.riri.rir.r.IdRequisition);

                if (existing != null)
                {
                    foreach (var e in existing)
                    { 
                        if (!ExistingIdRequisition.Any())
                            ExistingIdRequisition.Add(e);
                        else if (!ExistingIdRequisition.Contains(e))
                            ExistingIdRequisition.Add(e);
                    }
                }
            }

            List<Retrieval> NewRetrievalItem = new List<Retrieval>();
            List<Retrieval> ExistingRetrievalItem = new List<Retrieval>();
            if (ExistingIdRequisition.Any())
            {
                foreach (int eir in ExistingIdRequisition)
                {
                    var existing = RetrievalItem.Where(x => x.IdRequisition == eir);

                    foreach (var newri in existing)
                    {
                        ExistingRetrievalItem.Add(newri);
                    }
                }
                NewRetrievalItem = RetrievalItem.Except(ExistingRetrievalItem).ToList();
                return NewRetrievalItem;
            }
            
            return RetrievalItem;
        }

        public List<Disbursement> GetAllDisbursements()
        {
            List<Disbursement> models = new List<Disbursement>();
            using (SSISContext db = new SSISContext())
            {
                models = db.Disbursements
                    .Include("DisbursementItems.Item")
                    .Include("Department")
                    .Include("CollectedBy")
                    .Include("DisbursedBy")
                    .Include("CollectionPoint")
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
                    .Include("Department")
                    .Include("CollectedBy")
                    .Include("DisbursedBy")
                    .Include("CollectionPoint")
                    .Include("Status")
                    .Where(x=>x.IdDisbursement == id)
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
                    .Include("Department")
                    .Include("CollectedBy")
                    .Include("DisbursedBy")
                    .Include("CollectionPoint")
                    .Include("Status")
                    .Where(x => x.CodeDepartment == codeDepartment)
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
                    .Include("Department")
                    .Include("CollectedBy")
                    .Include("DisbursedBy")
                    .Include("CollectionPoint")                    
                    .Include("Status")
                    .Where(x => x.IdStatus == 10 && x.CodeDepartment.Equals(codeDepartment))
                    .FirstOrDefault();
            }
            return model;
        }

        public List<Disbursement> GetReceivedDisbursements(string codeDepartment, string searchContext = "")
        {
            List<Disbursement> model = new List<Disbursement>();
            using (SSISContext db = new SSISContext())
            {
                if (string.IsNullOrEmpty(searchContext))
                {
                    model = db.Disbursements
                        .Include("DisbursementItems.Item")
                        .Include("Department")
                        .Include("CollectedBy")
                        .Include("DisbursedBy")
                        .Include("CollectionPoint")
                        .Include("Status")
                        .Where(x => x.IdStatus == 11 && x.CodeDepartment == codeDepartment)
                        .ToList();
                }
                else
                {
                    model = db.Disbursements
                        .Include("DisbursementItems.Item")
                        .Include("Department")
                        .Include("CollectedBy")
                        .Include("DisbursedBy")
                        .Include("CollectionPoint")
                        .Include("Status")
                        .Where(x => x.IdStatus == 11 && x.CodeDepartment == codeDepartment && 
                        (x.Date.ToString().Contains(searchContext) || x.CollectionPoint.Location.ToString().Contains(searchContext) || x.DisbursedBy.Name.ToString().Contains(searchContext) || x.CollectedBy.Name.ToString().Contains(searchContext)))
                        .ToList();
                }

            }
            return model;
        }

        public bool AcknowledgeCollection(int idDisbursement, int IdCollectedBy)
        {
            using (SSISContext db = new SSISContext())
            {
                Disbursement disbursement = db.Disbursements.OfType<Disbursement>()
                    .Where(x => x.IdDisbursement == idDisbursement)
                    .FirstOrDefault();
                if (disbursement == null) return false;
                disbursement.IdStatus = 11;//Received
                disbursement.IdCollectedBy = IdCollectedBy;
                db.SaveChanges();
            }
            return true;
        }

        public bool UpdateCollectionPt(int idDisbursement, int idCollectionPt)
        {
            Disbursement model = null;
            using (SSISContext db = new SSISContext())
            {
                model = db.Disbursements.OfType<Disbursement>()
                    .Where(x => x.IdDisbursement == idDisbursement)
                    .FirstOrDefault();
                if (model == null) return false;
/*                CollectionPoint collectionPt = db.CollectionPoints
                   .Include("CPClerks")
                   .OfType<CollectionPoint>()
                   .Where(x => x.IdCollectionPt == idCollectionPt)
                   .FirstOrDefault();
                if (collectionPt == null) return false;*/
                model.IdCollectionPt = idCollectionPt;
                //model.IdDisbursedBy = collectionPt.CPClerks.FirstOrDefault().IdStoreClerk;
                db.SaveChanges();
            }
            return true;
        }

        //James
        public List<Disbursement> FindByStatus(String status, int IdStoreClerk)
        {
            // Check IdStoreClerk selected collection point
            List<int> CPClerk = new List<int>();
            CPClerk = context.CPClerks
                      .Where(x => x.IdStoreClerk == IdStoreClerk)
                      .Select(x => x.IdCollectionPt).ToList();

            var list = context.Disbursements
                .Where(x => x.Status.Label == status && CPClerk.Contains((int)x.IdCollectionPt))
                .ToList();

            return list;
        }

        //James
        public void UpdateStatus(IEnumerable<int> disbIdsToSchedule, int idStatus, DateTime SDate, int? IdStoreClerk)
        {
            try
            {
                context.Disbursements.Where(x => disbIdsToSchedule.Contains(x.IdDisbursement))
                    .ToList()
                    .ForEach(x => {
                        x.IdStatus = idStatus;
                        x.Date = SDate;
                        x.IdDisbursedBy = IdStoreClerk;
                        });
                context.SaveChanges();
            } catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
        }

        //James
        public Disbursement FindById(int disbId)
        {
            return context.Disbursements.OfType<Disbursement>().Where(x => x.IdDisbursement == disbId).Include(y => y.CollectionPoint).FirstOrDefault();
                
        }
    }
}