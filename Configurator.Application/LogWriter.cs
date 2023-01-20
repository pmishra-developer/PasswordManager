using System.Text;

namespace Configurator.Application
{
    public class LogWriter
    {
        private static string _logFileName;

        private static readonly ReaderWriterLockSlim ReadWriteLock = new ReaderWriterLockSlim();

        public static void CreateLogFile()
        {

            _logFileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var iCodeDirectoryPath = Path.Combine(_logFileName, "iCodeBootLoader");
            // if directory is not there, create directory and file
            if (!Directory.Exists(iCodeDirectoryPath))
            {
                Directory.CreateDirectory(iCodeDirectoryPath);
            }

            _logFileName = Path.Combine(iCodeDirectoryPath, "logfile.txt");

            if (File.Exists(_logFileName) != false)
                return;

            using (File.Create(_logFileName)) { }
        }

        public static void WriteToLog(string message)
        {
            if (string.IsNullOrEmpty(_logFileName))
                return;

            // Set Status to Locked
            ReadWriteLock.EnterWriteLock();
            try
            {
                using (StreamWriter sw = File.AppendText(_logFileName))
                {
                    var now = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                    sw.WriteLine(now + " -- " + message);
                    sw.Flush();
                }
            }
            finally
            {
                // Release lock
                ReadWriteLock.ExitWriteLock();
            }
        }

        public static void WriteToLog(List<string> messagesList)
        {
            if (string.IsNullOrEmpty(_logFileName))
                return;

            // Set Status to Locked
            ReadWriteLock.EnterWriteLock();
            try
            {
                var resultList = new List<string>();
                var now = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + " -- ";
                foreach (var messsage in messagesList)
                {
                    resultList.Add(now + messsage);
                }

                File.AppendAllLines(_logFileName, resultList);
            }
            finally
            {
                // Release lock
                ReadWriteLock.ExitWriteLock();
            }
        }

        public static void Clean()
        {
            if (string.IsNullOrEmpty(_logFileName))
                return;

            // Set Status to Locked
            ReadWriteLock.EnterWriteLock();
            try
            {
                File.WriteAllText(_logFileName, string.Empty);
            }
            finally
            {
                // Release lock
                ReadWriteLock.ExitWriteLock();
            }
        }

        public static void AddBlankLine(bool newLine = false)
        {
            List<string> blankStrings = new List<string> { "                                       " };
            if (newLine)
            {
                blankStrings.Add(Environment.NewLine);
            }
            WriteToLog(blankStrings);
        }

        public static void AddSeperator(bool newLine = false)
        {
            List<string> seperatorStrings = new List<string> { "----------------------------------------------------------------" };
            if (newLine)
            {
                seperatorStrings.Add(Environment.NewLine);
            }

            WriteToLog(seperatorStrings);
        }

        public static void WriteToLog(string format, params object[] args)
        {
            var builder = new StringBuilder(format.Length + (args.Length * 8));
            builder.AppendFormat(format, args);
            WriteToLog(builder.ToString());
        }
    }
}
