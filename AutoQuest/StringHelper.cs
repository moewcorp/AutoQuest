using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestResolve
{
    internal static class StringHelper
    {
        public static byte ToNumber(this string str) => byte.Parse(str);
    }
}
