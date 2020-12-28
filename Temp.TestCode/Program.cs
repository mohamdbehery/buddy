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

        static Helper helper = new Helper();
        DataTable dt = new DataTable();
        static void Main(string[] args)
        {
            

            Console.ReadLine();
        }

        public void GetXMLData()
        {
            var watch = new Stopwatch();
            watch.Start();
            Dictionary<string, string> Params = new Dictionary<string, string>()
            {
                {"@BEID", "1" }
            };

            using (SqlConnection SQLCon = new SqlConnection("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=AdventureWorks2014;Data Source=EPESMALW006D"))
            {
                using (SqlCommand SQLCMD = new SqlCommand("spTest", SQLCon))
                {
                    SQLCMD.CommandType = CommandType.StoredProcedure;
                    if (Params != null && Params.Count > 0)
                    {
                        foreach (var Param in Params)
                        {
                            if (string.IsNullOrEmpty(Param.Value))
                                SQLCMD.Parameters.Add(new SqlParameter(Param.Key, DBNull.Value));
                            else
                                SQLCMD.Parameters.Add(new SqlParameter(Param.Key, Param.Value));
                        }
                    }
                    SQLCon.Open();
                    SqlDataAdapter SQLDataAd = new SqlDataAdapter(SQLCMD);
                    SQLDataAd.Fill(dt);

                    SQLCon.Close();
                    SQLDataAd.Dispose();
                }
            }

            List<Person> personList = new List<Person>();
            if (dt.Rows.Count > 0)
            {
                string xmlPersonString = dt.Rows[0]["xmlData"].ToString();
                XDocument xmlPersons = XDocument.Parse(xmlPersonString);
                personList = xmlPersons.Root.Elements("person")
                .Select(p => new Person
                {
                    PersonType = (string)p.Element("PersonType"),
                    PersonPhones = p.Elements("phone").Select(phone => new Phone
                    {
                        PhoneNumber = (string)phone.Element("PhoneNumber")
                    }).ToList()
                }).ToList();
            }
            watch.Stop();
            Console.WriteLine($"get data: {watch.ElapsedMilliseconds}");
            var xx = personList;
        }

        public static void XMLStringBuilderBulkInsertData()
        {
            var watch = new Stopwatch();
            watch.Start();
            StringBuilder xmlDox = new StringBuilder("<phones>");
            int total = 500000;
            helper.Log("start");
            for (int i = 0; i < total; i++)
            {
                helper.Log(i.ToString());
                xmlDox.Append($"<phone BusinessEntityID='2' PhoneNumber='NPX{i}' IsActive='{true}' PhoneNumberTypeID='1' ModifiedDate='{DateTime.Now}'/>");
            }
            xmlDox.Append("</phones>");
            watch.Stop();
            helper.Log("xml is ready after " + watch.ElapsedMilliseconds + "ms");

            watch = new Stopwatch();
            watch.Start();
            Dictionary<string, string> Params = new Dictionary<string, string>()
            {
                {"@xmlData", xmlDox.ToString() }
            };
            int Rows = 0;
            using (SqlConnection SQLCon = new SqlConnection("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=AdventureWorks2014;Data Source=EPESMALW006D"))
            {
                using (SqlCommand SQLCMD = new SqlCommand("spBulkInsert", SQLCon))
                {
                    SQLCMD.CommandType = CommandType.StoredProcedure;
                    if (Params != null && Params.Count > 0)
                    {
                        foreach (var Param in Params)
                        {
                            SQLCMD.Parameters.Add(new SqlParameter(Param.Key, Param.Value));
                        }
                    }
                    SQLCon.Open();
                    Rows = SQLCMD.ExecuteNonQuery();
                    SQLCon.Close();
                }
            }
            watch.Stop();
            helper.Log("xml is processed after " + watch.ElapsedMilliseconds + "ms");
            Console.WriteLine(Rows);
        }

        public static void XMLStringBuilderBulkUpdateData()
        {
            StringBuilder xmlDox = new StringBuilder("<phones>");
            int ff = 2;
            int pp = 3;
            xmlDox.Append($"<phone BusinessEntityID='{ff}' PhoneNumber='444445' IsActive='{true}' PhoneNumberTypeID='1' ModifiedDate='{DateTime.Now}'/>");
            xmlDox.Append($"<phone BusinessEntityID='{pp}' PhoneNumber='555444444' IsActive='{true}' PhoneNumberTypeID='1' ModifiedDate='{DateTime.Now}'/>");
            xmlDox.Append("</phones>");
            Dictionary<string, string> Params = new Dictionary<string, string>()
            {
                {"@xmlData", xmlDox.ToString() }
            };
            int Rows = 0;
            using (SqlConnection SQLCon = new SqlConnection("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=AdventureWorks2014;Data Source=EPESMALW006D"))
            {
                using (SqlCommand SQLCMD = new SqlCommand("spBulkUpdate", SQLCon))
                {
                    SQLCMD.CommandType = CommandType.StoredProcedure;
                    if (Params != null && Params.Count > 0)
                    {
                        foreach (var Param in Params)
                        {
                            SQLCMD.Parameters.Add(new SqlParameter(Param.Key, Param.Value));
                        }
                    }
                    SQLCon.Open();
                    Rows = SQLCMD.ExecuteNonQuery();
                    SQLCon.Close();
                }
            }

            Console.WriteLine(Rows);
        }

        public void XMLDocumentBulkUpdateData()
        {
            XElement root = new XElement("phones");
            root.Add(new XElement("phone", new XAttribute("BusinessEntityID", "2"), new XAttribute("PhoneNumber", "444"), new XAttribute("IsActive", true), new XAttribute("PhoneNumberTypeID", "1"), new XAttribute("ModifiedDate", DateTime.Now)));
            root.Add(new XElement("phone", new XAttribute("BusinessEntityID", "3"), new XAttribute("PhoneNumber", "555"), new XAttribute("IsActive", false), new XAttribute("PhoneNumberTypeID", "1"), new XAttribute("ModifiedDate", DateTime.Now.AddDays(4))));

            XDocument doc = new XDocument();
            doc.Add(root);

            Dictionary<string, string> Params = new Dictionary<string, string>()
            {
                {"@xmlData", doc.ToString() }
            };
            int Rows = 0;
            using (SqlConnection SQLCon = new SqlConnection("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=AdventureWorks2014;Data Source=EPESMALW006D"))
            {
                using (SqlCommand SQLCMD = new SqlCommand("spBulkUpdate", SQLCon))
                {
                    SQLCMD.CommandType = CommandType.StoredProcedure;
                    if (Params != null && Params.Count > 0)
                    {
                        foreach (var Param in Params)
                        {
                            SQLCMD.Parameters.Add(new SqlParameter(Param.Key, Param.Value));
                        }
                    }
                    SQLCon.Open();
                    Rows = SQLCMD.ExecuteNonQuery();
                    SQLCon.Close();
                }
            }

            Console.WriteLine(Rows);
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
