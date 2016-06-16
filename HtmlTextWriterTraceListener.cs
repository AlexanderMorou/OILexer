using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    internal class HtmlTextWriterTraceListener :
        TextWriterTraceListener
    {
        private static Dictionary<ConsoleColor, string> consoleColorVariants = new Dictionary<ConsoleColor, string>
        {
            { ConsoleColor.Black,       "#000000"       }, { ConsoleColor.Blue,         "#0000C0"       },
            { ConsoleColor.Cyan,        "#00B7EB"       }, { ConsoleColor.DarkBlue,     "#000080"       },
            { ConsoleColor.DarkCyan,    "#008B8B"       }, { ConsoleColor.DarkGray,     "#646464"       },
            { ConsoleColor.DarkGreen,   "#006400"       }, { ConsoleColor.DarkMagenta,  "#8B008B"       },
            { ConsoleColor.DarkRed,     "#8B0000"       }, { ConsoleColor.DarkYellow,   "#9B870C"       },
            { ConsoleColor.Gray,        "#808080"       }, { ConsoleColor.Green,        "#00FF00"       },
            { ConsoleColor.Magenta,     "#EE82EE"       }, { ConsoleColor.Red,          "#FF0000"       },
            { ConsoleColor.White,       "#FFFFFF"       }, { ConsoleColor.Yellow,       "#FEDF00"       },
        };

        private ConsoleColor? currentForecolor = null;
        private Stream stream;
        public HtmlTextWriterTraceListener(Stream stream)
            : base(stream)
        {
            this.stream = stream;
            base.WriteLine(@"<body><head><title>Trace Listener</title></head><body style=""font-family: 'Lucida Console', Courier, Serif;background-color:" + consoleColorVariants[Console.BackgroundColor] + @";white-space:nowrap;"">");
        }

        public override void Write(string message)
        {
            CheckColorDelta();
            base.Write(ProcessMessageBody(message));
        }

        public override void WriteLine(string message)
        {
            CheckColorDelta();
            base.Write(ProcessMessageBody(message));
            base.WriteLine("<br/>");
        }

        private static string ProcessMessageBody(string message)
        {
            StringBuilder sb = new StringBuilder();
            var carriageReturnBreak = message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            bool first = true;
            foreach (var subMessage in carriageReturnBreak)
            {
                if (first)
                    first = false;
                else
                    sb.AppendLine("<br/>");
                foreach (var c in subMessage)
                    switch (c)
                    {
                        case ' ':
                            sb.Append("&nbsp;");
                            break;
                        case '<':
                            sb.Append("&lt;");
                            break;
                        case '>':
                            sb.Append("&gt;");
                            break;
                        case '\t':
                            sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
                            break;
                        default:
                            sb.Append(c);
                            break;
                    }
            }
            return sb.ToString();
        }

        private void CheckColorDelta()
        {
            if (Console.ForegroundColor != currentForecolor)
            {
                if (currentForecolor != null)
                    base.Write("</span>");
                currentForecolor = Console.ForegroundColor;
                base.Write(@"<span style=""color:" + consoleColorVariants[Console.ForegroundColor] + @""">");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this.currentForecolor != null)
                base.Write("</span>");
            base.WriteLine("</body></html>");
            base.Flush();
            stream.Flush();
            stream.Dispose();
            base.Dispose(disposing);
        }
    }
}
