using System;

namespace StaticFilesTest.Api.Office
{
    public class EnrollmentInfo
    {
        public string School{get;set;}
        public int Population{get;set;}
        public CollegeMajorEnrollment[] Professions{get;set;}
    }
}