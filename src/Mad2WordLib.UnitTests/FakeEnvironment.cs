// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

namespace Mad2WordLib.UnitTests
{
    public class FakeEnvironment : IEnvironment
    {
        public FakeEnvironment(string currentDirectory = @"C:\")
        {
            CurrentDirectory = currentDirectory;
        }

        public string CurrentDirectory { get; set; }
    }
}
