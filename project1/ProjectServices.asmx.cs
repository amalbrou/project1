using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

// need these to talk to mysql
using MySql.Data;
using MySql.Data.MySqlClient;
// need this to manipulate data from a database
using System.Data;

namespace project1
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        // test method to make sure everything is working! can be commented out later :)
        [WebMethod]
        public int NumberofAccounts()
        {
            // grab the connection string from the web.config file
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            // the query to run on the database
            string sqlSelect = "SELECT * from accounts";

            // connection object
            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            // command object uses connection and query
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            // data adapter is a bridge between command and data we're wanting back
            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
            // table we want to fill with query results
            DataTable sqlDt = new DataTable();
            // filling the table
            sqlDa.Fill(sqlDt);
            // return number of rows
            return sqlDt.Rows.Count;
        }

        //EXAMPLE OF A SIMPLE SELECT QUERY (PARAMETERS PASSED IN FROM CLIENT)
        [WebMethod(EnableSession = true)] //NOTICE: gotta enable session on each individual method
        public bool LogOn(string uid, string pass)
        {
            //we return this flag to tell them if they logged in or not
            bool success = false;

            //our connection string comes from our web.config file like we talked about earlier
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            //here's our query.  A basic select with nothing fancy.  Note the parameters that begin with @
            //NOTICE: we added admin to what we pull, so that we can store it along with the id in the session
            string sqlSelect = "SELECT Id, Admin FROM accounts WHERE Username=@idValue and Password=@passValue";

            //set up our connection object to be ready to use our connection string
            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            //set up our command object to use our connection, and our query
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            //tell our command to replace the @parameters with real values
            //we decode them because they came to us via the web so they were encoded
            //for transmission (funky characters escaped, mostly)
            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
            sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));

            //a data adapter acts like a bridge between our command object and 
            //the data we are trying to get back and put in a table object
            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
            //here's the table we want to fill with the results from our query
            DataTable sqlDt = new DataTable();
            //here we go filling it!
            sqlDa.Fill(sqlDt);
            //check to see if any rows were returned.  If they were, it means it's 
            //a legit account
            if (sqlDt.Rows.Count > 0)
            {
                //if we found an account, store the id and admin status in the session
                //so we can check those values later on other method calls to see if they 
                //are 1) logged in at all, and 2) and admin or not
                Session["id"] = sqlDt.Rows[0]["id"];
                Session["admin"] = sqlDt.Rows[0]["admin"];
                success = true;
            }
            //return the result!
            return success;
        }


        [WebMethod(EnableSession = true)]
        public bool LogOff()
        {
            //if they log off, then we remove the session.  That way, if they access
            //again later they have to log back on in order for their ID to be back
            //in the session!
            Session.Abandon();
            return true;
        }

        //EXAMPLE OF AN INSERT QUERY WITH PARAMS PASSED IN.  BONUS GETTING THE INSERTED ID FROM THE DB!
        [WebMethod(EnableSession = true)]
        public void RequestAccount(string uid, string pass, string firstName, string lastName, string email, string phone)
        {
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            //the only thing fancy about this query is SELECT LAST_INSERT_ID() at the end.  All that
            //does is tell mySql server to return the primary key of the last inserted row.
            string sqlSelect = "insert into accounts (Username, Firstname, Lastname, Email, Phone, Password) " +
                "values(@idValue, @fnameValue, @lnameValue, @emailValue, @phoneValue, @passValue); SELECT LAST_INSERT_ID();";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
            sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));
            sqlCommand.Parameters.AddWithValue("@fnameValue", HttpUtility.UrlDecode(firstName));
            sqlCommand.Parameters.AddWithValue("@lnameValue", HttpUtility.UrlDecode(lastName));
            sqlCommand.Parameters.AddWithValue("@emailValue", HttpUtility.UrlDecode(email));
            sqlCommand.Parameters.AddWithValue("@phoneValue", HttpUtility.UrlDecode(phone));

            //this time, we're not using a data adapter to fill a data table.  We're just
            //opening the connection, telling our command to "executescalar" which says basically
            //execute the query and just hand me back the number the query returns (the ID, remember?).
            //don't forget to close the connection!
            sqlConnection.Open();
            //we're using a try/catch so that if the query errors out we can handle it gracefully
            //by closing the connection and moving on
            try
            {
                int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
                //here, you could use this accountID for additional queries regarding
                //the requested account.  Really this is just an example to show you
                //a query where you get the primary key of the inserted row back from
                //the database!
            }
            catch (Exception e)
            {
            }
            sqlConnection.Close();
        }

        [WebMethod(EnableSession = true)]
        public void RequestClass(string uid, string professor, string days, string time, string comments)
        {
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            //the only thing fancy about this query is SELECT LAST_INSERT_ID() at the end.  All that
            //does is tell mySql server to return the primary key of the last inserted row.
            string sqlSelect = "insert into classes (ClassName, Professor, Days, Time, Comments) " +
                "values(@idValue, @profValue, @dayValue, @timeValue, @comValue); SELECT LAST_INSERT_ID();";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
            sqlCommand.Parameters.AddWithValue("@profValue", HttpUtility.UrlDecode(professor));
            sqlCommand.Parameters.AddWithValue("@dayValue", HttpUtility.UrlDecode(days));
            sqlCommand.Parameters.AddWithValue("@timeValue", HttpUtility.UrlDecode(time));
            sqlCommand.Parameters.AddWithValue("@comValue", HttpUtility.UrlDecode(comments));

            //this time, we're not using a data adapter to fill a data table.  We're just
            //opening the connection, telling our command to "executescalar" which says basically
            //execute the query and just hand me back the number the query returns (the ID, remember?).
            //don't forget to close the connection!
            sqlConnection.Open();
            //we're using a try/catch so that if the query errors out we can handle it gracefully
            //by closing the connection and moving on
            try
            {
                int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
                //here, you could use this accountID for additional queries regarding
                //the requested account.  Really this is just an example to show you
                //a query where you get the primary key of the inserted row back from
                //the database!
            }
            catch (Exception e)
            {
            }
            sqlConnection.Close();
        }

        [WebMethod(EnableSession = true)]
        public void RequestAssignment(string uid, string date, string time, string comments, string className)
        {
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            //the only thing fancy about this query is SELECT LAST_INSERT_ID() at the end.  All that
            //does is tell mySql server to return the primary key of the last inserted row.
            string sqlSelect = "insert into assignments (AssignmentName, DueDate, DueTime, Comments, ClassName) " +
                "values(@idValue, @dateValue, @timeValue, @comValue, @classValue); SELECT LAST_INSERT_ID();";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
            sqlCommand.Parameters.AddWithValue("@dateValue", HttpUtility.UrlDecode(date));
            sqlCommand.Parameters.AddWithValue("@timeValue", HttpUtility.UrlDecode(time));
            sqlCommand.Parameters.AddWithValue("@comValue", HttpUtility.UrlDecode(comments));
            sqlCommand.Parameters.AddWithValue("@classValue", HttpUtility.UrlDecode(className));

            //this time, we're not using a data adapter to fill a data table.  We're just
            //opening the connection, telling our command to "executescalar" which says basically
            //execute the query and just hand me back the number the query returns (the ID, remember?).
            //don't forget to close the connection!
            sqlConnection.Open();
            //we're using a try/catch so that if the query errors out we can handle it gracefully
            //by closing the connection and moving on
            try
            {
                int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
                //here, you could use this accountID for additional queries regarding
                //the requested account.  Really this is just an example to show you
                //a query where you get the primary key of the inserted row back from
                //the database!
            }
            catch (Exception e)
            {
            }
            sqlConnection.Close();
        }

        [WebMethod(EnableSession = true)]
        public void DeleteClass(string className)
        {
                string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
                //this is a simple update, with parameters to pass in values
                string sqlSelect = "delete from classes where ClassName=@classValue";

                MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
                MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

                sqlCommand.Parameters.AddWithValue("@classValue", HttpUtility.UrlDecode(className));

                sqlConnection.Open();
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                }
                sqlConnection.Close();
        }

        [WebMethod(EnableSession = true)]
        public ClassInfo[] GetClasses()
        {
            //check out the return type.  It's an array of Account objects.  You can look at our custom Account class in this solution to see that it's 
            //just a container for public class-level variables.  It's a simple container that asp.net will have no trouble converting into json.  When we return
            //sets of information, it's a good idea to create a custom container class to represent instances (or rows) of that information, and then return an array of those objects.  
            //Keeps everything simple.

            //WE ONLY SHARE ACCOUNTS WITH LOGGED IN USERS!
            if (Session["id"] != null)
            {
                DataTable sqlDt = new DataTable("classes");

                string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
                string sqlSelect = "select CID, UID, ClassName, Professor, Days, Time from classes";

                MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
                MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

                //gonna use this to fill a data table
                MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
                //filling the data table
                sqlDa.Fill(sqlDt);

                //loop through each row in the dataset, creating instances
                //of our container class Account.  Fill each acciount with
                //data from the rows, then dump them in a list.
                List<ClassInfo> classes = new List<ClassInfo>();
                for (int i = 0; i < sqlDt.Rows.Count; i++)
                {
                        classes.Add(new ClassInfo
                        {
                            cid = Convert.ToInt32(sqlDt.Rows[i]["cid"]),
                            uid = Convert.ToInt32(sqlDt.Rows[i]["uid"]),
                            className = sqlDt.Rows[i]["className"].ToString(),
                            professor = sqlDt.Rows[i]["professor"].ToString(),
                            days = sqlDt.Rows[i]["days"].ToString(),
                            time = sqlDt.Rows[i]["time"].ToString()
                        });
                }
                //convert the list of accounts to an array and return!
                return classes.ToArray();
            }
            else
            {
                //if they're not logged in, return an empty array
                return new ClassInfo[0];
            }
        }
    }
}
