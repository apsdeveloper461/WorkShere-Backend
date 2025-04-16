using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkShere_Backend
{
    public class Feedback
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ProjectId { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public string SenderEmail { get; set; } // This attribute is not stored in the database

        public Feedback() { }
        public Feedback(int id, int senderId, int projectId, string message, DateTime time, string senderEmail)
        {
            Id = id;
            SenderId = senderId;
            ProjectId = projectId;
            Message = message;
            Time = time;
            SenderEmail = senderEmail;
        }
        public Feedback(int id, int senderId, int projectId, string message, DateTime time)
        {
            Id = id;
            SenderId = senderId;
            ProjectId = projectId;
            Message = message;
            Time = time;
        }



    }
}