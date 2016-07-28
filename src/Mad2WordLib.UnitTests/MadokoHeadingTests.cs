// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoHeadingTests
    {
        [Fact(DisplayName = nameof(MadokoHeading_IgnoresNonLeadingHashCharacters))]
        public void MadokoHeading_IgnoresNonLeadingHashCharacters()
        {
            MadokoHeading.CreateFrom("Heading #1").Should().BeNull();
        }

        [Fact(DisplayName = nameof(MadokoHeading_ReturnsNullOnInvalidHeading))]
        public void MadokoHeading_ReturnsNullOnInvalidHeading()
        {
            MadokoHeading.CreateFrom("Heading 1").Should().BeNull();
        }

        [Fact(DisplayName = nameof(MadokoHeading_ParsesLevel1Heading))]
        public void MadokoHeading_ParsesLevel1Heading()
        {
            SingleRunTestCase("# Heading 1", 1, "Heading 1");
        }

        [Fact(DisplayName = nameof(MadokoHeading_ParsesLevel2Heading))]
        public void MadokoHeading_ParsesLevel2Heading()
        {
            SingleRunTestCase("## Heading 2", 2, "Heading 2");
        }

        [Fact(DisplayName = nameof(MadokoHeading_TrimsHeadingText))]
        public void MadokoHeading_TrimsHeadingText()
        {
            SingleRunTestCase("# Heading 1  ", 1, "Heading 1");
        }

        [Fact(DisplayName = nameof(MadokoHeading_StopsAtOpenCurlyBrace))]
        public void MadokoHeading_StopsAtOpenCurlyBrace()
        {
            SingleRunTestCase("# Heading 1  { #heading-1 }", 1, "Heading 1");
        }

        [Fact(DisplayName = nameof(MadokoHeading_SupportsRunFormatting))]
        public void MadokoHeading_SupportsRunFormatting()
        {
            const string Line = "## `runs` property";

            MadokoHeading madokoHeading = MadokoHeading.CreateFrom(Line);

            madokoHeading.Level.Should().Be(2);
            madokoHeading.Runs[0].RunType.Should().Be(MadokoRunType.Code);
            madokoHeading.Runs[0].Text.Should().Be("runs");
            madokoHeading.Runs[1].RunType.Should().Be(MadokoRunType.PlainText);
            madokoHeading.Runs[1].Text.Should().Be(" property");
        }

        private void SingleRunTestCase(string line, int expectedLevel, string expectedText)
        {
            MadokoHeading madokoHeading = MadokoHeading.CreateFrom(line);

            madokoHeading.Level.Should().Be(expectedLevel);
            madokoHeading.Runs[0].RunType.Should().Be(MadokoRunType.PlainText);
            madokoHeading.Runs[0].Text.Should().Be(expectedText);
        }
    }
}
