using App.Business;
using App.Business.BusinessObjects;
using Buddy.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Temp.TestCode
{
    class Program
    {
        static Helper helper = Helper.CreateInstance();
        static void Main(string[] args)
        {
            //LinkedTest<string> linkedTest = new LinkedTest<string>();
            //linkedTest.ListCount
            //using (UnitOfWork unitOfWork = new UnitOfWork())
            //{
            //    unitOfWork.AppUserRepository.AddUser(new App.Data.LogicalModelsDTO.AppUserModel()
            //    {
            //        eMailAddress = "mohamd.behery.s@gmail.com",
            //        EntryDate = DateTime.Now,
            //        FamilyName = "Behery",
            //        FirstName = "Mohamed",
            //        IsActive = true,
            //        IsDeleted = false,
            //        LocationAddress = "Calle Moreti 9",
            //        ModifyDate = DateTime.Now,
            //        Password = "1234@$",
            //        SecondName = "Sabbah",
            //        UserName = "MBehery"
            //    });
            //    var affectedRowsCount = unitOfWork.Save();
            //}
            var x = BinarySearch(new int[] { 1, 3, 5, 7, 10, 12 }, 7);
            Console.WriteLine(x);
            Console.ReadLine();
        }

        public int Factorial(int n)
        {
            if (n >= 1) { return n * Factorial(n - 1); }
            else { return 1; }
        }

        public int Fibonacci(int n)
        {
            if (n >= 3) { return Fibonacci(n - 1) + Fibonacci(n + 2); }
            else { return 1; }
        }

        public static int BinarySearch(int[] source, int target)
        {
            int left = 0, middle = 0;
            int right = source.Length - 1;
            while (left < right)
            {
                middle = (left + right) / 2;
                if (source[middle] == target)
                    return middle;
                else if (target < source[middle])
                    right = middle - 1;
                else if (target > source[middle])
                    left = middle + 1;
            }
            return middle;
        }

        private static void writeTest(Phone v)
        {
            v.PhoneNumber = "ch";
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

        //public void RazorCompile()
        //{
        //    string filePath = @"c:\Inetpub\advisoryhome.com\/Templates/FileWatcher/UserNotificationTest";
        //    var model = new Person() { FirstName = "" };
        //    var config = new TemplateServiceConfiguration
        //    {
        //        TemplateManager = new ResolvePathTemplateManager(new[] { "MailNotification" }),
        //        DisableTempFileLocking = true
        //    };
        //    Engine.Razor = RazorEngineService.Create(config);
        //    var emailHtmlBody = Engine.Razor.RunCompile(filePath, null, model);
        //}
    }

    public class LinkedTest<T> where T : class
    {
        public int ListCount(NodeTest<T> head)
        {
            int count = 0;
            NodeTest<T> currentNode = head;
            while (currentNode != null)
            {
                currentNode = currentNode.Next;
                count++;
            }
            return count;
        }
    }
    public class NodeTest<T> where T : class
    {
        public int Value { get; set; }
        public NodeTest<T> Next { get; set; }
    }

    public class Person
    {
        public Person()
        {
            Console.WriteLine("Person Ctor");
        }
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
