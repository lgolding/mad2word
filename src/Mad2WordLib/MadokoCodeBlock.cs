// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;

namespace Mad2WordLib
{
    public class MadokoCodeBlock : MadokoBlock
    {
        private const string CodeBlockMarker = "```";

        public static MadokoCodeBlock CreateFrom(string firstLine, LineSource lineSource)
        {
            MadokoCodeBlock codeBlock = null;

            if (IsCodeBlockMarker(firstLine))
            {
                codeBlock = new MadokoCodeBlock();

                string line;
                while (lineSource.MoreLines)
                {
                    line = lineSource.GetNextLine().Trim();
                    if (IsCodeBlockMarker(line))
                    {
                        break;
                    }

                    codeBlock.Runs.Add(new MadokoRun(MadokoRunType.PlainText, line + '\n'));
                }
            }

            return codeBlock;
        }

        public override void Accept(IMadokoVisitor visitor)
        {
            visitor.Visit(this);
        }

        private static bool IsCodeBlockMarker(string line)
        {
            return line.Equals(CodeBlockMarker, StringComparison.Ordinal);
        }
    }
}