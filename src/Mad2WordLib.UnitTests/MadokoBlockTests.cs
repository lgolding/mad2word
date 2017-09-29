// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.IO;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoBlockTests
    {
        [Fact(DisplayName = nameof(MadokoBlock_CanOccupyASingleLine))]
        public void MadokoBlock_CanOccupyASingleLine()
        {
            MadokoBlock block = MakeBlock("one");

            block.Runs.Count.Should().Be(1);
            block.Runs[0].Text.Should().Be("one");
        }

        [Fact(DisplayName = nameof(MadokoBlock_InsertsSpaceWhenSpanningSourceLines))]
        public void MadokoBlock_InsertsSpaceWhenSpanningSourceLines()
        {
            MadokoBlock block = MakeBlock(
@"one
two");

            block.Runs.Count.Should().Be(2);
            block.Runs[0].Text.Should().Be("one");
            block.Runs[1].Text.Should().Be(" two");
        }

        [Fact(DisplayName = nameof(MadokoBlock_EndsWithBlankLine))]
        public void MadokoBlock_EndsWithBlankLine()
        {
            MadokoBlock block = MakeBlock(
@"one

two");

            block.Runs.Count.Should().Be(1);
            block.Runs[0].Text.Should().Be("one");
        }

        [Fact(DisplayName = nameof(MadokoBlock_EndsWithHeader))]
        public void MadokoBlock_EndsWithHeader()
        {
            MadokoBlock block = MakeBlock(
@"one
#two");

            block.Runs.Count.Should().Be(1);
            block.Runs[0].Text.Should().Be("one");
        }

        private static MadokoBlock MakeBlock(string input)
        {
            MadokoBlock block = null;

            var environment = new FakeEnvironment();
            var fileSystem = new FakeFileSystem(environment);
            using (TextReader reader = new StringReader(input))
            {
                var lineSource = new LineSource(reader, fileSystem, environment, inputPath: null);
                block = new MadokoBlock(lineSource);
            }

            return block;
        }
    }
}
