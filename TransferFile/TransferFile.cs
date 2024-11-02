using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferFileLib
{
    public class TransferFile
    {

        public int maxCountFile { get; set; }
        public DateTime time { get; set; }


        public void Start(string filePath)
        {


            string jsonContent = File.ReadAllText(filePath);
            JObject jsonObject = JObject.Parse(jsonContent);
            // پیمایش آرایه Directions
            JArray Directions = (JArray)jsonObject["Directions"];
            foreach (var item in Directions)
            {



                maxCountFile = (int)jsonObject["maxCountFile"];
                time = DateTime.ParseExact((string)jsonObject["time"], "HH:mm", CultureInfo.InvariantCulture);

                Transfer((string)item["src"], (string)item["name"], item["ext"].ToObject<string[]>(), (string)item["des"]);

            }
        }

        public void Transfer(string src, string name, string[] exts, string des)
        {
            //var maxCountFile = 3;
            //string type = "zip";
            try
            {



                // ایجاد یک پوشه موقت برای فایل‌های زیپ نشده
                string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(tempPath);

                List<string> allFiles = new List<string>();

                foreach (var ext in exts)
                {
                    string[] files = Directory.GetFiles(src, $"*.{ext}");
                    allFiles.AddRange(files);
                }

                if (!Directory.Exists(des))
                {
                    // ایجاد دایرکتوری اگر وجود نداشته باشد
                    Directory.CreateDirectory(des);
                }
                // استفاده از Zip برای ترکیب دو لیست


                var Existfiles = Directory.GetFiles(des, name + "*").ToList();
                int count = Existfiles.Count;
                if (count >= this.maxCountFile)
                {
                    var oldestFile = Existfiles.OrderBy(f => new FileInfo(f).CreationTime).First();
                    File.Delete(oldestFile);
                }


                PersianCalendar pc = new PersianCalendar();


                // کپی فایل‌ها به پوشه موقت
                foreach (string file in allFiles)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(tempPath, fileName);
                    File.Copy(file, destFile, true);
                }

                // ایجاد فایل زیپ
                DateTime dateTime = new DateTime();
                string zipPath = Path.Combine(des, name + "_" + pc.GetYear(DateTime.Now) + "_" + pc.GetMonth(DateTime.Now) + "_" + pc.GetDayOfMonth(DateTime.Now) + "_" + pc.GetHour(DateTime.Now) + "_" + pc.GetMinute(DateTime.Now) + "_" + pc.GetSecond(DateTime.Now) + ".zip");
                ZipFile.CreateFromDirectory(tempPath, zipPath);



                // پاک کردن پوشه موقت
                Directory.Delete(tempPath, true);

                //// حذف فایل‌های اصلی (اختیاری)
                //foreach (string file in allFiles)
                //{
                //    File.Delete(file);
                //}
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("فایل مورد نظر یافت نشد.");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"خطای غیرمنتظره: {ex.Message}");
            }
        }
    }
}
