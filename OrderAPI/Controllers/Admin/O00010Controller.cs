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
                strSql.AppendLine("select orderId as value,orderName as text, orderId ,orderName,shopId,isLimit,limitTime,memo,statusId from C30_order where statusId in ('10','00')");
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
        [HttpGet]
        public IActionResult GetShopList()
        {
            JObject jo = new JObject();
            try
            {
                strSql.AppendLine("select shopId,shopName as title,shopImg as src from C10_shop where statusId = '10'");
                JArray shopList = JArray.Parse(Tools.Dapper.QueryDataToJson(Tools.System.getConStr("MyDB"), strSql.ToString()));
                jo.Add("slides", shopList);
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
        public IActionResult ActOrder(order actRow)
        {
            JObject jo = new JObject();
            try
            {
                //var param = new DynamicParameters();

                if (actRow.orderId.ToString() == "-1")
                {
                    //string a = actRow.memo == null ? "DB" : actRow.memo.ToString();
                    //DateTime.Parse(actRow.limitTime.ToString());
                    //var memo = actRow.memo == null ? DBNull.Value : actRow.memo.ToString();
                    /*
                    param.Add("orderName", actRow.orderName.ToString());
                    param.Add("shopId", actRow.shopId.ToString());
                    param.Add("isLimit", actRow.isLimit.ToString() == "true" ? true : false);
                    param.Add("limitTime", DBNull.Value);
                    param.Add("memo", DBNull.Value);
                    param.Add("statusType", "C2");
                    param.Add("statusId", "00");
                    param.Add("creatUser", "1");
                    */
                    var order = new order()
                    {
                        orderName = actRow.orderName.ToString(),
                        shopId = actRow.shopId.ToString(),
                        isLimit = actRow.isLimit.ToString() == "true" ? true : false,
                        //limitTime = actRow.limitTime == null ? string.Empty : actRow.limitTime.ToString(),
                        limitTime = actRow.limitTime == null ? null : actRow.limitTime,
                        memo = actRow.memo == null ? string.Empty : actRow.memo.ToString(),
                        statusType = "C2",
                        statusId = "10",
                        creatUser = "1",
                    };

                    strSql.Clear();
                    strSql.AppendLine("insert into C30_order (orderName,shopId,isLimit,limitTime,memo,statusType,statusId,creatUser,creatTime)");
                    strSql.AppendLine("VALUES (@orderName,@shopId,@isLimit,@limitTime,@memo,@statusType,@statusId,@creatUser,getDate()) ");
                    strSql.AppendLine("SELECT CAST(SCOPE_IDENTITY() as int) as orderId");
                    var orderId = Tools.Dapper.QuerySingleOrDefault(Tools.System.getConStr("MyDB"), strSql.ToString(), order);
                    //param.Add("isLimit", actRow.l);
                    jo.Add("orderId", orderId);
                }
                else
                {

                    var order = new order()
                    {
                        orderId = int.Parse(actRow.orderId.ToString()),
                        orderName = actRow.orderName.ToString(),
                        shopId = actRow.shopId.ToString(),
                        isLimit = actRow.isLimit.ToString() == "true" ? true : false,
                        //limitTime = DBNull.Value,
                        limitTime = actRow.limitTime == null ? null : actRow.limitTime,
                        //memo = actRow.memo == null ? string.Empty : actRow.memo.ToString(),
                        memo = actRow.memo == null ? null : actRow.memo,
                        actUser = "1",
                    };
                    strSql.Clear();
                    strSql.AppendLine("update C30_order set orderName = @orderName , shopId=@shopId ,isLimit =@isLimit ,limitTime=@limitTime,memo=@memo,actUser=@actUser ");
                    //strSql.AppendLine("update C30_order set orderName = @orderName , shopId=@shopId ,isLimit =@isLimit ,memo=@memo,actUser=@actUser ");
                    strSql.AppendLine(" where orderId=@orderId");
                    errStr = Tools.Dapper.ExecuteNonQuery(Tools.System.getConStr("MyDB"), strSql.ToString(), order);
                }
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
        public IActionResult GetOrderDetail(order actRow)
        {
            JObject jo = new JObject();
            try
            {
                var order = new order()
                {
                    orderId = actRow.orderId.ToString(),
                    statusId1 = actRow.statusId1.ToString()
                };
                strSql.AppendLine("select t1.userName,t2.mealName,t.mealQuantity,t.orderPrice,t.mealPrice,t.memo ");
                strSql.AppendLine(" from C30_orderDetail t ");
                strSql.AppendLine("inner join S10_users t1 on t.sysUserId = t1.sysUserId ");
                strSql.AppendLine("left join C20_shopMenu t2 on t.mealId = t2.mealId ");
                strSql.AppendLine(" where t.orderId = @orderId and t.statusId = '10' and t.statusId1 = @statusId1");
                JArray orderDetail = JArray.Parse(Tools.Dapper.QueryDataToJson(Tools.System.getConStr("MyDB"), strSql.ToString(),order));
                jo.Add("orderDetail", orderDetail);                
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
        public IActionResult GetNotYetOrderedUser(order actRow)
        {
            JObject jo = new JObject();
            try
            {
                var order = new order()
                {
                    orderId = actRow.orderId.ToString()
                };
                strSql.AppendLine("select t.userName as title from S10_users t ");
                strSql.AppendLine(" left join C30_orderDetail t1 ");
                strSql.AppendLine("on t.sysUserId = t1.sysUserId and t1.orderId = @orderId ");
                strSql.AppendLine("where t1.rowId is null ");                
                JArray NotYetOrderedUser = JArray.Parse(Tools.Dapper.QueryDataToJson(Tools.System.getConStr("MyDB"), strSql.ToString(), order));
                jo.Add("NotYetOrderedUser", NotYetOrderedUser);
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
        public IActionResult GetShopDetail(order actRow)
        {
            JObject jo = new JObject();
            try
            {
                var order = new order()
                {
                    shopId = actRow.shopId.ToString()
                };
                strSql.AppendLine("select shopName,shopAddr,shopTel,limitTime,isDeliver,minCost from C10_shop ");                
                strSql.AppendLine("where statusId = '10' and shopId = @shopId");                
                JArray ShopDetail = JArray.Parse(Tools.Dapper.QueryDataToJson(Tools.System.getConStr("MyDB"), strSql.ToString(), order));
                jo.Add("ShopDetail", ShopDetail);
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
        public IActionResult OrderCheckEnd(order actRow)
        {
            JObject jo = new JObject();
            try
            {
                var order = new order()
                {
                    orderId = actRow.orderId.ToString()
                };
                strSql.Clear();
                strSql.AppendLine("update C30_order set statusId = 20 ");                
                strSql.AppendLine(" where orderId=@orderId");
                errStr = Tools.Dapper.ExecuteNonQuery(Tools.System.getConStr("MyDB"), strSql.ToString(), order);
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
