using System;
using System.Text;

namespace DashboardCode.Routines
{
    public static class ExceptionExtensions
    {
        public static string Markdown(this Exception exception, Action<StringBuilder, Exception> specificAppender = null)
        {
            var stringBuilder = new StringBuilder();

            void appendHeadersRecursive(Exception ex)
            {
                stringBuilder.Append(" - ").AppendMarkdownLine(ex.Message);
                if (ex.InnerException != null)
                    appendHeadersRecursive(ex.InnerException);
                if (ex is AggregateException aException)
                    foreach (var e in aException.InnerExceptions)
                        appendHeadersRecursive(e);
            }

            if (exception.InnerException != null || (exception is AggregateException aggregateException))
            {
                stringBuilder.AppendMarkdownHeaderLine("SUMMARY");
                appendHeadersRecursive(exception);
                stringBuilder.AppendLine();
            }

            void appendStackTraceAndDataRecursive(Exception ex)
            {
                stringBuilder.Append("### ").Append(" ")
                      .Append(ex.GetType().FullName)
                      .Append(" - ").AppendMarkdownLine(ex.Message)
                      .AppendException(ex);
                if (ex is ArgumentException argumentException)
                    stringBuilder.AppendArgumentException(argumentException);
                if (ex is System.IO.FileLoadException fileLoadException)
                    stringBuilder.AppendFileLoadException(fileLoadException);
                if (ex is System.IO.FileNotFoundException fileNotFoundException)
                    stringBuilder.AppendFileNotFoundException(fileNotFoundException);
                specificAppender?.Invoke(stringBuilder, ex);
                if (ex.StackTrace != null)
                {
                    // TODO 1: highlight <new line>--- End of stack trace from previous location where exception was thrown ---<end line>
                    // TODO 2: highlight <new line>at
                    stringBuilder.AppendMarkdownLineBlock(ex.StackTrace.Replace(Environment.NewLine, "  " + Environment.NewLine));
                }
                if (ex.InnerException != null)
                    appendStackTraceAndDataRecursive(ex.InnerException);
                if (ex is AggregateException aException)
                    foreach (var e in aException.InnerExceptions)
                        appendStackTraceAndDataRecursive(e);
            }

            appendStackTraceAndDataRecursive(exception);
            var text = stringBuilder.ToString();
            return text;
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

        private static void AppendFileLoadException(this StringBuilder stringBuilder, System.IO.FileLoadException exception)
        {
            stringBuilder.AppendMarkdownLine("FileLoadException specific:");
            stringBuilder.Append("   ").AppendMarkdownLine($"[FileName] {exception.FileName}");
#if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7 || NETSTANDARD2_0)
                stringBuilder.Append("   ").AppendMarkdownLine($"[FusionLog] {exception.FusionLog}");
#endif
        }

        private static void AppendFileNotFoundException(this StringBuilder stringBuilder, System.IO.FileNotFoundException exception)
        {
            stringBuilder.AppendMarkdownLine("FileLoadException specific:");
            stringBuilder.Append("   ").AppendMarkdownLine($"[FileName] {exception.FileName}");
#if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7 || NETSTANDARD2_0)
                stringBuilder.Append("   ").AppendMarkdownLine($"[FusionLog] {exception.FusionLog}");
#endif
        }

        private static void AppendArgumentException(this StringBuilder stringBuilder, ArgumentException exception)
        {
            stringBuilder.AppendMarkdownLine("ArgumentException specific:");
            stringBuilder.Append("   ").AppendMarkdownProperty("ParamName", exception.ParamName);
        }
    }
}