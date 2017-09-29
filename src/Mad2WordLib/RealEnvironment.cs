// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;

namespace Mad2WordLib
{
    public class RealEnvironment : IEnvironment
    {
        public string CurrentDirectory
        {
            get => Environment.CurrentDirectory;
            set { Environment.CurrentDirectory = value; }
        }
    }
}
