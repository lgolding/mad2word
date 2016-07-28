// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Mad2WordLib
{
    internal static class MadokoLine
    {
        internal static readonly ReadOnlyDictionary<string, string> s_entityDictionary = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>
            {
                ["HELLIP"] = "\u2026",
                ["SECT"] = "\u00a7"
            });

        internal static MadokoRun[] Parse(string line)
        {
            var madokoRuns = new List<MadokoRun>();
            var sb = new StringBuilder();
            var runType = MadokoRunType.PlainText;
            bool inEntity = false;
            var entityBuilder = new StringBuilder();

            foreach (char c in line)
            {
                if (inEntity)
                {
                    if (c == ';')
                    {
                        string entityName = entityBuilder.ToString();
                        string replacement;
                        if (s_entityDictionary.TryGetValue(entityName.ToUpperInvariant(), out replacement))
                        {
                            sb.Append(replacement);
                        }

                        entityBuilder.Clear();
                        inEntity = false;
                    }
                    else
                    {
                        entityBuilder.Append(c);
                    }
                }
                else
                {
                    if (c == '&')
                    {
                        inEntity = true;
                    }
                    else
                    {
                        switch (runType)
                        {
                            case MadokoRunType.PlainText:
                                switch (c)
                                {
                                    case '`':
                                        AddRun(madokoRuns, sb, runType);
                                        runType = MadokoRunType.Code;
                                        break;

                                    default:
                                        sb.Append(c);
                                        break;
                                }
                                break;

                            case MadokoRunType.Code:
                                switch (c)
                                {
                                    case '`':
                                        AddRun(madokoRuns, sb, runType);
                                        runType = MadokoRunType.PlainText;
                                        break;

                                    default:
                                        sb.Append(c);
                                        break;
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            AddRun(madokoRuns, sb, runType);

            return madokoRuns.ToArray();
        }

        private static void AddRun(List<MadokoRun> madokoRuns, StringBuilder sb, MadokoRunType runType)
        {
            if (sb.Length > 0)
            {
                madokoRuns.Add(new MadokoRun(runType, sb.ToString()));
                sb.Clear();
            }
        }
    }
}
