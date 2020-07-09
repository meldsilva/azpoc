using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace azpoc
{

    public static class PageUtility
    {
        public static void MessageBox(System.Web.UI.Page page, string strMsg)
        {
            //+ character added after strMsg "')"
            ScriptManager.RegisterClientScriptBlock(page, page.GetType(), "alertMessage", "alert('" + strMsg + "')", true);

        }
    }

    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //DB Read-Write connection
                connstr_rw.Text = GetDBServer(true);
                connstr_r.Text = GetDBServer(false);
                //Web app Host
                webserver.Text = System.Web.Hosting.HostingEnvironment.ApplicationHost.GetSiteName();

                this.BindGrid();
            }
        }

        private void BindGrid()
        {
            string strConnString = ConfigurationManager.ConnectionStrings["AZ-POCDbRead"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "select * from Employee";
                    cmd.Connection = con;
                    con.Open();
                    GridView1.DataSource = cmd.ExecuteReader();
                    GridView1.DataBind();
                    con.Close();
                }
            }
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            if (fname.Text == "" || lname.Text == "" || city.Text == "" || state.Text == "")
            {
                PageUtility.MessageBox(this, "Please enter all values!");
                return;
            }

            string strConnString = ConfigurationManager.ConnectionStrings["AZ-POCDbWrite"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO [EMPLOYEE](FirstName,LastName, City, State) VALUES(@fname,@lname,@city,@state)";
                    cmd.Connection = con;
                    con.Open();
                    cmd.Parameters.AddWithValue("fname", fname.Text);
                    cmd.Parameters.AddWithValue("lname", lname.Text);
                    cmd.Parameters.AddWithValue("city", city.Text);
                    cmd.Parameters.AddWithValue("state", state.Text);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                this.BindGrid();
            }
            //Clear text boxes
            fname.Text = lname.Text = city.Text = state.Text = "";
        }

        //protected void submit_Click(object sender, EventArgs e)
        //{
        //    if(fname.Text == "" || lname.Text == "" || city.Text == "" || state.Text == "")
        //    {
        //        PageUtility.MessageBox(this, "Please enter all values!");
        //        return;
        //    }


        //    SqlDataSource1.InsertCommandType = SqlDataSourceCommandType.Text;
        //    SqlDataSource1.InsertCommand = "INSERT INTO [EMPLOYEE](FirstName,LastName, City, State) VALUES(@fname,@lname,@city,@state)";
        //    SqlDataSource1.InsertParameters.Add("fname", fname.Text);
        //    SqlDataSource1.InsertParameters.Add("lname", lname.Text);
        //    SqlDataSource1.InsertParameters.Add("city", city.Text);
        //    SqlDataSource1.InsertParameters.Add("state", state.Text);
        //    SqlDataSource1.Insert();

        //    GridView1.DataBind();
        //    SqlDataSource1.DataBind();

        //    //Clear text boxes
        //    fname.Text = lname.Text = city.Text = state.Text = "";
        //}

        //protected void submit_Click(object sender, EventArgs e)
        //{
        //    string connStr = GetConnectionStrings();
        //    //PageUtility.MessageBox(this, connStr);
        //    string f = fname.Text;
        //    string l = lname.Text;
        //    string c = city.Text;
        //    string s = state.Text;

        //    try
        //    {
        //        SqlConnection conn = new SqlConnection(connStr);
        //        string sql = String.Format("INSERT INTO DBO.Employee (FirstName, LastName, City, [State]) VALUES ('{0}', '{1}', '{2}', '{3}'" + ")", f, l, c, s);

        //        SqlCommand cmd = new SqlCommand(sql, conn);
        //        conn.Open();
        //        cmd.ExecuteNonQuery();
        //        conn.Close();
        //    }
        //    catch {
        //        int v1 = 0;
        //    }

        //    //PageUtility.MessageBox(this, (String.Format("Employe {0} {1} successfully added!",f, l)));
        //    fname.Text = lname.Text = city.Text = state.Text = "";
        //}

        protected void clear_Click(object sender, EventArgs e)
        {
            fname.Text = lname.Text = city.Text = state.Text = "";
        }

        protected string GetDBServer(bool writeDB = false)
        {
            string connStr = GetConnectionStrings(writeDB);
            string dbName = "ERROR: Check connection string";
            try
            {
                SqlConnection conn = new SqlConnection(connStr);
                SqlCommand cmd = new SqlCommand("Select @@servername", conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        dbName = reader.GetString(0);
                    }
                }
                reader.Close();
                conn.Close();
            }
            catch
            {
                throw;
            }

            return dbName;
        }

        static string GetConnectionStrings(bool writeDB = false)
        {
            ConnectionStringSettingsCollection settings =
                ConfigurationManager.ConnectionStrings;

            string connstr = null;

            if (settings != null)
            {
                foreach (ConnectionStringSettings cs in settings)
                {
                    if (cs.Name == "AZ-POCDbWrite" && writeDB)
                    {
                        connstr = cs.ConnectionString;
                    }
                    else if(cs.Name == "AZ-POCDbRead" && !writeDB)
                    {
                        connstr = cs.ConnectionString;
                    }
                
                }
            }
            return connstr;
        }
        protected void GridView1_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            //NewEditIndex property used to determine the index of the row being edited.  
            GridView1.EditIndex = e.NewEditIndex;
            BindGrid();
        }
        protected void GridView1_RowUpdating(object sender, System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            //Finding the controls from Gridview for the row which is going to update  
            string id = (GridView1.Rows[e.RowIndex].FindControl("Id") as Label).Text;
            string fname = (GridView1.Rows[e.RowIndex].FindControl("FirstName") as TextBox).Text;
            TextBox lname = GridView1.Rows[e.RowIndex].FindControl("lastName") as TextBox;
            TextBox city = GridView1.Rows[e.RowIndex].FindControl("City") as TextBox;
            TextBox state = GridView1.Rows[e.RowIndex].FindControl("State") as TextBox;

            string strConnString = ConfigurationManager.ConnectionStrings["AZ-POCDbWrite"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE Employee SET FirstName = @FirstName, LastName = @LastName, City = @City , State = @State WHERE ID = @ID";
                    cmd.Connection = con;
                    con.Open();
                    cmd.Parameters.AddWithValue("fname", fname);
                    cmd.Parameters.AddWithValue("lname", lname.Text);
                    cmd.Parameters.AddWithValue("city", city.Text);
                    cmd.Parameters.AddWithValue("state", state.Text);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                GridView1.EditIndex = -1;
                //Call ShowData method for displaying updated data  
                this.BindGrid();
            }
        }

        protected void GridView1_RowCancelingEdit(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            //Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
            GridView1.EditIndex = -1;
            this.BindGrid();
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

    }

}

