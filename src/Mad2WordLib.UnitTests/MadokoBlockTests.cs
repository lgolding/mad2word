// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.IO;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoBlockTests
    {
        [Fact(DisplayName = nameof(MadokoBlock_can_occupy_a_single_line))]
        public void MadokoBlock_can_occupy_a_single_line()
        {
            MadokoBlock block = MakeBlock("one");

            block.Runs.Count.Should().Be(1);
            block.Runs[0].Text.Should().Be("one");
        }

        [Fact(DisplayName = nameof(MadokoBlock_inserts_space_when_spanning_source_lines))]
        public void MadokoBlock_inserts_space_when_spanning_source_lines()
        {
            MadokoBlock block = MakeBlock(
@"one
two");

            block.Runs.Count.Should().Be(2);
            block.Runs[0].Text.Should().Be("one");
            block.Runs[1].Text.Should().Be(" two");
        }

        [Fact(DisplayName = nameof(MadokoBlock_ends_with_blank_line))]
        public void MadokoBlock_ends_with_blank_line()
        {
            MadokoBlock block = MakeBlock(
@"one

two");

            block.Runs.Count.Should().Be(1);
            block.Runs[0].Text.Should().Be("one");
        }

        [Fact(DisplayName = nameof(MadokoBlock_ends_with_header))]
        public void MadokoBlock_ends_with_header()
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
                var lineSource = new LineSource(reader, null, fileSystem, environment);
                block = new MadokoBlock(lineSource);
            }

            return block;
        }
    }
}
