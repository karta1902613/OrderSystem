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
                    string strSql = "select top(1) orderId,shopId from C30_order t where t.statusId = '10' order by orderId desc";
                    //results = conn.QuerySingleOrDefault<MyModel>(strSql).ToList();
                    var query = conn.Query(strSql).FirstOrDefault();
                    if (query == null) throw new Exception("無任何建立的訂單");

                    jo.Add("orderId", query.orderId);
                    strSql = "select " + query.orderId + "as orderId, t.shopId,t.mealId,t.mealName,t.mealPrice,t.statusId,t.statusId1,t1.statusName ";
                    strSql += "from C20_shopMenu t ";
                    strSql += "inner join S00_statusId t1 on t.statusId1 = t1.statusId and t.statusType1 = t1.statusType ";
                    strSql += "where t.statusId = 10 and t.shopId = " + query.shopId;
                    //var queryMenu = conn.Query(strSql);
                    //json = JsonConvert.SerializeObject(queryMenu);
                    JArray menu = JArray.Parse(Tools.Dapper.QueryDataToJson(Tools.System.getConStr("MyDB"), strSql));
                    strSql = "select t.rowId,t.orderId,orderPrice,t.shopId,t.mealId,t.mealPrice,t.mealQuantity,t.memo,t1.mealName";
                    strSql += " from C30_orderDetail t ";
                    strSql += " left join C20_shopMenu t1 on t.shopId = t1.shopId and t.mealId = t1.mealId ";
                    strSql += "where t.creatUser = '" + HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier).Value + "' and t.orderId = '" + query.orderId + "' and t.statusId = '10'";

                    JArray orderDetail = JArray.Parse(Tools.Dapper.QueryDataToJson(Tools.System.getConStr("MyDB"), strSql));
                    query.Dump();
                    jo.Add("menu", menu);
                    jo.Add("orderDetail", orderDetail);

                }

            }
            catch (Exception ex)
            {
                resultCode = "01";
                jo.Add("resultCode", resultCode);
                jo.Add("errMsg", ex.Message);
            }

            return Content(JsonConvert.SerializeObject(jo), "application/json");
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
                param.Add("orderPrice", int.Parse(actRow.mealPrice.ToString()) * int.Parse(actRow.mealQuantity.ToString()));
                param.Add("mealPrice", actRow.mealPrice.ToString());
                param.Add("mealQuantity", actRow.mealQuantity.ToString());
                param.Add("sysUserId", HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier).Value);
                param.Add("stausType", "S0");
                param.Add("statusId", "10");
                param.Add("statusId1", actRow.statusId1.ToString());
                param.Add("statusType1", "C3");
                param.Add("memo", actRow.memo.ToString());
                param.Add("creatUser", HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier).Value);
                param.Add("creatTime", DateTime.Now);

                strSql.Clear();
                strSql.AppendLine("select COUNT(*) from C30_orderDetail where orderId = @orderId and sysUserId = @sysUserId and statusId1 = '20' and statusId = '10'");

                var countPass = Tools.Dapper.QuerySingleOrDefault(Tools.System.getConStr("MyDB"), strSql.ToString(), param);
                if (int.Parse(countPass) > 0) throw new Exception("若要訂餐請先取消PASS");
                strSql.Clear();
                strSql.AppendLine("select COUNT(*) from C30_orderDetail where orderId = @orderId and sysUserId = @sysUserId and statusId1 = '10' and statusId = '10'");

                var countOrder = Tools.Dapper.QuerySingleOrDefault(Tools.System.getConStr("MyDB"), strSql.ToString(), param);
                if (actRow.statusId1.ToString() == "20" && int.Parse(countOrder) > 0) throw new Exception("訂單中尚有餐點");
                strSql.Clear();
                strSql.AppendLine("insert into C30_orderDetail (orderId,shopId,mealId,orderPrice,mealPrice,mealQuantity,sysUserId,statusId,stausType,statusId1,statusType1,memo,creatUser,creatTime)");
                strSql.AppendLine("VALUES (@orderId,@shopId,@mealId,@orderPrice,@mealPrice,@mealQuantity,@sysUserId,@statusId,@stausType,@statusId1,@statusType1,@memo,@creatUser,@creatTime) ");
                strSql.AppendLine("SELECT CAST(SCOPE_IDENTITY() as int) as rowId");
                var rowId = Tools.Dapper.QuerySingleOrDefault(Tools.System.getConStr("MyDB"), strSql.ToString(), param);
                //errStr = Tools.Dapper.ExecuteNonQuery(Tools.System.getConStr("MyDB"), strSql.ToString(), param);
                jo.Add("rowId", rowId);
                jo.Add("resultCode", resultCode);
            }
            catch (Exception ex)
            {
                resultCode = "01";
                jo.Add("resultCode", resultCode);
                jo.Add("errMsg", ex.Message);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
        [HttpPost]
        public IActionResult DeleteOrderDetail(orderDetail actRow)
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            JObject jo = new JObject();
            try
            {
                var param = new DynamicParameters();
                param.Add("rowId", actRow.rowId.ToString());
                param.Add("statusId", "30");
                param.Add("sysUserId", HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier).Value);
                param.Add("actUser", HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier).Value);
                strSql.Clear();
                strSql.AppendLine("update C30_orderDetail set statusId=@statusId,actUser=@actUser,actTime=getDate() where rowId=@rowId and sysUserId = @sysUserId");

                errStr = Tools.Dapper.ExecuteNonQuery(Tools.System.getConStr("MyDB"), strSql.ToString(), param);
                if (errStr != "") throw new Exception(errStr);
                jo.Add("resultCode", resultCode);
            }
            catch (Exception ex)
            {
                resultCode = "01";
                jo.Add("resultCode", resultCode);
                jo.Add("errMsg", ex.Message);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
    }
}
