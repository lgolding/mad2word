// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Mad2WordLib
{
    public class MadokoReaderException : Exception
    {
        public MadokoReaderException() : base() { }

        public MadokoReaderException(string message) : base(message) { }

        public MadokoReaderException(string message, Exception innerException) : base(message, innerException) { }

        public MadokoReaderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
