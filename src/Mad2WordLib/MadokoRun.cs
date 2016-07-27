// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

namespace Mad2WordLib
{
    public class MadokoRun
    {
        public MadokoRun(MadokoRunType runType, string text)
        {
            RunType = runType;
            Text = text;
        }

        public MadokoRunType RunType { get; }
        public string Text { get; }
    }
}