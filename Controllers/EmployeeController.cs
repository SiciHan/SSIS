using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;
using Team8ADProjectSSIS.Filters;

//@phyu
namespace Team8ADProjectSSIS.Controllers
{
    [AuthenticateFilter]
    [AuthorizeFilter]
    public class EmployeeController : Controller
    {
        public static string connectionString = "Server=.;" +
              "Database=SSIS; Integrated Security=true;MultipleActiveResultSets=True ";
        // GET: Employee





        public ActionResult Index(string cmd, int? id, string searchStr = " ")
        {
            if (Session["IdEmployee"] == null)
                return RedirectToAction("Login", "Home");

            int idEmployee = (int)Session["IdEmployee"];

            ViewBag.items = ListProducts(searchStr);
            ViewBag.searchStr = searchStr;

            return View();
        }

        public ActionResult Catalog(string cmd, int? id, string searchStr = " ")
        {
            if (Session["IdEmployee"] == null)
                return RedirectToAction("Login", "Home");

            int idEmployee = (int)Session["IdEmployee"];

            ViewBag.items = ListProducts(searchStr);
            ViewBag.searchStr = searchStr;

            return View();
        }

        public ActionResult PopUp(string cmd, int? id, string searchStr = " ")
        {

            if (Session["IdEmployee"] == null)
                return RedirectToAction("Login", "Home");

            int idEmployee = (int)Session["IdEmployee"];

            ViewBag.items = ListProducts(searchStr);
            ViewBag.searchStr = searchStr;

            return View();
        }

        public JsonResult reqId(string username)
        {

         //   if (Session["IdEmployee"] == null)
            //    return RedirectToAction("Login", "Home");

            int idEmployee = (int)Session["IdEmployee"];

            // int idEmployee = 2;



            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Random rand = new Random();
                int orderNum = rand.Next(10000000, 100000000);

                string sql = @"INSERT INTO Requisitions(IdStatusCurrent,RaiseDate,HeadRemark,ApprovedDate,WithdrawlDate,IdEmployee)
                                    VALUES(1,GETDATE(),null,GETDATE(),GETDATE()," + idEmployee + ")";

                SqlCommand cmdd = new SqlCommand(sql, conn);

                cmdd.ExecuteNonQuery();
            }

            return Json(new
            {
                result = "OK"
            });

        }


        public JsonResult OnJSON(string username, string itemName, int quantity)
        {
           
            int length = itemName.Length;

            if (itemName.EndsWith("\""))
            {
                itemName.Remove(length - 1);
                itemName.Remove(length - 2);
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Random rand = new Random();
                int orderNum = rand.Next(10000000, 100000000);

                string details = @"INSERT INTO RequisitionItems(IdRequisiton,IdItem,Unit)
                    VALUES( (SELECT MAX([IdRequisition]) from Requisitions ),
                    (SELECT IdItem from Items WHERE Description='" + itemName + "')," + quantity + ")";

                SqlCommand cmddd = new SqlCommand(details, conn);

                cmddd.ExecuteNonQuery();
            }

            return Json(new
            {
                result = "OK"
            });
        }

        public JsonResult updateReq(string username,int? selectedId,string itemName, int? quantity)
        {
            int length = itemName.Length;

            if (itemName.EndsWith("\""))
            {
                itemName.Remove(length - 1);
                itemName.Remove(length - 2);
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Random rand = new Random();
                int orderNum = rand.Next(10000000, 100000000);

                string details = @"Update RequisitionItems SET Unit = " + quantity  + " WHERE IdItem = (SELECT IdItem from Items WHERE Description ='"
                                  + itemName + "') AND IdRequisiton = " + selectedId + "";


                SqlCommand cmddd = new SqlCommand(details, conn);

                cmddd.ExecuteNonQuery();
            }

            return Json(new
            {
                result = "OK"
            });
        }


        public JsonResult insertReq(string username, int? selectedId, string itemName, int? quantity)
        {
            int length = itemName.Length; 

            if (itemName.EndsWith("\""))
            {
                itemName.Remove(length-1);
                itemName.Remove(length - 2);
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Random rand = new Random();
                int orderNum = rand.Next(10000000, 100000000);

                string details = @"INSERT INTO RequisitionItems VALUES (" + selectedId + ",(SELECT IdItem from Items WHERE Description ='"
                                  + itemName + "'),"+ quantity + ")";


                SqlCommand cmddd = new SqlCommand(details, conn);

                cmddd.ExecuteNonQuery();
            }

            return Json(new
            {
                result = "OK"
            });
        }

