using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StaticFilesTest.Models
{
    public class Admission
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Sid{get;set;}//考生号,FK
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Uname{get;set;}//学校名,FK
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Mid{get;set;}//专业号,FK
        public string AdmissionMethod{get;set;}//录取方式

        //以下是导航元素
        public AcceptedStudent AcceptedStudent{get;set;}
        public University University{get;set;}
        public Major Major{get;set;}
    }
}