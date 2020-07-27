using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LagrangianDesign.IO.ExtendedAttributes.Mac.ExtendedAttributeExtensions {
#pragma warning disable IDE1006
    internal static class NativeMethods {
        [DllImport("libSystem.dylib", SetLastError=true)]
        public static extern Int64 getxattr(
            String path,
            String name,
            Byte[] value,
            Int64 size,
            UInt32 position,
            Int32 options);

        [DllImport("libSystem.dylib", SetLastError=true)]
        public static extern Int64 listxattr(
            String path,
            Byte[] namebuf,
            Int64 size,
            Int32 options);

        [DllImport("libSystem.dylib", SetLastError=true)]
        public static extern Int32 removexattr(
            String path,
            String name,
            Int32 options);

        [DllImport("libSystem.dylib", SetLastError=true)]
        public static extern Int32 setxattr(
            String path,
            String name,
            in Byte value,
            Int64 size,
            UInt32 position,
            Int32 options);

        [DllImport("libSystem.dylib", SetLastError = true)]
        public static extern Int32 strerror_r(
            Int32 errnum,
            Byte[] strerrbuf,
            Int64 buflen);
    }
#pragma warning restore IDE1006
}
