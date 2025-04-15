using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkShere_Backend
{
    public class Project
    {

        private int id;
        private string title;
        private string description;
        private List<User> assignedTo;
        private bool status;
        private DateTime startDate;
        private DateTime? endDate;

        public Project() { }

        public Project(int id, string title, string description, List<User> assignedTo, bool status, DateTime startDate, DateTime? endDate)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.assignedTo = assignedTo;
            this.status = status;
            this.startDate = startDate;
            this.endDate = endDate;
        }
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public List<User> AssignedTo
        {
            get { return assignedTo; }
            set { assignedTo = value; }
        }

        public bool Status
        {
            get { return status; }
            set { status = value; }
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }
        public DateTime? EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }


    }
}