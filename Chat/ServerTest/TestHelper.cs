using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServerTest
{
    internal static class TestHelper
    {
        public static Task BecomeTrue(Func<bool> condition, TimeSpan timeout)
        {
            return BecomeTrue(condition, timeout, string.Empty);
        }

        public static async Task BecomeTrue(Func<bool> condition, TimeSpan timeout, string message)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var until = DateTime.UtcNow + timeout;
            while (!condition())
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                if (until <= DateTime.UtcNow)
                {
                    Assert.Fail(message);
                }
            }
        }

        public static async Task Expect(this BufferBlock<string> queue, string expected, TimeSpan timeout)
        {
            var value = await queue.ReceiveAsync(timeout);
            Assert.AreEqual(value, expected);
        }
    }
}
