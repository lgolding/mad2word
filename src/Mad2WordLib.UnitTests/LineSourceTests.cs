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

        private readonly string[] AllLines = new[]
        {
            "# Top-level document",
            "## Chapter 1",
            "How it began",
            "The extra content",
            "## Chapter 2",
            "How it ended",
            "The end"
        };

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

            lines.Length.Should().Be(AllLines.Length);
            for (int i = 0; i < AllLines.Length; ++i)
            {
                lines[i].Should().Be(AllLines[i]);
            }
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

            lines.Length.Should().Be(AllLines.Length);
            for (int i = 0; i < AllLines.Length; ++i)
            {
                lines[i].Should().Be(AllLines[i]);
            }
        }

        [Fact(DisplayName = nameof(LineSource_tracks_input_file_and_line_number))]
        public void LineSource_tracks_input_file_and_line_number()
        {
            var environment = new FakeEnvironment();
            var fileSystem = new FakeFileSystem(environment);
            fileSystem.AddFile("document.mdk", Document);
            fileSystem.AddFile("Chapter1.mdk", Chapter1);
            fileSystem.AddFile("Chapter2.mdk", Chapter2);
            fileSystem.AddFile("Extra.mdk", Extra);

            const string InputPath = "document.mdk";
            TextReader reader = fileSystem.OpenText(InputPath);
            var lineSource = new LineSource(reader, InputPath, fileSystem, environment);

            string line = lineSource.GetLine();
            line.Should().Be(AllLines[0]);
            lineSource.LineNumber.Should().Be(1);
            lineSource.FilePath.Should().Be(Path.Combine(FakeEnvironment.DefaultWorkingDirectory, "document.mdk"));

            line = lineSource.GetLine();
            line.Should().Be(AllLines[1]);
            lineSource.LineNumber.Should().Be(1);
            lineSource.FilePath.Should().Be(Path.Combine(FakeEnvironment.DefaultWorkingDirectory, "Chapter1.mdk"));

            line = lineSource.GetLine();
            line.Should().Be(AllLines[2]);
            lineSource.LineNumber.Should().Be(2);
            lineSource.FilePath.Should().Be(Path.Combine(FakeEnvironment.DefaultWorkingDirectory, "Chapter1.mdk"));

            line = lineSource.GetLine();
            line.Should().Be(AllLines[3]);
            lineSource.LineNumber.Should().Be(1);
            lineSource.FilePath.Should().Be(Path.Combine(FakeEnvironment.DefaultWorkingDirectory, "Extra.mdk"));

            line = lineSource.GetLine();
            line.Should().Be(AllLines[4]);
            lineSource.LineNumber.Should().Be(1);
            lineSource.FilePath.Should().Be(Path.Combine(FakeEnvironment.DefaultWorkingDirectory, "Chapter2.mdk"));

            line = lineSource.GetLine();
            line.Should().Be(AllLines[5]);
            lineSource.LineNumber.Should().Be(2);
            lineSource.FilePath.Should().Be(Path.Combine(FakeEnvironment.DefaultWorkingDirectory, "Chapter2.mdk"));

            line = lineSource.GetLine();
            line.Should().Be(AllLines[6]);
            lineSource.LineNumber.Should().Be(4);
            lineSource.FilePath.Should().Be(Path.Combine(FakeEnvironment.DefaultWorkingDirectory, "document.mdk"));
        }
    }
}
