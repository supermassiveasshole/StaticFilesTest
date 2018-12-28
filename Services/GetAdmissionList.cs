using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.POIFS.FileSystem;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using StaticFilesTest.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace StaticFilesTest.Services
{
    public class GetAdmissionList
    {
        public string userName{get;set;}
        public byte[] GetExcel(AddmissionContext _context)
        {
            XSSFWorkbook wb=new XSSFWorkbook();
            NPOI.SS.UserModel.ISheet sheet=wb.CreateSheet("录取学生表");
            var students=_context.Admissions.Include(a => a.AcceptedStudent).Where(a => a.Uname==userName).ToList();
            NPOI.SS.UserModel.IRow row;
            row=sheet.CreateRow(0);
            row.CreateCell(0).SetCellValue("考号");
            row.CreateCell(1).SetCellValue("姓名");
            row.CreateCell(2).SetCellValue("总分");
            row.CreateCell(3).SetCellValue("毕业学校");
            row.CreateCell(4).SetCellValue("录取方式");
            int count=1;//行号游标
            foreach(var student in students)
            {
                row=sheet.CreateRow(count);
                row.CreateCell(0).SetCellValue(student.Sid);
                row.CreateCell(1).SetCellValue(student.AcceptedStudent.Sname);
                row.CreateCell(2).SetCellValue(student.AcceptedStudent.TotalGrade);
                row.CreateCell(3).SetCellValue(student.AcceptedStudent.GraduateSchool);
                row.CreateCell(4).SetCellValue(student.AdmissionMethod);
                count++;
            }
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            wb.Write(ms);
            return ms.ToArray();
        }
    }
}