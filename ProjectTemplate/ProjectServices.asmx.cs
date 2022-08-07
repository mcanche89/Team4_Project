using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.Script.Serialization;

namespace ProjectTemplate
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]

	public class ProjectServices : System.Web.Services.WebService
	{
		private string dbID = "440sum20224";
		private string dbPass = "440sum20224";
		private string dbName = "440sum20224";

		private string getConString()
		{
			return "SERVER=107.180.1.16; PORT=3306; DATABASE=" + dbName + "; UID=" + dbID + "; PASSWORD=" + dbPass;
		}


		//Maximo added to get log on function working

		[WebMethod(EnableSession = true)]
		public bool LogOn(string UserID, string Password)
		{
			//we return this flag to tell them if they logged in or not
			bool success = false;


			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["440sum20224"].ConnectionString;

			string sqlSelect = "SELECT UserID FROM Employee WHERE UserID=@idValue and Password=@passValue";


			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);

			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(UserID));
			sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(Password));


			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);

			DataTable sqlDt = new DataTable();

			sqlDa.Fill(sqlDt);

			if (sqlDt.Rows.Count > 0)
			{
				success = true;
			}

			//return the result!
			return success;

		}

		// end what Maximo added


		//Maximo added this to send an insert statement to the database

		[WebMethod(EnableSession = true)]
		public void CreateSuggestion(string UserID, string Title, string SuggestionBody, string Department, string Anonymous)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["440sum20224"].ConnectionString;

			string sqlSelect = "insert into Suggestions (UserID, Title, SuggestionBody, Department, Anonymous) " +
				"values(@userID, @titleValue, @suggestionBodyValue, @departmentValue, @anonymous);";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);


			sqlCommand.Parameters.AddWithValue("@userID", HttpUtility.UrlDecode(UserID));
			sqlCommand.Parameters.AddWithValue("@titleValue", HttpUtility.UrlDecode(Title));
			sqlCommand.Parameters.AddWithValue("@suggestionBodyValue", HttpUtility.UrlDecode(SuggestionBody));
			sqlCommand.Parameters.AddWithValue("@departmentValue", HttpUtility.UrlDecode(Department));
			sqlCommand.Parameters.AddWithValue("@anonymous", HttpUtility.UrlDecode(Anonymous));


			sqlConnection.Open();

			try
			{
				int SuggestionID = Convert.ToInt32(sqlCommand.ExecuteScalar());
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		//end what Maximo added


		//Maximo added log off function service, needs to be implimented on the homepage.html

		[WebMethod(EnableSession = true)]
		public bool LogOff()
		{
			Session.Abandon();
			return true;
		}


		//this is just to test the database connection string 
		[WebMethod(EnableSession = true)]

		public string TestConnection()
		{
			try
			{
				string testQuery = "select * from Employee";

				MySqlConnection con = new MySqlConnection(getConString());

				MySqlCommand cmd = new MySqlCommand(testQuery, con);
				MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
				DataTable table = new DataTable();
				adapter.Fill(table);
				return "Success!";
			}
			catch (Exception e)
			{
				return "Something went wrong, please check your credentials and db name and try again.  Error: " + e.Message;
			}
		}


		//this function returns all suggestions for a specific userid in json
		[WebMethod(EnableSession = true)]

		public String MySuggestionQuery(string UserID)
		{

			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["440sum20224"].ConnectionString;

			string sqlSelect = "SELECT Title FROM Suggestions WHERE UserID=@idValue";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(UserID));

			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);


			DataTable sqlDt = new DataTable();

			sqlDa.Fill(sqlDt);

			JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
			List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
			Dictionary<string, object> childRow;
			foreach (DataRow row in sqlDt.Rows)
			{
				childRow = new Dictionary<string, object>();
				foreach (DataColumn col in sqlDt.Columns)
				{
					childRow.Add(col.ColumnName, row[col]);
				}
				parentRow.Add(childRow);
			}
			return jsSerializer.Serialize(parentRow);
		}


		//this function returns all suggestions in json
		[WebMethod(EnableSession = true)]

		public String AllSuggestionQuery()
		{

			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["440sum20224"].ConnectionString;

			string sqlSelect = "SELECT Title FROM Suggestions";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);


			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);


			DataTable sqlDt = new DataTable();

			sqlDa.Fill(sqlDt);

			JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
			List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
			Dictionary<string, object> childRow;
			foreach (DataRow row in sqlDt.Rows)
			{
				childRow = new Dictionary<string, object>();
				foreach (DataColumn col in sqlDt.Columns)
				{
					childRow.Add(col.ColumnName, row[col]);
				}
				parentRow.Add(childRow);
			}
			return jsSerializer.Serialize(parentRow);
		}


		//this function returns the detail info for a specific suggestion in json, suggestion title is passed as parameter
		[WebMethod(EnableSession = true)]

		public String GetMySuggestionInfo(string UserID, string Title)
		{

			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["440sum20224"].ConnectionString;

			string sqlSelect = "SELECT UserID, Title, SuggestionBody, Status, Department FROM Suggestions WHERE Title = @titleValue AND UserID = @idValue";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(UserID));
			sqlCommand.Parameters.AddWithValue("@titleValue", HttpUtility.UrlDecode(Title));

			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);


			DataTable sqlDt = new DataTable();

			sqlDa.Fill(sqlDt);

			JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
			List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
			Dictionary<string, object> childRow;
			foreach (DataRow row in sqlDt.Rows)
			{
				childRow = new Dictionary<string, object>();
				foreach (DataColumn col in sqlDt.Columns)
				{
					childRow.Add(col.ColumnName, row[col]);
				}
				parentRow.Add(childRow);
			}
			return jsSerializer.Serialize(parentRow);
		}


		//this function returns the detail info for a specific suggestion in json, suggestion title is passed as parameter
		[WebMethod(EnableSession = true)]

		public String GetCommunitySuggestionInfo(string Title)
		{

			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["440sum20224"].ConnectionString;

			string sqlSelect = "SELECT Title, Department, SuggestionBody, Status, UserID FROM Suggestions WHERE Title = @titleValue";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@titleValue", HttpUtility.UrlDecode(Title));

			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);


			DataTable sqlDt = new DataTable();

			sqlDa.Fill(sqlDt);

			JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
			List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
			Dictionary<string, object> childRow;
			foreach (DataRow row in sqlDt.Rows)
			{
				childRow = new Dictionary<string, object>();
				foreach (DataColumn col in sqlDt.Columns)
				{
					childRow.Add(col.ColumnName, row[col]);
				}
				parentRow.Add(childRow);
			}
			return jsSerializer.Serialize(parentRow);
		}


		[WebMethod(EnableSession = true)]

		public String UserNameQuery(string UserID)
		{

			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["440sum20224"].ConnectionString;

			string sqlSelect = "SELECT FirstName FROM Employee WHERE UserID=@idValue";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(UserID));

			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);


			DataTable sqlDt = new DataTable();

			sqlDa.Fill(sqlDt);

			JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
			List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
			Dictionary<string, object> childRow;
			foreach (DataRow row in sqlDt.Rows)
			{
				childRow = new Dictionary<string, object>();
				foreach (DataColumn col in sqlDt.Columns)
				{
					childRow.Add(col.ColumnName, row[col]);
				}
				parentRow.Add(childRow);
			}
			return jsSerializer.Serialize(parentRow);
		}
	}
}
