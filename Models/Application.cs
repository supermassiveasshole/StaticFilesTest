using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaticFilesTest.Models
{
    public class Application
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Sid{get;set;}
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Uname{get;set;}
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Mid{get;set;}
        public int No{get;set;}//填报志愿时的序号

        //Navigator Property
        public UnacceptedStudent UnacceptedStudent{get;set;}
        public University University{get;set;}
        public Major Major{get;set;}
    }
}