using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaticFilesTest.Models
{
    public class AcceptedStudent
    {
        [Key]
        public string Sid{get;set;}
        public string Sname{get;set;}
        public char Gender{get;set;}
        public string Grades{get;set;}
        public float TotalGrade{get;set;}
        public int Rank{get;set;}
        public string GraduateSchool{get;set;}//毕业学校

        //Navigator Property
        public Admission Admission{get;set;}

    }
}