using System;
using System.Collections.Generic;

namespace Cake.Curl.Tests
{
    public class CurlTimeSpanTestData
    {
        public static IEnumerable<object[]> TimeSpanData =>
            new List<object[]>
            {
                new object[] { TimeSpan.FromSeconds(60) },
                new object[] { TimeSpan.FromMinutes(2) },
                new object[] { TimeSpan.FromHours(0.1) },
                new object[] { TimeSpan.FromSeconds(50.7) }
            };
    }
}
