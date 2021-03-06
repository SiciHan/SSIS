﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;
using Team8ADProjectSSIS.Filters;
using Microsoft.AspNet.SignalR;
using Team8ADProjectSSIS.Hubs;
using Team8ADProjectSSIS.EmailModel;

//@phyu
namespace Team8ADProjectSSIS.Controllers
{
    [AuthenticateFilter]
    [AuthorizeFilter]
    [DelegationActionFilter]
    public class EmployeeController : Controller
    {

        private readonly EmployeeDAO _employeeDAO;
        private readonly RequisitionDAO _requisitionDAO;
        private readonly RequisitionItemDAO _requisitionItemDAO;
        private readonly NotificationChannelDAO _notificationChannelDAO;
        private readonly ItemDAO _itemDAO;
        public EmployeeController()
        {
            _employeeDAO = new EmployeeDAO();
            _requisitionDAO = new RequisitionDAO();
            _requisitionItemDAO = new RequisitionItemDAO();
            _notificationChannelDAO = new NotificationChannelDAO();
            _itemDAO = new ItemDAO();
        }
        public static string connectionString = "Server=.;" +
              "Database=SSIS; Integrated Security=true;MultipleActiveResultSets=True ";
        // GET: Employee

        public ActionResult Index(string cmd, int? id, string searchStr = " ")
        {
            if (Session["IdEmployee"] == null)
                return RedirectToAction("Login", "Home");

            int idEmployee = (int)Session["IdEmployee"];
            List<Item> items = new List<Item>();

            items = RecentProducts(idEmployee);

             
            ViewBag.items = ListProducts(searchStr);
            ViewBag.searchStr = searchStr;
            ViewBag.recentItems = items; 
            ViewData["Emp"] = _employeeDAO.FindEmployeeById(idEmployee);
            return View();
        }

        //@Shutong
        public ActionResult Notification()
        {


            int IdReceiver = (int)Session["IdEmployee"];

            ViewData["NCs"] = _notificationChannelDAO.FindAllNotificationsByIdReceiver(IdReceiver);

            return View();

        }

        public ActionResult Catalog(string cmd, int? id, string searchStr = " ")
        {
            if (Session["IdEmployee"] == null)
                return RedirectToAction("Login", "Home");

            List<Item> items = new List<Item>();

            int idEmployee = (int)Session["IdEmployee"];
            items = RecentProducts(idEmployee);

            ViewBag.items = ListProducts(searchStr);
            ViewBag.searchStr = searchStr;
            ViewData["Emp"] = _employeeDAO.FindEmployeeById(idEmployee);
            ViewBag.recentItems = items;

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


        public ActionResult CreatePopup(string cmd, int? id, string searchStr = " ")
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

            Requisition req=_requisitionDAO.CreateRequisition(idEmployee);

            //@Shutong: send notification here
            int IdHead = _employeeDAO.FindHeadIdByIdEmployee(idEmployee);
            int IdActingHead = _employeeDAO.FindActingHeadIdByIdEmployee(idEmployee);
            var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hub.Clients.All.receiveNotification(IdHead);
            EmailClass emailClass = new EmailClass();
            string message = "Hi Department Head," + _employeeDAO.FindEmployeeById(idEmployee).Name
                + "has raised a stationery requisition: " + req.IdRequisition + " on " + req.RaiseDate + ". Please kindly approve or reject it.";
            _notificationChannelDAO.CreateNotificationsToIndividual(IdHead, (int)Session["IdEmployee"], message);
            emailClass.SendTo(_employeeDAO.FindEmployeeById(IdHead).Email, "SSIS System Email", message);
            if (IdActingHead != 0)
            {
                message = "Hi Department Acting Head," + _employeeDAO.FindEmployeeById(idEmployee).Name
               + "has raised a stationery requisition: " + req.IdRequisition + " on " + req.RaiseDate + ". Please kindly approve or reject it.";
                hub.Clients.All.receiveNotification(IdActingHead);
                _notificationChannelDAO.CreateNotificationsToIndividual(IdActingHead, (int)Session["IdEmployee"], message);
                emailClass.SendTo(_employeeDAO.FindEmployeeById(IdActingHead).Email, "SSIS System Email", message);
            }

            message = "Hi "+_employeeDAO.FindEmployeeById(idEmployee).Name+ "you has raised a stationery requisition: " + req.IdRequisition + " on " + req.RaiseDate + ". Your Head/Acting Head will handle it in 5 working days.";
            hub.Clients.All.receiveNotification(idEmployee);
            _notificationChannelDAO.CreateNotificationsToIndividual(idEmployee, idEmployee, message);
            emailClass.SendTo(_employeeDAO.FindEmployeeById(idEmployee).Email, "SSIS System Email", message);
            //end of notification sending 

            //replace with below

            /*            using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();

                            Random rand = new Random();
                            int orderNum = rand.Next(10000000, 100000000);

                            string sql = @"INSERT INTO Requisitions(IdStatusCurrent,RaiseDate,HeadRemark,ApprovedDate,WithdrawlDate,IdEmployee)
                                                VALUES(1,GETDATE(),null,GETDATE(),GETDATE()," + idEmployee + ")";

                            SqlCommand cmdd = new SqlCommand(sql, conn);

                            cmdd.ExecuteNonQuery();
                        }*/

            return Json(new
            {
                result = "OK"
            });

        }

