using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Utilities;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Diagnostics;
using System.Xml.Linq;
using System.Reflection;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System.IO;

namespace TestCode
{
    class Program
    {
        static Helper helper = Helper.CreateInstance();
        DataTable dt = new DataTable();
        static void Main(string[] args)
        {

            Console.ReadLine();
        } 

        public void CheckParallelLoopsPerformance()
        {
            int total = 1000000;
            List<Person> personList = new List<Person>();
            for (int i = 0; i < total; i++)
            {
                personList.Add(new Person()
                {
                    BusinessEntityID = i,
                    FirstName = "Person" + i,
                });
            }

            var watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < personList.Count; i++)
            {
                var xx = personList.Where(x => x.FirstName.Contains("Person2345"));
                if (i == personList.Count - 1)
                    Console.WriteLine(xx.FirstOrDefault().FirstName);
            }
            watch.Stop();
            Console.WriteLine($"For: {watch.ElapsedMilliseconds}");

            watch = new Stopwatch();
            watch.Start();
            Parallel.For(0, personList.Count, person =>
            {
                var xx = personList.Where(x => x.FirstName.Contains("Person2345"));
                if (person == personList.Count - 1)
                    Console.WriteLine(xx.FirstOrDefault().FirstName);
            });
            watch.Stop();
            Console.WriteLine($"Parallel For: {watch.ElapsedMilliseconds}");

            watch = new Stopwatch();
            watch.Start();
            foreach (var person in personList)
            {
                var xx = personList.Where(x => x.FirstName.Contains("Person2345"));
                if (person.BusinessEntityID == personList.Count - 1)
                    Console.WriteLine(xx.FirstOrDefault().FirstName);
            }
            watch.Stop();
            Console.WriteLine($"ForEach: {watch.ElapsedMilliseconds}");

            watch = new Stopwatch();
            watch.Start();
            Parallel.ForEach(personList, person =>
            {
                var xx = personList.Where(x => x.FirstName.Contains("Person2345"));
                if (person.BusinessEntityID == personList.Count - 1)
                    Console.WriteLine(xx.FirstOrDefault().FirstName);
            });
            watch.Stop();
            Console.WriteLine($"Parallel ForEach: {watch.ElapsedMilliseconds}");
        }

        public void RazorCompile()
        {
            string filePath = @"c:\Inetpub\advisoryhome.com\/Templates/FileWatcher/UserNotificationTest";
            var model = new Person() { FirstName = "" };
            var config = new TemplateServiceConfiguration
            {
                TemplateManager = new ResolvePathTemplateManager(new[] { "MailNotification" }),
                DisableTempFileLocking = true
            };
            Engine.Razor = RazorEngineService.Create(config);
            var emailHtmlBody = Engine.Razor.RunCompile(filePath, null, model);
        }
    }

    public class Person
    {
        public int BusinessEntityID { get; set; }
        public string PersonType { get; set; }
        public string rowguid { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public List<Phone> PersonPhones { get; set; }
    }

    public class Phone
    {
        public string PhoneNumber { get; set; }
        public int PhoneNumberTypeID { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
