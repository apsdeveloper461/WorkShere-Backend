using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkShere_Backend
{
    public class Finance
    {
        public int id { get; set; }
       
        public int project_id { get; set; }
        public string name { get; set; }

        public float hourly_rate { get; set; }

        public float management_cost { get; set; }

        public float total_hours { get; set; }

        public float total_cost_of_project { get; set; }
        public DateTime created_date { get; set; }

        public Finance() { }

        public Finance(int id, int project_id, string name, float hourly_rate, float management_cost, float total_hours, float total_cost_of_project, DateTime created_date)
        {
            this.id = id;
            this.project_id = project_id;
            this.name = name;
            this.hourly_rate = hourly_rate;
            this.management_cost = management_cost;
            this.total_hours = total_hours;
            this.total_cost_of_project = total_cost_of_project;
            this.created_date = created_date;
        }




    }

}