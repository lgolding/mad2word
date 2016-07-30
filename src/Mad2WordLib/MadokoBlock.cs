// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.Collections.Generic;

namespace Mad2WordLib
{
    public class MadokoBlock : MadokoNode
    {
        public MadokoBlock()
        {
            Runs = new List<MadokoRun>();
        }

       public MadokoBlock(LineSource lineSource) : this()
        {
            AppendRemainderOfParagraph(lineSource);
        }

        public List<MadokoRun> Runs { get; }

        protected void AppendRemainderOfParagraph(LineSource lineSource)
        {
            string line;
            while (!lineSource.AtEnd && !string.IsNullOrWhiteSpace(line = lineSource.GetLine()))
            {
                // This paragraph is continued from the preceding source line,
                // so make sure there's a blank space between the end of that
                // line and the start of this one.
                if (!char.IsWhiteSpace(line[0]))
                {
                    line = " " + line;
                }

                Runs.AddRange(MadokoLine.Parse(line));
            }
        }

        public override void Accept(IMadokoVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}