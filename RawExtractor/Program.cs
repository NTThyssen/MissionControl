using System;
using System.Collections.Generic;
using System.IO;
using MissionControl.Data;

namespace RawExtractor
{
    class Program
    {
        private const string parentDirectory = @"/Users/TobiJes/Google Drev/DanSTAR/Data/ColdFlow/";
        
        static void Main(string[] args)
        {
            string path = "/Users/TobiJes/Google Drev/DanSTAR/Data/ColdFlow/Fuel/raw_100319_172511_172518.danstar";
            
            Program p = new Program();
            p.ProcessDirectory(parentDirectory);
            
        }
        
        public void ProcessDirectory(string targetDirectory) 
        {
            // Process the list of files found in the directory.
            string [] fileEntries = Directory.GetFiles(targetDirectory, "*.danstar");
            foreach (string fileName in fileEntries)
            {
                GenerateCSVFromRaw(fileName);
            }

            // Recurse into subdirectories of this directory.
            string [] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                ProcessDirectory(subdirectory);
            }
        }

        private void GenerateCSVFromRaw(string path)
        {
            string filename = Path.GetFileNameWithoutExtension(path);
            if (filename == null && File.Exists(path))
            {
                Console.WriteLine("Path: {0} was not a file", path);
                return;
            }
            
            string directoryPath = Path.GetDirectoryName(path);
            string newFileName = filename.Replace("raw_", "gen_");
            string newFilePath = Path.Combine(directoryPath, newFileName) + ".csv";
            
            using (FileStream fs = File.OpenRead(path))
            {
                byte[] start = {0xFD, 0xFF, 0xFF, 0xFF, 0xFF};
                byte[] end   = {0xFE, 0xFF, 0xFF, 0xFF, 0xFF};
                Protocol protocol = new Protocol(start, end);
                
                byte[] b = new byte[1024];
                while (fs.Read(b,0,b.Length) > 0)
                {
                    protocol.Add(b);
                }
                
                List<DataPacket> frames = protocol.FindFrames();
                
                ComponentMapping mapping = new TestStandMapping();
                Session session = new Session(mapping);
                
                using (StreamWriter sw = new StreamWriter(newFilePath))
                {
                    string header = FormatWriter.PrettyHeader(session.Mapping.Loggables());
                    sw.WriteLine(header);
                    foreach (DataPacket dp in frames)
                    {
                        session.UpdateComponents(dp.Bytes);
                        string pretty = FormatWriter.PrettyLine(session.SystemTime, dp.Bytes, session.Mapping.Loggables());
                        sw.WriteLine(pretty);
                    }
                }
            }
        }
    }
}