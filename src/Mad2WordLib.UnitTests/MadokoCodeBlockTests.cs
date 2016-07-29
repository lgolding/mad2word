// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoCodeBlockTests
    {
        [Fact(DisplayName = nameof(MadokoCodeBlock_HandlesSingleLineFencedBlock))]
        public void MadokoCodeBlock_HandlesSingleLineFencedBlock()
        {
            const string Input =
@"```
code
```";
            MadokoCodeBlock codeBlock = MakeCodeBlock(Input);

            codeBlock.Runs.Count.Should().Be(1);
            codeBlock.Runs[0].RunType.Should().Be(MadokoRunType.PlainText);
            codeBlock.Runs[0].Text.Should().Be("code");
        }

        [Fact(DisplayName = nameof(MadokoCodeBlock_HandlesEmptyFencedBlock))]
        public void MadokoCodeBlock_HandlesEmptyFencedBlock()
        {
            const string Input =
@"```
```";
            MadokoCodeBlock codeBlock = MakeCodeBlock(Input);

            codeBlock.Runs.Count.Should().Be(1);
            codeBlock.Runs[0].RunType.Should().Be(MadokoRunType.PlainText);
            codeBlock.Runs[0].Text.Should().Be(string.Empty);
        }

        [Fact(DisplayName = nameof(MadokoCodeBlock_HandlesMultipleLineFencedBlock))]
        public void MadokoCodeBlock_HandlesMultipleLineFencedBlock()
        {
            const string Input =
@"```
code1
code2
```";
            MadokoCodeBlock codeBlock = MakeCodeBlock(Input);

            codeBlock.Runs.Count.Should().Be(1);
            codeBlock.Runs[0].RunType.Should().Be(MadokoRunType.PlainText);
            codeBlock.Runs[0].Text.Should().Be("code1" + Environment.NewLine + "code2");
        }

        [Fact(DisplayName = nameof(MadokoCodeBlock_AllowsFenceTerminatorInMiddleOfLine))]
        public void MadokoCodeBlock_AllowsFenceTerminatorInMiddleOfLine()
        {
            const string Input =
@"```
code1
code2```";
            MadokoCodeBlock codeBlock = MakeCodeBlock(Input);

            codeBlock.Runs.Count.Should().Be(1);
            codeBlock.Runs[0].RunType.Should().Be(MadokoRunType.PlainText);
            codeBlock.Runs[0].Text.Should().Be("code1" + Environment.NewLine + "code2");
        }

        private static MadokoCodeBlock MakeCodeBlock(string input)
        {
            MadokoCodeBlock codeBlock = null;

            var fileSystem = new FakeFileSystem();
            using (TextReader reader = new StringReader(input))
            {
                var lineSource = new LineSource(reader, fileSystem);
                string firstLine = lineSource.GetNextLine();

                codeBlock = MadokoCodeBlock.CreateFrom(firstLine, lineSource);
            }

            return codeBlock;
        }
    }
}
