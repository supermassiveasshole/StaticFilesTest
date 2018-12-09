using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaticFilesTest.Models
{
    public class DeliverFile
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public string Sid{get;set;}
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public string Uname{get;set;}
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Mid{get;set;}

        //Navigator Property
        public UnacceptedStudent UnacceptedStudent{get;set;}
        public University University{get;set;}
        public Major Major{get;set;}
    }
}