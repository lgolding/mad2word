// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.Collections.Generic;

namespace Mad2WordLib
{
    public class MadokoBlock
    {
        public MadokoBlock()
        {
            Runs = new List<MadokoRun>();
        }

        public List<MadokoRun> Runs { get; }
    }
}