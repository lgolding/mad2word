// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using DocumentFormat.OpenXml.Wordprocessing;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class RunExtensionTests
    {
        [Fact]
        public void Run_SetsStyle()
        {
            const string StyleId = StyleNames.CodeChar;

            var r = new Run();
            r.SetStyle(StyleId);

            r.RunProperties.RunStyle.Val.Value.Should().Be(StyleId);
        }
    }
}
