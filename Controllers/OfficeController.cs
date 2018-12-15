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
using StaticFilesTest.Api.Office;
using StaticFilesTest.Models;
using System.Threading.Tasks;
using System.Linq;

namespace StaticFilesTest.Controllers
{
    public class OfficeController:Controller
    {
        private readonly AddmissionContext _context;
        private readonly ILogger<OfficeController> _logger;
        public OfficeController(AddmissionContext context,ILogger<OfficeController> logger)
        {
            _context=context;
            _logger=logger;
        }
        [HttpGet,ActionName("approval")]
        public JsonResult ApprovalGet()
        {
            var response=new Response{Code=1,Data=null};
            DateTime now=DateTime.Now;
            if(_context.Batches.SingleOrDefault(b => b.Bname=="提前批").ApplicationBeginTime<now)//若志愿填报工作已经开始
            {
                return Json(response);
            }
            try
            {
                var universities=_context.Universities.Include(u => u.Majors).ThenInclude(m => m.Major).ToList();
                EnrollmentInfo[] info=new EnrollmentInfo[universities.Count];
                for(int i=0;i<universities.Count;i++)
                {
                    info[i]=new EnrollmentInfo();
                    info[i].School=universities[i].Uname;
                    info[i].Population=universities[i].Enrollment;
                    info[i].Professions=new CollegeMajorEnrollment[universities[i].Majors.Count];
                    var majors=universities[i].Majors.ToList();
                    for(int j=0;j<universities[i].Majors.Count;j++)
                    {
                        info[i].Professions[j]=new CollegeMajorEnrollment();
                        info[i].Professions[j].Profession=majors[j].Major.Mname;
                        info[i].Professions[j].Population=majors[j].Menrollment;
                    }
                }
                response.Code=0;
                response.Data=info.ToArray();
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex,"An error occured while getting college enrollment info");
            }
            
            return Json(response);
        }

        [HttpPost,ActionName("approval")]
        public JsonResult ApprovalPost([FromBody] string[] colleges)
        {
            var response=new Response{Code=1,Data=null};
            try
            {
                foreach(var i in colleges)
                {
                    var college=_context.Universities.SingleOrDefault(u => u.Uname==i);
                    college.Aname=HttpContext.Session.GetString("UserName");
                    college.ApprovalStatus=true;
                    _context.Universities.Update(college);
                }
                _context.SaveChanges();
                response.Code=0;
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex,"Failed to update universities' approval status");
            }
            return Json(response);   
        }

        public JsonResult Deliver(StaticFilesTest.Services.DeliverFiles delivery)//投档
        {
            var response=new Response{Code=1,Data=null};
            DateTime now=DateTime.Now;
            if(!_context.Batches.Any(b => b.ApplicationEndTime<now&&now<b.EnrollmentBeginTime))//若不满足时间约束
            {
                return Json(response);
            }
            if(delivery.ExecuteFileDelivery(_context))
            {
                response.Code=0;
            }
            return Json(response);
        }
    }
}