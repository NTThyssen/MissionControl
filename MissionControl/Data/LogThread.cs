using System;
using System.IO;
using System.Text;
using System.Threading;

namespace MissionControl.Data
{
    public interface ILogThread
    {
        void StartLogging();
        void StopLogging();
    }

    public class LogThread : ILogThread
    {
        Thread t;
        string _rawFilename;
        string _prettyFilename;
        IDataLog _dataLog;

        bool _isLogging;

        public LogThread(IDataLog dataLog)
        {

            _dataLog = dataLog;
        }


        public void StartLogging() 
        {
            t = new Thread(RunMethod) { Name = "Logger Thread" };
            _rawFilename = _dataLog.GetCurrentSession().LogFilePath + "/" + "raw_" + DateTime.Now.ToString("ddMMyy_HHmmss_");
            _prettyFilename = _dataLog.GetCurrentSession().LogFilePath + "/" + "pretty_" + DateTime.Now.ToString("ddMMyy_HHmmss_");
            _isLogging = true;
            t.Start();
        }

        public void StopLogging() 
        {
            if (!_isLogging)
            {
                return;
            }

            _isLogging = false;
            t.Join();

            string newFilename;
           
            newFilename = _rawFilename + DateTime.Now.ToString("HHmmss") + ".danstar";
            File.Move(_rawFilename, newFilename);

            newFilename = _prettyFilename + DateTime.Now.ToString("HHmmss") + ".csv";
            File.Move(_prettyFilename, newFilename);
        }

        private void RunMethod() {

            if (!Directory.Exists(_dataLog.GetCurrentSession().LogFilePath))
            {
                throw new Exception("Directory does not exist");
            }

            // Clear raw file
            if(File.Exists(_rawFilename))
            {
                File.Delete(_rawFilename);
            }

            // Clear pretty file
            if (File.Exists(_prettyFilename))
            {
                File.Delete(_prettyFilename);
            }

            using (FileStream fs = File.Create(_rawFilename))
            {
                using (StreamWriter sw = new StreamWriter(_prettyFilename))
                {
                    string header = FormatWriter.PrettyHeader(_dataLog.GetCurrentSession().Mapping.Loggables());
                    sw.WriteLine(header);

                    while (true && _isLogging)
                    {

                        while (!_dataLog.Empty())
                        {
                            DataPacket packet = _dataLog.Dequeue();

                            // Write raw as backup
                            byte[] raw = FormatWriter.ToRaw(packet.Bytes);
                            fs.Write(raw, 0, raw.Length);
                            Console.WriteLine("Raw length written: {0}", raw.Length);

                            // Update components
                            _dataLog.GetCurrentSession().UpdateComponents(packet.Bytes);

                            // Write pretty
                            string pretty = FormatWriter.PrettyLine(_dataLog.GetCurrentSession().SystemTime, packet.Bytes, _dataLog.GetCurrentSession().Mapping.Loggables());
                            sw.WriteLine(pretty);
                        }
                    }
                }
            }
        }
    }
}
