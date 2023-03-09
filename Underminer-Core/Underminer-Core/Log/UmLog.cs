using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Underminer_Core.Log
{
    public static class UmLog
    {
        // trace -> debug -> info -> error -> fatal

        // 日志的输出模板
        private static string _coreLogFormat = @"[Engine] {Timestamp:HH:mm-dd } [{Level:u5}] {Message:lj} {NewLine}";      // 时间戳 告警显示字数 告警信息
        private static string _clientLogFormat = @"[Client] {Timestamp:HH:mm-dd } [{Level:u5}] {Message:lj} {NewLine}";

        private static Logger _coreLogger;
        private static Logger _clientLogger;
        static UmLog() 
        {
            _coreLogger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console(outputTemplate: _coreLogFormat).CreateLogger();
            _clientLogger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console(outputTemplate: _clientLogFormat).CreateLogger();

        }

        public static void DebugLogCore(string message) => _coreLogger.Debug(message);
        public static void InfoLogCore(string message) => _coreLogger.Information(message);
        public static void ErrorLogCore(string message) => _coreLogger.Error(message);
        public static void FatalLogCore(string message) => _coreLogger.Fatal(message);


        public static void DebugLogClient(string message) => _clientLogger.Debug(message);
        public static void InfoLogClient(string message) => _clientLogger.Information(message);
        public static void ErrorLogClient(string message) => _clientLogger.Error(message);
        public static void FatalLogClient(string message) => _clientLogger.Fatal(message);
    }
}
