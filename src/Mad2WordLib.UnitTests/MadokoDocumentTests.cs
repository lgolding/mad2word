// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoDocumentTests
    {
        [Fact(DisplayName = nameof(MadokoDocument_ReadsHeaders))]
        public void MadokoDocument_ReadsHeaders()
        {
            const string Input = "# Chapter 1\n## Section 1.1\n# Chapter 2";

            using (var reader = new StringReader(Input))
            {
                IFileSystem fileSystem = new FakeFileSystem();

                var document = MadokoDocument.Read(reader, fileSystem);

                document.Blocks.Count.Should().Be(3);
                var headings = document.Blocks.Cast<MadokoHeading>().ToList();
                headings[0].Level.Should().Be(1);
                headings[0].Runs[0].Text.Should().Be("Chapter 1");
                headings[1].Level.Should().Be(2);
                headings[1].Runs[0].Text.Should().Be("Section 1.1");
                headings[2].Level.Should().Be(1);
                headings[2].Runs[0].Text.Should().Be("Chapter 2");
            }
        }

        [Fact(DisplayName = nameof(MadokoDocument_HeadingsCanSpanSourceLines))]
        public void MadokoDocument_HeadingsCanSpanSourceLines()
        {
            const string Input = "# Chapter 1\nThe beginning";

            using (var reader = new StringReader(Input))
            {
                IFileSystem fileSystem = new FakeFileSystem();

                var document = MadokoDocument.Read(reader, fileSystem);

                document.Blocks.Count.Should().Be(1);
                var heading = document.Blocks.Cast<MadokoHeading>().Single();
                heading.Level.Should().Be(1);
                heading.Runs[0].Text.Should().Be("Chapter 1");
                heading.Runs[1].Text.Should().Be(" The beginning");
            }
        }

        [Fact(DisplayName = nameof(MadokoDocument_BulletListItemsCanSpanSourceLines))]
        public void MadokoDocument_BulletListItemsCanSpanSourceLines()
        {
            const string Input = "* This is a long\nbullet list item.";

            using (var reader = new StringReader(Input))
            {
                IFileSystem fileSystem = new FakeFileSystem();

                var document = MadokoDocument.Read(reader, fileSystem);

                document.Blocks.Count.Should().Be(1);
                var bulletListItem = document.Blocks.Cast<MadokoBulletListItem>().Single();
                bulletListItem.Runs[0].Text.Should().Be("This is a long");
                bulletListItem.Runs[1].Text.Should().Be(" bullet list item.");
            }
        }
    }
}
