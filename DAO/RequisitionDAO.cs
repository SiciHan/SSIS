﻿using System;
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
                                        i => i.IdItem, ri => ri.IdItem,
                                        (i, ri) => new { i, ri })
                                        .Join(context.Requisitions,
                                        iri => iri.ri.IdRequisiton, r => r.IdRequisition,
                                        (iri, r) => new { iri, r })
                                        .Join(context.Employees,
                                        riri => riri.r.IdEmployee, e => e.IdEmployee,
                                        (riri, e) => new Retrieval
                                        {
                                            Description = riri.iri.i.Description,
                                            IdItem = riri.iri.i.IdItem,
                                            StockUnit = riri.iri.i.StockUnit,
                                            Unit = riri.iri.ri.Unit,
                                            CodeDepartment = e.CodeDepartment,
                                            IdRequisition = riri.r.IdRequisition,
                                            ApprovedDate = riri.r.ApprovedDate
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