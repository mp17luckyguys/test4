using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Data.SqlClient;

namespace FamilyGo.Utils
{
    public class ExcelToDB
    {
        public void Page_Load()
        {
            FileSvr fileSvr = new FileSvr();
            System.Data.DataTable dt = fileSvr.GetExcelDatatable("E:\\MIT\\5120\\Project\\data\\Games.xlsx", "Sheet1");
            fileSvr.InsetData(dt);
        }
    }
    class FileSvr
    {
        /// <summary>  
        /// Excel数据导入Datable  
        /// </summary>  
        /// <param name="fileUrl"></param>  
        /// <param name="table"></param>  
        /// <returns></returns>  
        public System.Data.DataTable GetExcelDatatable(string fileUrl, string table)
        {
            //office2007之前 仅支持.xls  
            //const string cmdText = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;IMEX=1';";  
            //支持.xls和.xlsx，即包括office2010等版本的 HDR=Yes代表第一行是标题，不是数据；  
            const string cmdText = "Provider=Microsoft.Ace.OleDb.12.0;Data Source={0};Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";
            System.Data.DataTable dt = null;
            //建立连接  
            OleDbConnection conn = new OleDbConnection(string.Format(cmdText, fileUrl));
            try
            {
                //打开连接  
                if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                System.Data.DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                //获取Excel的第一个Sheet名称  
                string sheetName = schemaTable.Rows[0]["TABLE_NAME"].ToString().Trim();
                //查询sheet中的数据  
                string strSql = "select * from [" + sheetName + "]";
                OleDbDataAdapter da = new OleDbDataAdapter(strSql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds, table);
                dt = ds.Tables[0];
                return dt;
            }
            catch (Exception exc)
            {
                throw exc;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }
        /// <summary>  
        /// 从System.Data.DataTable导入数据到数据库  
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public int InsetData(System.Data.DataTable dt)
        {
            int i = 0;
            string name = "";
            string description = "";
            foreach (DataRow dr in dt.Rows)
            {
                name = dr["Name"].ToString().Trim();
                description = dr["Description"].ToString().Trim();
                //sw = string.IsNullOrEmpty(sw) ? "null" : sw;  
                //kr = string.IsNullOrEmpty(kr) ? "null" : kr;  
                string strSql = string.Format("Insert into Games (Name,Description) Values ('{0}','{1}')", name, description);
                string strConnection = ConfigurationManager.ConnectionStrings["FamilyGoModelContainer"].ToString();
                SqlConnection sqlConnection = new SqlConnection(strConnection);
                try
                {
                    // SqlConnection sqlConnection = new SqlConnection(strConnection);  
                    sqlConnection.Open();
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.CommandText = strSql;
                    sqlCmd.Connection = sqlConnection;
                    SqlDataReader sqlDataReader = sqlCmd.ExecuteReader();
                    i++;
                    sqlDataReader.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
                //if (opdb.ExcSQL(strSql))  
                // i++;  
            }
            return i;
        }
    }
}