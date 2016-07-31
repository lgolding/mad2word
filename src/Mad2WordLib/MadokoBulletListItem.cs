// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mad2WordLib
{
    public class MadokoBulletListItem : MadokoBlock
    {
        private static readonly Regex s_itemPattern = new Regex(@"^(?<indent>[ \t]*)(?<bullet>[-+*])(?<text>.*)", RegexOptions.Compiled);

        private static readonly Dictionary<char, MadokoBulletType> s_bulletTypeDictionary;
        private static readonly char[] s_bulletSymbols;

        static MadokoBulletListItem()
        {
            s_bulletTypeDictionary = new Dictionary<char, MadokoBulletType>
            {
                ['*'] = MadokoBulletType.Star,
                ['+'] = MadokoBulletType.Plus,
                ['-'] = MadokoBulletType.Dash
            };

            s_bulletSymbols = new char[s_bulletTypeDictionary.Count];
            s_bulletTypeDictionary.Keys.CopyTo(s_bulletSymbols, 0);
        }

        public MadokoBulletListItem(LineSource lineSource)
        {
            string line = lineSource.PeekLine();
            Match match = s_itemPattern.Match(line);
            if (!match.Success)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Unexpected attempt to create a bullet list item from line {0}:\n{1}",
                        lineSource.LineNumber,
                        line));
            }

            char bulletSymbol = match.Groups["bullet"].Value[0];
            BulletType = MakeBulletType(bulletSymbol);

            string text = match.Groups["text"].Value.Trim();
            Runs.AddRange(MadokoLine.Parse(text));

            lineSource.Advance();

            AppendRemainderOfBlock(lineSource);
        }

        internal MadokoBulletType BulletType { get; }

        public static bool MatchesLine(string line)
        {
            return s_bulletSymbols.Contains(line[0]);
        }

        public override void Accept(IMadokoVisitor visitor)
        {
            visitor.Visit(this);
        }

        private static MadokoBulletType MakeBulletType(char bulletSymbol)
        {
            MadokoBulletType bulletType;
            if (!s_bulletTypeDictionary.TryGetValue(bulletSymbol, out bulletType))
            {
                throw new ArgumentException("Invalid bullet specifier: " + bulletSymbol, nameof(bulletSymbol));
            }

            return bulletType;
        }
    }
}
