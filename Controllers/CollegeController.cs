using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaticFilesTest.Data;
using StaticFilesTest.Api;
using StaticFilesTest.Api.College;
using StaticFilesTest.Models;
using System.Threading.Tasks;
using System.Linq;

namespace StaticFilesTest.Controllers
{
    public class CollegeController:Controller
    {
        private readonly AddmissionContext _context;
        private readonly ILogger<Program> _logger;
        public CollegeController(AddmissionContext context,ILogger<Program> logger)
        {
            _context=context;
            _logger=logger;
        }
        
        [HttpPost,ActionName("declare")]
        public async Task<JsonResult> InfoRegister([FromBody] EnrollmentInfoRegister registers)//注册招生信息
        {
             var collegeName=HttpContext.Session.GetString("UserID");
            var now =DateTime.Now;
            var response=new Response{Code=0,Data=null};
            if(_context.Batches.SingleOrDefaultAsync(b => b.Bname=="提前批").Result.ApplicationBeginTime<now
            ||_context.CollegeEnrollments.Any(u => u.Uname==collegeName))//若招生工作已开始或已提交招生信息，则不予注册招生信息
            {
                response.Code=1;
            }
            else//满足时间约束，尝试录入注册信息
            {
                CollegeEnrollment tempEnrollmentInfo=new CollegeEnrollment();//要装入数据库的高校招生信息
                try
                {
                    float rate=registers.Proportion;
                    _context.Universities.SingleOrDefault(u => u.Uname==collegeName).ExpandRate=rate;
                    foreach(var register in registers.Profession)
                    {
                        tempEnrollmentInfo.Uname=collegeName;
                        tempEnrollmentInfo.Mid=register.Id;
                        tempEnrollmentInfo.Menrollment=register.Population;
                        switch(register.Batch)
                        {
                            case 0:
                                tempEnrollmentInfo.Bname="提前批";
                                break;
                            case 1:
                                tempEnrollmentInfo.Bname="第一批";
                                break;
                            case 2:
                                tempEnrollmentInfo.Bname="第二批";
                                break;
                            case 3:
                                tempEnrollmentInfo.Bname="第三批";
                                break;
                        }
                        
                        _context.CollegeEnrollments.Add(tempEnrollmentInfo);
                    }
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    response.Code=1;
                    Console.WriteLine(ex);
                }
                
            }
            return Json(response);
        }
    }
}