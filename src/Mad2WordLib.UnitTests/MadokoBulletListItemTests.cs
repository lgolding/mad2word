// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoBulletListItemTests
    {
        [Fact(DisplayName = nameof(MadokoBulletListItem_SupportsAsterisk))]
        public void MadokoBulletListItem_SupportsAsterisk()
        {
            const string Line = "* Bullet list item";

            MadokoBulletListItem bulletListItem = MadokoBulletListItem.CreateFrom(Line);

            bulletListItem.Runs.Count.Should().Be(1);
            bulletListItem.Runs[0].RunType.Should().Be(MadokoRunType.PlainText);
            bulletListItem.Runs[0].Text.Should().Be("Bullet list item");
        }

        [Fact(DisplayName = nameof(MadokoBulletListItem_ParsesMultipleRuns))]
        public void MadokoBulletListItem_ParsesMultipleRuns()
        {
            const string Line = "* Bullet `list` item";

            MadokoBulletListItem bulletListItem = MadokoBulletListItem.CreateFrom(Line);

            bulletListItem.Runs.Count.Should().Be(3);
            bulletListItem.Runs[0].RunType.Should().Be(MadokoRunType.PlainText);
            bulletListItem.Runs[0].Text.Should().Be("Bullet ");
            bulletListItem.Runs[1].RunType.Should().Be(MadokoRunType.Code);
            bulletListItem.Runs[1].Text.Should().Be("list");
            bulletListItem.Runs[2].RunType.Should().Be(MadokoRunType.PlainText);
            bulletListItem.Runs[2].Text.Should().Be(" item");
        }
    }
}
