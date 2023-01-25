using Newtonsoft.Json;
using System;
using System.IO;

namespace OB_Net_Core_Tutorial.BLL.Test.Common
{
    public class CommonHelper
    {
        public static T LoadDataFromFile<T>(string folderFilePath)
        {
            Console.WriteLine(folderFilePath);
            string path = Path.Combine("C:\\Kerja\\Onboarding\\onboarding\\Day3-4\\OB-Net-Core-Tutorial\\BLL.TEST\\", folderFilePath);
            T result = default;
            using (var reader = new StreamReader(path))
            {
                var data = reader.ReadToEnd();
                result = JsonConvert.DeserializeObject<T>(data);
            }
            return result;
        }
    }
}
