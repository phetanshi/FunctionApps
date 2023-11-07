using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp3
{
    public class Repository
    {
        private string _sqlConn = "Data Source=PADMASEKHAR-NEW;Initial Catalog=SkillCentral;Persist Security Info=True;User ID=DevCommon;Password=123456;TrustServerCertificate=True";
        public bool CreateEmployee(List<string> lines)
        {
            int count = 0;
            using (SqlConnection con = new SqlConnection(_sqlConn))
            {
                StringBuilder sqlCmd = new StringBuilder();
                

                foreach (string line in lines)
                {
                    var lineData = line.Trim().Replace("\r", "").Split(',');
                    if(lineData.Length == 3 )
                    {
                        sqlCmd.Append($"INSERT INTO emp.tblEmployees (EMPLOYEE_ID, FIRST_NAME, LAST_NAME) VALUES ('{lineData[0]}','{lineData[1]}','{lineData[2]}');");
                        sqlCmd.AppendLine();
                    }
                }

                using (SqlCommand cmd = new SqlCommand(sqlCmd.ToString(), con))
                {
                    con.Open();
                    count = cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return count > 0;
        }
    }
}
