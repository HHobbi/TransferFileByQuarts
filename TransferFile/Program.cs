using System;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Impl;

namespace QuartzConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                string exePath = AppDomain.CurrentDomain.BaseDirectory;
                string jsonPath = Path.Combine(exePath, "data.json");


                if (File.Exists(jsonPath))
                {



                    string jsonContent = File.ReadAllText(jsonPath);
                    JObject jsonObject = JObject.Parse(jsonContent);
                    var time = DateTime.ParseExact((string)jsonObject["time"], "HH:mm", CultureInfo.InvariantCulture);

                    // ایجاد scheduler
                    ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                    IScheduler scheduler = await schedulerFactory.GetScheduler();
                    await scheduler.Start();

                    IJobDetail job = JobBuilder.Create<TransferJob>()
                        .WithIdentity("TransferJob", "group1")
                        .UsingJobData("jsonpath", jsonPath)
                        .Build();



                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity("myTrigger", "group1")
                        .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(time.Hour, time.Minute))
                        .Build();



                    await scheduler.ScheduleJob(job, trigger);

                    Console.WriteLine("app is running. to exit enter one key");
                    Console.ReadLine();

                    await scheduler.Shutdown();
                }
                else
                {
                    Console.WriteLine($"{jsonPath} not Exists.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}