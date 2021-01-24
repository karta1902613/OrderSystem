using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace OrderAPI.Controllers.Admin
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration configuration;
        /*
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "V1", "V2" };
        }
        */
        public AdminController(IConfiguration config)
        {
            this.configuration = config;
        }
        [HttpPost]
        public IActionResult test()
        {
            JObject jo = new JObject();
            jo.Add("TEst", "test");
            return Content(JsonConvert.SerializeObject(jo), "application/json");            
        }
        [HttpPost]
        public IActionResult test2()
        {
            JObject jo = new JObject();
            
            string connectionstring = configuration.GetConnectionString("MyDb");
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(connectionstring);
            connection.Open();
            SqlCommand com = new SqlCommand("select * from S00_menus", connection);
            com.CommandText = "select * from S00_menus";
            SqlDataAdapter da = new SqlDataAdapter(com);
            da.Fill(dt);
            connection.Close();
            JArray ja = new JArray();
            
            jo.Add("TEst", "test2");
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
        [HttpGet]
        public IActionResult getMenuTree()
        {
            JObject jo = new JObject();
            string sql = "select sysMenuId,SysParentId,menuId,menuName,funcType,url,sortValue,icon from S00_menus where statusId = '10'";
            DataTable dt = Tools.Db.GetDataTable(Tools.System.getConStr("MyDB"), sql);
            JArray ja = new JArray();
            
            foreach (DataRow dr in dt.Rows)
            {
                var itemObject = new JObject();


                if (dr["SysParentId"].ToString() == "0")
                {
                    foreach (DataColumn dc in dt.Columns)
                    {
                        itemObject.Add(dc.ColumnName, dr[dc].ToString());
                    }
                    JArray ja1 = new JArray();
                    
                    foreach (DataRow dr1 in dt.Rows)
                    {
                        var itemObject1 = new JObject();
                        if (dr["SysMenuId"].ToString() == dr1["SysParentId"].ToString())
                        {
                            foreach (DataColumn dc in dt.Columns)
                            {
                                itemObject1.Add(dc.ColumnName, dr1[dc].ToString());
                            }
                            ja1.Add(itemObject1);
                        }
                        
                    }
                    itemObject.Add("detail",ja1);
                    ja.Add(itemObject);
                }
                
                
                
            }
            jo.Add("menu", ja);
            //return Ok(jo);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
    }
}
