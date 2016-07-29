// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Text;

namespace Mad2WordLib
{
    public class MadokoCodeBlock : MadokoBlock
    {
        private const string CodeBlockFence = "```";

        public static MadokoCodeBlock CreateFrom(string firstLine, LineSource lineSource)
        {
            MadokoCodeBlock codeBlock = null;
            if (firstLine.StartsWith(CodeBlockFence, StringComparison.Ordinal))
            {
                codeBlock = new MadokoCodeBlock();
                var sb = new StringBuilder();
                bool isFirstLine = true;
                bool isLastLine = false;
                string line;

                while (lineSource.MoreLines)
                {
                    line = lineSource.GetNextLine().TrimEnd();

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

                codeBlock.Runs.Add(new MadokoRun(MadokoRunType.PlainText, sb.ToString()));
            }

            return codeBlock;
        }

        public override void Accept(IMadokoVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}