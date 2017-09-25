using System;
using System.Runtime.InteropServices;

namespace DotCanal.Driver.Common
{
    internal class Platform
    {
        private static bool _inited;
        private static bool _isMono;

        private Platform() { }

        public static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public static bool IsMono()
        {
            if (!_inited)
                Init();
            return _isMono;
        }

        private static void Init()
        {
            _inited = true;
            Type t = Type.GetType("Mono.Runtime");
            _isMono = t != null;
        }

        public static bool IsDotNetCore()
        {
            return true;
        }
    }
}
