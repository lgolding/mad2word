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
            AppendRemainderOfBlock(lineSource);
        }

        public List<MadokoRun> Runs { get; }

        protected void AppendRemainderOfBlock(LineSource lineSource)
        {
            string line;
            while (!lineSource.AtEnd && IsContinuationLine(line = lineSource.PeekLine()))
            {
                lineSource.Advance();

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

        /// <summary>
        /// Returns a value indicating whether the current line belongs to the block
        /// being constructed.
        /// </summary>
        /// <param name="line">
        /// The line being examined.
        /// </param>
        /// <returns>
        /// <code>true</code> if <paramref name="line"/> belongs to the block being
        /// constructed; otherwise <code>false</code>.
        /// </returns>
        /// <remarks>
        /// Blocks end with a blank line or the start of a heading.
        /// </remarks>
        protected bool IsContinuationLine(string line)
        {
            return !string.IsNullOrWhiteSpace(line) && ! MadokoHeading.MatchesLine(line);
        }

        public override void Accept(IMadokoVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}