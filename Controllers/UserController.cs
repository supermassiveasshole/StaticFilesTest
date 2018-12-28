using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using StaticFilesTest.Data;
using StaticFilesTest.Api.User;
using StaticFilesTest.Api;

namespace StaticFilesTest.Controllers
{
    public class UserController:Controller
    {
        private readonly AddmissionContext dbContext;
        public UserController(AddmissionContext _dbContext)
        {
            dbContext=_dbContext;
        }
        [HttpGet]
        public JsonResult Info()
        {
            string name="null";
            int identity=3;
            bool isLogin=false;
            try
            {
                name=HttpContext.Session.GetString("UserName");
                identity=int.Parse(HttpContext.Session.GetString("Identity"));
                isLogin=bool.Parse(HttpContext.Session.GetString("IsLogin"));
            }
            catch(Exception ex)
            {
                name="session failed";
            }
            var response=new Response{Code=0,Data=new InfoResponse{IsLogin=isLogin,Name=name,Identity=identity}};
            return Json(response);
        }
        [HttpPost]
        public JsonResult LogOut()
        {
            HttpContext.Session.SetString("IsLogin","false");
            var response=new Response{Code=0,Data=new Object()};
            return Json(response);
        }
        [HttpPost]
        public JsonResult Login([FromBody] LoginRequest loginRequest)
        {
            Response response=new Response{Code=1,Data=new Object()};
            if(dbContext.StudentsAccounts.AnyAsync(s => s.Sid.Equals(loginRequest.UserName)).Result)
            {
                if(loginRequest.Password.Equals(dbContext.StudentsAccounts.Find(loginRequest.UserName).Spassword))
                {
                    HttpContext.Session.SetString("IsLogin","true");
                    HttpContext.Session.SetString("UserID",loginRequest.UserName);
                    string userName=dbContext.UnacceptedStudents.Find(loginRequest.UserName)!=null
                    ?dbContext.UnacceptedStudents.Find(loginRequest.UserName).Sname
                    :dbContext.AcceptedStudents.Find(loginRequest.UserName).Sname;
                    HttpContext.Session.SetString("UserName",userName);
                    HttpContext.Session.SetString("Identity","0");
                    response.Code=0;
                    return Json(response);
                }
            }
            else if(dbContext.AdmissionsOffices.AnyAsync(a => a.Aname.Equals(loginRequest.UserName)).Result)
            {
                if(loginRequest.Password.Equals(dbContext.AdmissionsOffices.Find(loginRequest.UserName).Apassword))
                {
                    HttpContext.Session.SetString("IsLogin","true");
                    HttpContext.Session.SetString("UserID",loginRequest.UserName);
                    HttpContext.Session.SetString("UserName",loginRequest.UserName);
                    HttpContext.Session.SetString("Identity","1");
                    response.Code=0;
                    return Json(response);
                }
            }
            else if(dbContext.Universities.AnyAsync(u => u.Uname.Equals(loginRequest.UserName)).Result)
            {
                if(loginRequest.Password.Equals(dbContext.Universities.Find(loginRequest.UserName).Upassword))
                {
                    HttpContext.Session.SetString("IsLogin","true");
                    HttpContext.Session.SetString("UserID",loginRequest.UserName);
                    HttpContext.Session.SetString("UserName",loginRequest.UserName);
                    HttpContext.Session.SetString("Identity","2");
                    response.Code=0;
                    return Json(response);
                }
            }
            return Json(response);
        }
    }
}
