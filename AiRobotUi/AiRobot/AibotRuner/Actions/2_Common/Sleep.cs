using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

namespace Aibot
{
    [AibotItem("时间-等待", ActionType = ActionType.CommonServer)]
    public class Sleep : BaseAibotAction,IAibotAction
    {
        [AibotProperty("毫秒(Int)", AibotKeyType.Integer, Usage=AibotKeyUsage.Input)]
        public AibotProperty SleepTime { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var i = SleepTime.Value?.TryInt() ?? 1000;
            Thread.Sleep(i);
            return Task.CompletedTask;
        }
    }

    [AibotItem("时间-定时任务", ActionType = ActionType.CommonServer)]
    public class SleepToTime : BaseAibotAction, IAibotAction
    {
        [AibotProperty("Cron表达式(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty SleepTime { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            try
            {
                var cron = SleepTime.Value?.ToString();
                var schedule = NCrontab.CrontabSchedule.Parse(cron);
                var nextRun = schedule.GetNextOccurrence(DateTime.Now);
                blackboard.Node.Title = $"定时执行({nextRun.ToString("yyyy-MM-dd HH:mm:ss")})";
                CustomOverlayManager.AddLogMessage($"[{DateTime.Now.ToString("MM/dd HH:mm:ss")}]:({blackboard.Node.Title})");

                WaitUntil(nextRun).Wait();
            }
            catch
            {
                blackboard.Node.Title = $"定时执行(Cron表达式 解析失败)";
                CustomOverlayManager.AddLogMessage($"[{DateTime.Now.ToString("MM/dd HH:mm:ss")}]:({blackboard.Node.Title})");

            }

            return Task.CompletedTask;
        }

        private DateTime ParseDateTime(string timeString)
        {
            DateTime currentDate = DateTime.Today;
            DateTime targetTime;

            if (DateTime.TryParse(timeString, out targetTime))
            {
                // 只输入了时间,不包含日期
                return currentDate.Date + targetTime.TimeOfDay;
            }
            else
            {
                // 输入了日期和时间
                string[] parts = timeString.Split(' ');
                string datePart = parts[0];
                string timePart = parts[1];

                DateTime dateValue = DateTime.MinValue;
                DateTime timeValue = DateTime.MinValue;

                if (DateTime.TryParse(datePart, out dateValue) && DateTime.TryParse(timePart, out timeValue))
                {
                    DateTime combined = new DateTime(dateValue.Year, dateValue.Month, dateValue.Day, timeValue.Hour, timeValue.Minute, timeValue.Second);

                    if (combined < currentDate)
                    {
                        combined = combined.AddYears(1);
                    }


                    return combined;
                }
            }

            throw new FormatException("Invalid date/time format.");
        }

        private async Task WaitUntil(DateTime targetTime)
        {
            TimeSpan timeToWait = targetTime - DateTime.Now;
            if (timeToWait.TotalMilliseconds > 0)
            {
                Console.WriteLine($"Waiting until {targetTime}...");
                await Task.Delay(timeToWait);
            }
            else
            {
                Console.WriteLine("Target time has already passed.");
            }
        }
    }
}
