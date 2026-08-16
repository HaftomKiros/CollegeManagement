using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace CollegeManagement
{


    public class DBConnect
    {
        public MySqlConnection conn = null;
        private string host;
        private string dbname;
        private string user;
        private string pasword;

        public DBConnect(string A, string B, string C, string D)
        {
            this.host = A + "." + B + "." + C + "." + D;
            this.dbname = "ecc_dof_wukrostmarycollege";
            this.user = "root";
            this.pasword = "";
        }

        public DBConnect()
        {
            this.host = "127.0.0.1";
            this.dbname = "ecc_dof_wukrostmarycollege";
            this.user = "root";
            this.pasword = "";
        }

        public MySqlConnection getConnection (){
            try
            {
                conn = new MySqlConnection("Server=" + host + ";" + "Database=" + dbname + ";" + "UserID=" + user + ";" + "Password=" + pasword);
                return conn;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Connection Faild: "+ex.Message);
                return conn = null;
            }
        }

        public bool Open()
        {
            try
            {
                conn.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Cannot Open Connection  " + ex.Message);
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                conn.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Cannot Close Connection " + ex.Message);
                return false;
            }
        }
    }
}
