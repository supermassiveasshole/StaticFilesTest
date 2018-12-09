using StaticFilesTest.Models;
using Microsoft.EntityFrameworkCore;

namespace StaticFilesTest.Data
{
    public class AddmissionContext:DbContext
    {
        public AddmissionContext(DbContextOptions<AddmissionContext> options):base(options)
        {

        }
        public DbSet<AcceptedStudent> AcceptedStudents{get;set;}
        public DbSet<Admission> Admissions{get;set;}
        public DbSet<AdmissionsOffice> AdmissionsOffices{get;set;}
        public DbSet<Batch> Batches{get;set;}
        public DbSet<CollegeEnrollment> CollegeEnrollments{get;set;}
        public DbSet<DeliverFile> DeliverFiles{get;set;}
        public DbSet<Major> Majors{get;set;}
        public DbSet<StudentsAccount> StudentsAccounts{get;set;}
        public DbSet<UnacceptedStudent> UnacceptedStudents{get;set;}
        public DbSet<University> Universities{get;set;}
        public DbSet<Application> Applications{get;set;}
        public DbSet<StudentUniversityAdjustment> StudentUniversityAdjustments{get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Application>().HasKey(t => new{t.Sid,t.Uname,t.Mid});
            modelBuilder.Entity<CollegeEnrollment>().HasKey(t => new{t.Uname,t.Mid,t.Bname});
            modelBuilder.Entity<DeliverFile>().HasKey(t => new{t.Sid,t.Uname,t.Mid});
            modelBuilder.Entity<StudentUniversityAdjustment>().HasKey(t => new{t.Sid,t.Uname});
            modelBuilder.Entity<Admission>().HasKey(t => new{t.Sid,t.Uname,t.Mid});
        }
    }
}