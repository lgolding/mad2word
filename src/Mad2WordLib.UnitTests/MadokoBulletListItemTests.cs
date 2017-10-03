// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoBulletListItemTests
    {
        [Fact(DisplayName = "BulletListItem_ThrowsOnInvalidBulletListItem")]
        public void BulletListItem_ThrowsOnInvalidBulletListItem()
        {
            Action action = () => MakeBulletListItem("Bullet list item");

            action.ShouldThrow<InvalidOperationException>();
        }

        [Fact(DisplayName = nameof(MadokoBulletListItem_SupportsAsterisk))]
        public void MadokoBulletListItem_SupportsAsterisk()
        {
            MadokoBulletListItem bulletListItem = MakeBulletListItem("* Bullet list item");

            bulletListItem.Runs.Count.Should().Be(1);
            bulletListItem.Runs[0].RunType.Should().Be(MadokoRunType.PlainText);
            bulletListItem.Runs[0].Text.Should().Be("Bullet list item");
        }

        [Fact(DisplayName = nameof(MadokoBulletListItem_ParsesMultipleRuns))]
        public void MadokoBulletListItem_ParsesMultipleRuns()
        {
            MadokoBulletListItem bulletListItem = MakeBulletListItem("* Bullet `list` item");

            bulletListItem.Runs.Count.Should().Be(3);
            bulletListItem.Runs[0].RunType.Should().Be(MadokoRunType.PlainText);
            bulletListItem.Runs[0].Text.Should().Be("Bullet ");
            bulletListItem.Runs[1].RunType.Should().Be(MadokoRunType.Code);
            bulletListItem.Runs[1].Text.Should().Be("list");
            bulletListItem.Runs[2].RunType.Should().Be(MadokoRunType.PlainText);
            bulletListItem.Runs[2].Text.Should().Be(" item");
        }

        [Fact(DisplayName = nameof(BulletListItem_CanSpanSourceLines))]
        public void BulletListItem_CanSpanSourceLines()
        {
            MadokoBulletListItem bulletListItem = MakeBulletListItem("* This is a long\nbullet list item.");

            bulletListItem.Runs[0].Text.Should().Be("This is a long");
            bulletListItem.Runs[1].Text.Should().Be(" bullet list item.");
        }

        private static MadokoBulletListItem MakeBulletListItem(string input)
        {
            MadokoBulletListItem bulletListItem = null;

            var environment = new FakeEnvironment();
            var fileSystem = new FakeFileSystem(environment);
            using (TextReader reader = new StringReader(input))
            {
                var lineSource = new LineSource(reader, null, fileSystem, environment);
                bulletListItem = new MadokoBulletListItem(lineSource);
            }

            return bulletListItem;
        }
    }
}
