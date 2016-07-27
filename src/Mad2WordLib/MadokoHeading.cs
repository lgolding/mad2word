// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.Globalization;
using System.Text.RegularExpressions;

namespace Mad2WordLib
{
    internal class MadokoHeading
    {
        internal static MadokoHeading CreateFrom(string line)
        {
            return new MadokoHeading(line);
        }

        private static readonly Regex s_headingPattern = new Regex(@"(?<level>#+)\s*(?<text>[^{]*)", RegexOptions.Compiled);

        private MadokoHeading(string line)
        {
            Match match = s_headingPattern.Match(line);
            if (match.Success)
            {
                Level = match.Groups["level"].Value.Length;
                Text = match.Groups["text"].Value.Trim();
            }
            else
            {
                Level = 1;
                Text = string.Format(CultureInfo.CurrentCulture, Resources.ErrorInvalidHeading, line);
            }
        }

        internal int Level { get; }
        internal string Text { get; }
    };
}
