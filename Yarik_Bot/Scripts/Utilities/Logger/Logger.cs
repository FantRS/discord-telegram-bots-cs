using Newtonsoft.Json;

namespace MainSpace
{
    public sealed class Logger
    {
        public static Logger Instance => _lazyInstance.Value;

        private readonly string _path = @".\logging\";
        private readonly object _logLock = new();
        private readonly Dictionary<LogPlace, StreamWriter> _writersMap = new();

        private static readonly Lazy<Logger> _lazyInstance = new(() => new Logger());

        private Logger()
        {
            Directory.CreateDirectory(_path);

            foreach (var logPlace in Enum.GetValues(typeof(LogPlace)).Cast<LogPlace>())
            {
                if (logPlace == LogPlace.All)
                    continue;

                var writer = new StreamWriter(PathCombine(logPlace.ToString()), true)
                {
                    AutoFlush = true,
                };

                _writersMap[logPlace] = writer;
            }
        }

        ~Logger()
        {
            foreach (var writer in _writersMap.Values)
            {
                writer.Dispose();
            }

            _writersMap.Clear();
        }

        public void LogDebug<T>(T message, LogPlace logPlace = LogPlace.All)
            => Log("DEBUG", message, logPlace);
        public void LogInfo<T>(T message, LogPlace logPlace = LogPlace.All)
            => Log("INFO", message, logPlace);
        public void LogFatal<T>(T message, LogPlace logPlace = LogPlace.All)
            => Log("FATAL", message, logPlace);

        private void Log<T>(string logType, T message, LogPlace logPlace)
        {
            string logDate = DateTime.Now.ToString();
            string logMessage = message?.ToString() ?? string.Empty;

            var data = new LogData
            {
                Date = logDate,
                Type = logType,
                Message = logMessage,
            };

            Console.WriteLine($"[{logDate}] {logType} : {logMessage}");

            lock (_logLock)
            {
                if (logPlace != LogPlace.All)
                {
                    WriteDataByPath(data, logPlace);
                    return;
                }

                foreach (var key in _writersMap.Keys)
                {
                    WriteDataByPath(data, key);
                }
            }
        }

        private void WriteDataByPath(LogData data, LogPlace key)
        {
            if (_writersMap.TryGetValue(key, out var writer))
            {
                string jsonData = JsonConvert.SerializeObject(data);
                writer.WriteLine(jsonData);
            }
        }

        private string PathCombine(string combinedString)
        {
            return _path + combinedString + ".txt";
        }
    }
}
