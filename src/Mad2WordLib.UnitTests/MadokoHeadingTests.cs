// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoHeadingTests
    {
        [Fact(DisplayName = nameof(MadokoHeading_ThrowsOnNonLeadingHashCharacters))]
        public void MadokoHeading_ThrowsOnNonLeadingHashCharacters()
        {
            Action action = () => MakeHeading("Heading #1");

            action.ShouldThrow<InvalidOperationException>();
        }

        [Fact(DisplayName = nameof(MadokoHeading_ThrowsOnInvalidHeading))]
        public void MadokoHeading_ThrowsOnInvalidHeading()
        {
            Action action = () => MakeHeading("Heading 1");

            action.ShouldThrow<InvalidOperationException>();
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
            MadokoHeading madokoHeading = MakeHeading("## `runs` property");

            madokoHeading.Level.Should().Be(2);
            madokoHeading.Runs[0].RunType.Should().Be(MadokoRunType.Code);
            madokoHeading.Runs[0].Text.Should().Be("runs");
            madokoHeading.Runs[1].RunType.Should().Be(MadokoRunType.PlainText);
            madokoHeading.Runs[1].Text.Should().Be(" property");
        }

        [Fact(DisplayName = nameof(MadokoHeadings_CanSpanSourceLines))]
        public void MadokoHeadings_CanSpanSourceLines()
        {
            const string Input =
@"# Chapter 1
The beginning";

            MadokoHeading madokoHeading = MakeHeading(Input);

            madokoHeading.Level.Should().Be(1);
            madokoHeading.Runs[0].Text.Should().Be("Chapter 1");
            madokoHeading.Runs[1].Text.Should().Be(" The beginning");
        }

        private void SingleRunTestCase(string line, int expectedLevel, string expectedText)
        {
            MadokoHeading madokoHeading = MakeHeading(line);

            madokoHeading.Level.Should().Be(expectedLevel);
            madokoHeading.Runs[0].RunType.Should().Be(MadokoRunType.PlainText);
            madokoHeading.Runs[0].Text.Should().Be(expectedText);
        }

        private static MadokoHeading MakeHeading(string input)
        {
            MadokoHeading heading = null;

            var fileSystem = new FakeFileSystem();
            using (TextReader reader = new StringReader(input))
            {
                var lineSource = new LineSource(reader, fileSystem);
                heading = new MadokoHeading(lineSource);
            }

            return heading;
        }
    }
}
