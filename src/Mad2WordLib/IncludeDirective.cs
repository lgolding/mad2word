// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.Text.RegularExpressions;

namespace Mad2WordLib
{
    internal class IncludeDirective
    {
        private static readonly Regex s_includeDirectivePattern = new Regex(@"^\[INCLUDE=(?<path>[^]]+)\]$", RegexOptions.Compiled);

        internal static IncludeDirective CreateFrom(string line)
        {
            IncludeDirective includeDirective = null;

            Match match = s_includeDirectivePattern.Match(line);
            if (match.Success)
            {
                string path = match.Groups["path"].Value.Trim();

                includeDirective = new IncludeDirective(path);
            }

            return includeDirective;
        }

        private IncludeDirective(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}