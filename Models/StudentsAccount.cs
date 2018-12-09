using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaticFilesTest.Models
{
    public class StudentsAccount
    {
    //[DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Key]
    public string Sid{get;set;}
    public string Spassword{get;set;}
    }
}
