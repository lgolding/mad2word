// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
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
                new Dictionary<string, MadokoAttribute>() {
                    { "key1", new MadokoAttribute("key1", "value1") },
                    { "key2", new MadokoAttribute("key2", "value2") },
                });
        }

        private class HeadingVisitor : TestVisitorBase
        {
            private List<MadokoHeading> _headings = new List<MadokoHeading>();

            public override void Visit(MadokoHeading heading)
            {
                _headings.Add(heading);
            }

            internal MadokoHeading[] Headings => _headings.ToArray();
        }

        [Fact(DisplayName = nameof(MadokoHeading_assigns_heading_numbers_to_headings))]
        public void MadokoHeading_assigns_heading_numbers_to_headings()
        {
            const string InputPath = @"C:\docs\test.mdk";
            const string Contents =
@"
# Chapter 1
Text

## Section 1.1
### Section 1.1.1
### Section 1.1.2
## Section 1.2
### Section 1.2.1

# Chapter 2
More text
## Section 2.1
### Section 2.1.1
";

            var environment = new FakeEnvironment();
            var fileSystem = new FakeFileSystem(environment);
            fileSystem.AddFile(InputPath, Contents);

            MadokoDocument document = MadokoDocument.Read(InputPath, fileSystem, environment);

            var visitor = new HeadingVisitor();

            foreach (MadokoBlock madokoBlock in document.Blocks)
            {
                madokoBlock.Accept(visitor);
            }

            visitor.Headings.Length.Should().Be(9);

            visitor.Headings[0].Numbers.Length.Should().Be(1);
            visitor.Headings[0].Numbers[0].Should().Be(1);

            visitor.Headings[1].Numbers.Length.Should().Be(2);
            visitor.Headings[1].Numbers[0].Should().Be(1);
            visitor.Headings[1].Numbers[1].Should().Be(1);

            visitor.Headings[2].Numbers.Length.Should().Be(3);
            visitor.Headings[2].Numbers[0].Should().Be(1);
            visitor.Headings[2].Numbers[1].Should().Be(1);
            visitor.Headings[2].Numbers[2].Should().Be(1);

            visitor.Headings[3].Numbers.Length.Should().Be(3);
            visitor.Headings[3].Numbers[0].Should().Be(1);
            visitor.Headings[3].Numbers[1].Should().Be(1);
            visitor.Headings[3].Numbers[2].Should().Be(2);

            visitor.Headings[4].Numbers.Length.Should().Be(2);
            visitor.Headings[4].Numbers[0].Should().Be(1);
            visitor.Headings[4].Numbers[1].Should().Be(2);

            visitor.Headings[5].Numbers.Length.Should().Be(3);
            visitor.Headings[5].Numbers[0].Should().Be(1);
            visitor.Headings[5].Numbers[1].Should().Be(2);
            visitor.Headings[5].Numbers[2].Should().Be(1);

            visitor.Headings[6].Numbers.Length.Should().Be(1);
            visitor.Headings[6].Numbers[0].Should().Be(2);

            visitor.Headings[7].Numbers.Length.Should().Be(2);
            visitor.Headings[7].Numbers[0].Should().Be(2);
            visitor.Headings[7].Numbers[1].Should().Be(1);

            visitor.Headings[8].Numbers.Length.Should().Be(3);
            visitor.Headings[8].Numbers[0].Should().Be(2);
            visitor.Headings[8].Numbers[1].Should().Be(1);
            visitor.Headings[8].Numbers[2].Should().Be(1);
        }

        private void SingleRunTestCase(string line, int expectedLevel, string expectedText, IDictionary<string, MadokoAttribute> expectedAttributes = null)
        {
            MadokoHeading madokoHeading = MakeHeading(line);

            madokoHeading.Level.Should().Be(expectedLevel);
            madokoHeading.Runs[0].RunType.Should().Be(MadokoRunType.PlainText);
            madokoHeading.Runs[0].Text.Should().Be(expectedText);

            if (expectedAttributes == null)
            {
                expectedAttributes = MadokoAttribute.EmptyAttributes;
            }

            madokoHeading.Attributes.Count.Should().Be(expectedAttributes.Count);
            foreach (string key in expectedAttributes.Keys)
            {
                madokoHeading.Attributes[key].Key.Should().Be(expectedAttributes[key].Key);
                madokoHeading.Attributes[key].Value.Should().Be(expectedAttributes[key].Value);
            }
        }

        private static MadokoHeading MakeHeading(string input)
        {
            MadokoHeading heading = null;

            var environment = new FakeEnvironment();
            var fileSystem = new FakeFileSystem(environment);
            using (TextReader reader = new StringReader(input))
            {
                var lineSource = new LineSource(reader, null, fileSystem, environment);
                heading = new MadokoHeading(lineSource, new int[MadokoHeading.MaxDepth]);
            }

            return heading;
        }
    }
}
