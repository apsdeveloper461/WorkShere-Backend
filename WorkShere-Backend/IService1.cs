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
        List<Project> displayProject(User admin);

        [OperationContract]
        List<Project> projectOnWhichWork(User user);

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
