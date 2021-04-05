using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrderAPI.Model;

namespace OrderAPI.Controllers.Admin
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class O00010Controller : ControllerBase
    {
        string resultCode = "10";
        string errStr = "";
        StringBuilder strSql = new StringBuilder(200);
        [HttpGet]
        public IActionResult GetOrderList()
        {
            JObject jo = new JObject();
            try
            {
                strSql.AppendLine("select orderId as value,orderName as text,shopId,isLimit,limitTime,memo,statusId from C30_order where statusId in ('10','00')");
                JArray order = JArray.Parse(Tools.Dapper.QueryDataToJson(Tools.System.getConStr("MyDB"), strSql.ToString()));
                jo.Add("order", order);
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
        [HttpPost]
        public IActionResult ActOrder(order actRow)
        {
            JObject jo = new JObject();
            try
            {
                var param = new DynamicParameters();
                if (actRow.orderId.ToString() == "-1")
                {
                    string a = actRow.memo == null ? "DB": actRow.memo.ToString() ;
                    DateTime.Parse(actRow.limitTime.ToString());
                    //var memo = actRow.memo == null ? DBNull.Value : actRow.memo.ToString();
                    param.Add("orderName", actRow.orderName.ToString());
                    param.Add("shopId", actRow.shopId.ToString());
                    param.Add("isLimit", actRow.isLimit.ToString() == "true" ? true:false);
                    param.Add("limitTime", DBNull.Value);
                    param.Add("memo", DBNull.Value);
                    param.Add("statusType", "C2");
                    param.Add("statusId", "00");
                    param.Add("creatUser", "1");
                    strSql.Clear();
                    strSql.AppendLine("insert into C30_order (orderName,shopId,isLimit,limitTime,memo,statusType,statusId,creatUser,creatTime)");
                    strSql.AppendLine("VALUES (@orderName,@shopId,@isLimit,@limitTime,@memo,@statusType,@statusId,@creatUser,getDate()) ");
                    strSql.AppendLine("SELECT CAST(SCOPE_IDENTITY() as int) as orderId");
                    var orderId = Tools.Dapper.QuerySingleOrDefault(Tools.System.getConStr("MyDB"), strSql.ToString(), param);
                    //param.Add("isLimit", actRow.l);
                    jo.Add("orderId", orderId);
                    jo.Add("resultCode", resultCode);
                }
                else
                {
                    //todo update
                }
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
