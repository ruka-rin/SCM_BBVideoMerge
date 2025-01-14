using System;
using System.IO;

namespace SCM_BBVideoMerge
{
    public static class EnvironmentEx
    {
        /// <summary>
        /// 系统环境变量 PATH 中是否有指定的文件(程序)
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="path">文件目录的路径，如果找到</param>
        public static bool IsInPATH(string fileName, out string path)
        {
            path = string.Empty;
            var PATH = Environment.GetEnvironmentVariable("PATH");

            if (PATH == null)
            {
                return false;
            }

            foreach (var pathItem in PATH.Split(';'))
            {
                path = pathItem.Trim();
                if (!string.IsNullOrEmpty(path))
                {
                    if (File.Exists(Path.Combine(path, fileName)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}