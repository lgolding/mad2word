// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.IO;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class LineSourceTests
    {

        [Fact(DisplayName = nameof(LineSource_HandlesIncludes))]
        public void LineSource_HandlesIncludes()
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
            string[] lines = new LineSource(reader, fileSystem).GetAllLines();

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
