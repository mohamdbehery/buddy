using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Patterns
{
    public class ChainOfResponsibility
    {
        public void ConsumeThePattern()
        {
            CORTaskHandler DEVHandler = new CORDEVTashHandler();
            CORTaskHandler QAHandler = new CORQATashHandler();

            Console.WriteLine(">>> solved by 1st handler");
            DEVHandler.SetNextHandler(QAHandler);
            DEVHandler.ProcessTask(new CORTask("DEV"));

            Console.WriteLine(">>> solved by 2nd handler");
            DEVHandler.ProcessTask(new CORTask("QA"));

            Console.WriteLine(">>> no handler found");
            DEVHandler.ProcessTask(new CORTask("ANALYSIS"));
        }
    }

    public class CORTask
    {
        public string TaskCode { get; set; }
        // other props and methods

        public CORTask(string code)
        {
            this.TaskCode = code;
        }
    }

    public abstract class CORTaskHandler
    {
        public bool IsTaskHandled { get; set; }
        public abstract CORTaskHandler NextHandler { get; set; }
        public abstract void SetNextHandler(CORTaskHandler taskHandler);
        public virtual bool ProcessTask(CORTask cORTask)
        {
            // if this method called, then no handler can handle the task.
            Console.WriteLine("Task can't be handled");
            return false;
        }
    }

    public class CORDEVTashHandler : CORTaskHandler
    {
        public override CORTaskHandler NextHandler { get; set; }

        public override void SetNextHandler(CORTaskHandler taskHandler)
        {
            this.NextHandler = taskHandler;
        }

        public override bool ProcessTask(CORTask cORTask)
        {
            if(cORTask.TaskCode == "DEV")
            {
                Console.WriteLine($"{cORTask.TaskCode} task has been handled successfuly.");
                return true;
            }
            else
            {
                if (this.NextHandler != null)
                {
                    return this.NextHandler.ProcessTask(cORTask);
                }
                else
                    return base.ProcessTask(cORTask);
            }
        }
    }

    public class CORQATashHandler : CORTaskHandler
    {
        public override CORTaskHandler NextHandler { get; set; }

        public override void SetNextHandler(CORTaskHandler taskHandler)
        {
            this.NextHandler = taskHandler;
        }

        public override bool ProcessTask(CORTask cORTask)
        {
            if (cORTask.TaskCode == "QA")
            {
                Console.WriteLine($"{cORTask.TaskCode} task has been handled successfuly.");
                return true;
            }
            else
            {
                if (this.NextHandler != null)
                {
                    return this.NextHandler.ProcessTask(cORTask);
                }
                else
                    return base.ProcessTask(cORTask);
            }
        }
    }
}
