using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WorkShere_Backend
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        string GetData(int value);

        [OperationContract]
        User login(string email, string password);

        [OperationContract]

        string addNewUser(User admin,string role, string name, string email, string password="password", bool activationStatus=true);

        [OperationContract]
        List<User> displayUsers(User admin);

        [OperationContract]
        string updateUser(User admin, int id, string role, string name, string email, string password, bool activationStatus );
        [OperationContract]
        string addNewProject(User admin, string title, string description, List<string> AssignedToUserEmails);

        [OperationContract]
        string updateProject(User admin, int pid, string title, string description);

        [OperationContract]
        List<Project> displayProject(User admin);

        [OperationContract]
        string markProjectAsComplete(User admin, int id);

        [OperationContract]
        List<Project> projectOnWhichWork(User user);

        [OperationContract]

        List<Project> projectCurrentlyOnWhichWork(User user);

        [OperationContract]
        string addNewFeedback(User user, int projectId, string message);

        [OperationContract]

        List<Feedback> getFeedbacks(User user, int projectId);

        [OperationContract]
        string addTimeLog(User user, int projectId, string description, float workedHours, string status, DateTime date);

        [OperationContract]

        List<TimeLog> getTimeLogsByUserId(User user);

        [OperationContract]

        List<TimeLog> getTimeLogsByProjectId(User user, int projectid);

        [OperationContract]

        string updateTimeLogStatus(User user, int timeLogId, string status);

        [OperationContract]

        string updatePassword(User user, string newPassword);

        [OperationContract]

        string addFinance(User user, int projectId, float hourlyRate, float managementCost);

        [OperationContract]
        List<Finance> getFinance(User user);

        [OperationContract]

        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
