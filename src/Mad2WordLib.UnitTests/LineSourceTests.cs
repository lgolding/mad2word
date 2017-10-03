// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.IO;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class LineSourceTests
    {

        private const string Document =
@"# Top-level document
[INCLUDE=Chapter1.mdk]
[INCLUDE=Chapter2]
The end";

        private const string Chapter1 =
@"## Chapter 1
How it began
[INCLUDE=Extra]";

        private const string Chapter2 =
@"## Chapter 2
How it ended";

        private const string Extra = "The extra content";

        [Fact(DisplayName = nameof(LineSource_reads_includes))]
        public void LineSource_reads_includes()
        {
            var environment = new FakeEnvironment();
            var fileSystem = new FakeFileSystem(environment);
            fileSystem.AddFile("document.mdk", Document);
            fileSystem.AddFile("Chapter1.mdk", Chapter1);
            fileSystem.AddFile("Chapter2.mdk", Chapter2);
            fileSystem.AddFile("Extra.mdk", Extra);

            const string InputPath = "document.mdk";
            TextReader reader = fileSystem.OpenText(InputPath);
            string[] lines = new LineSource(reader, InputPath, fileSystem, environment).GetAllLines();

            lines.Length.Should().Be(7);
            lines[0].Should().Be("# Top-level document");
            lines[1].Should().Be("## Chapter 1");
            lines[2].Should().Be("How it began");
            lines[3].Should().Be("The extra content");
            lines[4].Should().Be("## Chapter 2");
            lines[5].Should().Be("How it ended");
            lines[6].Should().Be("The end");
        }

        [Fact(DisplayName = nameof(LineSource_reads_includes_from_document_directory))]
        public void LineSource_reads_includes_from_document_directory()
        {
            const string DocumentDirectory = @"C:\Users\Larry\Documents\MyDoc";
            const string CurrentDirectory = @"C:\Code";

            var environment = new FakeEnvironment
            {
                CurrentDirectory = DocumentDirectory
            };

            var fileSystem = new FakeFileSystem(environment);

            fileSystem.AddFile("document.mdk", Document);
            fileSystem.AddFile("Chapter1.mdk", Chapter1);
            fileSystem.AddFile("Chapter2.mdk", Chapter2);
            fileSystem.AddFile("Extra.mdk", Extra);

            environment.CurrentDirectory = CurrentDirectory;
            string inputPath = Path.Combine(DocumentDirectory, "document.mdk");
            TextReader reader = fileSystem.OpenText(inputPath);

            string[] lines = new LineSource(reader, inputPath, fileSystem, environment).GetAllLines();

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
