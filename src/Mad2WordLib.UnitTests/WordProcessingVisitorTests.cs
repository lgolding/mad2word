// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class WordProcessingVisitorTests : MadokoTestBase
    {
        [Fact(DisplayName = nameof(WordProcessingVisitor_FormatsCodeRun))]
        public void WordProcessingVisitor_FormatsCodeRun()
        {
            var body = new Body();
            var target = new WordProcessingVisitor(body);

            LineSource lineSource = MakeLineSource("`f()`");
            MadokoBlock madokoBlock = new MadokoBlock(lineSource);

            target.Visit(madokoBlock);

            var paras = body.Descendants<Paragraph>().ToList();
            paras.Count.Should().Be(1);
            var para = paras.Single();

            var runs = para.Descendants<Run>().ToList();
            runs.Count.Should().Be(1);
            var run = runs.Single();

            var runPropertiesList = run.Descendants<RunProperties>().ToList();
            runPropertiesList.Count.Should().Be(1);
            var runProperties = runPropertiesList.Single();

            var runStyles = runProperties.Descendants<RunStyle>().ToList();
            runStyles.Count.Should().Be(1);
            var runStyle = runStyles.Single();

           runStyle.Val.Value.Should().Be(StyleIds.CodeChar);

            run.Descendants<Text>().Count().Should().Be(1);
            var text = run.Descendants<Text>().Single();
            text.InnerText.Should().Be("f()");
        }

        [Fact(DisplayName = nameof(WordProcessingVisitor_FormatsHeading))]
        public void WordProcessingVisitor_FormatsHeading()
        {
            var body = new Body();
            var target = new WordProcessingVisitor(body);

            LineSource lineSource = MakeLineSource("## abc");
            MadokoHeading madokoHeading = new MadokoHeading(lineSource);

            target.Visit(madokoHeading);

            VerifySingleParagraphFormatting(body, "Heading2", "abc");
        }

        [Fact(DisplayName = nameof(WordProcessingVisitor_FormatsTitle))]
        public void WordProcessingVisitor_FormatsTitle()
        {
            var body = new Body();
            var target = new WordProcessingVisitor(body);

            MadokoTitle madokoTitle = new MadokoTitle("The Title");

            target.Visit(madokoTitle);

            VerifySingleParagraphFormatting(body, "Title", "The Title");
        }

        private void VerifySingleParagraphFormatting(Body body, string expectedStyleId, string expectedText)
        {
            var paras = body.Descendants<Paragraph>().ToList();
            paras.Count.Should().Be(1);
            var para = paras.Single();

            var paraPropertiesList = para.Descendants<ParagraphProperties>().ToList();
            paraPropertiesList.Count.Should().Be(1);
            var paraProperties = paraPropertiesList.Single();

            var styles = paraProperties.Descendants<ParagraphStyleId>().ToList();
            styles.Count.Should().Be(1);
            var style = styles.Single();

            string actualStyleId = style.Val.Value;
            actualStyleId.Should().Be(expectedStyleId);

            var runs = para.Descendants<Run>().ToList();
            runs.Count.Should().Be(1);
            var run = runs.Single();

            run.Descendants<RunProperties>().Should().BeEmpty();

            run.Descendants<Text>().Count().Should().Be(1);
            var actualText = run.Descendants<Text>().Single();
            actualText.InnerText.Should().Be(expectedText);
        }
    }
}
