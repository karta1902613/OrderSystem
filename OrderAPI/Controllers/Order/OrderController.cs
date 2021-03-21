using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OrderAPI.Model;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace OrderAPI.Controllers.Order
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        string resultCode = "10";
        StringBuilder strSql = new StringBuilder(200);
        string errStr = "";
        [HttpGet]
        public IActionResult GetOrderData()
        {
            var json = "";
            JObject jo = new JObject();
            try
            {
                
                using (SqlConnection conn = new SqlConnection(Tools.System.getConStr("MyDB")))
                {
                    string strSql = "select top(1) orderId,shopId from C30_order t where t.statusId = '00' order by orderId asc";
                    //results = conn.QuerySingleOrDefault<MyModel>(strSql).ToList();
                    var query = conn.Query(strSql).FirstOrDefault();
                    if (query == null) throw new Exception("無任何建立的訂單");
                    
                    jo.Add("orderId", query.orderId);
                    strSql  = "select " + query.orderId + "as orderId, t.shopId,t.mealId,t.mealName,t.mealPrice,t.statusId,t.statusId1,t1.statusName ";
                    strSql += "from C20_shopMenu t ";
                    strSql += "inner join S00_statusId t1 on t.statusId1 = t1.statusId and t.statusType1 = t1.statusType ";
                    strSql += "where t.statusId = 10 and t.shopId = " + query.shopId;
                    var queryMenu = conn.Query(strSql);
                    json = JsonConvert.SerializeObject(queryMenu);
                    query.Dump();                
                    jo.Add("menu", json);
                }
            }
            catch(Exception ex)
            {
                resultCode = "01";
                jo.Add("resultCode", resultCode);
                jo.Add("errMsg", ex.Message);
            }
          
            return Content(json, "application/json");
        }
        [HttpPost]
        public IActionResult ActOrderDetail(orderDetail actRow)
        {
            var identity = (ClaimsIdentity)User.Identity;
            
            IEnumerable<Claim> claims = identity.Claims;
            //string c = claims.FirstOrDefault(x => x.Type == "sysUserId").ToString();
            JObject jo = new JObject();
            try
            {
                var param = new DynamicParameters();
                param.Add("orderId", actRow.orderId.ToString());
                param.Add("shopId", actRow.shopId.ToString());
                param.Add("mealId", actRow.mealId.ToString());
                param.Add("orderPrice",int.Parse(actRow.mealPrice.ToString()) * int.Parse(actRow.mealQuantity.ToString()));
                param.Add("mealPrice", actRow.mealPrice.ToString());
                param.Add("mealQuantity", actRow.mealQuantity.ToString());
                param.Add("sysUserId", HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier).Value);
                param.Add("stausType", "S0");
                param.Add("statusId", "10");
                param.Add("statusId1", actRow.statusId1.ToString());
                param.Add("statusType1", "C3");
                param.Add("creatUser", HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier).Value);
                param.Add("creatTime", DateTime.Now);
                strSql.Clear();
                strSql.AppendLine("insert into C30_orderDetail (orderId,shopId,mealId,orderPrice,mealPrice,mealQuantity,sysUserId,statusId,stausType,statusId1,statusType1,creatUser,creatTime)");
                strSql.AppendLine("VALUES (@orderId,@shopId,@mealId,@orderPrice,@mealPrice,@mealQuantity,@sysUserId,@statusId,@stausType,@statusId1,@statusType1,@creatUser,@creatTime)");
                errStr = Tools.Dapper.ExecuteNonQuery(Tools.System.getConStr("MyDB"), strSql.ToString(), param);
                jo.Add("resultCode", resultCode);
            }
            catch(Exception ex)
            {
                resultCode = "01";
                jo.Add("resultCode", resultCode);
                jo.Add("errMsg", ex.Message);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
    }
}
