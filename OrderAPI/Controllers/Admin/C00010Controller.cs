using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OrderAPI.Controllers.Admin
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class C00010Controller : ControllerBase
    {
        [HttpPost]
        public IActionResult QueryShop()
        {

            JObject jo = new JObject();
            string sql = "select * from C10_store where statusId = '10'";
            DataTable dt = Tools.Db.GetDataTable(Tools.System.getConStr("MyDB"), sql);            
            return Content(JsonConvert.SerializeObject(Tools.Db.getJsonForGrid(dt)), "application/json");
        }
    }
}
