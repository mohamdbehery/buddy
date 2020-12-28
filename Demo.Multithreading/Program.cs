using Buddy.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Multithreading
{
    class Program
    {
        static void Main(string[] args)
        {
            //ThreadHandler threadHandler = new ThreadHandler();
            //Thread myThread = new Thread(threadHandler.DoSometing);
            //myThread.Start();

            int totalNumber = 10000;
            List<int> bigList = Enumerable.Range(0, totalNumber).ToList();

            new TaskHandler().HandleBigListParallel(bigList);
            Console.ReadLine();
        }
    }

    public class TaskHandler
    {
        Helper helper = new Helper(true);
        public void HandleBigListSequential(List<int> bigList)
        {
            var watch = new Stopwatch();
            watch.Start();
            HandleList(bigList);
            watch.Stop();
            Console.WriteLine($"Time $$$ {watch.ElapsedMilliseconds} from thread {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("Done...");
        }

        public void HandleBigListParallel(List<int> bigList)
        {
            int singleBatchValue = 1000;
            var dataBatches = SplitList(bigList, singleBatchValue);

            var watch = new Stopwatch();
            watch.Start();
            List<Task> taskList = new List<Task>();
            foreach (var dataList in dataBatches)
            {
                taskList.Add(Task.Run(() => { HandleList(dataList); }));
            }
            Task.WaitAll(taskList.ToArray());
            Task.WhenAll(taskList).ContinueWith((res) =>
            {
                watch.Stop();
                Console.WriteLine($"Time $$$ {watch.ElapsedMilliseconds} from thread {Thread.CurrentThread.ManagedThreadId}");
                Console.WriteLine("Done...");
            });
        }

        public void HandleList(List<int> dataList)
        {
            Console.WriteLine($"Hello from thread {Thread.CurrentThread.ManagedThreadId}");
            foreach (int item in dataList)
            {
                helper.Log($"Hello from {item}");
            }
        }

        public IEnumerable<List<T>> SplitList<T>(List<T> allData, int nSize = 30)
        {
            for (int i = 0; i < allData.Count; i += nSize)
            {
                yield return allData.GetRange(i, Math.Min(nSize, allData.Count - i));
            }
        }
    }
}
