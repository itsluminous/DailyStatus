using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DailyStatus.Models
{
    public class WeeklyStatusModel
    {
        [Display(Name = "S.No")]
        public long SNo
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }

        [Display(Name = "Task")]
        [Required(ErrorMessage = "Kindly enter Ticket Number")]
        public string Task
        {
            get;
            set;
        }

        [Display(Name = "Assign To")]
        [Required(ErrorMessage = "Kindly enter your name here.")]
        public string AssignTo
        {
            get;
            set;
        }

        [Display(Name = "Priority")]
        [Required(ErrorMessage = "Kindly enter ticket priority as per JIRA.")]
        public int Priority
        {
            get;
            set;
        }

        [Display(Name = "Complexity")]
        [Required(ErrorMessage = "Kindly enter complexity of task which you feel.")]
        public string Complexity
        {
            get;
            set;
        }

        [Display(Name = "Status")]
        [Required(ErrorMessage = "Kindly enter what is progress status of ticket.")]
        public string TaskStatus
        {
            get;
            set;
        }

        [Display(Name = "Comments")]
        [DataType(DataType.MultilineText)]
        public string Comments
        {
            get;
            set;
        }

        [Display(Name = "Date")]
        public string DateOfInsert
        {
            get;
            set;
        }

        public WeeklyStatusModel(long sno, int id, string task, string assignTo, int priority, string complexity, string status, string comments)
        {
            this.SNo = sno;
            this.ID = id;
            this.Task = task;
            this.AssignTo = assignTo;
            this.Priority = priority;
            this.Complexity = complexity;
            this.TaskStatus = status;
            this.Comments = comments;
        }

        public WeeklyStatusModel(long sno, int id, string task, string assignTo, int priority, string complexity, string status, string comments, string DateOfInsert)
        {
            this.SNo = sno;
            this.ID = id;
            this.Task = task;
            this.AssignTo = assignTo;
            this.Priority = priority;
            this.Complexity = complexity;
            this.TaskStatus = status;
            this.Comments = comments;
            this.DateOfInsert = DateOfInsert;
        }

        public WeeklyStatusModel() { }
    }

    public enum Complexity
    {
        Simple,
        Medium,
        Complex
    }
    public enum TaskStatus
    {
        Not_Started,
        In_Progress,
        Done,
        On_Hold
    }

    public class WeeklyStatusSummary
    {
        public string TaskStatus { get; set; }
        public int P0 { get; set; }
        public int P1 { get; set; }
        public int P2 { get; set; }
        public int P3 { get; set; }
        public string Comments { get; set; }

        public WeeklyStatusSummary(string TaskStatus, int P0, int P1, int P2, int P3, string Comments)
        {
            this.TaskStatus = TaskStatus;
            this.P0 = P0;
            this.P1 = P1;
            this.P2 = P2;
            this.P3 = P3;
            this.Comments = Comments;
        }
    }
    
    public class WeeklyStatusGroup
    {
        public List<WeeklyStatusModel> WeeklyStatus { get; set; }
        public List<WeeklyStatusSummary> WeeklySummary { get; set; }
        [DataType(DataType.MultilineText)]
        public string WeeklyBrief { get; set; }
        public string DateOfInsert { get; set; }

        public WeeklyStatusGroup(List<WeeklyStatusModel> WSM, List<WeeklyStatusSummary> WSS, string WSB, string DOI)
        {
            this.WeeklyStatus = WSM;
            this.WeeklySummary = WSS;
            this.WeeklyBrief = WSB;
            this.DateOfInsert = DOI;
        }
    }
}