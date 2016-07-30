// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Globalization;
using System.Text;

namespace Mad2WordLib
{
    public class MadokoCodeBlock : MadokoBlock
    {
        private const string CodeBlockFence = "```";

        public MadokoCodeBlock(LineSource lineSource)
        {
            string line = lineSource.PeekLine();
            if (!MatchesLine(line))
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Unexpected attempt to create a code block from line {0}:\n{1}",
                        lineSource.LineNumber,
                        line));
            }

            lineSource.Advance();

            var sb = new StringBuilder();
            bool isFirstLine = true;
            bool isLastLine = false;

            while (!lineSource.AtEnd)
            {
                line = lineSource.GetLine().TrimEnd();

                int fenceIndex = line.IndexOf(CodeBlockFence, StringComparison.Ordinal);
                if (fenceIndex != -1)
                {
                    line = line.Substring(0, fenceIndex);
                    isLastLine = true;
                }

                if (!isFirstLine && !(isLastLine && line.Length == 0))
                {
                    sb.Append(Environment.NewLine);
                }

                if (isLastLine)
                {
                    if (line.Length > 0)
                    {
                        sb.Append(line);
                    }
                    break;
                }
                else
                {
                    sb.Append(line);
                }

                isFirstLine = false;
            }

            Runs.Add(new MadokoRun(MadokoRunType.PlainText, sb.ToString()));
        }

        public static bool MatchesLine(string line)
        {
            return line.StartsWith(CodeBlockFence, StringComparison.Ordinal);
        }

        public override void Accept(IMadokoVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}