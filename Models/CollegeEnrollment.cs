using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaticFilesTest.Models
{
    public class CollegeEnrollment
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public string Uname{get;set;}
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Mid{get;set;}
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public string Bname{get;set;}
        public int Menrollment{get;set;}
       // public int EnrollmentRemaning{get;set;}//剩余名额
        
        //navigator property
        public University University{get;set;}
        public Major Major{get;set;}
        public Batch Batch{get;set;}
    }
}