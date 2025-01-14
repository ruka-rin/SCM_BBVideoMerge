using System.IO;
using System.Linq;

namespace SCM_BBVideoMerge
{
    public static class PathEx
    {
        /// <summary>
        /// 获得一个安全的文件名，过滤不合法的字符
        /// </summary>
        /// <param name="fileName">文件名</param>
        public static string GetSafeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            //return new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
            var chars = fileName.ToCharArray();
            var length = chars.Length;
            for (int i = 0; i < length; i++)
            {
                if (invalidChars.Contains(chars[i]))
                {
                    length--;
                    for (int j = i--; j < length; j++)
                    {
                        chars[j] = chars[j + 1];
                    }
                }
            }
            return new string(chars, 0, length);
        }
    }
}
