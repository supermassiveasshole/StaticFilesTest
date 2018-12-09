using System;
using System.ComponentModel.DataAnnotations;

namespace StaticFilesTest.Models
{
    public class StudentUniversityAdjustment
    {
        public string Sid{get;set;}
        public string Uname{get;set;}
        public bool Adjustment{get;set;}

        //Navigator Property
        public UnacceptedStudent UnacceptedStudent{get;set;}
        public University University{get;set;}
    }
}