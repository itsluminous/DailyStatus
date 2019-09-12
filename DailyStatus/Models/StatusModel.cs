using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DailyStatus.Models
{
    public class StatusModel
    {
        public int ID
        {
            get;
            set;
        }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Kindly enter your name.")]
        public string Name
        {
            get;
            set;
        }
        [Display(Name = "So Far")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Kindly enter what you have done today.")]
        public string SoFar
        {
            get;
            set;
        }
        [Display(Name = "Next Day")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Kindly enter what you will be doing next day.")]
        public string NextDay
        {
            get;
            set;
        }
        [Display(Name = "Impediments")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Kindly enter any impediments.")]
        public string Impediments
        {
            get;
            set;
        }

        [Display(Name = "Date Of Insert")]
        public string DateOfInsert
        {
            get;
            set;
        }

        public StatusModel(int id, string name, string soFar, string nextDay, string impediments)
        {
            this.ID = id;
            this.Name = name;
            this.SoFar = soFar;
            this.NextDay = nextDay;
            this.Impediments = impediments;
        }

        public StatusModel(string name, string soFar, string nextDay, string impediments, string dateOfInsert)
        {
            this.Name = name;
            this.SoFar = soFar;
            this.NextDay = nextDay;
            this.Impediments = impediments;
            this.DateOfInsert = dateOfInsert;
        }

        public StatusModel() { }
    }
}