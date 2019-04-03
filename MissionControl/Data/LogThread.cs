using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace MissionControl.Data
{
    public interface ILogThread
    {
        void StartLogging();
        void StopLogging();
    }

    public class LogThread : ILogThread
    {
        private readonly string FN_RAW = "raw.danstar";
        private readonly string FN_PRETTY = "pretty.csv";
        private readonly string FN_SETTINGS = "settings.json";
        
        Thread t;
        string _rawFilename;
        string _prettyFilename;
        private string _settingsFilename;
        private string _folderName;
        IDataLog _dataLog;

        private bool _isLogging;
        
        public LogThread(IDataLog dataLog)
        {
            _dataLog = dataLog;
        }


        public void StartLogging() 
        {
            t = new Thread(RunMethod) { Name = "Logger Thread" };
            string logPath = PreferenceManager.Manager.Preferences.System.LogFilePath;
            
            string timestamp = DateTime.Now.ToString("ddMMyy_HHmmss_");
            _folderName = Path.Combine(logPath, "log_" + timestamp);
            Directory.CreateDirectory(_folderName);

            _rawFilename = Path.Combine(_folderName, FN_RAW);
            _prettyFilename = Path.Combine(_folderName, FN_PRETTY);
            _settingsFilename = Path.Combine(_folderName, FN_SETTINGS);
            
            _isLogging = true;
            _dataLog.GetCurrentSession().IsLogging = true;
            t.Start();
        }

        public void StopLogging() 
        {
            if (!_isLogging)
            {
                return;
            }

            _isLogging = false;
            _dataLog.GetCurrentSession().IsLogging = false;
            t.Join();

            string newFolderName = _folderName + DateTime.Now.ToString("HHmmss");
            Directory.Move(_folderName, newFolderName);
        }

        private void RunMethod() {

            if (!Directory.Exists(PreferenceManager.Manager.Preferences.System.LogFilePath))
            {
                throw new Exception("Directory does not exist");
            }

            // Clear raw file
            if(File.Exists(_rawFilename))
            {
                File.Delete(_rawFilename);
            }

            using (FileStream fs = File.Create(_rawFilename))
            {
                while (_isLogging)
                {

                    while (!_dataLog.Empty())
                    {
                        DataPacket packet = _dataLog.Dequeue();
                        if (packet == null) { continue; }

                        // Write raw as backup
                        byte[] raw = FormatRaw.ToRaw(packet.Bytes);
                        fs.Write(raw, 0, raw.Length);

                    }
                    Thread.Sleep(5);
                }
            }
            
            // Clear pretty file
            if (File.Exists(_prettyFilename))
            {
                File.Delete(_prettyFilename);
            }
            
            using (StreamWriter sw = new StreamWriter(_prettyFilename))
            {
                string header = FormatPretty.PrettyHeader(_dataLog.GetCurrentSession().Mapping.Loggables());
                sw.WriteLine(header);
                
                byte[] fileBytes = File.ReadAllBytes(_rawFilename);

                byte[][] frames = FormatRaw.FromRaw(fileBytes);

                Session sessionCopy = _dataLog.GetCurrentSession();
                
                foreach (byte[] frame in frames)
                {
                    sessionCopy.UpdateComponents(frame);
                    // Write pretty
                    string pretty = FormatPretty.PrettyLine(sessionCopy.SystemTime, sessionCopy.Mapping.Loggables());
                    //string pretty = FormatWriter.PrettyLine(stopWatch.ElapsedMilliseconds, packet.Bytes, _dataLog.GetCurrentSession().Mapping.Loggables());
                    sw.WriteLine(pretty);
                }
            }
            
            
            // Write preferences to log
            using (StreamWriter sw = new StreamWriter(_settingsFilename, false))
            {
                string text = JsonConvert.SerializeObject(PreferenceManager.Manager.Preferences);
                sw.Write(text);
            }
        }
    }
}
