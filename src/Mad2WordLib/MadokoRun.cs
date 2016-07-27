// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

namespace Mad2WordLib
{
    internal class MadokoRun
    {
        internal MadokoRun(MadokoRunType runType, string text)
        {
            RunType = runType;
            Text = text;
        }

        internal MadokoRunType RunType { get; }
        internal string Text { get; }
    }
}