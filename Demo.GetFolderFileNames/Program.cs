using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.GetFolderFileNames
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Folder Path: ");
            string folderPath = Console.ReadLine();
            DirectoryInfo d = new DirectoryInfo(folderPath);
            FileInfo[] Files = d.GetFiles("*.dll"); 
            foreach (FileInfo file in Files)
            {
                Console.WriteLine(file.Name);
            }
            Console.ReadLine();
        }
    }
}
