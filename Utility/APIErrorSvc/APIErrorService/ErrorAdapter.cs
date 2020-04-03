using System;
using System.Data;
using System.Data.Odbc;

namespace BC.Integration.APIError
{
    public static class ErrorAdapter
    {

        static ErrorDBConnection connection;

        /// <summary>
        /// Inserts error in the staging table. Returns true if insert is succesful
        /// </summary>
        /// <param name="error"></param>

        public static bool InsertError(Error error)
        {
            bool success = false;
            try
            {
                connection = new ErrorDBConnection();
                //open connection
                if (connection.Open())
                {
                    //create command and assign the query and connection from the constructor
                    OdbcCommand cmd = new OdbcCommand();

                    cmd = new OdbcCommand("{Call ErrorInsert(?,?,?,?,?,?,?)}", connection.GetConnection());
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(error.ErrorType, OdbcType.VarChar);
                    cmd.Parameters.Add(error.ErrorTypeId.ToString(), OdbcType.VarChar);
                    cmd.Parameters.Add(error.CreateErrorMsg(), OdbcType.VarChar);
                    cmd.Parameters.Add(error.DocumentId,OdbcType.VarChar);
                    cmd.Parameters.Add(error.Division, OdbcType.VarChar);
                    cmd.Parameters.Add(error.Location, OdbcType.VarChar);
                    cmd.Parameters.Add(error.Customer, OdbcType.VarChar);

                    //Execute command

                    int i = cmd.ExecuteNonQuery();

                    //close connection
                    connection.Close();                                                                                                                                                                                            

                }

            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured in InsertNotification() method while trying to insert Notifications", ex);
            }
            return success;
        }
        public static void GetLastError()
        {

        }
        /// <summary>
        /// Fetches the error in the ERP system and populates the Error Object with the data 
        /// returned by the Stored Procedure executed.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        /*    public static Error PopulateError(Error error)
            {


                connection = new ErrorDBConnection();
                connection.Initialize();

                SqlCommand command = new SqlCommand("FETCH_error_DETAILS", connection.GetConnection())
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@SOPNUMBE", error.header);
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                List<ErrorLine> ErrorLines = new List<ErrorLine>();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    error = new Error();

                    while (reader.Read())
                    {

                     //   error.Header.DocumentID = reader["DOCID"].ToString();


                    }
                    error.errorLines = PopulateErrorLine("");

                }
                catch (Exception ex)
                {

                    throw ex;
                }
                finally
                {
                    connection.Close();

                }
                return error;
            }*/
        /*private static List<ErrorLine> PopulateErrorLine(string errorNumber)
        {
            connection = new ErrorDBConnection();
            connection.Initialize();

            SqlCommand command = new SqlCommand("FETCH_error_LINES", connection.GetConnection())
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@errorNumber", errorNumber.Trim());
            command.Connection = connection.GetConnection();
            connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(command);


            List<ErrorLine> ErrorLines = new List<ErrorLine>();

            try
            {
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    ErrorLine line = new ErrorLine();
                    ErrorLines.Add(line);

                }
            }
            catch (SqlException ex)
            {

                throw ex;
            }
            finally
            {
                connection.Close();
            }


            return ErrorLines;
        }*/

    }


}