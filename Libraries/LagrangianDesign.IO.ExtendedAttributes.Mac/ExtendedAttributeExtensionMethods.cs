using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using static LagrangianDesign.IO.ExtendedAttributes.Mac.ExtendedAttributeExtensions.NativeMethods;

namespace LagrangianDesign.IO.ExtendedAttributes.Mac.ExtendedAttributeExtensions {
    public static class ExtendedAttributeExtensionMethods {
        public static IList<String> ListExtendedAttributes(this FileSystemInfo file) {
            if (file is null) {
                throw new ArgumentNullException(nameof(file));
            }
            Byte[] namebuf;
            while (true) {
                var size = listxattr(file.FullName, null, 0, 0);
                if (size == -1) {
                    throw GetLastErrorException();
                }
                namebuf = new Byte[size];
                var outSize = listxattr(file.FullName, namebuf, size, 0);
                if (outSize == -1) {
                    throw GetLastErrorException();
                }
                if (outSize == size) {
                    break;
                }
            }
            var names = UTF8Encoding.Default.GetString(namebuf).Split('\0');
            return new ArraySegment<String>(names, 0, names.Length - 1);
        }

        public static Memory<Byte> GetExtendedAttribute(this FileSystemInfo file, String name) {
            if (file is null) {
                throw new ArgumentNullException(nameof(file));
            }
            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }
            var size = getxattr(file.FullName, name, null, 0, 0, 0);
            if (size == -1) {
                throw GetLastErrorException();
            }
            if (size > Int32.MaxValue) {
                throw new OverflowException("Attributes with values larger than {Int32.MaxValue:n0} < {size:n0} bytes are not supported.");
            }
            var buffer = new Byte[size];
            var readSize = getxattr(file.FullName, name, buffer, size, 0, 0);
            if (readSize == -1) {
                throw GetLastErrorException();
            }
            if (readSize > Int32.MaxValue) {
                throw new OverflowException("Attributes with values larger than {Int32.MaxValue:n0} < {readSize:n0} bytes are not supported.");
            }
            return new ArraySegment<Byte>(buffer, 0, (Int32)readSize);
        }

        public static void SetExtendedAttribute(this FileSystemInfo file, String name, ReadOnlySpan<Byte> value) {
            if (file is null) {
                throw new ArgumentNullException(nameof(file));
            }
            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (-1 == setxattr(file.FullName, name, MemoryMarshal.GetReference<Byte>(value), value.Length, 0, 0)) {
                throw GetLastErrorException();
            }
        }

        public static void DeleteExtendedAttribute(this FileSystemInfo file, String name) {
            if (file is null) {
                throw new ArgumentNullException(nameof(file));
            }
            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (-1 == removexattr(file.FullName, name, 0)) {
                throw GetLastErrorException();
            }
        }

        public static String GetExtendedAttributeString(this FileSystemInfo file, String name)
            => GetExtendedAttributeString(file, name, UTF8Encoding.Default);

        public static String GetExtendedAttributeString(this FileSystemInfo file, String name, Encoding encoding) {
            if (file is null) {
                throw new ArgumentNullException(nameof(file));
            }
            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (encoding is null) {
                throw new ArgumentNullException(nameof(encoding));
            }
            return encoding.GetString(GetExtendedAttribute(file, name).Span);
        }

        public static void SetExtendedAttributeString(this FileSystemInfo file, String name, String value)
            => SetExtendedAttributeString(file, name, value, UTF8Encoding.Default);


        public static void SetExtendedAttributeString(this FileSystemInfo file, String name, String value, Encoding encoding) {
            if (file is null) {
                throw new ArgumentNullException(nameof(file));
            }
            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (value is null) {
                throw new ArgumentNullException(nameof(value));
            }
            if (encoding is null) {
                throw new ArgumentNullException(nameof(encoding));
            }
            SetExtendedAttribute(file, name, encoding.GetBytes(value));
        }

        static Exception GetLastErrorException() {
            var errno = Marshal.GetLastWin32Error();
            var mapped = Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
            if (mapped.GetType() != typeof(COMException)) {
                return mapped;
            }
            var buffer = new Byte[1024];
            if (0 != strerror_r(errno, buffer, buffer.Length)) {
                return new IOException($"errno = {errno}.");
            } else {
                return new IOException(UTF8Encoding.Default.GetString(buffer));
            }
        }
    }
}
