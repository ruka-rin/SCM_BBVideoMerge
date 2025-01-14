using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SharpShell.SharpContextMenu;
using SharpShell.Attributes;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace SCM_BBVideoMerge
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.Directory)]
    public partial class SCM_BBVideoMerge : SharpContextMenu
    {
        public const string VideoInfoFileName = "videoInfo.json";
        public const string FFmpegProgramName = "ffmpeg.exe";
        public const string FFmpegReleasesURL = "https://github.com/BtbN/FFmpeg-Builds/releases";
        public const string ConfigFileName = "config.json";

        public static Regex OutputNameRegex { get; } = new Regex(@"(?<!\\){(?<InfoField>\S+?[^\\])}");
        public static string AssemblyDirectory { get; } = Path.GetDirectoryName(typeof(SCM_BBVideoMerge).Assembly.Location);
        public static string ConfigPath { get; } = Path.Combine(AssemblyDirectory, ConfigFileName);
        public static string TempPath { get; } = Path.GetTempPath();

        public string SelectedItemPath => SelectedItemPaths.FirstOrDefault();
        public string VideoInfoPath => Path.Combine(SelectedItemPath, VideoInfoFileName);
        public Configuration Config { get; private set; }

        public struct Configuration
        {
            public string OutputFileName;
            public bool OverwriteExistingOutputFile;
            public string CommandArgumets;

            public Configuration(string outputFileName, bool overwriteExistingOutputFile, string commandArgumets)
            {
                OutputFileName = outputFileName;
                OverwriteExistingOutputFile = overwriteExistingOutputFile;
                CommandArgumets = commandArgumets;
            }
        }

        public SCM_BBVideoMerge()
        {
            //正常的Winform程序确实应该在此调用InitializeComponent
            //但是在CreateMenu时调用会更好
            //因为每一次右键菜单弹出时，宿主程序都会创建此类的实例
            //InitializeComponent();
        }

        protected override bool CanShowMenu()
        {
            if (SelectedItemPaths.Count() != 1)
            {
                return false;
            }

            return File.Exists(VideoInfoPath);
        }

        protected override ContextMenuStrip CreateMenu()
        {
            InitializeComponent();

            CMS_Main.Items.Add(TSS1);
            if (EnvironmentEx.IsInPATH(FFmpegProgramName, out _))
            {
                CMS_Main.Items.Add(TSMI_BBVideoMerge);
            }
            else
            {
                CMS_Main.Items.Add(TSMI_DownloadFFmpeg);
            }
            CMS_Main.Items.Add(TSS2);

            return CMS_Main;
        }

        private void LoadConfig()
        {
            if (File.Exists(ConfigPath))
            {
                var jsonText = File.ReadAllText(ConfigPath);
                Config = JsonConvert.DeserializeObject<Configuration>(jsonText);
            }
            else
            {
                Config = new Configuration("{title}.mp4", false, "-c:v copy");
                var jsonText = JsonConvert.SerializeObject(Config, Formatting.Indented);
                File.WriteAllText(ConfigPath, jsonText);
            }
        }

        private void TSMI_DownloadFFmpeg_Click(object sender, EventArgs e)
        {
            Process.Start(FFmpegReleasesURL);
        }

        private void TSMI_BBVideoMerge_Click(object sender, EventArgs e)
        {
            var zeros = Encoding.ASCII.GetBytes("000000000");
            var header = new byte[9];
            var argsString = new StringBuilder(250);
            var cacheFiles = new List<string>();

            LoadConfig();

            foreach (var file in Directory.GetFiles(SelectedItemPath, "*.m4s"))
            {
                using (var stream = File.OpenRead(file))
                {
                    stream.Read(header, 0, 9);
                    if (!header.SequenceEqual(zeros))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                    }

                    var tempFile = Path.Combine(TempPath, Path.GetFileName(file));
                    using (var fileStream = File.OpenWrite(tempFile))
                    {
                        stream.CopyTo(fileStream);
                        argsString.AppendWithSpaces("-i");
                        argsString.AppendWithQuotes(tempFile);
                    }
                    cacheFiles.Add(tempFile);
                }
            }

            argsString.AppendWithSpaces(Config.CommandArgumets);

            var jsonText = File.ReadAllText(VideoInfoPath);
            var videoInfo = (JObject)JsonConvert.DeserializeObject(jsonText);
            var directoryInfo = new DirectoryInfo(SelectedItemPath);
            var parentPath = directoryInfo.Parent.FullName;
            var outputFileName = Config.OutputFileName.Trim();

            if (string.IsNullOrEmpty(outputFileName))
            {
                outputFileName = $"{directoryInfo.Name}.mp4";
            }
            else
            {
                outputFileName = OutputNameRegex.Replace(outputFileName, match =>
                {
                    var field = match.Groups["InfoField"].Value;
                    return Regex.Unescape(videoInfo[field]?.Value<string>() ?? field);
                });
            }

            argsString.AppendWithQuotes(Path.Combine(parentPath, PathEx.GetSafeFileName(outputFileName)));
            if (Config.OverwriteExistingOutputFile)
            {
                argsString.AppendWithSpaces("-y");
            }
            var tws = new ThreadWithState(argsString.ToString(), cacheFiles);
            var thread = new Thread(tws.ThreadProcess);
            thread.Start();
        }
    }
}
