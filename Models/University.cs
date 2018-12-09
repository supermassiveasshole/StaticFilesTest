using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaticFilesTest.Models
{
    public class University
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public string Uname{get;set;}//高校名称
        public string Upassword{get;set;}//密码
        public int Enrollment{get;set;}//总招生数
        public float ExpandRate{get;set;}//调档比例
        public bool ApprovalStatus{get;set;}//审批情况
        public string Aname{get;set;}//招生办名称，FK
        
        //navigator property
        public AdmissionsOffice AdmissionsOffice{get;set;}//审核此校的招生办
        public ICollection<DeliverFile> DeliverFiles{get;set;}//投档
        public ICollection<Admission> Admissions{get;set;}//录取
        public ICollection<CollegeEnrollment> Majors{get;set;}//此校开设的专业
    }
}
