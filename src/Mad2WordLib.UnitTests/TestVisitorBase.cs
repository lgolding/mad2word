// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

namespace Mad2WordLib.UnitTests
{
    /// <summary>
    /// Base class for MadokoVisitor classes used in unit tests. By providing empty
    /// implementations of all the methods defined by <see cref="IMadokoVisitor"/>,
    /// this class allows derived classes to implement only the methods they need
    /// for a particular test. For example, a test that needs to validate headings
    /// could choose to override only the MadokoHeading method.
    /// </summary>
    internal abstract class TestVisitorBase : IMadokoVisitor
    {
        public virtual void Visit(MadokoBlock block)
        {
        }

        public virtual void Visit(MadokoBulletListItem listItem)
        {
        }

        public virtual void Visit(MadokoCodeBlock codeBlock)
        {
        }

        public virtual void Visit(MadokoHeading heading)
        {
        }

        public virtual void Visit(MadokoTitle title)
        {
        }
    }
}
