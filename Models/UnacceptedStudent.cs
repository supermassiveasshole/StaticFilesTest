using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaticFilesTest.Models
{
    public class UnacceptedStudent
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public string Sid{get;set;}
        public string Sname{get;set;}
        public char Gender{get;set;}
        public string Grades{get;set;}
        public float TotalGrade{get;set;}
        public int Rank{get;set;}
        public string GraduateSchool{get;set;}//毕业学校

        //Navigator Property
        public ICollection<Application> Application{get;set;}//此考生的申报
        public ICollection<DeliverFile> DeliverFiles{get;set;}//此考生的投档
        public ICollection<StudentUniversityAdjustment> StudentUniversityAdjustments{get;set;}//考生服从调剂情况
    }
}