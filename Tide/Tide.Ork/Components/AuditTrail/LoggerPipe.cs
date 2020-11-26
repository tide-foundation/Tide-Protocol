using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tide.Core;
using Tide.Ork.Models;
using Tide.Ork.Repo;

namespace Tide.Ork.Components.AuditTrail
{
    public class LoggerPipe : ILogger
    {
        private static readonly SendingState lockState;
        
        private readonly ILogger logger;
        private readonly SimulatorAuditManager manager;
        private readonly LoggerConfig _config;
        
        private readonly string orkId;
        private string[] keyParams => _config.KeyParams;
        public int AllowedLevel => _config.AllowedLevel;
        public int DeniedLevel => _config.DeniedLevel;

        private string path => _config.Path;
        private int maxEntries => _config.MaxEntries;
        private long maxDiff => _config.MaxDiff;

        private string sendingFile => $"{_config.Path}/{this.orkId}-ready.txt";
        private string workingFile => $"{_config.Path}/{this.orkId}-working.txt";

        static LoggerPipe()
        {
            lockState = new SendingState();
        }

        public LoggerPipe(ILogger logger, Settings settings, LoggerConfig config)
        {
            this.logger = logger;
            this.orkId = settings.Instance.Username;
            this.manager = new SimulatorAuditManager(this.orkId, settings.BuildClient());
            _config = config;
        }

        public IDisposable BeginScope<TState>(TState state) => logger.BeginScope(state);

        public bool IsEnabled(LogLevel logLevel) => logger.IsEnabled(logLevel);

        public void LoginSuccessful(string method, object tran, Guid uid, string msg)
            => LogInformation(AllowedLevel, method, tran, uid, msg);


        public void LoginUnsuccessful(string method, object tran, Guid uid, string msg)
            => LogInformation(DeniedLevel, method, tran, uid, msg);

        private void LogInformation(EventId eventId, string method, object tran, Guid uid, string msg)
        {
            var template = string.Join(", ", keyParams.Select(prm => $"{prm}: {{{prm}}}"));

            this.LogInformation(eventId, $"[{template}] message: {msg}", method, tran, uid);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            logger.Log(logLevel, eventId, state, exception, formatter);
            
            var infoLog = state as IEnumerable<KeyValuePair<string, object>>;
            if (!(eventId.Id == AllowedLevel || eventId.Id == DeniedLevel) || infoLog == null)
                return;
                
            var missings = keyParams.Where(key => !infoLog.Any(itm => itm.Key == key)).ToList();
            if (missings.Count > 0) {
                logger.LogError(4000, $"Configuration missing in log processing {string.Join(", ", missings)}");
                return;
            }

            var lastLoging = Interlocked.Exchange(ref lockState.LastLoging, DateTimeOffset.UtcNow.UtcTicks);
            var times = Interlocked.Increment(ref lockState.CountLoging);
            Write(MapCSV(eventId, times, new Dictionary<string, object>(infoLog)));

            var isMaxEntries = Interlocked.CompareExchange(ref lockState.CountLoging, 0, maxEntries) == maxEntries;
            var isMaxDiff = DateTimeOffset.UtcNow.UtcTicks - lastLoging >= maxDiff;

            var shouldSend = (isMaxEntries || isMaxDiff)
                && Interlocked.Exchange(ref lockState.Sending, 1) == 0;

            if (shouldSend)
                Send();
        }

        private Task Send()
        {
            return Task.Run(() => {
                try
                {
                    lock (lockState)
                    {
                        if (File.Exists(sendingFile)) {
                            manager.Report(File.ReadAllText(sendingFile)).GetAwaiter().GetResult();
                        }

                        File.Copy(this.workingFile, sendingFile, true);
                        File.Create(this.workingFile).Close();
                    }
                }
                finally { Interlocked.Exchange(ref lockState.Sending, 0); }
            });
        }

        private string MapCSV(EventId eventId, int times, Dictionary<string, object> info)
        {
            var auth = new AuthPending()
            {
                TranId = (Guid)info[keyParams[1]],
                Uid = (Guid)info[keyParams[2]],
                OrkId = orkId,
                Method = info[keyParams[0]].ToString(),
                Successful = eventId == AllowedLevel,
                Metadata = times.ToString()
            };

            return auth.ToString();
        }

        private Task Write(string message)
        {
            return Task.Run(() => {
                lock (lockState)
                {
                    using (StreamWriter stream = File.AppendText(workingFile))
                    {
                        stream.WriteLine(message);
                    }
                }
            });
        }

        private class SendingState
        {
            public int Sending;
            public long LastSending;
            public int CountLoging;
            public long LastLoging;

            public SendingState() {
                LastSending = DateTimeOffset.UtcNow.UtcTicks;
                LastLoging = DateTimeOffset.UtcNow.UtcTicks;
            }
        }
    }
}
