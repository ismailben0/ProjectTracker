using System;
using System.Data;
using System.Data.SqlClient;

namespace PT_DAL
{
    public static class clsProjectData
    {
        // In clsProjectData — PT_DAL\clsProject.cs
        //public static readonly string ConnectionString =
        //    System.Configuration.ConfigurationManager
        //        .ConnectionStrings["DevProjectsDB"]
        //        ?.ConnectionString  // ← null-safe
        //    ?? throw new Exception(
        //        "Connection string 'DevProjectsDB' not found in App.config. " +
        //        "Make sure the WPF startup project (PT) has App.config with this key.");

    //    public static readonly string ConnectionString =
    //"Server=.;Database=DevProjectsDB;User Id=sa;Password=sa123456;";

        // REMOVE the readonly field, replace with a property
        public static string ConnectionString = clsDataAccessSettings.ConnectionString;

        public static bool GetProjectByID(int ProjectID, ref string Name, ref string Goal, ref string Description,
                                                         ref DateTime StartDate, ref DateTime? EndDate)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_FindProjectByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProjectID", ProjectID);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                

                                Name = reader["Name"].ToString();

                                if (reader["Goal"] == DBNull.Value)
                                    Goal = null;
                                else
                                    Goal = reader["Goal"].ToString();

                                Description = reader["Description"].ToString();

                                StartDate = (DateTime)reader["StartDate"];

                                if (reader["EndDate"] == DBNull.Value)
                                    EndDate = null;
                                else
                                    EndDate = (DateTime)reader["EndDate"];

                                isFound = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("C# Error: " + ex.Message);
                    }
                }
            }

            return isFound;
        }

        public static int AddNewProject(string Name, string Goal, string Description,
                                        DateTime StartDate, DateTime? EndDate = null)
        {
            int newProjectID = -1; // default for error

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_AddNewProject", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Goal", string.IsNullOrEmpty(Goal) ? (object)DBNull.Value : Goal);
                    command.Parameters.AddWithValue("@Description", Description);
                    command.Parameters.AddWithValue("@StartDate", StartDate);

                    // Output parameter
                    SqlParameter outputParam = new SqlParameter("@NewProjectID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();

                        newProjectID = (int)outputParam.Value;

                        // Check if SP returned error
                        if (newProjectID == -1)
                        {
                            Console.WriteLine("Error occurred in SP_AddNewProject.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("C# Error: " + ex.Message);
                    }
                }
            }

            return newProjectID;
        }

        public static bool UpdateProjectByID(int ProjectID, string Name, string Goal, string Description, 
                                             DateTime StartDate, DateTime? EndDate)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_UpdateProjectByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@ProjectID", ProjectID);
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Goal", string.IsNullOrEmpty(Goal) ? (object)DBNull.Value : Goal);
                    command.Parameters.AddWithValue("@Description", Description);
                    command.Parameters.AddWithValue("@StartDate", StartDate);
                    command.Parameters.AddWithValue("@EndDate", EndDate.HasValue ? (object)EndDate.Value : DBNull.Value);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int affectedRows = (reader["AffectedRows"] != DBNull.Value) ? Convert.ToInt32(reader["AffectedRows"]) : 0;
                                if (affectedRows > 0)
                                {
                                    success = true;
                                }
                                else if (affectedRows == -1 && reader["ErrorMessage"] != DBNull.Value)
                                {
                                    Console.WriteLine("SQL Error: " + reader["ErrorMessage"].ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("C# Error: " + ex.Message);
                    }
                }
            }

            return success;
        }

        public static bool DeleteProject(int ProjectID)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_DeleteProject", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProjectID", ProjectID);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int affectedRows = (reader["AffectedRows"] != DBNull.Value) ? Convert.ToInt32(reader["AffectedRows"]) : 0;

                                if (affectedRows > 0)
                                    success = true;
                                else if (affectedRows == -1 && reader["ErrorMessage"] != DBNull.Value)
                                    Console.WriteLine("SQL Error: " + reader["ErrorMessage"].ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("C# Error: " + ex.Message);
                    }
                }
            }

            return success;
        }

        public static DataTable GetAllProjects()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetAllProjects", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        connection.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("C# Error: " + ex.Message);
                    }
                }
            }

            return dt;
        }

        public static bool IsProjectExistByName(string Name)
        {
            bool isExist = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_IsExistByName", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Name", Name);

                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();

                        if (result != null && Convert.ToInt32(result) == 1)
                            isExist = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("C# Error: " + ex.Message);
                    }
                }
            }

            return isExist;
        }

        public static bool IsProjectExistByNameAndID(string Name, int ProjectID)
        {
            bool isExist = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_IsExistByNameAndID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@ProjectID", ProjectID);

                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();

                        if (result != null && Convert.ToInt32(result) == 1)
                            isExist = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("C# Error: " + ex.Message);
                    }
                }
            }

            return isExist;
        }

    }
}