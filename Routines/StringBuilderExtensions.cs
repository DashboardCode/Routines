using System;
using System.Text;

namespace DashboardCode.Routines
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendMarkdownLine(this StringBuilder stringBuilder, string text) =>
            stringBuilder.Append(text).Append("   ").Append(Environment.NewLine);

        public static StringBuilder AppendMarkdownHeaderLine(this StringBuilder stringBuilder, string text) =>
            stringBuilder.Append("### ").Append(text).Append("   ").Append(Environment.NewLine);

        public static StringBuilder AppendMarkdownLineBlock(this StringBuilder stringBuilder, string text) =>
            stringBuilder.AppendLine("```").AppendMarkdownLine(text).AppendLine("```");

        public static StringBuilder AppendMarkdownProperty(this StringBuilder stringBuilder, string name, string value) =>
            stringBuilder.Append("" + name + ": " + value).Append("   ").Append(Environment.NewLine);

        public static StringBuilder AppendMarkdownEnumeration(this StringBuilder stringBuilder, int number, string value) =>
            stringBuilder.Append("" + number + ") " + value).Append("    ").Append(Environment.NewLine);
    }
}