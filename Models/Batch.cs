using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaticFilesTest.Models
{
    public class Batch
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public string Bname{get;set;}
        public DateTime ApplicationBeginTime{get;set;}
        public DateTime ApplicationEndTime{get;set;}
        public DateTime EnrollmentBeginTime{get;set;}
        public DateTime EnrollmentEndTime{get;set;}
        public float GradeLine{get;set;}

        //Navigator Property
        public ICollection<CollegeEnrollment> CollegeEnrollment{get;set;}
    }
}