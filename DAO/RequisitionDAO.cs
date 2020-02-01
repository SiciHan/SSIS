using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class RequisitionDAO
    {
        private readonly SSISContext context;

        public RequisitionDAO()
        {
            this.context = new SSISContext();
        }

        public List<Retrieval> RetrieveRequisition(List<string> DClerk, DateTime StartDate, DateTime EndDate)
        {
            // Get Employee That is working in Department from DClerk
            List<int> IdEmployee = new List<int>();
            foreach (string CodeDpt in DClerk)
            {
                var IdEmp = context.Employees
                                    .Where(x => x.CodeDepartment.Equals(CodeDpt))
                                    .Select(x => x.IdEmployee);
                if (IdEmp != null)
                {
                    foreach (var ie in IdEmp)
                        IdEmployee.Add(ie);
                }
            }

            // Get approved requisition between startdate and enddate
            // Ignore employee for the moment
            List<int> IdRequestedItem = new List<int>();
            foreach (int ie in IdEmployee) 
            {
                var IdReqItem = context.Requisitions
                                    .Where(x => x.IdStatusCurrent == 3)
                                    .Where(x => x.ApprovedDate <= EndDate)
                                    .Where(x => x.ApprovedDate >= StartDate)
                                    .Where(x => x.IdEmployee == ie)
                                    .Select(x => x.IdRequisition);
                if (IdReqItem != null)
                {
                    foreach (var iri in IdReqItem)
                        IdRequestedItem.Add(iri);
                }
                
            }
            Debug.WriteLine(IdRequestedItem);

            // Get DepartmentCode from approved requistion Id and where id Employee equals to IdEmplloyee

            // Get Retrieval Form 
            List<Retrieval> RetrievalItem = new List<Retrieval>();
            foreach (var sr in IdRequestedItem)
            {
                var retrieval = context.Items
                                        .Join(context.RequisitionItems,
                                        items => items.IdItem, ri => ri.IdItem,
                                        (items, ri) => new { items, ri })
                                        .Join(context.Requisitions,
                                        r => r.ri.IdRequisiton, re => re.IdRequisition,
                                        (r, re) => new { r, re })
                                        .Join(context.Employees,
                                        e => e.re.IdEmployee, emp => emp.IdEmployee,
                                        (e, emp) => new Retrieval
                                        {
                                            Description = e.r.items.Description,
                                            IdItem = e.r.items.IdItem,
                                            StockUnit = e.r.items.StockUnit,
                                            Unit = e.r.ri.Unit,
                                            CodeDepartment = emp.CodeDepartment,
                                            IdRequisition = e.re.IdRequisition
                                        }).Where(x => x.IdRequisition == sr);
                if (retrieval != null)
                {
                    foreach (var r in retrieval)
                        RetrievalItem.Add(r);
                }
                
            }

            return RetrievalItem;

        }
    }
}