        public JsonResult searchReqHistory(string startDate,string endDate,string status)
        {

          
            List<Requisition> reqs = ListHistoryReqItems(startDate, endDate, status);       
            var result = new { Req = reqs };
            return Json(result, JsonRequestBehavior.AllowGet);

        }



        public JsonResult OnJSON(string username, string itemName, int quantity)
        {

            /*            int length = itemName.Length;

                        if (itemName.EndsWith("\""))
                        {
                            itemName.Remove(length - 1);
                            itemName.Remove(length - 2);
                        }
            */
            int idEmployee = (int)Session["IdEmployee"];
            //_requisitionItemDAO.CreateRequisitionItem(idEmployee,itemName, quantity);//add requisitonitem to incomplete requisition
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

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string details = @"Update Items SET AvailableUnit = (Select AvailableUnit from Items Where Description = '" + itemName +
                                   "') -" + quantity + "WHERE IdItem =(SELECT IdItem from Items Where Description = '" +
                                   itemName + "')";


                SqlCommand cmddd = new SqlCommand(details, conn);

                cmddd.ExecuteNonQuery();
            }


            return Json(new
            {
                result = "OK"
            });
        }

        public JsonResult updateReq(string username, int? selectedId, string itemName, int? quantity)
        {

            int idEmployee = (int)Session["IdEmployee"];


            int length = itemName.Length;

            RequisitionItem req = new RequisitionItem();

            if (itemName.EndsWith("\""))
            {
                itemName.Remove(length - 1);
                itemName.Remove(length - 2);
            }

            int quan= quantity.GetValueOrDefault();
            int selectId = selectedId.GetValueOrDefault();
            int raisedStock = 0; int diff = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Random rand = new Random();
                int orderNum = rand.Next(10000000, 100000000);

                string details = @"Update Requisitions SET IdStatusCurrent = 2 Where IdRequisition = " + selectedId + "";


                SqlCommand cmddd = new SqlCommand(details, conn);

                cmddd.ExecuteNonQuery();
            }
     

           req = GetRaiseUnit(selectId, itemName);

            Console.WriteLine(req.Unit);

            raisedStock = req.Unit;

            

