using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Utils
{
    public static class Util
    {
        public const int MAX_INPUT_BUFFER_SIZE = 32;

        public static string GetImGuiInputText(byte[] bytes)
        {
            for (int i = 0; i < MAX_INPUT_BUFFER_SIZE; i++)
            {
                if (bytes[i] == 0)
                {
                    return Encoding.UTF8.GetString(new ReadOnlySpan<byte>(bytes, 0, i));
                }
            }
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
