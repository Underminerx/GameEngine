using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Underminer_Core.Log
{
    /// <summary>
    /// 引擎日志类
    /// </summary>
    public static class UmLog
    {
        // trace -> debug -> info -> error -> fatal

        // 日志的输出模板
        private static string CoreLogFormat = @"[Engine] {Timestamp:HH:mm-dd } [{Level:u5}] {Message:lj} {NewLine}";      // 时间戳 告警显示字数 告警信息
        private static string ClientLogFormat = @"[Client] {Timestamp:HH:mm-dd } [{Level:u5}] {Message:lj} {NewLine}";

        private static Logger CoreLogger;
        private static Logger ClientLogger;
        static UmLog() 
        {
            CoreLogger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console(outputTemplate: CoreLogFormat).CreateLogger();
            ClientLogger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console(outputTemplate: ClientLogFormat).CreateLogger();

        }

        public static void DebugLogCore(string message) => CoreLogger.Debug(message);
        public static void InfoLogCore(string message) => CoreLogger.Information(message);
        public static void ErrorLogCore(string message) => CoreLogger.Error(message);
        public static void FatalLogCore(string message) => CoreLogger.Fatal(message);


        public static void DebugLogClient(string message) => ClientLogger.Debug(message);
        public static void InfoLogClient(string message) => ClientLogger.Information(message);
        public static void ErrorLogClient(string message) => ClientLogger.Error(message);
        public static void FatalLogClient(string message) => ClientLogger.Fatal(message);
    }
}
