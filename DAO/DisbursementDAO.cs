using System;
using System.Collections.Generic;
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

        public void UpdateDisbursement(List<int> IdDisbursement)
        {
            foreach (int id in IdDisbursement) 
            {
                Disbursement disbursement = context.Disbursements
                                                    .Where(d => d.IdDisbursement == id)
                                                    .FirstOrDefault();
                if (disbursement != null && disbursement.IdDisbursement != 9) 
                {
                    Status status = context.Status.Where(s => s.IdStatus == 9).FirstOrDefault();
                    disbursement.IdStatus = 9;
                    disbursement.Status = status;
                    context.SaveChanges();
                }
            }
            
        }

        public List<Retrieval> RetrievePreparingItem(List<string> DClerk)
        {
            // Join DisbursementItem and Item Entity return status "preparing"
            List<Retrieval> PItem = context.Items
                                            .Join(context.DisbursementItems,
                                            i => i.IdItem, di => di.IdItem,
                                            (i, di) => new { i, di })
                                            .Join(context.Disbursements,
                                            d => d.di.IdDisbursement, dd => dd.IdDisbursement,
                                            (d, dd) => new Retrieval
                                            {
                                                Description = d.i.Description,
                                                IdItem = d.i.IdItem,
                                                StockUnit = d.i.StockUnit,
                                                Unit = d.di.UnitRequested,
                                                CodeDepartment = dd.CodeDepartment,
                                                IdStatus = d.di.IdStatus
                                            }).Where(x => x.IdStatus == 8).ToList();
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
                                                Unit = y.Sum(z => z.Unit)
                                            }).ToList();

            return RetrievalForm;
        }

        public List<Retrieval> CheckRetrievalFormExist(List<Retrieval> RetrievalItem)
        {
            List<int> IdDisbursementItem = context.DisbursementItems
                                        .Select(x => x.IdDisbursementItem).ToList();
            List<int> ExistingIdRequisition = new List<int>();
            foreach (var id in IdDisbursementItem)
            {
                var existing = context.RequisitionItems
                            .Join(context.Requisitions,
                            ri => ri.IdRequisiton, r => r.IdRequisition,
                            (ri, r) => new { ri, r })
                            .Join(context.Items,
                            i => i.ri.IdItem, ir => ir.IdItem,
                            (i, ir) => new { i, ir })
                            .Join(context.DisbursementItems,
                            d => d.ir.IdItem, dir => dir.IdItem,
                            (d, dir) => new { d, dir })
                            .Where(x => x.dir.IdDisbursementItem == id)
                            .Select(x => x.d.i.r.IdRequisition);

                if (existing != null)
                {
                    foreach (var e in existing)
                        ExistingIdRequisition.Add(e);
                }
                
            }
            List<Retrieval> NewRetrievalItem = new List<Retrieval>();

            if (ExistingIdRequisition != null)
            {
                foreach (int eir in ExistingIdRequisition)
                {
                    var newreqitem = RetrievalItem.Where(x => x.IdRequisition != eir);

                    foreach (var newri in newreqitem)
                    {
                        NewRetrievalItem.Add(newri);
                    }
                }
                return NewRetrievalItem;
            }
            
            return RetrievalItem;
        }
        public List<JoinDandDI> FindAllDisbursement()
        {
            var JoinDanDi = context.Disbursements
                            .Join(context.DisbursementItems,
                            d => d.IdDisbursement, di => di.IdDisbursement,
                            (d, di) => new { d, di})
                            .Join(context.Departments,
                            ddi => ddi.d.CodeDepartment, dpt => dpt.CodeDepartment,
                            (ddi, dpt) => new { ddi, dpt})
                            .Join(context.Status,
                            ddidpt => ddidpt.ddi.d.IdStatus, s => s.IdStatus,
                            (ddidpt, s) => new JoinDandDI
                            {
                                disbursement = ddidpt.ddi.d,
                                disbursementItem = ddidpt.ddi.di,
                                department = ddidpt.dpt,
                                status = s
                            }).ToList();

            return JoinDanDi;
        }

    }

}