// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.Text.RegularExpressions;

namespace Mad2WordLib
{
    public class MadokoTitle : MadokoBlock
    {
        private static readonly Regex s_titlePattern = new Regex(@"^\[TITLE\]\s*$", RegexOptions.Compiled);

        public MadokoTitle(string title)
        {
            Runs.AddRange(MadokoLine.Parse(title));
        }

        public static bool MatchesLine(string nextLine)
        {
            return s_titlePattern.IsMatch(nextLine);
        }

        public override void Accept(IMadokoVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}