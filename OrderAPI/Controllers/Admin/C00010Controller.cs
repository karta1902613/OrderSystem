using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Dapper;
using OrderAPI.Model;
using System.Data.SqlClient;

namespace OrderAPI.Controllers.Admin
{    
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class C00010Controller : ControllerBase
    {
        string resultCode = "10";
        string errStr = "";
        StringBuilder strSql = new StringBuilder(200);
        [HttpPost]
        public IActionResult QueryShop(QueryShop actRow)
        {            
            JObject jo = new JObject();
            string sqlCon = "where 1 = 1 ";
            try
            {
                if (!string.IsNullOrEmpty(actRow.shopName))
                {
                    sqlCon += "and t.shopName like '%"+ actRow.shopName + "%' ";
                }
                strSql.Clear();
                strSql.AppendLine("select t.* ,t1.statusName from C10_shop t ");
                strSql.AppendLine("inner join S00_statusId t1 on t.shopType = t1.statusType and t.shopStatus = t1.statusId ");
                strSql.AppendLine(sqlCon + "and  t.statusId = '10' ");
                DataTable dt = Tools.Db.GetDataTable(Tools.System.getConStr("MyDB"), strSql.ToString());
                jo.Add("shopData", Tools.Db.getJsonForGrid(dt));
                jo.Add("resultCode", resultCode);
            }
            catch(Exception ex)
            {
                resultCode = "01";
                jo.Add("resultCode", resultCode);
                jo.Add("errMsg",ex.Message);
            }                     
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
        [HttpPost]
        public IActionResult UpdateShop(ShopData actRow)
        {
            JObject jo = new JObject();
            string strCond = "";
            try
            {           
                if (!string.IsNullOrEmpty(actRow.shopTel))
                {
                    strCond += ", shopTel =  '" + actRow.shopTel + "' ";
                }
                if (!string.IsNullOrEmpty(actRow.shopAddr))
                {
                    strCond += ", shopAddr =  '" + actRow.shopAddr + "' ";
                }
                if (!string.IsNullOrEmpty(actRow.shopStatus))
                {
                    strCond += ", shopStatus =  '" + actRow.shopStatus + "' ";
                }
                if (!string.IsNullOrEmpty(actRow.isDeliver.ToString()))
                {
                    if(actRow.isDeliver)
                    {
                        strCond += ", isDeliver =  1";
                    }
                    else
                    {
                        strCond += ", isDeliver =  0";
                    }
                    
                }
                if (!string.IsNullOrEmpty(actRow.limitTime.ToString()))
                {
                    strCond += ", limitTime =  '" + actRow.limitTime + "' ";
                }
                if (!string.IsNullOrEmpty(actRow.minCost))
                {
                    strCond += ", minCost =  '" + actRow.minCost + "' ";
                }
                strSql.Clear();
                strSql.Append("update C10_shop set shopName = '" + actRow.shopName + "' " + strCond);
                strSql.Append(" where  shopId = '" + actRow.shopId + "' ");                
                errStr = Tools.Db.ExecuteNonQuery(Tools.System.getConStr("MyDB"), strSql.ToString());
                if (errStr != "") throw new Exception(errStr);
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

        public IActionResult AddShop(ShopData actRow)
        {
            JObject jo = new JObject();
            string strCond = "";
            try
            {                        
                actRow.creatUser = "1";//TODO 待之後登入cookie寫好 再來處理

                var param = new DynamicParameters();
                param.Add("shopName", actRow.shopName);
                param.Add("shopAddr", actRow.shopAddr);
                param.Add("shopTel", actRow.shopTel);
                param.Add("isDeliver", actRow.isDeliver);
                param.Add("minCost", actRow.minCost);
                param.Add("statusType", "S0");
                param.Add("statusId", actRow.statusId);
                param.Add("shopType", "C0");
                param.Add("shopStatus", actRow.shopStatus);
                param.Add("creatUser", actRow.creatUser);
                param.Add("creatTime", DateTime.Now);
                param.Add("limitTime", actRow.limitTime);
                strSql.Clear();
                strSql.AppendLine("INSERT INTO C10_shop (shopName, shopAddr,shopTel,isDeliver,minCost,limitTime,statusType,statusId,shopType,shopStatus,creatUser,creatTime) ");
                strSql.AppendLine("VALUES (@shopName, @shopAddr,@shopTel,@isDeliver,@minCost,@limitTime,@statusType,@statusId,@shopType,@shopStatus,@creatUser,@creatTime)");
                
                errStr = Tools.Dapper.ExecuteNonQuery(Tools.System.getConStr("MyDB"), strSql.ToString(), param);
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

        public IActionResult DeleteShop(ShopData actRow)
        {
            JObject jo = new JObject();            
            try
            {
                actRow.creatUser = "1";//TODO 待之後登入cookie寫好 再來處理

                var param = new DynamicParameters();
                param.Add("shopId", actRow.shopId);
                param.Add("statusId", "30");
                strSql.Clear();
                strSql.AppendLine("update C10_shop set statusId=@statusId where shopId=@shopId");
                
                errStr = Tools.Dapper.ExecuteNonQuery(Tools.System.getConStr("MyDB"), strSql.ToString(), param);
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
