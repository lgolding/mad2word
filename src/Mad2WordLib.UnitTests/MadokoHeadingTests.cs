// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoHeadingTests : MadokoTestBase
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

        [Fact(DisplayName = nameof(MadokoHeading_CannotSpanSourceLines))]
        public void MadokoHeading_CannotSpanSourceLines()
        {
            const string Input =
@"# Chapter 1
The beginning";

            MadokoHeading madokoHeading = MakeHeading(Input);

            madokoHeading.Level.Should().Be(1);
            madokoHeading.Runs.Count.Should().Be(1);
            madokoHeading.Runs[0].Text.Should().Be("Chapter 1");
        }

        [Fact(DisplayName = nameof(MadokoHeading_CanAppearOnConsecutiveLines))]
        public void MadokoHeading_CanAppearOnConsecutiveLines()
        {
            const string Input =
@"# Chapter 1
The beginning
## Section 1.1
Some thoughts

# Chapter 2
# Chapter 3

# Chapter 4";

            MadokoDocument document = ReadDocument(Input);

            var blocks = document.Blocks.ToList();
            blocks.Count.Should().Be(7);

            blocks[0].Should().BeOfType<MadokoHeading>();
            var heading = (MadokoHeading)blocks[0];
            heading.Level.Should().Be(1);
            heading.Runs.Count.Should().Be(1);
            heading.Runs[0].Text.Should().Be("Chapter 1");

            var block = blocks[1];
            block.GetType().Should().NotBe(typeof(MadokoHeading));

            // The following test fails with an InvalidCastException: "Unable to cast
            // object of type 'Mad2WordLib.MadokoBlock' to type 'Mad2WordLib.MadokoHeading'."
            // This seems to be a bug in FluentAssertions.
            //
            //block.Should().NotBeOfType<MadokoHeading>();

            // Instead, we have to write this:
            block.GetType().Should().NotBe(typeof(MadokoHeading));

            block.Runs.Count.Should().Be(1);
            block.Runs[0].Text.Should().Be("The beginning");

            blocks[2].Should().BeOfType<MadokoHeading>();
            heading = (MadokoHeading)blocks[2];
            heading.Level.Should().Be(2);
            heading.Runs.Count.Should().Be(1);
            heading.Runs[0].Text.Should().Be("Section 1.1");

            block = blocks[3];

            // Again, because Should().NotBeOfType<T> fails:
            block.GetType().Should().NotBe(typeof(MadokoHeading));

            block.Runs.Count.Should().Be(1);
            block.Runs[0].Text.Should().Be("Some thoughts");

            blocks[4].Should().BeOfType<MadokoHeading>();
            heading = (MadokoHeading)blocks[4];
            heading.Level.Should().Be(1);
            heading.Runs.Count.Should().Be(1);
            heading.Runs[0].Text.Should().Be("Chapter 2");

            blocks[5].Should().BeOfType<MadokoHeading>();
            heading = (MadokoHeading)blocks[5];
            heading.Level.Should().Be(1);
            heading.Runs.Count.Should().Be(1);
            heading.Runs[0].Text.Should().Be("Chapter 3");

            blocks[6].Should().BeOfType<MadokoHeading>();
            heading = (MadokoHeading)blocks[6];
            heading.Level.Should().Be(1);
            heading.Runs.Count.Should().Be(1);
            heading.Runs[0].Text.Should().Be("Chapter 4");
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
