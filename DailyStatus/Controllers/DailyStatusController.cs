using DailyStatus.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace DailyStatus.Controllers
{
    public class DailyStatusController : Controller
    {
        private SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["getconn"].ToString();
            con = new SqlConnection(constr);
        }
        //Daily status controllers
        public ActionResult IndividualStatus(string Name)
        {
            ViewBag.Overwrite = 0;      //No
            return View(new StatusModel(0, Name, "", "", "None"));
        }
        [HttpPost]
        public ActionResult IndividualStatus(StatusModel Status, int Overwrite)
        {
            if (ModelState.IsValid)
            {
                Status.Name = Status.Name.Trim();
                Status.Name = Status.Name.First().ToString().ToUpper() + Status.Name.Substring(1);  //To make first letter capital, it looks good ;)
                connection();
                SqlCommand com = new SqlCommand("InsertData", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Name", Status.Name);
                com.Parameters.AddWithValue("@SoFar", Status.SoFar.TrimEnd());
                com.Parameters.AddWithValue("@NextDay", Status.NextDay.TrimEnd());
                com.Parameters.AddWithValue("@Impediments", Status.Impediments.TrimEnd());
                com.Parameters.AddWithValue("@Overwrite", Overwrite);
                con.Open();
                try
                {
                    if (com.ExecuteNonQuery() >= 1)
                    {
                        ViewBag.Success = "Status Saved Successfully";                        
                        ModelState.Clear();
                        ViewBag.Overwrite = 0;      //No
                        TempData.Remove("DateList");
                    }
                }
                catch(Exception ex)
                {
                    if(ex.Message.Contains("Violation of PRIMARY KEY"))
                    {
                        ViewBag.Error = "You have already saved status for today. Click Submit again to overwrite.";
                        ViewBag.Overwrite = -1;      //Yes
                    }
                    else
                    {
                        ViewBag.Error = ex.Message;                        
                        ModelState.Clear();
                        ViewBag.Overwrite = 0;      //No
                    }
                    
                }
                con.Close();
            }
            else
            {
                ViewBag.Overwrite = Overwrite;
            }
            return View(new StatusModel(0, "", "", "", "None"));
        }        
        public ActionResult StatusReport(DateTime? selectedDate)
        {            
            if (TempData["DateList"] == null)
            {
                List<string> DatesList = new List<string>();
                ViewBag.isDateSelected = false;

                connection();
                SqlCommand com = new SqlCommand("GetAllDates", con);
                com.CommandType = CommandType.StoredProcedure;
                con.Open();

                using (SqlDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime DateValue = reader.GetDateTime(0);
                        DatesList.Add(DateValue.ToString("dd-MM-yyyy"));
                    }
                }
                con.Close();
                TempData["DateList"] = String.Join(",", DatesList.Select(x => string.Format("'{0}'", x)));
                TempData.Keep("DateList");
            }  
            
            if(selectedDate == null)
            {
                selectedDate = System.DateTime.Today;
            }

            if (selectedDate != null)
            {
                ViewBag.isDateSelected = true;
                List<StatusModel> StatusList = new List<StatusModel>();
                connection();
                SqlCommand com = new SqlCommand("GetStatusForSelectedDate", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@DateOfStatus", selectedDate);
                con.Open();
                using (SqlDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StatusModel mod = new StatusModel(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                        StatusList.Add(mod);
                    }
                }
                con.Close();
                ViewBag.SelectedDate = selectedDate.Value.ToString("dddd, dd MMMM yyyy");
                return View(StatusList);
            }
            return View();
        }
        [HttpPost]
        public ActionResult EditDailyStatus(StatusModel Status)
        {
            string message = "Status updated. Reload page to see changes.";
            Status.Name = Status.Name.Trim();
            Status.Name = Status.Name.First().ToString().ToUpper() + Status.Name.Substring(1);  //To make first letter capital, it looks good ;)

            connection();
            SqlCommand com = new SqlCommand("InsertData", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Name", Status.Name);
            com.Parameters.AddWithValue("@SoFar", Status.SoFar.TrimEnd());
            com.Parameters.AddWithValue("@NextDay", Status.NextDay.TrimEnd());
            com.Parameters.AddWithValue("@Impediments", Status.Impediments.TrimEnd());
            com.Parameters.AddWithValue("@Overwrite", Status.ID);
            con.Open();
            try
            {
                if (com.ExecuteNonQuery() < 1)
                {
                    message = "Update failed.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            con.Close();
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Search(String Searchstring, bool[] SearchFilter)
        {
            ViewBag.ShowSearchResult = false;
            int NameFilter, SoFarFilter, NextDayFilter, ImpedimentsFilter;
            NameFilter = SoFarFilter = NextDayFilter = ImpedimentsFilter = -1;
            if (!string.IsNullOrEmpty(Searchstring))
            {
                #region find out values of checkboxes
                for (int i=0; i< SearchFilter.Length; i++)
                {
                    if(NameFilter == -1)
                    {
                        if (SearchFilter[i] == true)
                        {
                            NameFilter = 1;
                            i++;
                        }
                        else
                        {
                            NameFilter = 0;
                        }

                    }
                    else if (SoFarFilter == -1)
                    {
                        if (SearchFilter[i] == true)
                        {
                            SoFarFilter = 1;
                            i++;
                        }
                        else
                        {
                            SoFarFilter = 0;
                        }

                    }
                    else if (NextDayFilter == -1)
                    {
                        if (SearchFilter[i] == true)
                        {
                            NextDayFilter = 1;
                            i++;
                        }
                        else
                        {
                            NextDayFilter = 0;
                        }

                    }
                    else if (ImpedimentsFilter == -1)
                    {
                        if (SearchFilter[i] == true)
                        {
                            ImpedimentsFilter = 1;
                            i++;
                        }
                        else
                        {
                            ImpedimentsFilter = 0;
                        }

                    }
                }
                #endregion
                ViewBag.ShowSearchResult = true;
                List<StatusModel> SearchResult = new List<StatusModel>();
                connection();
                SqlCommand com = new SqlCommand("SearchString", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@FilterString", Searchstring.Replace("'", "''"));
                com.Parameters.AddWithValue("@NameFilter", NameFilter);
                com.Parameters.AddWithValue("@SoFarFilter", SoFarFilter);
                com.Parameters.AddWithValue("@NextDayFilter", NextDayFilter);
                com.Parameters.AddWithValue("@ImpedimentsFilter", ImpedimentsFilter);
                con.Open();
                using (SqlDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StatusModel mod = new StatusModel(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetDateTime(4).ToString("dd MMMM yyyy"));
                        SearchResult.Add(mod);
                    }
                }
                con.Close();
                ViewBag.Searchstring = Searchstring;
                return View(SearchResult);
            }
            return View();
        }

        //Weekly status controllers
        public ActionResult IndividualWeeklyStatus(string Name)
        {
            ViewBag.Overwrite = 0;      //No
            ViewBag.WeekSpan = GetMondayToFriday(System.DateTime.Today);
            return View(new WeeklyStatusModel(0, 0, "", Name, 2, "", "", ""));
        }
        [HttpPost]
        public ActionResult IndividualWeeklyStatus(WeeklyStatusModel Status, int Overwrite)
        {
            if (ModelState.IsValid)
            {
                Status.AssignTo = Status.AssignTo.Trim();
                Status.AssignTo = Status.AssignTo.First().ToString().ToUpper() + Status.AssignTo.Substring(1);  //To make first letter capital, it looks good ;)
                Status.Task = Regex.Replace(Status.Task, "bpf", "BPF", RegexOptions.IgnoreCase);

                bool allowBackEdit = ConfigurationManager.AppSettings["AllowBackEdit"].ToLower() == "true" ? true : false;
                string commandText = allowBackEdit ? "InsertWeeklyDataWithDate" : "InsertWeeklyData";

                connection();
                SqlCommand com = new SqlCommand(commandText, con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Task", Status.Task.TrimEnd());
                com.Parameters.AddWithValue("@AssignTo", Status.AssignTo.TrimEnd());
                com.Parameters.AddWithValue("@TaskPriority", Status.Priority);
                com.Parameters.AddWithValue("@Complexity", Status.Complexity.TrimEnd());
                com.Parameters.AddWithValue("@TaskStatus", Status.TaskStatus.TrimEnd());
                com.Parameters.AddWithValue("@Comments", string.IsNullOrEmpty(Status.Comments) ? "" : Status.Comments.TrimEnd());
                com.Parameters.AddWithValue("@Overwrite", Overwrite);
                if(allowBackEdit)
                    com.Parameters.AddWithValue("@WeekDate", Status.DateOfInsert.TrimEnd());
                con.Open();
                try
                {
                    if (com.ExecuteNonQuery() >= 1)
                    {
                        ViewBag.Success = Status.Task.TrimEnd() + " status saved successfully";
                        ViewBag.Overwrite = 0;      //No
                        TempData.Remove("WeeklyDateList");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Violation of PRIMARY KEY"))
                    {
                        ViewBag.Error = "You have already saved status for " + Status.Task.TrimEnd() + ". Click Submit again to overwrite.";
                        ViewBag.Overwrite = -1;      //Yes
                    }
                    else
                    {
                        ViewBag.Error = ex.Message;
                        ModelState.Clear();
                        ViewBag.Overwrite = 0;      //No
                    }

                }
                con.Close();
            }
            else
            {
                ViewBag.Overwrite = Overwrite;
            }
            ViewBag.WeekSpan = GetMondayToFriday(System.DateTime.Today);
            return View(new WeeklyStatusModel(0, 0, "", "", 2, "", "", ""));
        }
        public ActionResult WeeklyStatusReport(DateTime? selectedDate)
        {
            if (TempData["WeeklyDateList"] == null)
            {
                List<string> DatesList = new List<string>();
                ViewBag.isDateSelected = false;

                connection();
                SqlCommand com = new SqlCommand("GetAllWeeklyDates", con);
                com.CommandType = CommandType.StoredProcedure;
                con.Open();

                using (SqlDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime DateValue = reader.GetDateTime(0);
                        DatesList.Add(DateValue.ToString("dd-MM-yyyy"));
                    }
                }
                con.Close();
                TempData["WeeklyDateList"] = String.Join(",", DatesList.Select(x => string.Format("'{0}'", x)));
                TempData.Keep("WeeklyDateList");
            }

            if (selectedDate == null)
            {
                selectedDate = System.DateTime.Today;
            }

            if (selectedDate != null)
            {
                ViewBag.isDateSelected = true;
                List<WeeklyStatusModel> StatusList = new List<WeeklyStatusModel>();
                connection();
                SqlCommand com = new SqlCommand("GetStatusForSelectedWeek", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@DateOfStatus", selectedDate);
                con.Open();
                using (SqlDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        WeeklyStatusModel mod = new WeeklyStatusModel(reader.GetInt64(0), reader.GetInt32(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetString(5), reader.GetString(6), reader.GetString(7));
                        StatusList.Add(mod);
                    }
                }
                con.Close();
                List<WeeklyStatusSummary> WeeklySummaryList = new List<WeeklyStatusSummary>();
                #region set up WeeklySummaryList
                string[] TaskStatuses = Enum.GetNames(typeof(DailyStatus.Models.TaskStatus));
                for (int i=0; i< TaskStatuses.Length; i++)
                {
                    string taskStatus = TaskStatuses[i].Replace('_', ' ');
                    WeeklySummaryList.Add(new WeeklyStatusSummary(taskStatus,
                                    StatusList.Where(x => x.Priority == 0).Where(x => x.TaskStatus == taskStatus).Count(),
                                    StatusList.Where(x => x.Priority == 1).Where(x => x.TaskStatus == taskStatus).Count(),
                                    StatusList.Where(x => x.Priority == 2).Where(x => x.TaskStatus == taskStatus).Count(),
                                    StatusList.Where(x => x.Priority == 3).Where(x => x.TaskStatus == taskStatus).Count(),
                                    "")
                                    );
                }
                #endregion
                #region fetch WeeklyStatusBrief
                string WeeklyStatusBrief = "";
                com = new SqlCommand("GetBriefForSelectedWeek", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@DateOfStatus", selectedDate);
                con.Open();
                var returnValue = com.ExecuteScalar();
                if(returnValue != null)
                    WeeklyStatusBrief = returnValue.ToString();
                con.Close();
                #endregion
                WeeklyStatusGroup weeklyGroup = new WeeklyStatusGroup(StatusList, WeeklySummaryList, WeeklyStatusBrief, selectedDate.Value.ToString());
                ViewBag.WeekSpan = GetMondayToFriday(selectedDate.Value);
                ViewBag.WeekStarting = GetWeekStarting(selectedDate.Value);
                return View(weeklyGroup);
            }
            return View();
        }
        [HttpPost]
        public ActionResult EditWeeklyStatus(WeeklyStatusModel Status)
        {
            string message = "Status updated. Reload page to see changes.";
            Status.AssignTo = Status.AssignTo.Trim();
            Status.AssignTo = Status.AssignTo.First().ToString().ToUpper() + Status.AssignTo.Substring(1);  //To make first letter capital, it looks good ;)
            Status.Task = Regex.Replace(Status.Task, "bpf", "BPF", RegexOptions.IgnoreCase);

            connection();
            SqlCommand com = new SqlCommand("InsertWeeklyData", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Task", Status.Task.TrimEnd());
            com.Parameters.AddWithValue("@AssignTo", Status.AssignTo.TrimEnd());
            com.Parameters.AddWithValue("@TaskPriority", Status.Priority);
            com.Parameters.AddWithValue("@Complexity", Status.Complexity.TrimEnd());
            com.Parameters.AddWithValue("@TaskStatus", Status.TaskStatus.TrimEnd());
            com.Parameters.AddWithValue("@Comments", string.IsNullOrEmpty(Status.Comments) ? "" : Status.Comments.TrimEnd());
            com.Parameters.AddWithValue("@Overwrite", Status.ID);
            con.Open();
            try
            {
                if (com.ExecuteNonQuery() < 1)
                {
                    message = "Update failed.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            con.Close();
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EditWeeklyBrief(string Brief, DateTime DateOfInsert)
        {
            string message = "Success. Reload page to see changes.";
            connection();
            SqlCommand com = new SqlCommand("InsertWeeklyBrief", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Brief", Brief.TrimEnd());
            com.Parameters.AddWithValue("@BriefDate", DateOfInsert);
            con.Open();
            try
            {
                if (com.ExecuteNonQuery() < 1)
                {
                    message = "Update failed.";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            con.Close();
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        //Others
        public static string GetMondayToFriday(DateTime date)
        {
            DateTime Monday, Friday;
            if (date.DayOfWeek == DayOfWeek.Monday)
                Monday = date;
            else
                Monday = date.Subtract(new TimeSpan((int)date.DayOfWeek - 1, 0, 0, 0));            
            Friday = Monday.AddDays(4);

            return Monday.ToString("dd-MMM-yyy") + " to " + Friday.ToString("dd-MMM-yyy");
        }
        public static string GetWeekStarting(DateTime date)
        {
            DateTime Monday;
            if (date.DayOfWeek == DayOfWeek.Monday)
                Monday = date;
            else
                Monday = date.Subtract(new TimeSpan((int)date.DayOfWeek - 1, 0, 0, 0));

            string SuperScript = "th";
            if (Monday.Day % 10 == 1 && Monday.Day != 11)
                SuperScript = "st";
            if (Monday.Day % 10 == 2 && Monday.Day != 12)
                SuperScript = "nd";
            if (Monday.Day % 10 == 3 && Monday.Day != 13)
                SuperScript = "rd";

            string returnString = Monday.ToString("MMM dd") + "<sup>" + SuperScript + "</sup>" + Monday.ToString(" yyyy");
            return returnString;
        }
    }    
}