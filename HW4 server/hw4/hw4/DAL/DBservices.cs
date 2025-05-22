using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace hw4.Project
{
    public class DBservices
    {
        public SqlConnection connect(string conString)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build();

            string cStr = configuration.GetConnectionString("myProjDB");
            SqlConnection con = new SqlConnection(cStr);
            con.Open();
            return con;
        }

        private SqlCommand CreateCommandWithStoredProcedureGeneral(string spName, SqlConnection con, Dictionary<string, object> paramDic)
        {
            SqlCommand cmd = new SqlCommand()
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            if (paramDic != null)
            {
                foreach (KeyValuePair<string, object> param in paramDic)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            return cmd;
        }

        // ========== USER METHODS ==========

        public int InsertUser(Users user)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@name", user.Name },
                { "@password", user.Password },
                { "@email", user.Email }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("SP_InsertUser", con, paramDic);

            try
            {
                cmd.ExecuteNonQuery();
                return 0;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    return 3;
                throw;
            }
            finally
            {
                con.Close();
            }
        }

        public Users? LoginUser(string email, string password)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@email", email },
                { "@password", password }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("SP_LoginUser", con, paramDic);

            Users? u = null;

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    u = new Users
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Active = Convert.ToBoolean(reader["Active"])
                    };
                }

                return u;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        public int UpdateUser(Users user)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@name", user.Name },
                { "@password", user.Password },
                { "@email", user.Email }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("SP_UpdateUser", con, paramDic);

            try
            {
                cmd.ExecuteNonQuery();
                return 0;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    return 3;
                throw;
            }
            finally
            {
                con.Close();
            }
        }

        public int SoftDeleteUserByEmail(string email)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@userEmail", email }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("SP_DeleteUser", con, paramDic);

            try
            {
                cmd.ExecuteNonQuery();
                return 0;
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("User not found or already deleted"))
                    return 1;
                throw;
            }
            finally
            {
                con.Close();
            }
        }

        public List<Users> ReadAllUsers()
        {
            List<Users> users = new List<Users>();
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            cmd = CreateCommandWithStoredProcedureGeneral("SP_GetAllUsers", con, null);

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Users u = new Users
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Active = Convert.ToBoolean(reader["Active"])
                    };
                    users.Add(u);
                }

                return users;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        // ========== MOVIE METHODS ==========

        public int InsertMovie(Movies movie)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@Url", movie.Url },
                { "@PrimaryTitle", movie.PrimaryTitle },
                { "@Description", movie.Description },
                { "@PrimaryImage", movie.PrimaryImage },
                { "@Year", movie.Year },
                { "@ReleaseDate", movie.ReleaseDate },
                { "@Language", movie.Language },
                { "@Budget", movie.Budget },
                { "@GrossWorldwide", movie.GrossWorldwide },
                { "@Genres", movie.Genres },
                { "@IsAdult", movie.IsAdult },
                { "@RuntimeMinutes", movie.RuntimeMinutes },
                { "@AverageRating", movie.AverageRating },
                { "@NumVotes", movie.NumVotes }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("SP_InsertMovie", con, paramDic);

            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }


        public List<Movies> ReadMovies()
        {
            List<Movies> movies = new List<Movies>();
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            cmd = CreateCommandWithStoredProcedureGeneral("SP_GetAllMovies", con, null);

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Movies m = new Movies
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Url = reader["Url"].ToString(),
                        PrimaryTitle = reader["PrimaryTitle"].ToString(),
                        Description = reader["Description"].ToString(),
                        PrimaryImage = reader["PrimaryImage"].ToString(),
                        Year = Convert.ToInt32(reader["Year"]),
                        ReleaseDate = Convert.ToDateTime(reader["ReleaseDate"]),
                        Language = reader["Language"].ToString(),
                        Budget = Convert.ToDouble(reader["Budget"]),
                        GrossWorldwide = Convert.ToDouble(reader["GrossWorldwide"]),
                        Genres = reader["Genres"].ToString(),
                        IsAdult = Convert.ToBoolean(reader["IsAdult"]),
                        RuntimeMinutes = Convert.ToInt32(reader["RuntimeMinutes"]),
                        AverageRating = float.Parse(reader["AverageRating"].ToString()),
                        NumVotes = Convert.ToInt32(reader["NumVotes"])
                    };
                    movies.Add(m);
                }

                return movies;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        public List<Movies> GetMoviesByTitle(string title)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@title", title }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("SP_GetMoviesByTitle", con, paramDic);

            List<Movies> result = new List<Movies>();

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Movies m = new Movies
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        PrimaryTitle = reader["PrimaryTitle"].ToString(),
                        ReleaseDate = Convert.ToDateTime(reader["ReleaseDate"])
                    };
                    result.Add(m);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        public List<Movies> GetMoviesByReleaseDate(DateTime startDate, DateTime endDate)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@startDate", startDate },
                { "@endDate", endDate }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("SP_GetMoviesByDate", con, paramDic);

            List<Movies> result = new List<Movies>();

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Movies m = new Movies
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        PrimaryTitle = reader["PrimaryTitle"].ToString(),
                        ReleaseDate = Convert.ToDateTime(reader["ReleaseDate"])
                    };
                    result.Add(m);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        public int DeleteMovieById(int id)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "@id", id }
            };

            cmd = CreateCommandWithStoredProcedureGeneral("SP_DeleteMovieById", con, paramDic);

            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }
    }
}
