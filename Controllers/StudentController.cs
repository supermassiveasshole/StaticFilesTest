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
using StaticFilesTest.Api.Student;
using StaticFilesTest.Models;
using System.Threading.Tasks;
using System.Linq;

namespace StaticFilesTest.Controllers
{
    public class StudentController:Controller
    {
        private readonly AddmissionContext _context;
        private readonly ILogger<Program> _logger;
        public StudentController(AddmissionContext context,ILogger<Program> logger)
        {
            _context=context;
            _logger=logger;
        }
        [HttpGet,ActionName("volunteer")]
        public JsonResult VolunteerTime()//检查是否可以填报志愿
        {
            var now=DateTime.Now;
            string userID=HttpContext.Session.GetString("UserID");
            double grade=0.0;//考生总分
            var response=new Response{Code=0,Data=new Object()};
            if(_context.AcceptedStudents.AnyAsync(s => s.Sid==userID).Result||_context.Applications.AnyAsync(s => s.Sid==userID).Result)//已被录取或填报过志愿则不予填报志愿
            {
                response.Code=1;
            }
            else
            {
                grade=_context.UnacceptedStudents.SingleOrDefaultAsync(s => s.Sid==userID).Result.TotalGrade;
                if(!_context.Batches.AnyAsync(b => b.ApplicationBeginTime<=now&&b.ApplicationEndTime>=now&&b.GradeLine<=grade).Result)//时间不对或分数不够则不予填报
                {
                    response.Code=1;
                }
            }
            return Json(response);
            
        }
        [HttpPost,ActionName("volunteer")]
        public async Task<JsonResult> VolunteerStore([FromBody]Volunteer[] volunteers)
        {
            var response=new Response{Code=0,Data=new Object()};
            if(volunteers==null)//数据为空
            {
                response.Code=1;
            }
            var Sid=HttpContext.Session.GetString("UserID");
            try
            {
                foreach(var volunteer in volunteers)//对于该生的每个志愿
                {
                    var adjustment=new StudentUniversityAdjustment{Sid=Sid,Uname=volunteer.College,Adjustment=volunteer.IsObey};
                    _context.StudentUniversityAdjustments.Add(adjustment);
                    for(int i=1;i<=6;i++)//一共六个专业
                    {
                        var application=new Application{Sid=Sid,Uname=volunteer.College,Mid=volunteer.Professions[i-1],No=i};
                        _context.Applications.Add(application);
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                response.Code=1;
                _logger.LogError(ex,"An exception occured while add volunteers to database.");
            }
            return Json(response);
        }

        public JsonResult Situation()
        {
            var response=new Response{Code=0,Data=null};
            string Sid=HttpContext.Session.GetString("UserID");
            try
            {
                var admission=_context.Admissions.Include(s => s.Major).SingleOrDefaultAsync(s => s.Sid==Sid).Result;
                AdmissionResponse admissionResponse=new AdmissionResponse{College=admission.Uname,Profession=admission.Major.Mname};
                response.Data=admissionResponse;
            }
            catch(Exception ex)
            {
                response.Code=1;
                _logger.LogError(ex,"An error occurred while query the student's admission.");
            }
            return Json(response);
        }

        [HttpGet]
        public JsonResult Info([FromQuery] CollegeInfoRequest collegeName)
        {
            var response=new Response{Code=0,Data=null};
            try
            {
                var enrollment=_context.CollegeEnrollments.Include(c => c.Major).Where(c => c.Uname==collegeName.College).ToArrayAsync().Result;
                var enrollmentInfo=new EnrollmentInfo[enrollment.Length];
                for(int i=0;i<enrollment.Length;i++)
                {
                    enrollmentInfo[i]=new EnrollmentInfo{
                    Key=i,
                    Profession=enrollment[i].Major.Mname,
                    Batch=enrollment[i].Bname,
                    Count=enrollment[i].Menrollment};
                }
                response.Data=enrollmentInfo;
            }
            catch(Exception ex)
            {
                response.Code=1;
                _logger.LogError(ex,"An error occurred while get college enrollment infomation.");
            }
            return Json(response);
        }
        [HttpGet]
        public JsonResult Colleges()
        {
            var response=new Response{Code=1,Data=null};
            try
            {
                var colleges=_context.Universities.Select(u => u.Uname).ToArray();
                response.Data=colleges;
                response.Code=0;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"Failed to get Universities.");
            }
            return Json(response);
        }
    }
}