using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using StaticFilesTest.Data;
using StaticFilesTest.Models;

namespace StaticFilesTest.Services
{
    public class DeliverFiles
    {
        public bool ExecuteFileDelivery(AddmissionContext _context)
        {
            try
            {
                var delivery=_context.DeliverFiles;
                foreach(var deliverFile in delivery)
                {
                    _context.Remove(deliverFile);
                }
                _context.SaveChanges();//清空投档表
                var applications=_context.Applications
                .Include(s => s.UnacceptedStudent)
                .GroupBy(s => s.UnacceptedStudent)
                .OrderByDescending(s => s.Key.TotalGrade);
                var colleges=_context.Universities.ToList();
                int[] remaning=new int[colleges.Count];
                for(int i=0;i<remaning.Length;i++)//初始化各高校剩余名额
                {
                    remaning[i]=(int)(colleges[i].ExpandRate*colleges[i].Enrollment);
                }
                foreach(var application in applications)//对每个考生
                {
                    var tempStudentApplication=application.GroupBy(s => s.Uname).ToList();//该生志愿按学校组织
                    foreach(var tempMajorApplication in tempStudentApplication)//该生每个学校的相应志愿,tempMajorApplication为当前学校下的志愿集
                    {
                        int index=colleges.IndexOf(colleges.Find(u => u.Uname==tempMajorApplication.Key));
                        if(remaning[index]>0)//若该校还能投档
                        {
                            var majors=tempMajorApplication.OrderBy(m => m.No).ToList();//获取该生填报的具体到专业志愿
                            foreach(var major in majors)//对每个志愿都插入到投档表中
                            {
                                var file=new DeliverFile{Sid=major.Sid,Uname=major.Uname,Mid=major.Mid};
                                _context.Add(file);
                            }
                            remaning[index]--;//能投入该校的考生减1
                            break;//该生投档完成并结束循环
                        }
                    }
                }
                foreach(var a in _context.Applications)//清空申请表
                {
                    _context.Remove(a);
                }
                _context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}