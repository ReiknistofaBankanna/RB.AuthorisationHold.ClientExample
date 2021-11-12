using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using RB.AuthorisationHold.ClientSample.Entities.Enums;
using System;
using System.IO;
using System.Text;

namespace RB.AuthorisationHold.ClientSample.Utils
{
	public class Logger
	{

		private static readonly Level _logLevel = Level.All;
		private static readonly ILog _log = LogManager.GetLogger(LogNameSpace);
		private const string LogNameSpace = "ClientSample";

		/// <summary>
		/// Static constructor is called automatically to initialize the class before 
		/// the first instance is created or any static members are referenced.
		/// </summary>
		static Logger()
		{
			string logLayoutPattern = "%-5p %d{yyyy-MM-dd HH:mm:ss} %m%n";

			PatternLayout layout = new() { ConversionPattern = logLayoutPattern }; // Log line format: Include millisecond precision, thread ID, Log type,
			layout.ActivateOptions();                                       // Apply Configuration 

			string basePath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
			string configFile = Path.Combine(basePath, LogNameSpace + ".log");

			RollingFileAppender rfa = new();
			rfa.Name = LogNameSpace;                                        // Set name of appender
			rfa.File = configFile;                                          // Set file name prefix
			rfa.LockingModel = new FileAppender.MinimalLock();              // Minimum lock time required, makes file available for reading
			rfa.AppendToFile = true;                                        // Do not overwrite existing logs, append to them.
			rfa.Encoding = Encoding.UTF8;                                   // Set format of file to UTF8 for international characters.
			rfa.CountDirection = 1;                                         // Increment file name in bigger number is newest, instead of default backward.
			rfa.MaximumFileSize = "100MB";                                  // Maximum size of file that I could open with common notepad applications
			rfa.RollingStyle = RollingFileAppender.RollingMode.Composite;   // Increment file names by both size and date.
			rfa.StaticLogFileName = true;
			rfa.MaxSizeRollBackups = 0;
			rfa.PreserveLogFileNameExtension = true;                        // This plus extension added to DatePattern, causes to rolling size also work correctly
			rfa.Layout = layout;
			rfa.ActivateOptions();                                          // Apply Configuration 

			Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
			hierarchy.Root.RemoveAllAppenders();                            // Clear all previously added repositories.
			hierarchy.Root.Level = _logLevel;                               // Set Log level
			hierarchy.Root.AddAppender(rfa);
			hierarchy.Configured = true;
			BasicConfigurator.Configure(hierarchy);                         // Apply Configuration
		}
		public static void Log(string Description, LogType logtype = LogType.Info)
		{
			switch (logtype)
			{
				case LogType.Debug:
					_log.Debug(Description);
					break;
				case LogType.Info:
					_log.Info(Description);
					break;
				case LogType.Warn:
					_log.Warn(Description);
					break;
				case LogType.Error:
					_log.Error(Description);
					break;
				case LogType.Fatal:
					_log.Fatal(Description);
					break;
			}
		}
		public static void Log(string Message, Exception ex)
		{
			_log.Fatal(Message, ex);
		}
	}
}