using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaticFilesTest.Models
{
    public class Major
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Mid{get;set;}//专业号
        public string Mname{get;set;}//专业名
    }
}