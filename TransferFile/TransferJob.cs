using Quartz;
using TransferFileLib;

namespace QuartzConsoleApp
{
    public class TransferJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string jsonPath = dataMap.GetString("jsonpath") ?? "C:\\Users\\Hosein\\Desktop\\TransferFile\\TransferFile\\bin\\Debug\\net8.0\\data.json";
            Console.WriteLine("execute the task...");
            TransferFile TF = new TransferFile();
            TF.Start(jsonPath);
            return Task.CompletedTask;
        }
    }
}