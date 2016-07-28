// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.Text.RegularExpressions;

namespace Mad2WordLib
{
    public class MadokoHeading : MadokoBlock
    {
        private static readonly Regex s_headingPattern = new Regex(@"^(?<level>#+)\s*(?<text>[^{]*)", RegexOptions.Compiled);

        public static MadokoHeading CreateFrom(string line)
        {
            MadokoHeading madokoHeading = null;

            Match match = s_headingPattern.Match(line);
            if (match.Success)
            {
                int level = match.Groups["level"].Value.Length;
                string text = match.Groups["text"].Value.Trim();

                madokoHeading = new MadokoHeading(level, text);
            }

            return madokoHeading;
        }

        private MadokoHeading(int level, string text)
        {
            Level = level;
            Runs.Add(new MadokoRun(MadokoRunType.PlainText, text));
        }

        internal int Level { get; }
    };
}
