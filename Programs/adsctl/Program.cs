using System;
using System.IO;
using LagrangianDesign.IO.ExtendedAttributes.Mac.ExtendedAttributeExtensions;

using static System.Console;
using static System.Environment;
using static System.String;
using static System.StringComparison;

namespace LagrangianDesign.IO.AlternateDataStreams.adsctl {
    class Program {
        static Int32 Main(String[] args) {
            try {
                if (args.Length == 0) {
                    Error.WriteLine(Usage);
                    return ArgumentErrorCode;
                }
                var c = args[0];
                if (ListCommand.StartsWith(c, OrdinalIgnoreCase)) {
                    if (args.Length != 2) {
                        Error.WriteLine(Usage);
                        return ArgumentErrorCode;
                    }
                    WriteLine(Join(NewLine,
                       new FileInfo(args[1])
                            .ListExtendedAttributes()));
                    return SuccessCode;
                } else if (GetCommand.StartsWith(c, OrdinalIgnoreCase)) {
                    if (args.Length != 3) {
                        Error.WriteLine(Usage);
                        return ArgumentErrorCode;
                    }
                    OpenStandardOutput().Write(
                        new FileInfo(args[1])
                        .GetExtendedAttribute(args[2]).Span);
                    return SuccessCode;
                } else if (SetCommand.StartsWith(c, OrdinalIgnoreCase)) {
                    if (args.Length != 4) {
                        Error.WriteLine(Usage);
                        return ArgumentErrorCode;
                    }
                    new FileInfo(args[1])
                        .SetExtendedAttributeString(args[2], args[3]);
                    return SuccessCode;
                } else if (DeleteCommand.StartsWith(c, OrdinalIgnoreCase)) {
                    if (args.Length != 3) {
                        Error.WriteLine(Usage);
                        return ArgumentErrorCode;
                    }
                    new FileInfo(args[1]).DeleteExtendedAttribute(args[2]);
                    return SuccessCode;
                } else {
                    Error.WriteLine(Usage);
                    return ArgumentErrorCode;
                }
            } catch (Exception e) {
                Error.WriteLine(e.Message);
                return FailureCode;
            }
        }

        static readonly Int32 SuccessCode = 0;
        static readonly Int32 FailureCode = 1;
        static readonly Int32 ArgumentErrorCode = 2;

        static readonly String ListCommand = "list";
        static readonly String GetCommand = "get";
        static readonly String SetCommand = "set";
        static readonly String DeleteCommand = "delete";

        static readonly String Usage = @"
Usage: adsctl l[ist] <path>
              g[et] <path> <name>
              s[et] <path> <name> <value>
              d[elete] <path> <name>
        ".Trim();
    }
}
