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

    public partial class Default : Page
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

                //Load Gridview with the contents of Employee table
                this.BindGrid();
                //rowcount.Text = "0";
            }
        }

        private void BindGrid()
        {
            string constr = ConfigurationManager.ConnectionStrings["AZ-POCDbRead"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT [Id],[FirstName],[LastName],[City],[State] FROM [dbo].[Employee]"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            GridView1.DataSource = dt;
                            GridView1.DataBind();
                            
                            rowcount.Text = "Row Count: " + dt.Rows.Count.ToString();

                        }
                    }
                }
            }
        }

        private void GetRowCount()
        {
            string constr = ConfigurationManager.ConnectionStrings["AZ-POCDbRead"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM [dbo].[Employee]"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable())
                        {
                            //
                        }
                    }
                }
            }
        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowIndex != GridView1.EditIndex)
            {
                (e.Row.Cells[0].Controls[2] as LinkButton).Attributes["onclick"] = "return confirm('Do you want to delete this row?');";
            }


            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = "Actions";

                e.Row.Cells[1].Text = "ID";
                e.Row.Cells[1].Width = 100;
                e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Right;

                e.Row.Cells[2].Text = "First Name";
                e.Row.Cells[2].Width = 300;

                e.Row.Cells[3].Text = "Last Name";
                e.Row.Cells[3].Width = 300;

                e.Row.Cells[4].Text = "City";
                e.Row.Cells[4].Width = 300;
                
                e.Row.Cells[5].Text = "State";
                e.Row.Cells[5].Width = 300;
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Right;
            }
        }

        protected void OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            this.BindGrid();
        }


        protected void OnRowCancelingEdit(object sender, EventArgs e)
        {
            GridView1.EditIndex = -1;
            this.BindGrid();
        }


        protected void OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = GridView1.Rows[e.RowIndex];
            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
            string fname = (row.Cells[2].Controls[0] as TextBox).Text;
            string lname= (row.Cells[3].Controls[0] as TextBox).Text;
            string city = (row.Cells[4].Controls[0] as TextBox).Text;
            string state = (row.Cells[5].Controls[0] as TextBox).Text;
            

            string constr = ConfigurationManager.ConnectionStrings["AZ-POCDbWrite"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE DBO.Employee SET FirstName = @FirstName, LastName = @LastName, City = @City, State = @State WHERE ID = @ID"))
                {
                    cmd.Parameters.AddWithValue("ID", id);
                    cmd.Parameters.AddWithValue("FirstName", fname);
                    cmd.Parameters.AddWithValue("LastName", lname);
                    cmd.Parameters.AddWithValue("City", city);
                    cmd.Parameters.AddWithValue("State", state);
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            GridView1.EditIndex = -1;
            this.BindGrid();
        }

        protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
            string constr = ConfigurationManager.ConnectionStrings["AZ-POCDbWrite"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("DELETE FROM DBO.Employee WHERE ID = @ID"))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            this.BindGrid();
        }

        /// <summary>
        /// Insert Row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        protected void clear_Click(object sender, EventArgs e)
        {
            fname.Text = lname.Text = city.Text = state.Text = "";
        }

        protected void refresh_Click(object sender, EventArgs e)
        {
            this.BindGrid();
        }

        protected string GetDBServer(bool writeDB = false)
        {
            string connStr = "ERROR: Check connection string";
            connStr = GetConnectionStrings(writeDB);
            string hostname = "N/A";

            try
            {
                SqlConnection conn = new SqlConnection(connStr);
                SqlCommand cmd = new SqlCommand("Select @@Servername", conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        hostname = reader.GetString(0);
                    }
                }
                reader.Close();
                conn.Close();
            }
            catch
            {
                throw;
            }

            if (hostname != "N/A")
            {
                //string connectString = ConfigurationManager.ConnectionStrings["connStr"].ToString();
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connStr);
                hostname = builder.DataSource;
            }
            return hostname;
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
    }
}

