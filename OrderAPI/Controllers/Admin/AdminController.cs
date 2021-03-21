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

namespace OrderAPI.Controllers.Admin
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        string resultCode = "10";
        string errStr = "";
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
        [HttpGet]
        public bool IsLogin()
        {
            return User.Identity.IsAuthenticated;
        }
        [HttpPost]
        public async Task<IActionResult> Login(Login actRow)
        {
            JObject jo = new JObject();
            Login results = null;
            try
            {
                var param = new DynamicParameters();
                param.Add("userId", actRow.userId);
                param.Add("userPassword", actRow.userPassword);
                
                using (SqlConnection conn = new SqlConnection(Tools.System.getConStr("MyDB")))
                {
                    string strSql = "Select * from S10_users where userId = @userId and userPassword=@userPassword";
                    results = conn.QuerySingleOrDefault<Login>(strSql, param);
                }
                if (results == null) throw new Exception("請輸入正確的帳號密碼");

                jo.Add("resultCode", resultCode);

                #region cookie base
                var claims = new List<Claim>   
                {                    
                    new Claim("LoginTime", DateTime.Now.ToString()),
                    new Claim(ClaimTypes.Name, "Kevin"),
                    new Claim(ClaimTypes.Email, "karta1902613@gmail.com"),
                     new Claim(ClaimTypes.Role, "Administrator"),
                     new Claim(ClaimTypes.NameIdentifier, "1"),
                };

                var claimsIdentity = new ClaimsIdentity(
        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 設定 Authentication
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    // Refreshing the authentication session should be allowed.

                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };

                // 將user登入的資訊寫到session中
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,

                    new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties
                        {
                            IsPersistent = true
                        }
                    );
                #endregion
            }
            catch (Exception ex)
            {
                resultCode = "01";
                jo.Add("resultCode", resultCode);
                jo.Add("errMsg", ex.Message);
            }



            return Ok("ok");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok("ok");
        }
        [HttpGet]
        [Authorize]
        public UserInfo GetUserInfo()
        {
            var userInfo = new UserInfo(User.Claims.ToList());
            return userInfo;
        }
    }
}
