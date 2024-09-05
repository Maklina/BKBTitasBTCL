using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace BKBTitas.Models.AppManager
{
    
    public class LogWritter
    {
        private static readonly LogWritter _instance = new LogWritter();
        protected ILog monitoringLogger;
        protected static ILog debugLogger;
        private LogWritter()
        {
            monitoringLogger = LogManager.GetLogger("MonitoringLogger");
            debugLogger = LogManager.GetLogger("DebugLogger");
        }

        public static void Debug(string message)
        {
            debugLogger.Debug(message);
        }
        
        public static void Debug(string message, System.Exception exception)
        {
            debugLogger.Debug(message, exception);
        }

        
        public static void Info(string message)
        {
            _instance.monitoringLogger.Info(message);
        }

        
        public static void Info(string message, System.Exception exception)
        {
            _instance.monitoringLogger.Info(message, exception);
        }
        
        public static void Warn(string message)
        {
            _instance.monitoringLogger.Warn(message);
        }
        
        public static void Warn(string message, System.Exception exception)
        {
            _instance.monitoringLogger.Warn(message, exception);
        }
        
        public static void Error(string message)
        {
            _instance.monitoringLogger.Error(message);
        }
        
        public static void Error(string message, System.Exception exception)
        {
            _instance.monitoringLogger.Error(message, exception);
        }

        
        public static void Fatal(string message)
        {
            _instance.monitoringLogger.Fatal(message);
        }
 
        public static void Fatal(string message, System.Exception exception)
        {
            _instance.monitoringLogger.Fatal(message, exception);
        }

        //public string logDirectory = ConfigurationManager.AppSettings["logDirectory"];
        private static string m_exePath = string.Empty;
        public static void LogWrite(string logMessage)
        {
            m_exePath = Path.GetDirectoryName("C:\\BKBTitas\\ErrorLogFiles\\");
            if (!File.Exists(m_exePath + "\\" + "log.txt"))
                File.Create(m_exePath + "\\" + "log.txt");

            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
                AppendLog(logMessage, w);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private static void AppendLog(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\nLog Entry : ");
                txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                txtWriter.WriteLine("-------------------------------");
                txtWriter.WriteLine("{0}", logMessage);
                txtWriter.WriteLine("-------------------------------");
            }
            catch (Exception ex)
            {
            }
        }
    }
}