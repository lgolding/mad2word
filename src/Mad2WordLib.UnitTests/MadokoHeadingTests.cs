// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoHeadingTests
    {
        [Fact]
        public void MadokoHeading_IgnoresNonLeadingHashCharacters()
        {
            MadokoHeading.CreateFrom("Heading #1").Should().BeNull();
        }

        [Fact]
        public void MadokoHeading_ReturnsNullOnInvalidHeading()
        {
            MadokoHeading.CreateFrom("Heading 1").Should().BeNull();
        }

        [Fact]
        public void MadokoHeading_ParsesLevel1Heading()
        {
            SingleRunTestCase("# Heading 1", 1, "Heading 1");
        }

        [Fact]
        public void MadokoHeading_TrimsHeadingText()
        {
            SingleRunTestCase("# Heading 1  ", 1, "Heading 1");
        }

        [Fact]
        public void MadokoHeading_StopsAtOpenCurlyBrace()
        {
            SingleRunTestCase("# Heading 1  { #heading-1 }", 1, "Heading 1");
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
