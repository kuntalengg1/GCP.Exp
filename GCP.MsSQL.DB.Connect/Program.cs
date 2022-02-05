using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCP.MsSQL.DB.Connect
{
    class Program
    {
        /// <summary>
        /// https://www.youtube.com/watch?v=pNc9QnBGKUU
        /// https://www.youtube.com/watch?v=vMUpNoukwnM
        /// https://cloud.google.com/sql/docs/sqlserver/users ---- user info
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //gcp-project-001-mssql-db1
            //sqlserver / 123456

            //gc public ip : 35.184.47.197
            //my public ip : 75.7.117.141
            /*CREATE TABLE Persons(
                PersonID int,
                LastName varchar(255),
                FirstName varchar(255),
                Address varchar(255),
                City varchar(255)
            );*/


            string connectionString = "Data Source=35.184.47.197; Initial Catalog=UserDB; User ID=sqlserver; Password=123456;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string queryStatement = "insert into Persons values (1,'Maity','Kuntal','India','Kolkata');";
                //string queryStatement = "insert into Persons values (2,'Das','Koushik','India','Malda');";

                using (SqlCommand cmd = new SqlCommand(queryStatement, con))
                {
                    con.Open();
                    cmd.ExecuteNonQuery();


                    DataTable dt = new DataTable();
                    SqlDataAdapter dap = new SqlDataAdapter("SELECT * FROM dbo.Persons ORDER BY PersonID", con);


                    dap.Fill(dt);
                    con.Close();

                }
            }
        }
    }
}
