using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace QuerySamples
{
    class Benchmark
    {
        private static readonly int LOOP_STOPPER = 5;

        public static double getAverageTime(Func<IEnumerable<object>> func)
        {
            double result = 0;
            for (int i = 0; i < LOOP_STOPPER; i++)
            {
                Stopwatch watch = Stopwatch.StartNew();

                func
                    .Invoke()
                    .Count();

                watch.Stop();

                result += watch.Elapsed.TotalMilliseconds;
            }
            return result / LOOP_STOPPER;
        }

        public static double getTimeOf(Func<IEnumerable<object>> func)
        {
            double result = 0;
            for (int i = 0; i < LOOP_STOPPER; i++)
            {
                Stopwatch watch = Stopwatch.StartNew();

                func
                    .Invoke()
                    .Count();

                watch.Stop();

                result += watch.Elapsed.TotalMilliseconds;
            }
            return result / LOOP_STOPPER;
        }
    }
}
