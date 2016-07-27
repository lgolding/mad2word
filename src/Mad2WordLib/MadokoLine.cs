// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

namespace Mad2WordLib
{
    internal static class MadokoLine
    {
        internal static MadokoRun[] Parse(string line)
        {
            return new[] { new MadokoRun(MadokoRunType.PlainText, line) };
        }
    }
}
