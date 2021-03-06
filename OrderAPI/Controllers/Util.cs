﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Dapper;
using System.Linq;
using System.Threading.Tasks;
using OrderAPI.Model;
using Newtonsoft.Json;

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
        public static string ExecuteNonQuery(string conStr, string queryStr)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(queryStr, con);                
                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    cmd.Parameters.Clear();
                    return ex.Message;
                }
            }
            return "";
        }
        public static string ExecuteNonQuery(string conStr, string queryStr, SqlParameter[] param)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(queryStr, con);
                cmd.Parameters.AddRange(param);
                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    cmd.Parameters.Clear();
                    return ex.Message;
                }
            }
            return "";
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

    public class Dapper
    {
        public static string ExecuteNonQuery(string conStr, string queryStr, object parma)
        {
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Execute(queryStr, parma);
            }
            return "";
        }
        public static string QueryDataToJson(string conStr,string queryStr)
        {
            var json = "";
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                var queryResult = conn.Query(queryStr);
                json = JsonConvert.SerializeObject(queryResult);                
            }
            return json;
        }
        public static string QueryDataToJson(string conStr, string queryStr, object param)
        {
            var json = "";
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                var queryResult = conn.Query(queryStr,param); 
                json = JsonConvert.SerializeObject(queryResult);
            }
            return json;
        }
        public static string QueryFirstOrDefault(string conStr,string queryStr)
        {            
            using (SqlConnection conn = new SqlConnection(conStr))
            {                               
                var query = conn.Query(queryStr).FirstOrDefault();                                
                query.Dump();
                return query;
            }
        }
        public static string QuerySingleOrDefault(string conStr, string queryStr, object param)
        {
            //var id = "";
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                var id = conn.QuerySingle<int>(queryStr.ToString(), param);
                //var query = conn.QuerySingleOrDefault(queryStr.ToString(), param);
                //String id = query.values[0].toString();
                //query.Dump();
                return id.ToString();
            }
            
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
