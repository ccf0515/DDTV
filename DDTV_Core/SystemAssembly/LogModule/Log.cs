﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace DDTV_Core.SystemAssembly.Log
{
    /// <summary>
    /// 核心日志信息处理
    /// </summary>
    public class Log
    {
        private static LogClass.LogType LogLevel = LogClass.LogType.All;
        public static void AddLog(string Source, LogClass.LogType logType, string Message, bool IsError = false, Exception? exception = null)
        {
            Task.Run(() =>
            {
                LogClass logClass = new()
                {
                    RunningTime =TimeModule.Time.Operate.GetRunMilliseconds(),
                    Message = Message,
                    Source = Source,
                    Time = DateTime.Now,
                    Type = logType
                };
                if (IsError)
                {
                    ErrorLogFileWrite(Source, $"{Message},错误堆栈:\n{exception.ToString()}");
                }
                if (logType <= LogLevel)
                {
                    Console.WriteLine($"{logClass.Time}:[{Enum.GetName(typeof(LogClass.LogType), (int)logType)}][{Source}]{Message}");
                }
                else
                {
                    
                }
                if (!LogDB.Operate.AddDb(logClass))
                {
                    ErrorLogFileWrite("LogSystem", "日志数据库写入失败");
                }
            });
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Message"></param>
        public static void ErrorLogFileWrite(string Source, string Message)
        {
            string ErrorText = $"{DateTime.Now}:[Error][{Source}][{TimeModule.Time.Operate.GetRunMilliseconds()}]{Message}！";
            File.AppendAllText(LogDB.ErrorFilePath, ErrorText, Encoding.UTF8);
        }
        public static void LogInit(LogClass.LogType log = LogClass.LogType.All)
        {
            LogLevel= log;
            TimeModule.Time.Config.Init();
            LogDB.Config.SQLiteInit(false);
            AddLog(nameof(Log), LogClass.LogType.Info, "Log系统初始化完成");
        }
    }
}
