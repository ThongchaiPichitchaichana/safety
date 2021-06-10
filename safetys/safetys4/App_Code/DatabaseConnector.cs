using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;

    public class DatabaseConnector
    {

        //*********** sql connection function *****************
        public static string connectToDB(SqlConnection sqlConnection)
        {

            string connectionStr = ConfigurationManager.ConnectionStrings["safetys3ConnectionString"].ConnectionString;

            if (sqlConnection.State == ConnectionState.Open)
                sqlConnection.Close();
            sqlConnection.ConnectionString = connectionStr;
            sqlConnection.Open();

            return connectionStr;
        }

        public static void closeConnection(SqlConnection sqlConnection)
        {
            try
            {
                sqlConnection.Close();
            }
            catch
            {
                ;
            }
        }

        public static DataTable getAllColumns(string connectionString, string TableName, string condition)
        {
            string tempSql = "SELECT * FROM " + TableName + " WHERE " + condition;
            SqlDataAdapter tempDataAdapter = new SqlDataAdapter(tempSql, connectionString);
            DataSet tempDataSet = new DataSet();
            DataTable resultTable = new DataTable();
            tempDataAdapter.Fill(tempDataSet, "TEMP_TABLE");
            if (tempDataSet.Tables["TEMP_TABLE"].Rows.Count >= 1)
            {
                resultTable = tempDataSet.Tables["TEMP_TABLE"];
                return resultTable;
            }
            else
                return null;
        }


      

        public static DataSet GetData(string sqlCommand)
        {
            DataSet ds = new DataSet();
            string sConString = ConfigurationManager.ConnectionStrings["safetys3ConnectionString"].ToString();
            SqlConnection oConnection = new SqlConnection(sConString); // Connection String
            oConnection.Open();
            SqlCommand sqlcom = new SqlCommand(sqlCommand, oConnection);
            sqlcom.CommandTimeout = 1200;
            SqlDataAdapter sda = new SqlDataAdapter(sqlcom);
            sda.Fill(ds);
            oConnection.Close();
            return ds;
        }

       


    }