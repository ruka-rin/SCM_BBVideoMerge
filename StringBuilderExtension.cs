using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCM_BBVideoMerge
{
    public static class StringBuilderExtension
    {
        public static StringBuilder AppendWithQuotes(this StringBuilder self, string value)
        {
            self.Append('"');
            self.Append(value);
            self.Append('"');
            return self;
        }

        public static StringBuilder AppendWithSpaces(this StringBuilder self, string value)
        {
            self.Append(' ');
            self.Append(value);
            self.Append(' ');
            return self;
        }
    }
}
