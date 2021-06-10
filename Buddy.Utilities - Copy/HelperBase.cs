using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buddy.Utilities
{
    public class HelperBase
    {
        public HelperBase()
        {

        }

        public byte[] ConvertFileBase64StringToByteArray(string File)
        {
            string[] FileData = File.Split(',');
            string FileString = FileData[1];
            return Encoding.UTF8.GetBytes(FileString);
        }

        public string SpreadWord(string word)
        {
            string[] tempWord = new string[word.Length * 2];
            int counter = 0;
            foreach (var ch in word)
            {
                tempWord[counter] = ch + "_";
                counter++;
            }
            return tempWord.ToString();
        }

        public string GetConfigKey(string Key)
        {
            return ConfigurationManager.ConnectionStrings[Key].ToString();
        }

        public string GetAppKey(string Key)
        {
            if (ConfigurationManager.AppSettings[Key] != null)
                return ConfigurationManager.AppSettings[Key].ToString();
            return string.Empty;
        }
    }
}
