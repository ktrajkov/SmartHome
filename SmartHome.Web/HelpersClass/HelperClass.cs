using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartHome.Web.HelpersClass
{
    public static  class HelperClass
    {
        /// <summary>
        /// Converts a DateTime to a javascript timestamp.       
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The javascript timestamp.</returns>
        public static long ToJavascriptTimestamp(this DateTime input)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var time = input.Subtract(new TimeSpan(epoch.Ticks));
            return (long)(time.Ticks / 10000);
        }
    }
}