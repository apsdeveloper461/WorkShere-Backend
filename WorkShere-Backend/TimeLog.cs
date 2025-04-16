using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkShere_Backend
{
    public class TimeLog
    {
        public int Id { get; set; }
        public Project Project { get; set; }
        public User Developer { get; set; }
        public string Description { get; set; }
        public float WorkedHours { get; set; }
        public string Status { get; set; } // "pending", "approved", "reject"
        public DateTime Date { get; set; }

        public TimeLog() { }

        public TimeLog(int id, Project project, User developer, string description, float workedHours, string status, DateTime date)
        {
            Id = id;
            Project = project;
            Developer = developer;
            Description = description;
            WorkedHours = workedHours;
            Status = status;
            Date = date;
        }

       
        
    }
}