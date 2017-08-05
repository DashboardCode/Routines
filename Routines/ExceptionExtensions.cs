using System;
using System.Text;

namespace DashboardCode.Routines
{
    public static class ExceptionExtensions
    {
        public static string Markdown(this Exception exception, Action<StringBuilder, Exception> specificAppender = null)
        {
            var stringBuilder = new StringBuilder();
            if (exception.InnerException != null)
            {
                var iter = exception;
                stringBuilder.AppendMarkdownHeaderLine("SUMMARY");
                stringBuilder.Append(" - ").AppendMarkdownLine(iter.Message);
                while (iter.InnerException != null)
                {
                    iter = iter.InnerException;
                    stringBuilder.Append(" - ").AppendMarkdownLine(iter.Message);
                }
            }

            Action<Exception, string, StringBuilder> append = (ex, header, sb) => {
                sb.Append(header).Append(" ")
                  .Append(exception.GetType().FullName)
                  .Append(" - ").AppendMarkdownLine(exception.Message)
                  .AppendException(ex);
                if (ex is ArgumentException argumentException)
                    sb.AppendArgumentException(argumentException);
                if (ex is System.IO.FileLoadException fileLoadException)
                    sb.AppendFileLoadException(fileLoadException);
                if (ex is System.IO.FileNotFoundException fileNotFoundException)
                    sb.AppendFileNotFoundException(fileNotFoundException);
                specificAppender?.Invoke(sb, ex);
                if (exception.StackTrace != null)
                    sb.AppendMarkdownStackTrace(exception.StackTrace.Replace(Environment.NewLine, "  " + Environment.NewLine));
            };

            append(exception, "### ", stringBuilder);

            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                append(exception, "### ", stringBuilder);
            }
            var text = stringBuilder.ToString();
            return text;
        }

        private static void AppendFileLoadException(this StringBuilder stringBuilder, System.IO.FileLoadException exception)
        {
            stringBuilder.AppendMarkdownLine("FileLoadException specific:");
            stringBuilder.Append("   ").AppendMarkdownLine($"[FileName] {exception.FileName}");
#if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7)
                stringBuilder.Append("   ").AppendMarkdownLine($"[FusionLog] {exception.FusionLog}");
#endif
        }

        private static void AppendFileNotFoundException(this StringBuilder stringBuilder, System.IO.FileNotFoundException exception)
        {
            stringBuilder.AppendMarkdownLine("FileLoadException specific:");
            stringBuilder.Append("   ").AppendMarkdownLine($"[FileName] {exception.FileName}");
#if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7)
                stringBuilder.Append("   ").AppendMarkdownLine($"[FusionLog] {exception.FusionLog}");
#endif
        }

        private static void AppendException(this StringBuilder stringBuilder, Exception exception)
        {
            if (exception.HelpLink != null || exception.HResult != -2146233088 /*default hresult for .net exception*/)
            {
                if (exception.HelpLink != null)
                {
                    stringBuilder.Append("   ").AppendMarkdownProperty("HelpLink", exception.HelpLink);
                }
                if (exception.HResult != 0)
                {
                    stringBuilder.Append("   ").AppendMarkdownProperty("HResult", exception.HResult.ToString());
                }
            }
            if (exception.Data.Count > 0)
            {
                stringBuilder.AppendMarkdownLine("Data:");
                foreach (var key in exception.Data.Keys)
                {
                    var value = exception.Data[key];
                    stringBuilder.Append("   ").AppendMarkdownLine($"[{key}] {value}");
                }

            }
        }

        private static void AppendArgumentException(this StringBuilder stringBuilder, ArgumentException exception)
        {
            stringBuilder.AppendMarkdownLine("ArgumentException specific:");
            stringBuilder.Append("   ").AppendMarkdownProperty("ParamName", exception.ParamName);
        }
    }
}