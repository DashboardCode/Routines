using System;
using System.Text;
using DashboardCode.Routines.Logging;

namespace DashboardCode.Routines.AspNetCore
{
    public class TraceDocument
    {
        public readonly TraceDocumentBuilder Builder;
        readonly StringBuilder stringBuilder = new StringBuilder();
        public TraceDocument()
        {
            this.Builder = new TraceDocumentBuilder(stringBuilder);
        }

        public bool IsEmpty()
        {
            return stringBuilder.Length == 0;
        }

        public bool IsExceptionHandled { get { return Builder.isExceptionHandled; } }

        public string Build()
        {
            var text = stringBuilder.ToString();
            return text;
        }

        public class TraceDocumentBuilder : ITraceDocumentBuilder
        {
            readonly StringBuilder stringBuilder;
            internal bool isExceptionHandled = false;
            public TraceDocumentBuilder(StringBuilder stringBuilder)
            {
                this.stringBuilder = stringBuilder;
            }

            public void AddProperty(DateTime dateTime, string message)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendMarkdownProperty(dateTime.ToString("s"), message);
            }

            public void AddVerbose(DateTime dateTime, string message)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(dateTime.ToString("s"));
                stringBuilder.AppendLine(message);
            }

            public void AddInput(DateTime dateTime, string message)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(dateTime.ToString("s"));
                stringBuilder.AppendLine(message);
            }

            public void AddOutput(DateTime dateTime, string message)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(dateTime.ToString("s"));
                stringBuilder.AppendLine(message);
            }

            public void AddException(DateTime dateTime, string message)
            {
                AddVerbose(dateTime, message);
                isExceptionHandled = true;
            }
        }
    }
}
