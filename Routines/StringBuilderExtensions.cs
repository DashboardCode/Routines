using System;
using System.Text;

namespace DashboardCode.Routines
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendMarkdownLine(this StringBuilder stringBuilder, string text)
        {
            return stringBuilder.Append(text).Append("   ").Append(Environment.NewLine);
        }

        public static StringBuilder AppendMarkdownHeaderLine(this StringBuilder stringBuilder, string text)
        {
            return stringBuilder.Append("### ").Append(text).Append("   ").Append(Environment.NewLine);
        }

        public static StringBuilder AppendMarkdownStackTrace(this StringBuilder stringBuilder, string text)
        {
            return stringBuilder.AppendMarkdownLine("```").AppendMarkdownLine(text).AppendMarkdownLine("```");
        }

        public static StringBuilder AppendMarkdownProperty(this StringBuilder stringBuilder, string name, string value)
        {
            return stringBuilder.Append("**" + name + "**: " + value).Append("   ").Append(Environment.NewLine);
        }
    }
}
