﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.IO;

namespace Mad2WordLib
{
    public interface IFileSystem
    {
        TextReader OpenText(string path);
    }
}