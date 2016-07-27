// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using DocumentFormat.OpenXml.Wordprocessing;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoToWordConverterTests
    {
        [Fact]
        public void MadokoToWordConverter_ConvertsInlineCodeFragments()
        {
            const string Line = "Here is some `code` and some `more code`.";

            Run[] runs = MadokoToWordConverter.ConvertLineToRuns(Line);

            runs.Length.Should().Be(5);
            runs[0].RunProperties.Should().BeNull();
            runs[0].InnerText.Should().Be("Here is some ");
            runs[1].RunProperties.RunStyle.Val.Value.Should().Be(StyleNames.CodeChar);
            runs[1].InnerText.Should().Be("code");
            runs[2].RunProperties.Should().BeNull();
            runs[2].InnerText.Should().Be(" and some ");
            runs[3].RunProperties.RunStyle.Val.Value.Should().Be(StyleNames.CodeChar);
            runs[3].InnerText.Should().Be("more code");
            runs[4].RunProperties.Should().BeNull();
            runs[4].InnerText.Should().Be(".");
        }

        [Fact]
        public void MadokoToWordConverter_ConvertsLineStartingWithCodeFragment()
        {
            const string Line = "`string` is a type.";

            Run[] runs = MadokoToWordConverter.ConvertLineToRuns(Line);

            runs.Length.Should().Be(2);
            runs[0].RunProperties.RunStyle.Val.Value.Should().Be(StyleNames.CodeChar);
            runs[0].InnerText.Should().Be("string");
            runs[1].RunProperties.Should().BeNull();
            runs[1].InnerText.Should().Be(" is a type.");
        }

        [Fact]
        public void MadokoToWordConverter_ConvertsLineWithOnlyCodeFragment()
        {
            const string Line = "`function a()`";

            Run[] runs = MadokoToWordConverter.ConvertLineToRuns(Line);

            runs.Length.Should().Be(1);
            runs[0].RunProperties.RunStyle.Val.Value.Should().Be(StyleNames.CodeChar);
            runs[0].InnerText.Should().Be("function a()");
        }
    }
}
