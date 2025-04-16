using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using MySql.Data.MySqlClient;
using System.Diagnostics;


namespace WorkShere_Backend
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public User login(string email, string password)
        {
            if (email == null || password == null)
            {
                return null;
            }

            DatabaseConnection dbConnection = new DatabaseConnection();
            MySqlConnection connection = dbConnection.GetConnection();

            try
            {
                string query = "SELECT * FROM users WHERE email = @Email AND password = @Password";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    User user = new User(
                        reader.GetInt32("id"),
                        reader.GetString("role"),
                        reader.GetString("name"),
                        reader.GetString("email"),
                        reader.GetString("password"),
                        reader.GetBoolean("activationStatus"),
                        reader.GetBoolean("workingStatus")
                    );
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                // Log the exception (not shown here for brevity)
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public string addNewUser(User admin,string role, string name, string email, string password = "password", bool activationStatus = true)
        {
            Debug.WriteLine(role + name);
            return admin.AddUser(role, name, email, password, activationStatus);
        }

        public List<User> displayUsers(User admin)
        {
            return admin.GetAllUsers();
        }
        public string updateUser(User admin, int id, string role, string name, string email, string password, bool activationStatus)
        {
            return admin.UpdateUser(id,email, name, password,role, activationStatus);
        }

        public string addNewProject(User admin,string title, string description, List<string> AssignedToUserEmails)
        {
            return admin.AddProject(title, description, AssignedToUserEmails);
        }

        public List<Project> displayProject(User admin)
        {
            return admin.GetAllProject();
        }

        public List<Project> projectOnWhichWork(User user)
        {
            return user.ProjectOnWhichWork();
        }
        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
