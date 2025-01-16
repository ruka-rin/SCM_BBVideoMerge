using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SCM_BBVideoMerge
{
    public class ThreadWithState
    {
        public readonly string Arguments;
        public readonly List<string> CacheFiles;

        public ThreadWithState(string args, List<string> cacheFiles)
        {
            Arguments = args;
            CacheFiles = cacheFiles;
        }

        public void ThreadProcess()
        {
            var process = new Process();
            process.StartInfo.FileName = GlobalConstants.FFmpegProgramName;
            process.StartInfo.Arguments = Arguments;
            process.Start();
            process.WaitForExit();
            CacheFiles.ForEach(File.Delete);
        }
    }
}
