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

        public Boolean CheckExistDisbursement(int IdStoreClerk, DateTime Today, DateTime LastThu) 
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
                foreach (var cd in CodeDepartment)
                {
                    DClerk.Add(cd);
                }
            }
            Debug.WriteLine(DClerk);

            // Check for IdDisbursement that have DClerk and status "preparing"
            List<int> IdDisbursementClerk = new List<int>();
            foreach (string CodeDpt in DClerk)
            {
                var IdDisbursement = context.Disbursements
                                    .Where(x => x.CodeDepartment.Equals(CodeDpt))
                                    .Where(x => x.Date <= Today)
                                    .Where(x => x.Date >= LastThu)
                                    .Where(x => x.IdStatus.Equals(8))
                                    .Select(x => x.IdDisbursement);
                foreach (var idD in IdDisbursement)
                    if (idD != 0)
                        IdDisbursementClerk.Add(idD);
            }
            Debug.WriteLine(IdDisbursementClerk);

            if (IdDisbursementClerk.Any())
                return true;
            return false;
        }

        public void CreateDisbursement()
        {
            //context.Disbursements.Add();
        }

    }

}