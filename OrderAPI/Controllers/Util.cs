using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAPI.Tools
{
    public class Db
    {
        public static DataTable GetDataTable(string conStr,string queryStr)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    con.Open();
                    return GetDataTable(con, queryStr);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public static DataTable GetDataTable(SqlConnection con,string queryStr)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand(queryStr, con);
                cmd.CommandText = queryStr;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception e)
            {
                throw e;
            }
            return dt;
        }
        public static DataTable GetDataTable(string conStr, string queryStr, SqlParameter[] param)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    con.Open();
                    return GetDataTable(con, queryStr, param);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static DataTable GetDataTable(SqlConnection con, string queryStr, SqlParameter[] param)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand(queryStr, con);
                cmd.Parameters.AddRange(param);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                cmd.Parameters.Clear();
            }
            catch (Exception)
            {
                throw;
            }
            return dt;
        }
        private static JArray RowToJArray(DataRow row)
        {
            JArray ja = new JArray();
            foreach (var obj in row.ItemArray)
            {
                ja.Add(Convert.ToString(obj));
            }
            return ja;
        }
        public static JArray getJsonForGrid(DataTable tbl1)
        {
            JArray ja = new JArray();
            foreach (DataRow dr in tbl1.Rows)
            {
                var itemObject = new JObject();

                foreach (DataColumn dc in tbl1.Columns)
                {
                    if (dc.ColumnName == "RowNum")
                    {
                        continue;
                    }
                    else if ((dc.ColumnName == "questDateStart" && dr[dc].ToString() != "") || (dc.ColumnName == "questDateEnd" && dr[dc].ToString() != ""))
                    {
                        itemObject.Add(dc.ColumnName, Convert.ToDateTime(dr[dc]).ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        itemObject.Add(dc.ColumnName, dr[dc].ToString());
                    }
                }

                ja.Add(itemObject);
            }

            return ja;
        }
    }
    public class System
    {
   
        public static string getConStr(string conStrName)
        {

            var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json");
            var config = builder.Build();
            
            return config.GetConnectionString(conStrName);
        }
    }
}
