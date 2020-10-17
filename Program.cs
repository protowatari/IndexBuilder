using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
namespace IndexBuilder
{
    class Program
    {      
        public static DateTime PlannedStartTime = new DateTime(2020, 10, 16, 20, 30, 0, DateTimeKind.Local);
        public static DateTime RunningStartTime = PlannedStartTime;
        static void Main(string[] args)
        {
            
            int TotalCount = 0;
            var workingDirectory = "";
            var defaultDirectory = ConfigurationManager.AppSettings["DefaultDirectory"];
            var outputFileName = ConfigurationManager.AppSettings["ListOutputFilename"];
            if (args.Length == 0)
            {
                foreach(var arg in args)
                {
                    workingDirectory += arg;
                }
            }
            workingDirectory = String.IsNullOrWhiteSpace(workingDirectory) ? defaultDirectory : workingDirectory;
            var workingDirectoryHandle = Directory.EnumerateDirectories(workingDirectory);
            var output = new StringBuilder();
            output.Append($"<ul>");
            foreach(var directoryName in workingDirectoryHandle)
            {
                var result = RecurseDirectory(directoryName);
                TotalCount += result.Item2;
                output.Append(result.Item1);

            }
            if (Directory.EnumerateFiles(workingDirectory).Count() > 0) 
            {
                var RunningTotal = new TimeSpan();
                output.AppendLine($"<b>Titles (ESTIMATED start at {PlannedStartTime} Pacific Time).<b>");
                output.AppendLine($"<ul class='sublist'>");
                foreach (var fName in Directory.EnumerateFiles(workingDirectory))
                {
                    
                    // Skip our non video files by default.
                    if (fName.Contains(".mp4") ||
                        fName.Contains(".avi") ||
                        fName.Contains(".mpg") ||
                        fName.Contains(".mkv") ||
                        fName.Contains(".divx") ||
                        fName.Contains(".mov") ) 
                    {
                        output.AppendLine($"<li>");
                        var ffProbe = new NReco.VideoInfo.FFProbe();
                        var mediaInfo = ffProbe.GetMediaInfo(fName);                        
                        var cleanFileName = fName.Replace(workingDirectory, "");
                        cleanFileName = cleanFileName.Replace(@".mp4", "");
                        cleanFileName = cleanFileName.Replace(@".mkv", "");
                        cleanFileName = cleanFileName.Replace(@".mpg", "");
                        cleanFileName = cleanFileName.Replace(@".avi", "");
                        output.Append($"<i>{cleanFileName}</i> - Estimated Start Time {RunningStartTime}");
                        RunningStartTime = RunningStartTime.Add(mediaInfo.Duration);
                        RunningTotal += mediaInfo.Duration;
                        output.AppendLine($"</li>");
                    }
                }
                output.AppendLine($"<li>Directory Duration: {RunningTotal.Days} days {RunningTotal.Hours} hours {RunningTotal.Minutes} minutes</li>");
                output.AppendLine($"</ul>");
            }
            output.AppendLine($"<li>Total Titles: {TotalCount}</li>");
            output.AppendLine("</ul>");
            var outputStream = new FileStream(workingDirectory + outputFileName, FileMode.OpenOrCreate);
            var outputStreamWriter = new StreamWriter(outputStream);
            outputStreamWriter.WriteLine(output);
            outputStreamWriter.Flush();
            outputStreamWriter.Close();
            outputStream.Close();

        }
        public static Tuple<string,int> RecurseDirectory(string DirectoryName)
        {
            int count = 0;
            var RunningTotal = new TimeSpan();
            var output = new StringBuilder();

            output.AppendLine($"<li>");
            var cleanDirectory = DirectoryName.Replace(Directory.GetParent(DirectoryName).FullName + "\\","");
            var directoryEnumeration = Directory.EnumerateDirectories(DirectoryName);
            if (directoryEnumeration.Count() > 0) 
            { 
                output.AppendLine($"<b><h1>{cleanDirectory}</h1></b>");
            }
            else
            {
                output.AppendLine(cleanDirectory);
            }
            output.AppendLine($"<ul class='sublist'>");
            foreach (var fName in Directory.EnumerateFiles(DirectoryName))
            {
                if (fName.Contains(".mp4") ||
                    fName.Contains(".avi") ||
                    fName.Contains(".mpg") ||
                    fName.Contains(".mkv") ||
                    fName.Contains(".divx") ||
                    fName.Contains(".mov"))
                { 
                    output.AppendLine($"<li>");
                    var ffProbe = new NReco.VideoInfo.FFProbe();
                    try { 
                        var mediaInfo = ffProbe.GetMediaInfo(fName);
                        RunningTotal += mediaInfo.Duration;
                        var cleanFileName = fName.Replace(DirectoryName, "");
                        cleanFileName = cleanFileName.Replace(@"H:\Streaming Videos\First Playlist Series\", "");
                        cleanFileName = cleanFileName.Replace(@"H:\Videos\Spooktober\", "");
                        cleanFileName = cleanFileName.Replace(@".mp4", "");
                        cleanFileName = cleanFileName.Replace(@".mpg", "");
                        cleanFileName = cleanFileName.Replace(@".mkv", "");
                        cleanFileName = cleanFileName.Replace(@".mov", "");
                        cleanFileName = cleanFileName.Replace(@".avi", "");
                        cleanFileName = cleanFileName.Replace(@".divx", "");
                        output.Append($"<i>{cleanFileName}</i> - Estimated Start: {RunningStartTime}");
                        RunningStartTime = RunningStartTime.Add(mediaInfo.Duration);
                        output.AppendLine($"</li>");
                    }
                    catch (Exception ex)
                    {
                        
                        var cleanFileName = fName.Replace(DirectoryName, "");
                        cleanFileName = cleanFileName.Replace(@"H:\Streaming Videos\First Playlist Series\", "");
                        cleanFileName = cleanFileName.Replace(@"H:\Videos\Spooktober\", "");
                        cleanFileName = cleanFileName.Replace(@".mp4", "");
                        cleanFileName = cleanFileName.Replace(@".mpg", "");
                        cleanFileName = cleanFileName.Replace(@".mkv", "");
                        cleanFileName = cleanFileName.Replace(@".mov", "");
                        cleanFileName = cleanFileName.Replace(@".avi", "");
                        cleanFileName = cleanFileName.Replace(@".divx", "");
                        output.Append($"<i>{cleanFileName}</i> - Estimated Start: {RunningStartTime}");
                        RunningStartTime = RunningStartTime.Add(new TimeSpan(0, 25, 0));
                        output.AppendLine($"</li>");
                    }
                    count++;
                }
            }
            output.AppendLine($"<li>Directory Duration: {RunningTotal.Days} days {RunningTotal.Hours} hours {RunningTotal.Minutes} minutes</li>");
            output.AppendLine($"</ul>");
            if (directoryEnumeration != null && directoryEnumeration.Count() > 0)
            {
                foreach (string s in directoryEnumeration)
                {
                    var outputResult = RecurseDirectory(s);
                    output.AppendLine("<ul>");            
                    output.Append(outputResult.Item1);
                    count += outputResult.Item2;
                    //if (s.Contains("Stand") && s.Contains("Alone"))
                    //{
     
                    //}
                    output.AppendLine("</ul>");
                }
            }
            output.AppendLine($"</li>");
            Tuple<string, int> returnValue = new Tuple<string, int>(output.ToString(), count);
            return returnValue;

        }
    }
}
