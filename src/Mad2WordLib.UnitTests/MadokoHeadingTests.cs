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
        [Fact(DisplayName = nameof(MadokoHeading_throws_on_non_leading_hash_characters))]
        public void MadokoHeading_throws_on_non_leading_hash_characters()
        {
            Action action = () => MakeHeading("Heading #1");

            action.ShouldThrow<InvalidOperationException>();
        }

        [Fact(DisplayName = nameof(MadokoHeading_throws_on_invalid_heading))]
        public void MadokoHeading_throws_on_invalid_heading()
        {
            Action action = () => MakeHeading("Heading 1");

            action.ShouldThrow<InvalidOperationException>();
        }

        [Fact(DisplayName = nameof(MadokoHeading_parses_level_1_heading))]
        public void MadokoHeading_parses_level_1_heading()
        {
            SingleRunTestCase("# Heading 1", 1, "Heading 1");
        }

        [Fact(DisplayName = nameof(MadokoHeading_parses_level_2_heading))]
        public void MadokoHeading_parses_level_2_heading()
        {
            SingleRunTestCase("## Heading 2", 2, "Heading 2");
        }

        [Fact(DisplayName = nameof(MadokoHeading_trims_heading_text))]
        public void MadokoHeading_trims_heading_text()
        {
            SingleRunTestCase("# Heading 1  ", 1, "Heading 1");
        }

        [Fact(DisplayName = nameof(MadokoHeading_supports_run_formatting))]
        public void MadokoHeading_supports_run_formatting()
        {
            MadokoHeading madokoHeading = MakeHeading("## `runs` property");

            madokoHeading.Level.Should().Be(2);
            madokoHeading.Runs[0].RunType.Should().Be(MadokoRunType.Code);
            madokoHeading.Runs[0].Text.Should().Be("runs");
            madokoHeading.Runs[1].RunType.Should().Be(MadokoRunType.PlainText);
            madokoHeading.Runs[1].Text.Should().Be(" property");
        }

        [Fact(DisplayName = nameof(MadokoHeading_cannot_span_source_lines))]
        public void MadokoHeading_cannot_span_source_lines()
        {
            const string Input =
@"# Chapter 1
The beginning";

            MadokoHeading madokoHeading = MakeHeading(Input);

            madokoHeading.Level.Should().Be(1);
            madokoHeading.Runs.Count.Should().Be(1);
            madokoHeading.Runs[0].Text.Should().Be("Chapter 1");
        }

        [Fact(DisplayName = nameof(MadokoHeading_can_appear_on_consecutive_lines))]
        public void MadokoHeading_can_appear_on_consecutive_lines()
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

        [Fact(DisplayName = nameof(MadokoHeading_allows_attributes))]
        public void MadokoHeading_allows_attributes()
        {
            SingleRunTestCase("# Chapter 1 {key1: value1; key2: value2}", 1, "Chapter 1",
                new[] {
                    new MadokoAttribute("key1", "value1"),
                    new MadokoAttribute("key2", "value2")
                });
        }

        private void SingleRunTestCase(string line, int expectedLevel, string expectedText, MadokoAttribute[] expectedAttributes = null)
        {
            MadokoHeading madokoHeading = MakeHeading(line);

            madokoHeading.Level.Should().Be(expectedLevel);
            madokoHeading.Runs[0].RunType.Should().Be(MadokoRunType.PlainText);
            madokoHeading.Runs[0].Text.Should().Be(expectedText);

            if (expectedAttributes == null)
            {
                expectedAttributes = MadokoAttribute.EmptyAttributes;
            }

            madokoHeading.Attributes.Length.Should().Be(expectedAttributes.Length);
            for (int i = 0; i < madokoHeading.Attributes.Length; ++i)
            {
                madokoHeading.Attributes[i].Key.Should().Be(expectedAttributes[i].Key);
                madokoHeading.Attributes[i].Value.Should().Be(expectedAttributes[i].Value);
            }
        }

        private static MadokoHeading MakeHeading(string input)
        {
            MadokoHeading heading = null;

            var environment = new FakeEnvironment();
            var fileSystem = new FakeFileSystem(environment);
            using (TextReader reader = new StringReader(input))
            {
                var lineSource = new LineSource(reader, fileSystem, environment, inputPath: null);
                heading = new MadokoHeading(lineSource);
            }

            return heading;
        }
    }
}
