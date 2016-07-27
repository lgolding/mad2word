// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using DocumentFormat.OpenXml.Wordprocessing;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoToWordConverterTests
    {
        [Fact(Skip = "NYI")]
        public void MadokoToWordConverter_ConvertsInlineCodeFragments()
        {
            const string StyleId = StyleNames.CodeChar;
            const string Line = "Here is some `code` and some `more code`.";

            Run[] runs = MadokoToWordConverter.ConvertLineToRuns(Line);

            runs.Length.Should().Be(5);
            runs[0].RunProperties.RunStyle.Val.Value.Should().NotBe(StyleId);
            runs[1].RunProperties.RunStyle.Val.Value.Should().Be(StyleId);
            runs[2].RunProperties.RunStyle.Val.Value.Should().NotBe(StyleId);
            runs[3].RunProperties.RunStyle.Val.Value.Should().Be(StyleId);
            runs[4].RunProperties.RunStyle.Val.Value.Should().NotBe(StyleId);
        }
    }
}
