using System.Data;
using System.Data.SqlClient;

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
        { "@id", user.Id },
        { "@name", user.Name },
        { "@password", user.Password },
        { "@email", user.Email }
    };

            cmd = CreateCommandWithStoredProcedureGeneral("SP_UpdateUser", con, paramDic);

            // הוספת פרמטר פלט כדי לקבל את RETURN מה-SP
            SqlParameter returnValue = new SqlParameter("@ReturnVal", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(returnValue);

            try
            {
                cmd.ExecuteNonQuery();
                return (int)returnValue.Value; // 0 = הצלחה, 1 = לא נמצא
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

            // רק אם נשלח ערך מחיר – נוסיף לפרמטרים
            if (movie.PriceToRent.HasValue)
            {
                paramDic.Add("@PriceToRent", movie.PriceToRent.Value);
            }


            cmd = CreateCommandWithStoredProcedureGeneral("SP_InsertMovie", con, paramDic);

            try
            {
                cmd.ExecuteNonQuery();
                return 0;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    return 3; // Duplicate title
                throw;
            }
            finally
            {
                con.Close();
            }
        }


        public List<Movies> ReadMovies()
        {
            List<Movies> movies = new List<Movies>();
            SqlConnection con = connect("myProjDB");
            SqlCommand cmd = CreateCommandWithStoredProcedureGeneral("SP_GetAllMovies", con, null);

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
                        NumVotes = Convert.ToInt32(reader["NumVotes"]),
                        PriceToRent = reader["PriceToRent"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["PriceToRent"])
                    };
                    movies.Add(m);
                }

                return movies;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                con.Close();
            }
        }

        public (List<Movies>, int) ReadPagedMoviesWithTotal(int page, int pageSize)
        {
            SqlConnection con = connect("myProjDB");
            Dictionary<string, object> paramDic = new()
    {
        { "@PageNumber", page },
        { "@PageSize", pageSize }
    };

            SqlCommand cmd = CreateCommandWithStoredProcedureGeneral("SP_GetMoviesPaged", con, paramDic);
            List<Movies> movies = new();
            int totalPages = 1;

            SqlDataReader reader = cmd.ExecuteReader();

            bool isFirstRow = true;
            while (reader.Read())
            {
                if (isFirstRow)
                {
                    if (reader["TotalPages"] != DBNull.Value)
                        totalPages = Convert.ToInt32(reader["TotalPages"]);
                    isFirstRow = false;
                }

                Movies m = new()
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
                    NumVotes = Convert.ToInt32(reader["NumVotes"]),
                    PriceToRent = reader["PriceToRent"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["PriceToRent"])
                };

                movies.Add(m);
            }

        

            con.Close();
            return (movies, totalPages);
        }




        public List<Movies> GetMoviesByTitle(string title)
        {
            SqlConnection con = connect("myProjDB");
            SqlCommand cmd = CreateCommandWithStoredProcedureGeneral("SP_GetMoviesByTitle", con, new Dictionary<string, object>
    {
        { "@title", title }
    });

            List<Movies> movies = new List<Movies>();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Movies m = new Movies
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Url = reader["url"].ToString(),
                    PrimaryTitle = reader["primaryTitle"].ToString(),
                    Description = reader["description"].ToString(),
                    PrimaryImage = reader["primaryImage"].ToString(),
                    Year = Convert.ToInt32(reader["year"]),
                    ReleaseDate = Convert.ToDateTime(reader["releaseDate"]),
                    Language = reader["language"].ToString(),
                    Budget = Convert.ToDouble(reader["budget"]),
                    GrossWorldwide = Convert.ToDouble(reader["grossWorldwide"]),
                    Genres = reader["genres"].ToString(),
                    IsAdult = Convert.ToBoolean(reader["isAdult"]),
                    RuntimeMinutes = Convert.ToInt32(reader["runtimeMinutes"]),
                    AverageRating = Convert.ToSingle(reader["averageRating"]),
                    NumVotes = Convert.ToInt32(reader["numVotes"]),
                    PriceToRent = reader["priceToRent"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["priceToRent"])
                };
                movies.Add(m);
            }

            con.Close();
            return movies;
        }


        public List<Movies> GetMoviesByReleaseDate(DateTime startDate, DateTime endDate)
        {
            SqlConnection con = connect("myProjDB");
            SqlCommand cmd = CreateCommandWithStoredProcedureGeneral("SP_GetMoviesByDate", con, new Dictionary<string, object>
    {
        { "@startDate", startDate },
        { "@endDate", endDate }
    });

            List<Movies> movies = new List<Movies>();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Movies m = new Movies
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Url = reader["url"].ToString(),
                    PrimaryTitle = reader["primaryTitle"].ToString(),
                    Description = reader["description"].ToString(),
                    PrimaryImage = reader["primaryImage"].ToString(),
                    Year = Convert.ToInt32(reader["year"]),
                    ReleaseDate = Convert.ToDateTime(reader["releaseDate"]),
                    Language = reader["language"].ToString(),
                    Budget = Convert.ToDouble(reader["budget"]),
                    GrossWorldwide = Convert.ToDouble(reader["grossWorldwide"]),
                    Genres = reader["genres"].ToString(),
                    IsAdult = Convert.ToBoolean(reader["isAdult"]),
                    RuntimeMinutes = Convert.ToInt32(reader["runtimeMinutes"]),
                    AverageRating = Convert.ToSingle(reader["averageRating"]),
                    NumVotes = Convert.ToInt32(reader["numVotes"]),
                    PriceToRent = reader["priceToRent"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["priceToRent"])
                };
                movies.Add(m);
            }

            con.Close();
            return movies;
        }


        // מחיקת סרט רגיל
        public int DeleteMovieById(int movieId)
        {
            SqlConnection con = null;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");

                Dictionary<string, object> paramDic = new()
        {
            { "@movieID", movieId }
        };

                cmd = CreateCommandWithStoredProcedureGeneral("SP_DeleteMovie", con, paramDic);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected; // יחזיר 1 אם הסרט עודכן (Soft Delete)
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Movie not found or already deleted"))
                    return 0;

                // מומלץ לא לבלוע שגיאות – אלא לזרוק אותן הלאה עם הקשר
                throw new Exception($"SQL Error during DeleteMovieById: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error during DeleteMovieById: {ex.Message}", ex);
            }
            finally
            {
                con?.Close();
            }
        }



        // מחיקת סרט מושכר
        public int DeleteRentedMovieById(int userId, int movieId)
        {
            SqlConnection con = connect("myProjDB");

            Dictionary<string, object> paramDic = new()
    {
        { "@userId", userId },
        { "@movieId", movieId }
    };

            SqlCommand cmd = CreateCommandWithStoredProcedureGeneral("SP_DeleteRentedMovieById", con, paramDic);

            // פרמטר החזרה
            SqlParameter returnParam = new SqlParameter("@ReturnVal", SqlDbType.Int)
            {
                Direction = ParameterDirection.ReturnValue
            };
            cmd.Parameters.Add(returnParam);

            try
            {
                cmd.ExecuteNonQuery();
                return (int)returnParam.Value;
            }
            finally
            {
                con.Close();
            }
        }





        public int RentMovie(RentedMovie rent)
        {
            SqlConnection con = connect("myProjDB");

            Dictionary<string, object> paramDic = new()
    {
        { "@userId", rent.UserId },
        { "@movieId", rent.MovieId },
        { "@rentStart", rent.RentStart },
        { "@rentEnd", rent.RentEnd },
        { "@totalPrice", rent.TotalPrice }
    };

            SqlCommand cmd = CreateCommandWithStoredProcedureGeneral("SP_RentMovie", con, paramDic);

            try
            {
                // הוספת פרמטר קלט מסוג RETURN
                SqlParameter returnParam = new SqlParameter
                {
                    Direction = ParameterDirection.ReturnValue,
                    SqlDbType = SqlDbType.Int
                };
                cmd.Parameters.Add(returnParam);

                // ביצוע הקריאה ל-SP
                cmd.ExecuteNonQuery();

                // קבלת ערך RETURN מה-SP
                int result = (int)returnParam.Value;

                // אם יש שגיאה לוגית (למשל חפיפה בזמנים) → נחזיר -1
                if (result <= 0)
                    throw new Exception("No rows affected. Possibly duplicate rental or logic prevented execution.");

                return result;

            }
            catch (SqlException ex)
            {
                // לא צריך טיפול פנימי – נזרוק החוצה לטיפול בקונטרולר
                throw;
            }
            finally
            {
                con.Close();
            }
        }


        public static List<Movies> GetRentedMoviesByUserId(int userId)
        {
            SqlConnection con = new DBservices().connect("myProjDB");
            Dictionary<string, object> paramDic = new() { { "@userId", userId } };

            SqlCommand cmd = new DBservices().CreateCommandWithStoredProcedureGeneral("SP_GetRentedMoviesByID", con, paramDic);
            List<Movies> result = new();

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Movies m = new Movies
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    PrimaryTitle = reader["PrimaryTitle"].ToString(),
                    PrimaryImage = reader["PrimaryImage"].ToString(),
                    Description = reader["Description"].ToString(),
                    Year = Convert.ToInt32(reader["Year"]),
                    ReleaseDate = Convert.ToDateTime(reader["ReleaseDate"]),
                    Language = reader["Language"].ToString(),
                    Budget = Convert.ToDouble(reader["Budget"]),
                    GrossWorldwide = Convert.ToDouble(reader["GrossWorldwide"]),
                    Genres = reader["Genres"].ToString(),
                    IsAdult = Convert.ToBoolean(reader["IsAdult"]),
                    RuntimeMinutes = Convert.ToInt32(reader["RuntimeMinutes"]),
                    AverageRating = float.Parse(reader["AverageRating"].ToString()),
                    NumVotes = Convert.ToInt32(reader["NumVotes"]),
                    PriceToRent = Convert.ToInt32(reader["PriceToRent"]),
                };
                result.Add(m);
            }

            con.Close();
            return result;
        }

        public int ForwardRentedMovie(int fromUserId, int movieId, string toUserName)
        {
            SqlConnection con = connect("myProjDB");

            Dictionary<string, object> paramDic = new()
    {
        { "@fromUserId", fromUserId },
        { "@movieId", movieId },
        { "@toUserName", toUserName }
    };

            SqlCommand cmd = CreateCommandWithStoredProcedureGeneral("SP_ForwardRentedMovie", con, paramDic);

            SqlParameter returnParam = new SqlParameter("@ReturnVal", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(returnParam);


            try
            {
                cmd.ExecuteNonQuery();
                return (int)returnParam.Value;
            }
            catch (SqlException ex)
            {
                throw new Exception($"SQL Error during ForwardRentedMovie: {ex.Message}", ex);
            }
            finally
            {
                con.Close();
            }
        }


    }
}
