using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace StaticFilesTest.Models
{
    public class AdmissionsOffice
    {
        [Key]
        public string Aname{get;set;}//招生办名称
        public string Apassword{get;set;}//招生办密码

        //Navigator Property
        public ICollection<University> University{get;set;}
    }
}