            if (raisedStock > quantity)
            {
                diff = raisedStock - quan;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string details = @"Update Items SET AvailableUnit = (Select AvailableUnit from Items Where Description = '" + itemName +
                                       "') +" + diff + "WHERE IdItem =(SELECT IdItem from Items Where Description = '" +
                                       itemName + "')";


                    SqlCommand cmddd = new SqlCommand(details, con);

                    cmddd.ExecuteNonQuery();
                }

            }

            if (raisedStock < quantity)
            {
                diff = quan - raisedStock;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string details = @"Update Items SET AvailableUnit = (Select AvailableUnit from Items Where Description = '" + itemName +
                                       "') -" + diff + "WHERE IdItem =(SELECT IdItem from Items Where Description = '" +
                                       itemName + "')";


                    SqlCommand cmddd = new SqlCommand(details, con);

                    cmddd.ExecuteNonQuery();
                }

            }


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Random rand = new Random();
                int orderNum = rand.Next(10000000, 100000000);

                string details = @"Update RequisitionItems SET Unit = " + quantity + " WHERE IdItem = (SELECT IdItem from Items WHERE Description ='"
                                  + itemName + "') AND IdRequisiton = " + selectedId + "";


                SqlCommand cmddd = new SqlCommand(details, conn);

                cmddd.ExecuteNonQuery();
            }





            return Json(new
            {
                result = "OK"
            });
        }



        

        public JsonResult updateItemAvai(string username, int? selectedId, string itemName, int? quantity)
        {

            int idEmployee = (int)Session["IdEmployee"];
            //  _requisitionItemDAO.UpdateRequisitionItemUnit(selectedId, itemName, quantity);//add requisitonitem to incomplete requisition

            int length = itemName.Length;

            if (itemName.EndsWith("\""))
            {
                itemName.Remove(length - 1);
                itemName.Remove(length - 2);
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string details = @"Update Items SET AvailableUnit = (Select AvailableUnit from Items Where Description = '" + itemName +
                                   "') +" + quantity + "WHERE IdItem =(SELECT IdItem from Items Where Description = '" +
                                   itemName + "')";


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
            _requisitionItemDAO.CreateRequisitionItemByReqID(selectedId, itemName, quantity);


            return Json(new
            {
                result = "OK"
            });
        }

        public JsonResult deleteReqItem(string username, int? selectedId, string itemName,int itemQuantity)
        {

            //    _requisitionItemDAO.DeleteRequisitionItem(selectedId, itemName);
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

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string details = @"Update Items SET AvailableUnit = (Select AvailableUnit from Items Where Description = '" + itemName +
                                   "') +" + itemQuantity + "WHERE IdItem =(SELECT IdItem from Items Where Description = '" +
                                   itemName + "')";


                SqlCommand cmddd = new SqlCommand(details, conn);

                cmddd.ExecuteNonQuery();
            }

            //using (SqlConnection conn = new SqlConnection(connectionString))
            //{
            //    conn.Open();

            //    string details = @"Update Items SET AvailableUnit = (Select AvailableUnit from Items Where Description = '" + itemName +
            //                       "') +" + quantity + "WHERE IdItem =(SELECT IdItem from Items Where Description = '" +
            //                       itemName + "')";


            //    SqlCommand cmddd = new SqlCommand(details, conn);

            //    cmddd.ExecuteNonQuery();
            //}

            return Json(new
            {
                result = "OK"
            });
        }




       

        public JsonResult deleteReq(string username, int? selectedId)
        {

            //     _requisitionItemDAO.DeleteRequisitionItemByReqId(selectedId);
            //    _requisitionDAO.DeleteRequisition(selectedId);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();


                string RequisitionItems = @"UPDATE Requisitions SET IdStatusCurrent = 5 WHERE IdRequisition = " + selectedId + "";

                SqlCommand reqItems = new SqlCommand(RequisitionItems, conn);

                reqItems.ExecuteNonQuery();
            }




            return Json(new
            {
                result = "OK"
            });
        }


        public List<RequisitionItem> ListReqItems(int ReqID)
        {
            //List<RequisitionItem> items= _requisitionItemDAO.RetrieveRequisitionItemByReqId(ReqID);
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

       

      

        public RequisitionItem GetRaiseUnit(int ReqID, string itemName)
        {
            RequisitionItem requi = new RequisitionItem();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                conn.Open();


                string ch = @"SELECT * from RequisitionItems WHERE IdRequisiton = " + ReqID + 
                            "AND IdItem = (Select IdItem From Items Where Description = '"+
                             itemName+ "')";

                SqlCommand chh = new SqlCommand(ch, conn);

                SqlDataReader readerr = chh.ExecuteReader();
                while (readerr.Read())
                {
                    requi = new RequisitionItem()
                    {
                        IdReqItem = (int)readerr["IdReqItem"],
                        IdRequisiton = (int)readerr["IdRequisiton"],
                        IdItem = (int)readerr["IdItem"],
                        Unit = (int)readerr["Unit"],
                    };

                };
            }
            return requi;
        }



        public List<Requisition> ListHistoryReqItems(string startDate, string endDate, string status)
        {
             //if (Session["IdEmployee"] == null)
            //  return RedirectToAction("Login", "Home");

             int idEmployee = (int)Session["IdEmployee"];

            List<Requisition> items = new List<Requisition>();

            if (status == "All")
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();

                    string ch = @"SELECT IdRequisition,IdStatusCurrent,CAST (RaiseDate AS date)As RaiseDate,HeadRemark,CAST (ApprovedDate AS date) As ApprovedDate,CAST (WithdrawlDate AS date) As WithdrawlDate FROM Requisitions WHERE  LEFT (RaiseDate, 10) BETWEEN '" + startDate + "' AND '" + endDate + "' AND IdEmployee =" + idEmployee + "";

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
                        items.Add(req);
                    };
                }
            }
            else
            {

                int statusId = 0;

                switch (status)
                {
                    case "Incomplete":
                        statusId = 1;
                        break;
                    case "Pending":
                        statusId = 2;
                        break;
                    case "Approved":
                        statusId = 3;
                        break;
                    case "Rejected":
                        statusId = 4;
                        break;
                    case "Cancelled":
                        statusId = 5;
                        break;

                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();

                    string ch = @"SELECT IdRequisition,IdStatusCurrent,CAST (RaiseDate AS date)As RaiseDate,HeadRemark,CAST (ApprovedDate AS date) As ApprovedDate,CAST (WithdrawlDate AS date) As WithdrawlDate FROM Requisitions WHERE  LEFT (RaiseDate, 10) BETWEEN '" + startDate + "' AND '" + endDate + "' AND IdEmployee =" + idEmployee + "AND  IdStatusCurrent ="+ statusId + "";

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
                        items.Add(req);
                    };
                }


            }
            return items;
        }

        public String GetIdStatus(int ReqID)
        {

            //  return _requisitionDAO.GetStatusLabel(ReqID);
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


        
        
        public List<Item> GetItemDetails(int ReqID)
        {
            List<Item> ites = new List<Item>();
            //List<RequisitionItem> reqItemList = _requisitionItemDAO.RetrieveRequisitionItemByReqId(ReqID);

            //foreach (RequisitionItem ri in reqItemList)
            //{
            //    description.Add(ri.Item.Description);
            //}


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
                    Item ite = new Item()
                    {
                       Description = (string)readerr["Description"],
                       AvailableUnit = (int)readerr["AvailableUnit"]
                    };

                    ites.Add(ite);
                };

            }
            return ites;
        }




        public Requisition GetRequisition(int ReqID)
        {
            Requisition requi = new Requisition();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                conn.Open();


                string ch = @"SELECT * from Requisitions WHERE IdRequisition = " + ReqID + "";

                SqlCommand chh = new SqlCommand(ch, conn);

                SqlDataReader readerr = chh.ExecuteReader();
                while (readerr.Read())
                {
                    requi = new Requisition()
                    {
                        IdRequisition = (int)readerr["IdRequisition"],
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


        public List<Item> RecentProducts(int idEmployee)
        {
           // if (Session["IdEmployee"] == null)
              //  return RedirectToAction("Login", "Home");

           // int idEmployee = (int)Session["IdEmployee"];
            List<Item> items = new List<Item>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string ch = @"Select * from RequisitionItems Inner Join  Requisitions On RequisitionItems.IdRequisiton = Requisitions.IdRequisition
                              Inner Join Items On RequisitionItems.IdItem = Items.IdItem                                                  
                              Where RequisitionItems.IdRequisiton =(SELECT MAX([IdRequisition]) from Requisitions) And Requisitions.IdEmployee =" + idEmployee + "";


                SqlCommand chh = new SqlCommand(ch, conn);

                SqlDataReader readerr = chh.ExecuteReader();
                while (readerr.Read())
                {
                    Item it = new Item()
                    {
                     
                        Description = (String)readerr["Description"],
                        unitOfMeasure = (String)readerr["unitOfMeasure"],
                        AvailableUnit = (int)readerr["AvailableUnit"]

                    };
                    items.Add(it);
                };
            }

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
            List<Item> des = new List<Item>();
            Requisition requi = new Requisition();
            requi = GetRequisition(req);
            //  ViewBag.ReqItems = ListReqItems(reqID);
            //string username = "Sam Worthington";

            status = GetIdStatus(req);
            des = GetItemDetails(req);
            List<Requisition> reqs = ListReqID(idEmployee);
            List<RequisitionItem> reqq = ListReqItems(req);


            ViewData["reqq"] = reqq;
            ViewData["reqs"] = reqs;
            ViewData["status"] = status;


            var result = new { Req = requi, Items = reqq, status = status, descrip = des };
            return Json(result, JsonRequestBehavior.AllowGet);

            //  return View();
        }





        public ActionResult Update(string username, int? reqID)
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
   
         
            return View();
        }

        public List<Requisition> ListReqID(int idEmployee)
        {

            List<Requisition> reqs = _requisitionDAO.RetrieveRequisitionByEmpId(idEmployee);
            /* using (SqlConnection conn = new SqlConnection(connectionString))
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
             }*/
            return reqs;
        }

        public ActionResult CheckStatus(string username, string cmd, int? reqID)
        {

            if (Session["IdEmployee"] == null)
                return RedirectToAction("Login", "Home");

            int idEmployee = (int)Session["IdEmployee"];

            //  int idEmployee = 2;


            int req = reqID.GetValueOrDefault();
            //username = "Sam Worthington";

            List<Requisition> reqs = ListReqID(idEmployee);
            List<RequisitionItem> reqq = ListReqItems(req);

            ViewBag.ReqItems = ListReqItems(req);
            ViewData["reqq"] = reqq;
            ViewData["reqs"] = reqs;

            if (cmd == "delete")
            {
                //     _requisitionItemDAO.DeleteRequisitionItemByReqId(reqID);
                //     _requisitionDAO.DeleteRequisition(reqID);
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

        //public ActionResult History(string username, string cmd, int? reqID)
        //{

        //    if (Session["IdEmployee"] == null)
        //        return RedirectToAction("Login", "Home");

        //    int idEmployee = (int)Session["IdEmployee"];
        //    int req = reqID.GetValueOrDefault();
        //    //username = "Sam Worthington";

        //    List<Requisition> reqs = ListReqID(idEmployee);
        //    List<RequisitionItem> reqq = ListReqItems(req);

        //    ViewBag.ReqItems = ListReqItems(req);
        //    ViewData["reqq"] = reqq;
        //    ViewData["reqs"] = reqs;

        //    if (cmd == "delete")
        //    {
        //        //    _requisitionItemDAO.DeleteRequisitionItemByReqId(reqID);
        //        //    _requisitionDAO.DeleteRequisition(reqID);
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();


        //            string RequisitionItems = @"DELETE from RequisitionItems WHERE IdRequisiton =" + reqID + "";

        //            SqlCommand reqItems = new SqlCommand(RequisitionItems, conn);

        //            reqItems.ExecuteNonQuery();
        //        }
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();


        //            string Requisitions = @"DELETE from Requisitions WHERE IdRequisition = " + reqID + "";

        //            SqlCommand reqId = new SqlCommand(Requisitions, conn);

        //            reqId.ExecuteNonQuery();
        //        }
        //    }
        //    if (cmd == "update")
        //    {


        //    }
        //    return View();
        //}

        public ActionResult History(string username, string cmd, int? reqID)
        {

            if (Session["IdEmployee"] == null)
                return RedirectToAction("Login", "Home");

            int idEmployee = (int)Session["IdEmployee"];
            int req = reqID.GetValueOrDefault();
            //username = "Sam Worthington";

            List<Requisition> reqs = ListReqID(idEmployee);
            List<RequisitionItem> reqq = ListReqItems(req);

            ViewBag.ReqItems = ListReqItems(req);
            ViewData["reqq"] = reqq;
            ViewData["reqs"] = reqs;

            if (cmd == "delete")
            {
                //    _requisitionItemDAO.DeleteRequisitionItemByReqId(reqID);
                //    _requisitionDAO.DeleteRequisition(reqID);
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

        public ActionResult Create(string cmd, int? id, string searchStr = " ")
        {
            if (Session["IdEmployee"] == null)
                return RedirectToAction("Login", "Home");

            int idEmployee = (int)Session["IdEmployee"];

            ViewBag.items = ListProducts(searchStr);
            ViewBag.searchStr = searchStr;

            return View();
        }
    }
}