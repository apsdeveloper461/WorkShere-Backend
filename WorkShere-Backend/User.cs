using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WorkShere_Backend
{
    public class User
    {
        private int id;
        private string role;
        private string name;
        private string email;
        private string password;
        private bool activationStatus;
        private bool workingStatus;

        public User() { }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Role
        {
            get { return role; }
            set { role = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public bool WorkingStatus
        {
            get { return workingStatus; }
            set { workingStatus = value; }
        }

        public bool ActivationStatus
        {
            get { return activationStatus; }
            set { activationStatus = value; }
        }

        public User(int id, string role, string name, string email, string password, bool activationStatus, bool workingStatus)
        {
            Id = id;
            Role = role;
            Name = name;
            Email = email;
            Password = password;
            ActivationStatus = activationStatus;
            WorkingStatus = workingStatus;
        }

        public string AddUser(string role, string name, string email, string password = "password", bool activationStatus = true)
        {
            if (this.role == "admin")
            {
                DatabaseConnection dbConnection = new DatabaseConnection();
                MySqlConnection connection = dbConnection.GetConnection();

                try
                {
                    string query = "INSERT INTO users (role, name, email, password, activationStatus) VALUES (@Role, @Name, @Email, @Password, @ActivationStatus)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Role", role);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@ActivationStatus", activationStatus);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        return "User added successfully";
                    }
                    else
                    {
                        return "Failed to add user: \n User Already exist with this email or something went wrong";
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (not shown here for brevity)
                    return "Error: " + ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
            return "Only admin has access to add user";
        }

        public List<User> GetAllUsers()
        {
            if (this.role == "admin")
            {
                List<User> users = new List<User>();
                DatabaseConnection dbConnection = new DatabaseConnection();
                MySqlConnection connection = dbConnection.GetConnection();

                try
                {
                    string query = "SELECT * FROM users where id!=1";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
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
                        users.Add(user);
                    }
                    return users;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    // Log the exception (not shown here for brevity)
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
            return null;
        }

        public string UpdateUser(int userId, string email, string name, string password, string role, bool isActive)
        {
            if (this.role == "admin")
            {
                DatabaseConnection dbConnection = new DatabaseConnection();
                MySqlConnection connection = dbConnection.GetConnection();

                try
                {
                    string query = "UPDATE users SET name = @Name, password = @Password, role = @Role, activationStatus = @ActivationStatus, email = @Email WHERE id = @UserId";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Role", role);
                    cmd.Parameters.AddWithValue("@ActivationStatus", isActive);
                    cmd.Parameters.AddWithValue("@Email", email);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        return "User updated successfully";
                    }
                    else
                    {
                        return "Failed to update user";
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (not shown here for brevity)
                    return "Error: " + ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
            return "Only admin has access to update user";
        }

        public string AddProject(string title, string description, List<string> AssignedToUserEmails)
        {
            if (this.role == "admin")
            {
                DatabaseConnection dbConnection = new DatabaseConnection();
                MySqlConnection connection = dbConnection.GetConnection();

                try
                {
                    // Insert the new project into the projects table
                    string query = "INSERT INTO projects (title, description, status, startDate) VALUES (@Title, @Description, true, NOW())";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", description);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        int projectId = (int)cmd.LastInsertedId;

                        // Assign the project to the users and update their working status
                        foreach (var email in AssignedToUserEmails)
                        {
                            string userQuery = "SELECT id FROM users WHERE email = @Email";
                            MySqlCommand userCmd = new MySqlCommand(userQuery, connection);
                            userCmd.Parameters.AddWithValue("@Email", email);

                            object userIdObj = userCmd.ExecuteScalar();
                            if (userIdObj != null)
                            {
                                int userId = Convert.ToInt32(userIdObj);

                                // Insert into project_users table
                                string assignQuery = "INSERT INTO project_users (projectId, userId) VALUES (@ProjectId, @UserId)";
                                MySqlCommand assignCmd = new MySqlCommand(assignQuery, connection);
                                assignCmd.Parameters.AddWithValue("@ProjectId", projectId);
                                assignCmd.Parameters.AddWithValue("@UserId", userId);
                                assignCmd.ExecuteNonQuery();

                                // Update the working status of the user
                                string updateUserQuery = "UPDATE users SET workingStatus = 1 WHERE id = @UserId";
                                MySqlCommand updateUserCmd = new MySqlCommand(updateUserQuery, connection);
                                updateUserCmd.Parameters.AddWithValue("@UserId", userId);
                                updateUserCmd.ExecuteNonQuery();
                            }
                        }
                        return "Project added and users assigned successfully";
                    }
                    else
                    {
                        return "Failed to add project";
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (not shown here for brevity)
                    return "Error: " + ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
            return "Only admin has access to add project";
        }

        public string UpdateProject(int projectid,string title,string description)
        {
            if (this.role == "admin")
            {
                DatabaseConnection dbConnection = new DatabaseConnection();
                MySqlConnection connection = dbConnection.GetConnection();
                try
                {
                    string query = "UPDATE projects SET title = @Title, description = @Description WHERE id = @ProjectId";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@ProjectId", projectid);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", description);
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        return "Project updated successfully";
                    }
                    else
                    {
                        return "Failed to update project";
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (not shown here for brevity)
                    return "Error: " + ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
            return "Only admin has access to update project";
        }
        public List<Project> GetAllProject()
        {
            if (this.role == "admin")
            {
                List<Project> projects = new List<Project>();
                DatabaseConnection dbConnection = new DatabaseConnection();
                MySqlConnection connection = dbConnection.GetConnection();

                try
                {
                    string query = @"
                        SELECT 
                            p.id AS ProjectId, p.title, p.description, p.status, p.startDate, p.endDate,
                            u.id AS UserId, u.role, u.name, u.email, u.password, u.activationStatus, u.workingStatus
                        FROM 
                            projects p
                        LEFT JOIN 
                            project_users pu ON p.id = pu.projectId
                        LEFT JOIN 
                            users u ON pu.userId = u.id";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Project> projectDict = new Dictionary<int, Project>();

                    while (reader.Read())
                    {
                        int projectId = reader.GetInt32("ProjectId");
                        if (!projectDict.ContainsKey(projectId))
                        {
                            string title = reader.GetString("title");
                            string description = reader.GetString("description");
                            bool status = reader.GetBoolean("status");
                            DateTime startDate = reader.GetDateTime("startDate");
                            DateTime? endDate = reader.IsDBNull(reader.GetOrdinal("endDate")) ? (DateTime?)null : reader.GetDateTime("endDate");

                            projectDict[projectId] = new Project(projectId, title, description, new List<User>(), status, startDate, endDate);
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("UserId")))
                        {
                            User user = new User(
                                reader.GetInt32("UserId"),
                                reader.GetString("role"),
                                reader.GetString("name"),
                                reader.GetString("email"),
                                reader.GetString("password"),
                                reader.GetBoolean("activationStatus"),
                                reader.GetBoolean("workingStatus")
                            );
                            projectDict[projectId].AssignedTo.Add(user);
                        }
                    }

                    projects = projectDict.Values.ToList();
                    return projects;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    // Log the exception (not shown here for brevity)
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
            return null;
        }

        public string MarkProjectAsComplete(int pid)
        {
            if (this.role == "admin")
            {
                DatabaseConnection dbConnection = new DatabaseConnection();
                MySqlConnection connection = dbConnection.GetConnection();

                try
                {
                    // Check if the project is already completed
                    string checkProjectStatusQuery = "SELECT status FROM projects WHERE id = @ProjectId";
                    MySqlCommand checkProjectStatusCmd = new MySqlCommand(checkProjectStatusQuery, connection);
                    checkProjectStatusCmd.Parameters.AddWithValue("@ProjectId", pid);
                    bool projectStatus = (bool)checkProjectStatusCmd.ExecuteScalar();

                    if (!projectStatus)
                    {
                        return "Project is already marked as complete";
                    }

                    // Start a transaction
                    MySqlTransaction transaction = connection.BeginTransaction();

                    // Change the status of the project to "complete"
                    string updateProjectQuery = "UPDATE projects SET status = false, endDate = NOW() WHERE id = @ProjectId";
                    MySqlCommand updateProjectCmd = new MySqlCommand(updateProjectQuery, connection, transaction);
                    updateProjectCmd.Parameters.AddWithValue("@ProjectId", pid);
                    int projectResult = updateProjectCmd.ExecuteNonQuery();

                    if (projectResult > 0)
                    {
                        // Get the user IDs from the project_users table who are working on this project
                        string getUserIdsQuery = "SELECT userId FROM project_users WHERE projectId = @ProjectId";
                        MySqlCommand getUserIdsCmd = new MySqlCommand(getUserIdsQuery, connection, transaction);
                        getUserIdsCmd.Parameters.AddWithValue("@ProjectId", pid);
                        MySqlDataReader reader = getUserIdsCmd.ExecuteReader();

                        List<int> userIds = new List<int>();
                        while (reader.Read())
                        {
                            userIds.Add(reader.GetInt32("userId"));
                        }
                        reader.Close();

                        // Update their working status to false
                        foreach (int userId in userIds)
                        {
                            string updateUserQuery = "UPDATE users SET workingStatus = false WHERE id = @UserId";
                            MySqlCommand updateUserCmd = new MySqlCommand(updateUserQuery, connection, transaction);
                            updateUserCmd.Parameters.AddWithValue("@UserId", userId);
                            updateUserCmd.ExecuteNonQuery();
                        }

                        // Commit the transaction
                        transaction.Commit();
                        return "Project marked as complete and users' working status updated successfully";
                    }
                    else
                    {
                        // Rollback the transaction if the project update failed
                        transaction.Rollback();
                        return "Failed to mark project as complete";
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    // Log the exception (not shown here for brevity)
                    return "Error: " + ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                return "You do not have access to this function";
            }
        }


        public List<Project> ProjectOnWhichWork()
        {
            if (this.role == "product manager" || this.role == "developer")
            {
                List<Project> projects = new List<Project>();
                DatabaseConnection dbConnection = new DatabaseConnection();
                MySqlConnection connection = dbConnection.GetConnection();

                try
                {
                    string query = @"
                SELECT 
                    p.id AS ProjectId, p.title, p.description, p.status, p.startDate, p.endDate
                FROM 
                    projects p
                JOIN 
                    project_users pu ON p.id = pu.projectId
                WHERE 
                    pu.userId = @UserId";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@UserId", this.id);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int projectId = reader.GetInt32("ProjectId");
                        string title = reader.GetString("title");
                        string description = reader.GetString("description");
                        bool status = reader.GetBoolean("status");
                        DateTime startDate = reader.GetDateTime("startDate");
                        DateTime? endDate = reader.IsDBNull(reader.GetOrdinal("endDate")) ? (DateTime?)null : reader.GetDateTime("endDate");

                        Project project = new Project(projectId, title, description, new List<User>(), status, startDate, endDate);
                        projects.Add(project);
                    }

                    return projects;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    // Log the exception (not shown here for brevity)
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                return null;
            }
        }

        public List<Project> CurrentlyOnWhichWork()
        {
            if (this.role == "product manager" || this.role == "developer")
            {
                List<Project> projects = new List<Project>();
                DatabaseConnection dbConnection = new DatabaseConnection();
                MySqlConnection connection = dbConnection.GetConnection();

                try
                {
                    string query = @"
                SELECT 
                    p.id AS ProjectId, p.title, p.description, p.status, p.startDate, p.endDate
                FROM 
                    projects p
                JOIN 
                    project_users pu ON p.id = pu.projectId
                WHERE 
                    pu.userId = @UserId AND p.status = true";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@UserId", this.id);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int projectId = reader.GetInt32("ProjectId");
                        string title = reader.GetString("title");
                        string description = reader.GetString("description");
                        bool status = reader.GetBoolean("status");
                        DateTime startDate = reader.GetDateTime("startDate");
                        DateTime? endDate = reader.IsDBNull(reader.GetOrdinal("endDate")) ? (DateTime?)null : reader.GetDateTime("endDate");

                        Project project = new Project(projectId, title, description, new List<User>(), status, startDate, endDate);
                        projects.Add(project);
                    }

                    return projects;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    // Log the exception (not shown here for brevity)
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                return null;
            }
        }



        public string SendFeedback( int projectId, string message)
        {
            int senderId = this.id;
            DatabaseConnection dbConnection = new DatabaseConnection();
            MySqlConnection connection = dbConnection.GetConnection();

            try
            {
                string query = "INSERT INTO feedback (senderId, projectId, message, time) VALUES (@SenderId, @ProjectId, @Message, NOW())";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@SenderId", senderId);
                cmd.Parameters.AddWithValue("@ProjectId", projectId);
                cmd.Parameters.AddWithValue("@Message", message);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    return "yes";
                }
                else
                {
                    return "Failed to send feedback";
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here for brevity)
                return "Error: " + ex.Message;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<Feedback> GetFeedback(int projectId)
        {
            List<Feedback> feedbackList = new List<Feedback>();
            DatabaseConnection dbConnection = new DatabaseConnection();
            MySqlConnection connection = dbConnection.GetConnection();

            try
            {
                string query = @"
                    SELECT 
                        f.id, f.senderId, f.projectId, f.message, f.time, u.email AS senderEmail
                    FROM 
                        feedback f
                    JOIN 
                        users u ON f.senderId = u.id
                    WHERE 
                        f.projectId = @ProjectId";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ProjectId", projectId);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Feedback feedback = new Feedback(
                        reader.GetInt32("id"),
                        reader.GetInt32("senderId"),
                        reader.GetInt32("projectId"),
                        reader.GetString("message"),
                        reader.GetDateTime("time"),
                        reader.GetString("senderEmail") // Populate the senderEmail attribute
                    );
                    feedbackList.Add(feedback);
                }

                return feedbackList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // Log the exception (not shown here for brevity)
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public string AddTimeLog(int projectId, string description, float workedHours, string status, DateTime date)
        {
            if(this.role != "developer")
            {
                return "Only developers can add time logs";
            }
            int developerId=this.id;
            DatabaseConnection dbConnection = new DatabaseConnection();
            MySqlConnection connection = dbConnection.GetConnection();

            try
            {
                string query = "INSERT INTO timeLog (projectId, developerId, description, workedHours, status, date) VALUES (@ProjectId, @DeveloperId, @Description, @WorkedHours, @Status, @Date)";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ProjectId", projectId);
                cmd.Parameters.AddWithValue("@DeveloperId", developerId);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@WorkedHours", workedHours);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Date", date);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    return "Time log added successfully";
                }
                else
                {
                    return "Failed to add time log";
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here for brevity)
                return "Error: " + ex.Message;
            }
            finally
            {
                connection.Close();
            }
        }


        public List<TimeLog> GetTimeLogs(int projectId)
        {
            List<TimeLog> timeLogList = new List<TimeLog>();
            DatabaseConnection dbConnection = new DatabaseConnection();
            MySqlConnection connection = dbConnection.GetConnection();

            try
            {
                string query = @"
                        SELECT 
                            t.id, t.description, t.workedHours, t.status, t.date,
                            p.id AS ProjectId, p.title, p.description AS projectDescription, p.status AS projectStatus, p.startDate, p.endDate,
                            u.id AS developerId, u.role, u.name, u.email, u.password, u.activationStatus, u.workingStatus
                        FROM 
                            timeLog t
                        JOIN 
                            projects p ON t.projectId = p.id
                        JOIN 
                            users u ON t.developerId = u.id
                        WHERE 
                            t.projectId = @ProjectId";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ProjectId", projectId);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Project project = new Project(
                        reader.GetInt32("ProjectId"),
                        reader.GetString("title"),
                        reader.GetString("projectDescription"),
                        new List<User>(),
                        reader.GetBoolean("projectStatus"),
                        reader.GetDateTime("startDate"),
                        reader.IsDBNull(reader.GetOrdinal("endDate")) ? (DateTime?)null : reader.GetDateTime("endDate")
                    );

                    User developer = new User(
                        reader.GetInt32("developerId"),
                        reader.GetString("role"),
                        reader.GetString("name"),
                        reader.GetString("email"),
                        reader.GetString("password"),
                        reader.GetBoolean("activationStatus"),
                        reader.GetBoolean("workingStatus")
                    );

                    TimeLog timeLog = new TimeLog(
                        reader.GetInt32("id"),
                        project,
                        developer,
                        reader.GetString("description"),
                        reader.GetFloat("workedHours"),
                        reader.GetString("status"),
                        reader.GetDateTime("date")
                    );
                    timeLogList.Add(timeLog);
                }

                return timeLogList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // Log the exception (not shown here for brevity)
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<TimeLog> GetTimeLogsByUserId()
        {
            if (this.role != "developer")
            {
                Debug.WriteLine("No allowed to access this function");
                return null;
            }
            int userId= this.id;
            List<TimeLog> timeLogList = new List<TimeLog>();
            DatabaseConnection dbConnection = new DatabaseConnection();
            MySqlConnection connection = dbConnection.GetConnection();

            try
            {
                string query = @"
            SELECT 
                t.id, t.description, t.workedHours, t.status, t.date,
                p.id AS ProjectId, p.title, p.description AS projectDescription, p.status AS projectStatus, p.startDate, p.endDate,
                u.id AS developerId, u.role, u.name, u.email, u.password, u.activationStatus, u.workingStatus
            FROM 
                timeLog t
            JOIN 
                projects p ON t.projectId = p.id
            JOIN 
                users u ON t.developerId = u.id
            WHERE 
                t.developerId = @UserId";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Project project = new Project(
                        reader.GetInt32("ProjectId"),
                        reader.GetString("title"),
                        reader.GetString("projectDescription"),
                        new List<User>(),
                        reader.GetBoolean("projectStatus"),
                        reader.GetDateTime("startDate"),
                        reader.IsDBNull(reader.GetOrdinal("endDate")) ? (DateTime?)null : reader.GetDateTime("endDate")
                    );

                    User developer = new User(
                        reader.GetInt32("developerId"),
                        reader.GetString("role"),
                        reader.GetString("name"),
                        reader.GetString("email"),
                        reader.GetString("password"),
                        reader.GetBoolean("activationStatus"),
                        reader.GetBoolean("workingStatus")
                    );

                    TimeLog timeLog = new TimeLog(
                        reader.GetInt32("id"),
                        project,
                        developer,
                        reader.GetString("description"),
                        reader.GetFloat("workedHours"),
                        reader.GetString("status"),
                        reader.GetDateTime("date")
                    );
                    timeLogList.Add(timeLog);
                }

                return timeLogList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // Log the exception (not shown here for brevity)
                return null;
            }
            finally
            {
                connection.Close();
            }
        }


        public string UpdateTimeLogStatus(int TimeLogId, string newStatus)
        {
            if (this.role != "product manager")
            {
                return "You can't have access to this function";
            }

            if (newStatus != "pending" && newStatus != "approved" && newStatus != "reject")
            {
                return "Invalid status. Allowed values are 'pending', 'approved', or 'reject'.";
            }

            DatabaseConnection dbConnection = new DatabaseConnection();
            MySqlConnection connection = dbConnection.GetConnection();

            try
            {
                string query = "UPDATE timeLog SET status = @NewStatus WHERE id = @TimeLogId";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@NewStatus", newStatus);
                cmd.Parameters.AddWithValue("@TimeLogId", TimeLogId);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    return "Time log status updated successfully";
                }
                else
                {
                    return "Failed to update time log status. Time log not found.";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // Log the exception (not shown here for brevity)
                return "Error: " + ex.Message;
            }
            finally
            {
                connection.Close();
            }
        }


        public string UpdatePassword(string newPassword)
        {
            if (this.role == "product manager" || this.role == "developer")
            {
                if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                {
                    return "Password must be at least 6 characters long and cannot be empty.";
                }

                DatabaseConnection dbConnection = new DatabaseConnection();
                MySqlConnection connection = dbConnection.GetConnection();

                try
                {
                    string query = "UPDATE users SET password = @NewPassword WHERE id = @UserId";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@NewPassword", newPassword);
                    cmd.Parameters.AddWithValue("@UserId", this.id);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        return "Password updated successfully.";
                    }
                    else
                    {
                        return "Failed to update password. User not found.";
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    // Log the exception (not shown here for brevity)
                    return "Error: " + ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                return "Not allowed to update Password";
            }
        }


        public float TotalTimeLogOnProject(int projectid)
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            MySqlConnection connection = dbConnection.GetConnection();
            try
            {
                string query = "SELECT SUM(workedHours) FROM timeLog WHERE projectId = @ProjectId";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ProjectId", projectid);
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToSingle(result);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // Log the exception (not shown here for brevity)
                return 0;
            }
            finally
            {
                connection.Close();
            }

        }


        public string AddFinanceOfProject(int projectId, float hourly_rate, float managementCost)
        {
            if(this.role != "admin")
            {
                return "Only admin can add finance";
            }
            DatabaseConnection dbConnection = new DatabaseConnection();
            MySqlConnection connection = dbConnection.GetConnection();
            try
            {
                string query = "INSERT INTO finance (projectId, hourly_rate, managementCost) VALUES (@ProjectId, @HourlyRate, @ManagementCost)";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ProjectId", projectId);
                cmd.Parameters.AddWithValue("@HourlyRate", hourly_rate);
                cmd.Parameters.AddWithValue("@ManagementCost", managementCost);
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    return "Finance added successfully";
                }
                else
                {
                    return "Failed to add finance";
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here for brevity)
                return "Error: " + ex.Message;
            }
            finally
            {
                connection.Close();
            }

        }

        public List<Finance> GetAllFinance()
        {
            if (this.role != "admin")
            {
                return null; // Only admin can access this function
            }

            List<Finance> financeList = new List<Finance>();
            DatabaseConnection dbConnection = new DatabaseConnection();
            MySqlConnection connection = dbConnection.GetConnection();
            try
            {
                string query = @"
        SELECT 
            f.id, 
            f.projectId, 
            p.title AS name, 
            f.hourly_rate, 
            f.managementCost, 
            f.created_date,
            (SELECT SUM(t.workedHours) FROM timeLog t WHERE t.projectId = f.projectId) AS total_hours,
            (f.managementCost + ((SELECT SUM(t.workedHours) FROM timeLog t WHERE t.projectId = f.projectId) * f.hourly_rate)) AS total_cost_of_project
        FROM 
            finance f
        JOIN 
            projects p ON f.projectId = p.id";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Finance finance = new Finance(
                        reader.GetInt32("id"),
                        reader.GetInt32("projectId"),
                        reader.GetString("name"),
                        reader.GetFloat("hourly_rate"),
                        reader.GetFloat("managementCost"),
                        reader.IsDBNull(reader.GetOrdinal("total_hours")) ? 0 : reader.GetFloat("total_hours"),
                        reader.IsDBNull(reader.GetOrdinal("total_cost_of_project")) ? 0 : reader.GetFloat("total_cost_of_project"),
                        reader.GetDateTime("created_date")
                    );
                    financeList.Add(finance);
                }
                return financeList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // Log the exception (not shown here for brevity)
                return null;
            }
            finally
            {
                connection.Close();
            }
        }


    }
}