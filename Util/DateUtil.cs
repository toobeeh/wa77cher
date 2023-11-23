using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wa77cher.Util
{
    internal static class DateUtil
    {
        public static string DateTimeToDiscordTimestamp(DateTime source, string format = "R")
        {
            long unixTime = ((DateTimeOffset)source).ToUnixTimeSeconds();

            var result = string.Join(" ", format.Split().ToList().ConvertAll(mode => $"<t:{unixTime}:{mode}>"));
            return result;
        }
    }
}
