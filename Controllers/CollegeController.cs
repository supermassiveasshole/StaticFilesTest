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
            ||_context.Universities.Any(u => u.ApprovalStatus==true))//若招生工作已开始或该校已被招办审批，则不予注册招生信息
            {
                response.Code=1;
            }
            else//满足时间约束，尝试录入注册信息
            {
                try
                {
                    float rate=registers.Proportion;
                    _context.Universities.SingleOrDefault(u => u.Uname==collegeName).ExpandRate=rate;
                    foreach(var register in registers.Profession)
                    {
                        CollegeEnrollment tempEnrollmentInfo=new CollegeEnrollment();//要装入数据库的高校招生信息
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
                        tempEnrollmentInfo.EnrollmentRemaning=tempEnrollmentInfo.Menrollment;
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

        public JsonResult Enroll(string type)
        {
            var response=new Response{Code=1,Data=null};
            try
            {
                bool enrollmentInfo=_context.CollegeEnrollments.Include(e => e.Batch).AnyAsync(e => 
                    e.Uname==HttpContext.Session.GetString("UserName")
                    &&e.Batch.EnrollmentBeginTime<DateTime.Now
                    &&DateTime.Now<e.Batch.EnrollmentEndTime
                    &&e.IsComplete==true).Result;//本高校某仍在某批次招生时间内，且统招工作已经完成
                if(_context.Universities.SingleOrDefaultAsync(u => u.Uname==HttpContext.Session.GetString("UserName")).Result.ApprovalStatus==false)//若没通过审批则不允许招生
                    return Json(response);
                if(type.Equals("统招"))
                {
                    if((!_context.Batches.AnyAsync(b => 
                        !b.Bname.Equals("提前批")
                        &&b.EnrollmentBeginTime<DateTime.Now
                        &&DateTime.Now<b.EnrollmentEndTime).Result)&&enrollmentInfo==true)
                        {
                            return Json(response);
                        }
                }
                else if(type.Equals("特招"))
                {
                    var tempBatch=_context.Batches.FindAsync("提前批").Result;
                    if((!(tempBatch.EnrollmentBeginTime<DateTime.Now&&DateTime.Now<tempBatch.EnrollmentEndTime))&&enrollmentInfo==true)
                    {
                        return Json(response);
                    }
                }
                else if(type.Equals("调招"))
                {
                    if(enrollmentInfo==false)
                    {
                        return Json(response);
                    } 
                }
                else//Type非法则code=1
                {
                    return Json(response);
                }
                //下面查询该校招生信息
                IQueryable<CollegeEnrollment> info;
                switch(type)
                {
                    case "特招":
                        info=_context.CollegeEnrollments.Include(c => c.Major).Where(c => c.Bname=="提前批");//获取招生信息   
                        response.Data=info.Select(c => new MajorInfo{Name=c.Major.Mname+c.Bname,Population=c.EnrollmentRemaning}).ToArray();
                        response.Code=0;
                        break;
                    case "统招":
                        info=_context.CollegeEnrollments.Include(c => c.Major)
                        .Include(c => c.Batch)
                        .Where(c => c.Bname!="提前批"&&c.Batch.EnrollmentBeginTime<DateTime.Now&&DateTime.Now<c.Batch.EnrollmentEndTime);
                        response.Data=info.Select(c => new MajorInfo{Name=c.Major.Mname+c.Bname,Population=c.EnrollmentRemaning}).ToArray();
                        response.Code=0;
                        break;
                    case "调招":
                        info=_context.CollegeEnrollments.Include(c => c.Major);
                        response.Data=info.Select(c => new MajorInfo{Name=c.Major.Mname+c.Bname,Population=c.EnrollmentRemaning}).ToArray();
                        response.Code=0;
                        break;
                }
            }
            catch(Exception ex)
            {
                response.Code=1;
                _logger.LogError(ex,"Enrollment failed!");
            }
            HttpContext.Session.SetString("type",type);//将当前录取方式计入session
            return Json(response);

        }

        public async Task<JsonResult> Table([FromBody] TableRequestOuter request)
        {
            int page=0;//记录当前页码
            if(HttpContext.Session.GetString("page")==null)
            {
                HttpContext.Session.SetString("page","0");
                page=0;
            }
            else
            {
                page=int.Parse(HttpContext.Session.GetString("page"));
            }
            HttpContext.Session.SetString("page",(page+1).ToString());//当前页自增
            var response=new Response{Code=1,Data=null};
            try
            {
                //下面处理上一页的录取
                foreach(var i in request.Professions)
                {
                    i.Profession=i.Profession.Substring(0,i.Profession.Length-3);
                    _context.Admissions.Add(new Admission{Sid=i.Key.ToString(),
                    Uname=HttpContext.Session.GetString("UserName"),
                    Mid=_context.Majors.SingleOrDefaultAsync(m => m.Mname.Equals(i.Profession)).Result.Mid,
                    AdmissionMethod=HttpContext.Session.GetString("type")});
                    //下面将此学生转为已录取学生
                    var admittedStudent=_context.UnacceptedStudents.SingleOrDefaultAsync(s => s.Sid==i.Key.ToString()).Result;
                    await _context.AcceptedStudents.AddAsync(new AcceptedStudent{
                        Sid=admittedStudent.Sid,
                        Sname=admittedStudent.Sname,
                        Gender=admittedStudent.Gender,
                        Grades=admittedStudent.Grades,
                        TotalGrade=admittedStudent.TotalGrade,
                        Rank=admittedStudent.Rank,
                        GraduateSchool=admittedStudent.GraduateSchool});//在录取学生中加入此学生
                    _context.UnacceptedStudents.Remove(admittedStudent);//删除未被录取的学生中的此学生
                    var enrollment=_context.CollegeEnrollments.Include(c => c.Batch).Include(c => c.Major).SingleOrDefaultAsync(
                        c => c.Uname==HttpContext.Session.GetString("UserName")
                        &&c.Major.Mname==i.Profession
                        &&c.Batch.EnrollmentBeginTime<DateTime.Now
                        &&c.Batch.EnrollmentEndTime>DateTime.Now
                    ).Result;
                    enrollment.EnrollmentRemaning--;
                    _context.CollegeEnrollments.Update(enrollment);
                }
                await _context.SaveChangesAsync();
                //下面返回下一页
                var deliverFiles=_context.DeliverFiles
                .Include(d => d.UnacceptedStudent)
                .Where(d => d.Uname==HttpContext.Session.GetString("UserName"))
                .GroupBy(d => d.UnacceptedStudent)
                .OrderByDescending(s => s.Key.TotalGrade)
                .ToList();//获取该校按总分降序投档表
                List<TableResponse> students=new List<TableResponse>();
                for(int i=(page-1)*10;i<i+10&&(i%10)<deliverFiles.Count;i++)//获取前十位学生数据
                {
                    students.Insert(i,new TableResponse());
                    students[i]=new TableResponse();
                    students[i].Key=int.Parse(deliverFiles[i].Key.Sid);
                    students[i].Name=deliverFiles[i].Key.Sname;
                    students[i].Grade=deliverFiles[i].Key.TotalGrade;
                    int[] majors=deliverFiles[i].Select(d => d.Mid).ToArray();
                    students[i].Professions=new string[majors.Length];
                    for(int j=0;j<majors.Length;j++)
                    {
                        students[i].Professions[j]=_context.Majors.SingleOrDefaultAsync(m => m.Mid==majors[j]).Result.Mname;
                        students[i].Professions[j]+=_context.Batches.Where(m => m.EnrollmentBeginTime<DateTime.Now&&DateTime.Now<m.EnrollmentEndTime).First().Bname;
                    }
                }
                response.Data=students.ToArray();
                //上面不用考虑第几批次，因为每个批次有每个批次的投档
                response.Code=0;
            }
            catch(Exception ex)
            {
                //这里可能catch到out of index，上面的代码不处理了
                response.Code=1;
                _logger.LogInformation(ex,"Failed to record admissions or return students' info.");
            }
            return Json(response);
        }

        public async Task<JsonResult> Retreat()//退档
        {
            var response=new Response{Code=1,Data=null};
            try
            {
                var enrollments=_context.CollegeEnrollments
                .Include(e => e.Batch)
                .Where(e => e.Batch.EnrollmentBeginTime<DateTime.Now&&DateTime.Now<e.Batch.EnrollmentEndTime&&e.Uname==HttpContext.Session.GetString("UserName"));
                foreach(var i in enrollments)
                {
                    i.IsComplete=true;
                    _context.CollegeEnrollments.Update(i);
                }
                await _context.SaveChangesAsync();
                response.Code=0;
            }
            catch(Exception ex)
            {
                response.Code=1;
                _logger.LogError(ex,"failed to finish enrollment.");
            }
            return Json(response);
        }

        public JsonResult Situation()//查看此校录取学生情况
        {
            var response=new Response{Code=1,Data=null};
            try
            {
                var situation=_context.Admissions.Include(c => c.AcceptedStudent)
                .Where(c => c.Uname==HttpContext.Session.GetString("UserName"))
                .Select(c => 
                new Situation{
                    Key=int.Parse(c.Sid),
                    Name=c.AcceptedStudent.Sname,
                    Grade=c.AcceptedStudent.TotalGrade,
                    School=c.AcceptedStudent.GraduateSchool,
                    Type=c.AdmissionMethod
                }).ToArray();
                response.Code=0;
                response.Data=situation;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"error while getting college enrollment situation");
                response.Code=1;
            }
            return Json(response);
        }

        [HttpGet,ActionName("excel")]
        public JsonResult Excel(StaticFilesTest.Services.GetAdmissionList list)
        {
            var response=new Response{Code=1,Data=null};
            if(_context.Admissions.Any(a => a.Uname==HttpContext.Session.GetString("UserName")))
            {
                response.Code=0;
            }
            list.userName=HttpContext.Session.GetString("UserName");
            var file="data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,"+Convert.ToBase64String(list.GetExcel(_context));
            response.Data=file;
            return Json(response);
        }
    }
}