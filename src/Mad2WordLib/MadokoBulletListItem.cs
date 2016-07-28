// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Text.RegularExpressions;

namespace Mad2WordLib
{
    public class MadokoBulletListItem : MadokoBlock
    {
        private static readonly Regex s_itemPattern = new Regex(@"^(?<indent>[ \t]*)(?<bullet>[-+*])(?<text>.*)", RegexOptions.Compiled);

        public static MadokoBulletListItem CreateFrom(string line)
        {
            MadokoBulletListItem item = null;

            Match match = s_itemPattern.Match(line);
            if (match.Success)
            {
                string indent = match.Groups["indent"].Value;
                string bulletString = match.Groups["bullet"].Value;
                string text = match.Groups["text"].Value.Trim();

                int level = MakeLevel(indent);
                MadokoBulletType bulletType = MakeBulletType(bulletString);
                item = new MadokoBulletListItem(level, bulletType, text);
            }

            return item;
        }

        public override void Accept(IMadokoVisitor visitor)
        {
            visitor.Visit(this);
        }

        private static MadokoBulletType MakeBulletType(string bulletString)
        {
            switch (bulletString)
            {
                case "*": return MadokoBulletType.Star;
                case "+": return MadokoBulletType.Plus;
                case "-": return MadokoBulletType.Dash;

                default:
                    throw new ArgumentException("Invalid bullet specifier: " + bulletString, nameof(bulletString));
            }
        }

        private static int MakeLevel(string indent)
        {
            // TODO How are tabs handled?
            return indent.Length;
        }

        private MadokoBulletListItem(int level, MadokoBulletType bulletType, string text)
        {
            Level = level;
            BulletType = bulletType;
            Runs.AddRange(MadokoLine.Parse(text));
        }

        internal int Level { get; }
        internal MadokoBulletType BulletType { get; }
    }
}
