using StaticFilesTest.Models;
using System;
using System.Linq;

namespace StaticFilesTest.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AddmissionContext context)
        {
            context.Database.EnsureCreated();
            context.SaveChanges();
            
        }
    }
}