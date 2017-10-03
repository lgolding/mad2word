// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Text.RegularExpressions;

namespace Mad2WordLib
{
    public class MadokoHeading : MadokoBlock
    {
        private static readonly Regex s_headingPattern = new Regex(
            @"^(?<level>#+)\s*(?<text>[^{]*)({\s*(?<attributes>[^}]*)})?\s*$",
            RegexOptions.Compiled);

        public MadokoHeading(LineSource lineSource)
        {
            string line = lineSource.PeekLine();
            Match match = s_headingPattern.Match(line);
            if (!match.Success)
            {
                throw new InvalidOperationException(
                    StringUtil.Format(
                        "Unexpected attempt to create a heading from line {0}:\n{1}",
                        lineSource.LineNumber,
                        line));
            }

            lineSource.GetLine();

            Level = match.Groups["level"].Value.Length;
            Attributes = MadokoAttribute.Parse(match.Groups["attributes"].Value);

            string text = match.Groups["text"].Value.Trim();
            Runs.AddRange(MadokoLine.Parse(text));
        }

        public int Level { get; }

        public static bool MatchesLine(string line)
        {
            return line[0] == '#';
        }

        public override void Accept(IMadokoVisitor visitor)
        {
            visitor.Visit(this);
        }
    };
}