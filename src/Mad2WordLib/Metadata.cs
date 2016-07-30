// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mad2WordLib
{
    internal class Metadata
    {
        private static readonly Regex s_metadataPattern =
            new Regex(@"^\s*(?<key>[^:]+):\s*(?<value>.*)$", RegexOptions.Compiled);

        private readonly Dictionary<string, string> _dictionary = new Dictionary<string, string>();

        public Metadata(LineSource lineSource)
        {
            string key = null;
            string value;

            while (!lineSource.AtEnd)
            {
                string line = lineSource.GetLine();
                if (string.IsNullOrEmpty(line))
                {
                    key = null;
                    continue;
                }

                Match match = s_metadataPattern.Match(line);
                if (match.Success)
                {
                    key = match.Groups["key"].Value.Trim().ToUpperInvariant();
                    value = match.Groups["value"].Value.Trim();

                    _dictionary.Add(key, value);
                }
                else
                {
                    if (key == null)
                    {
                        lineSource.BackUp();
                        break;
                    }
                    else
                    {
                        if (line.StartsWith(" ") || line.StartsWith("\t"))
                        {
                            _dictionary[key] = _dictionary[key] + " " + line.Trim();
                        }
                        else
                        {
                            lineSource.BackUp();
                            break;
                        }
                    }
                }
            }
        }

        internal string GetValue(string key)
        {
            return _dictionary[key.ToUpperInvariant()];
        }
    }
}