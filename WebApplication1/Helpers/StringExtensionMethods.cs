using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WebApplication1.Helpers
{
    public static class StringExtensionMethods
    {
        public static string AppendZerosToStart(this string str, int count)
        {
            return new StringBuilder("0".Length * count).Insert(0, "0", count).Append(str).ToString();
        }
    }
}