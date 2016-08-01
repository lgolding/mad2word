// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoLineTests
    {
        [Fact(DisplayName = nameof(MadokoLine_ParsesCodeFragments))]
        public void MadokoLine_ParsesCodeFragments()
        {
            const string Line = "Here is some `code` and some `more code`.";

            MadokoRun[] runs = MadokoLine.Parse(Line);

            runs.Length.Should().Be(5);
            runs[0].RunType.Should().Be(MadokoRunType.PlainText);
            runs[0].Text.Should().Be("Here is some ");
            runs[1].RunType.Should().Be(MadokoRunType.Code);
            runs[1].Text.Should().Be("code");
            runs[2].RunType.Should().Be(MadokoRunType.PlainText);
            runs[2].Text.Should().Be(" and some ");
            runs[3].RunType.Should().Be(MadokoRunType.Code);
            runs[3].Text.Should().Be("more code");
            runs[4].RunType.Should().Be(MadokoRunType.PlainText);
            runs[4].Text.Should().Be(".");
        }

        [Fact(DisplayName = nameof(MadokoLine_ParsesLineStartingWithCodeFragment))]
        public void MadokoLine_ParsesLineStartingWithCodeFragment()
        {
            MadokoRun[] runs = MadokoLine.Parse("`string` is a type.");

            runs.Length.Should().Be(2);
            runs[0].RunType.Should().Be(MadokoRunType.Code);
            runs[0].Text.Should().Be("string");
            runs[1].RunType.Should().Be(MadokoRunType.PlainText);
            runs[1].Text.Should().Be(" is a type.");
        }

        [Fact(DisplayName = nameof(MadokoLine_ParsesLineWithOnlyCodeFragment))]
        public void MadokoLine_ParsesLineWithOnlyCodeFragment()
        {
            MadokoRun[] runs = MadokoLine.Parse("`function a()`");

            runs.Length.Should().Be(1);
            runs[0].RunType.Should().Be(MadokoRunType.Code);
            runs[0].Text.Should().Be("function a()");
        }

        [Fact(DisplayName = nameof(MadokoLine_ParsesEntities))]
        public void MadokoLine_ParsesEntities()
        {
            MadokoRun[] runs = MadokoLine.Parse("See &sect;1.1 &HeLLiP; &SECT;1.2.");

            runs.Length.Should().Be(1);
            runs[0].RunType.Should().Be(MadokoRunType.PlainText);
            runs[0].Text.Should().Be("See \u00a71.1 \u2026 \u00a71.2.");
        }

        [Fact(DisplayName = nameof(MadokoLine_ParsesItalics))]
        public void MadokoLine_ParsesItalics()
        {
            const string Line = "_This_ and _that_";

            MadokoRun[] runs = MadokoLine.Parse(Line);

            runs.Length.Should().Be(3);
            runs[0].RunType.Should().Be(MadokoRunType.Italic);
            runs[0].Text.Should().Be("This");
            runs[1].RunType.Should().Be(MadokoRunType.PlainText);
            runs[1].Text.Should().Be(" and ");
            runs[2].RunType.Should().Be(MadokoRunType.Italic);
            runs[2].Text.Should().Be("that");
        }

        [Fact(DisplayName = nameof(MadokoLine_DoesNotTreatUnderscoresInCodeFragmentsAsItalics))]
        public void MadokoLine_DoesNotTreatUnderscoresInCodeFragmentsAsItalics()
        {
            MadokoRun[] runs = MadokoLine.Parse("`function hello_world()`");

            runs.Length.Should().Be(1);
            runs[0].RunType.Should().Be(MadokoRunType.Code);
            runs[0].Text.Should().Be("function hello_world()");
        }
    }
}