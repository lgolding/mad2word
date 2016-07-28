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

        [Fact(DisplayName = nameof(MadokoDocument_HandlesIncludes))]
        public void MadokoDocument_HandlesIncludes()
        {
            const string Document =
@"# Top-level document
[INCLUDE=Chapter1.mdk]
[INCLUDE=Chapter2]
The end";

            const string Chapter1 =
@"## Chapter 1
How it began
[INCLUDE=Extra]";

            const string Chapter2 =
@"## Chapter 2
How it ended";

            const string Extra = "The extra content";

            var fileSystem = new FakeFileSystem();
            fileSystem.AddFile("document.mdk", Document);
            fileSystem.AddFile("Chapter1.mdk", Chapter1);
            fileSystem.AddFile("Chapter2.mdk", Chapter2);
            fileSystem.AddFile("Extra.mdk", Extra);

            TextReader reader = fileSystem.OpenText("document.mdk");
            string[] lines = MadokoDocument.ReadAllLines(reader, fileSystem);

            lines.Length.Should().Be(7);
            lines[0].Should().Be("# Top-level document");
            lines[1].Should().Be("## Chapter 1");
            lines[2].Should().Be("How it began");
            lines[3].Should().Be("The extra content");
            lines[4].Should().Be("## Chapter 2");
            lines[5].Should().Be("How it ended");
            lines[6].Should().Be("The end");
        }
    }
}
