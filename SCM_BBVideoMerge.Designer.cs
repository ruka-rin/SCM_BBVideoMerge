using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCM_BBVideoMerge
{
    public partial class SCM_BBVideoMerge
    {
        public bool IsChineseCulture { get; } = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "zh";

        private void InitializeComponent()
        {

            CMS_Main = new ContextMenuStrip();
            CMS_Main.Name = nameof(CMS_Main);
            CMS_Main.Font = SystemInformation.MenuFont;

            TSMI_DownloadFFmpeg = new ToolStripMenuItem();
            TSMI_DownloadFFmpeg.Name = nameof(TSMI_DownloadFFmpeg);
            TSMI_DownloadFFmpeg.Text = IsChineseCulture ? "未找到 FFmpeg (点击下载)" : "FFmpeg not found (download)";
            TSMI_DownloadFFmpeg.Click += TSMI_DownloadFFmpeg_Click;

            TSMI_BBVideoMerge = new ToolStripMenuItem();
            TSMI_BBVideoMerge.Name = nameof(TSMI_BBVideoMerge);
            TSMI_BBVideoMerge.Text = IsChineseCulture ? "合并B站缓存视频" : "Merge bilibili cache video";
            TSMI_BBVideoMerge.Click += TSMI_BBVideoMerge_Click;

            TSS1 = new ToolStripSeparator();
            TSS2 = new ToolStripSeparator();
        }

        private ContextMenuStrip CMS_Main;
        private ToolStripMenuItem TSMI_DownloadFFmpeg;
        private ToolStripMenuItem TSMI_BBVideoMerge;
        private ToolStripSeparator TSS1;
        private ToolStripSeparator TSS2;
    }
}