        public JsonResult deleteReqItem(string username, int? selectedId, string itemName)
        {
            int length = itemName.Length;

            if (itemName.EndsWith("\""))
            {
                itemName.Remove(length - 1);
                itemName.Remove(length - 2);
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Random rand = new Random();
                int orderNum = rand.Next(10000000, 100000000);

                string details = @"DELETE from RequisitionItems WHERE IdRequisiton = " + selectedId + "AND IdItem =(SELECT IdItem from Items WHERE Description = '" +
                                   itemName + "')";


                SqlCommand cmddd = new SqlCommand(details, conn);

                cmddd.ExecuteNonQuery();
            }

            return Json(new
            {
                result = "OK"
            });
        }




        public JsonResult deleteReq(string username, int? selectedId)
        {

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();


                string RequisitionItems = @"DELETE from RequisitionItems WHERE IdRequisiton =" + selectedId + "";

                SqlCommand reqItems = new SqlCommand(RequisitionItems, conn);

                reqItems.ExecuteNonQuery();
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();


                string Requisitions = @"DELETE from Requisitions WHERE IdRequisition = " + selectedId + "";

                SqlCommand reqId = new SqlCommand(Requisitions, conn);

                reqId.ExecuteNonQuery();
            }

            return Json(new
            {
                result = "OK"
            });
        }








        public List<RequisitionItem> ListReqItems(int ReqID)
        {
            List<RequisitionItem> items = new List<RequisitionItem>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                conn.Open();

                string ch = @"SELECT * from RequisitionItems WHERE IdRequisiton =" + ReqID + "";

                SqlCommand chh = new SqlCommand(ch, conn);

                SqlDataReader readerr = chh.ExecuteReader();
                while (readerr.Read())
                {
                    RequisitionItem req = new RequisitionItem()
                    {
                        IdReqItem = (int)readerr["IdReqItem"],

                        IdItem = (int)readerr["IdItem"],

                        Unit = (int)readerr["Unit"],


                    };
                    items.Add(req);
                };
            }
            return items;
        }

        public String GetIdStatus(int ReqID)
        {


            string status = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                conn.Open();


                string ch = @"SELECT * from Requisitions INNER JOIN Status ON Requisitions.IdStatusCurrent = Status.IdStatus
                                WHERE Requisitions.IdRequisition =" + ReqID + "";

                SqlCommand chh = new SqlCommand(ch, conn);

                SqlDataReader readerr = chh.ExecuteReader();
                while (readerr.Read())
                {

                    status = (string)readerr["Label"];

                };
            }
            return status;
        }

        public List<String> GetDescription(int ReqID)
        {
            List<String> description = new List<String>();

            string des = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                conn.Open();


                string ch = @"SELECT * from RequisitionItems INNER JOIN Items ON Items.IdItem = RequisitionItems.IdItem
                               WHERE RequisitionItems.IdRequisiton = " + ReqID + "";

                SqlCommand chh = new SqlCommand(ch, conn);

                SqlDataReader readerr = chh.ExecuteReader();
                while (readerr.Read())
                {
                   
                    des = (string)readerr["Description"];
                  
                    description.Add(des);
                };
              
            }
            return description;
        }




        public Requisition GetRequisition(int idEmployee)
        {
            Requisition requi = new Requisition();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                conn.Open();


                string ch = @"SELECT * from Requisitions WHERE IdEmployee = " + idEmployee + "";

                SqlCommand chh = new SqlCommand(ch, conn);

                SqlDataReader readerr = chh.ExecuteReader();
                while (readerr.Read())
                {
                    requi = new Requisition()
                    { IdRequisition = (int)readerr["IdRequisition"],
                      IdStatusCurrent = (int)readerr["IdStatusCurrent"],
                      RaiseDate = (DateTime)readerr["RaiseDate"],
                      HeadRemark = (readerr["HeadRemark"] == DBNull.Value) ? string.Empty : (string)readerr["HeadRemark"],
                      ApprovedDate = (readerr["ApprovedDate"] == DBNull.Value) ? DateTime.MinValue : (DateTime)readerr["ApprovedDate"],

                      WithdrawlDate = (readerr["WithdrawlDate"] == DBNull.Value) ? DateTime.MinValue : (DateTime)readerr["WithdrawlDate"],
                    };

                };
            }
            return requi;
        }



        public List<Item> ListProducts(string searchStr)
        {
            List<Item> items = new List<Item>();

            using (var db = new SSISContext())
            {
                if (searchStr == null)
                {

                    items = db.Items.ToList();
                }
                else

                    items = db.Items.Where(ite => ite.Description.Contains(searchStr) || ite.Description.Contains(searchStr)).ToList();
            }

            Session["searchStr"] = searchStr;
            ViewBag.products = items;
            return items;
        }

        public JsonResult UpdatereqId(int? reqID)
        {
           //  if (Session["IdEmployee"] == null)
           //        return RedirectToAction("Login", "Home");

               int idEmployee = (int)Session["IdEmployee"];

           // int idEmployee = 2;

            int req = reqID.GetValueOrDefault();
            string status = "";
            List<String> des = new List<String>();
            Requisition requi = new Requisition();
            requi = GetRequisition(idEmployee);
            //  ViewBag.ReqItems = ListReqItems(reqID);
            string username = "Sam Worthington";

            status = GetIdStatus(req);
            des = GetDescription(req);
            List<Requisition> reqs = ListReqID(idEmployee);
            List<RequisitionItem> reqq = ListReqItems(req);
         

            ViewData["reqq"] = reqq;
            ViewData["reqs"] = reqs;
            ViewData["status"] = status;
          

            var result = new { Req= requi, Items = reqq, status = status, descrip = des };
            return Json(result, JsonRequestBehavior.AllowGet);

            //  return View();
        }

 



        public ActionResult Update(string username, string cmd, int? reqID)
        {

            if (Session["IdEmployee"] == null)
                return RedirectToAction("Login", "Home");

            int idEmployee = (int)Session["IdEmployee"];

          //  int idEmployee = 2;


            int req = reqID.GetValueOrDefault();
            username = "Sam Worthington";

            List<Requisition> reqs = ListReqID(idEmployee);
            List<RequisitionItem> reqq = ListReqItems(req);

            ViewBag.ReqItems = ListReqItems(req);
            ViewData["reqq"] = reqq;
            ViewData["reqs"] = reqs;

            if (cmd == "delete")
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                 
                    string RequisitionItems = @"DELETE from RequisitionItems WHERE IdRequisiton =" + reqID + "";

                    SqlCommand reqItems = new SqlCommand(RequisitionItems, conn);

                    reqItems.ExecuteNonQuery();
                }
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();


                    string Requisitions = @"DELETE from Requisitions WHERE IdRequisition = " + reqID + "";

                    SqlCommand reqId = new SqlCommand(Requisitions, conn);

                    reqId.ExecuteNonQuery();
                }
            }
            if (cmd == "update")
            {


            }
                return View();
        }

        public List<Requisition> ListReqID(int idEmployee)
        {
          
            List<Requisition> reqs = new List<Requisition>();


            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                conn.Open();

                string ch = @"SELECT * From Requisitions Where IdEmployee = " + idEmployee;
                               

                SqlCommand chh = new SqlCommand(ch, conn);

                SqlDataReader readerr = chh.ExecuteReader();
                while (readerr.Read())
                {
                    Requisition req = new Requisition()
                    {
                        IdRequisition = (int)readerr["IdRequisition"],
                        IdStatusCurrent = (int)readerr["IdStatusCurrent"],
                        RaiseDate = (DateTime)readerr["RaiseDate"],

                        HeadRemark = (readerr["HeadRemark"] == DBNull.Value) ? string.Empty : (string)readerr["HeadRemark"],
                        ApprovedDate = (readerr["ApprovedDate"] == DBNull.Value) ? DateTime.MinValue : (DateTime)readerr["ApprovedDate"],

                        WithdrawlDate = (readerr["WithdrawlDate"] == DBNull.Value) ? DateTime.MinValue : (DateTime)readerr["WithdrawlDate"],

                    };
                    reqs.Add(req);
                };
            }

            return reqs;
        }

        public ActionResult CheckStatus()
        {
            return View();
        }

        public ActionResult History()
        {
            return View();
        }

        public ActionResult CheckOut()
        {
            return View();
        }
    }
}