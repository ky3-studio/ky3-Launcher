//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;

namespace kyxsan.UI.Xaml.View;

internal sealed partial class GuideView : UserControl
{
    public GuideView()
    {
        InitializeComponent();
        BuildAgreementContent();
    }

    private void AgreementScrollViewer_ViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
    {
        if (sender is not ScrollViewer sv)
        {
            return;
        }

        double distanceToBottom = sv.ScrollableHeight - sv.VerticalOffset;
        if (distanceToBottom <= 10)
        {
            TermOfServiceCheckBox.IsEnabled = true;
            PrivacyPolicyCheckBox.IsEnabled = true;
            ScrollHintInfoBar.IsOpen = false;
        }
    }

    private void BuildAgreementContent()
    {
        RichTextBlock rtb = AgreementRichText;
        rtb.LineHeight = 28;
        rtb.FontSize = 14;

        AddTitle(rtb, SH.ViewGuideTitle, 22);
        AddSpacer(rtb);

        AddWarning(rtb, SH.ViewGuideWarning);
        AddSpacer(rtb);

        BuildChapter1(rtb);
        BuildChapter2(rtb);
        BuildChapter3(rtb);
        BuildChapter4(rtb);
        BuildChapter5(rtb);
        BuildChapter6(rtb);
        BuildChapter7(rtb);
        BuildDeveloperDeclarations(rtb);

        AddSpacer(rtb);
        AddFooter(rtb, SH.ViewGuideFooter);
    }

    private static void BuildChapter1(RichTextBlock rtb)
    {
        AddChapterTitle(rtb, SH.ViewGuide1Title);

        AddSectionTitle(rtb, SH.ViewGuide1_1Title);
        AddText(rtb, SH.ViewGuide1_1Text1);
        AddText(rtb, SH.ViewGuide1_1Text2);

        AddSectionTitle(rtb, SH.ViewGuide1_2Title);
        AddText(rtb, SH.ViewGuide1_2Text);
        AddBulletList(rtb,
            SH.ViewGuide1_2Bullet1,
            SH.ViewGuide1_2Bullet2,
            SH.ViewGuide1_2Bullet3);

        AddSectionTitle(rtb, SH.ViewGuide1_3Title);
        AddText(rtb, SH.ViewGuide1_3Text);
        AddBulletList(rtb,
            SH.ViewGuide1_3Bullet1,
            SH.ViewGuide1_3Bullet2,
            SH.ViewGuide1_3Bullet3,
            SH.ViewGuide1_3Bullet4);
        AddSpacer(rtb);
    }

    private static void BuildChapter2(RichTextBlock rtb)
    {
        AddChapterTitle(rtb, SH.ViewGuide2Title);
        AddText(rtb, SH.ViewGuide2Text);

        AddSectionTitle(rtb, SH.ViewGuide2_1Title);
        AddBulletList(rtb,
            SH.ViewGuide2_1Bullet1,
            SH.ViewGuide2_1Bullet2,
            SH.ViewGuide2_1Bullet3,
            SH.ViewGuide2_1Bullet4);

        AddSectionTitle(rtb, SH.ViewGuide2_2Title);
        AddBulletList(rtb,
            SH.ViewGuide2_2Bullet1,
            SH.ViewGuide2_2Bullet2,
            SH.ViewGuide2_2Bullet3,
            SH.ViewGuide2_2Bullet4);

        AddSectionTitle(rtb, SH.ViewGuide2_3Title);
        AddBulletList(rtb,
            SH.ViewGuide2_3Bullet1,
            SH.ViewGuide2_3Bullet2,
            SH.ViewGuide2_3Bullet3);

        AddSectionTitle(rtb, SH.ViewGuide2_4Title);
        AddBulletList(rtb,
            SH.ViewGuide2_4Bullet1,
            SH.ViewGuide2_4Bullet2,
            SH.ViewGuide2_4Bullet3);

        AddSectionTitle(rtb, SH.ViewGuide2_5Title);
        AddBulletList(rtb,
            SH.ViewGuide2_5Bullet1,
            SH.ViewGuide2_5Bullet2,
            SH.ViewGuide2_5Bullet3);

        AddSectionTitle(rtb, SH.ViewGuide2_6Title);
        AddBulletList(rtb,
            SH.ViewGuide2_6Bullet1,
            SH.ViewGuide2_6Bullet2,
            SH.ViewGuide2_6Bullet3);

        AddSectionTitle(rtb, SH.ViewGuide2_7Title);
        AddBulletList(rtb,
            SH.ViewGuide2_7Bullet1,
            SH.ViewGuide2_7Bullet2,
            SH.ViewGuide2_7Bullet3);
        AddSpacer(rtb);
    }

    private static void BuildChapter3(RichTextBlock rtb)
    {
        AddChapterTitle(rtb, SH.ViewGuide3Title);

        AddSectionTitle(rtb, SH.ViewGuide3_1Title);
        AddBulletList(rtb,
            SH.ViewGuide3_1Bullet1,
            SH.ViewGuide3_1Bullet2,
            SH.ViewGuide3_1Bullet3);

        AddSectionTitle(rtb, SH.ViewGuide3_2Title);
        AddText(rtb, SH.ViewGuide3_2Text);
        AddText(rtb, "Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:");
        AddText(rtb, "The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.");
        AddText(rtb, "THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.");
        AddText(rtb, "IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.");

        AddSectionTitle(rtb, SH.ViewGuide3_3Title);
        AddBulletList(rtb,
            SH.ViewGuide3_3Bullet1,
            SH.ViewGuide3_3Bullet2,
            SH.ViewGuide3_3Bullet3,
            SH.ViewGuide3_3Bullet4);
        AddSpacer(rtb);
    }

    private static void BuildChapter4(RichTextBlock rtb)
    {
        AddChapterTitle(rtb, SH.ViewGuide4Title);
        AddText(rtb, SH.ViewGuide4Text);

        AddSectionTitle(rtb, SH.ViewGuide4_1Title);
        AddBulletList(rtb,
            SH.ViewGuide4_1Bullet1,
            SH.ViewGuide4_1Bullet2,
            SH.ViewGuide4_1Bullet3,
            SH.ViewGuide4_1Bullet4);

        AddSectionTitle(rtb, SH.ViewGuide4_2Title);
        AddBulletList(rtb,
            SH.ViewGuide4_2Bullet1,
            SH.ViewGuide4_2Bullet2,
            SH.ViewGuide4_2Bullet3);

        AddSectionTitle(rtb, SH.ViewGuide4_3Title);
        AddText(rtb, SH.ViewGuide4_3Text);
        AddBulletList(rtb,
            SH.ViewGuide4_3Bullet1,
            SH.ViewGuide4_3Bullet2,
            SH.ViewGuide4_3Bullet3,
            SH.ViewGuide4_3Bullet4,
            SH.ViewGuide4_3Bullet5);

        AddSectionTitle(rtb, SH.ViewGuide4_4Title);
        AddText(rtb, SH.ViewGuide4_4Text);
        AddBulletList(rtb,
            SH.ViewGuide4_4Bullet1,
            SH.ViewGuide4_4Bullet2,
            SH.ViewGuide4_4Bullet3);

        AddSectionTitle(rtb, SH.ViewGuide4_5Title);
        AddText(rtb, SH.ViewGuide4_5Text1);
        AddBulletList(rtb,
            SH.ViewGuide4_5Bullet1,
            SH.ViewGuide4_5Bullet2,
            SH.ViewGuide4_5Bullet3);
        AddText(rtb, SH.ViewGuide4_5Text2);
        AddBulletList(rtb,
            SH.ViewGuide4_5Promise1,
            SH.ViewGuide4_5Promise2,
            SH.ViewGuide4_5Promise3);

        AddSectionTitle(rtb, SH.ViewGuide4_6Title);
        AddText(rtb, SH.ViewGuide4_6Text);
        AddBulletList(rtb,
            SH.ViewGuide4_6Bullet1,
            SH.ViewGuide4_6Bullet2,
            SH.ViewGuide4_6Bullet3,
            SH.ViewGuide4_6Bullet4,
            SH.ViewGuide4_6Bullet5,
            SH.ViewGuide4_6Bullet6);

        AddSectionTitle(rtb, SH.ViewGuide4_7Title);
        AddBulletList(rtb,
            SH.ViewGuide4_7Bullet1,
            SH.ViewGuide4_7Bullet2,
            SH.ViewGuide4_7Bullet3,
            SH.ViewGuide4_7Bullet4);

        AddSectionTitle(rtb, SH.ViewGuide4_8Title);
        AddBulletList(rtb,
            SH.ViewGuide4_8Bullet1,
            SH.ViewGuide4_8Bullet2,
            SH.ViewGuide4_8Bullet3,
            SH.ViewGuide4_8Bullet4);
        AddSpacer(rtb);
    }

    private static void BuildChapter5(RichTextBlock rtb)
    {
        AddChapterTitle(rtb, SH.ViewGuide5Title);

        AddSectionTitle(rtb, SH.ViewGuide5_1Title);
        AddBulletList(rtb,
            SH.ViewGuide5_1Bullet1,
            SH.ViewGuide5_1Bullet2,
            SH.ViewGuide5_1Bullet3);

        AddSectionTitle(rtb, SH.ViewGuide5_2Title);
        AddBulletList(rtb,
            SH.ViewGuide5_2Bullet1,
            SH.ViewGuide5_2Bullet2,
            SH.ViewGuide5_2Bullet3,
            SH.ViewGuide5_2Bullet4);

        AddSectionTitle(rtb, SH.ViewGuide5_3Title);
        AddBulletList(rtb,
            SH.ViewGuide5_3Bullet1,
            SH.ViewGuide5_3Bullet2,
            SH.ViewGuide5_3Bullet3,
            SH.ViewGuide5_3Bullet4);

        AddSectionTitle(rtb, SH.ViewGuide5_4Title);
        AddText(rtb, SH.ViewGuide5_4Text);
        AddBulletList(rtb,
            SH.ViewGuide5_4Bullet1,
            SH.ViewGuide5_4Bullet2,
            SH.ViewGuide5_4Bullet3);

        AddSectionTitle(rtb, SH.ViewGuide5_5Title);
        AddText(rtb, SH.ViewGuide5_5Text);
        AddBulletList(rtb,
            SH.ViewGuide5_5Bullet1,
            SH.ViewGuide5_5Bullet2,
            SH.ViewGuide5_5Bullet3,
            SH.ViewGuide5_5Bullet4,
            SH.ViewGuide5_5Bullet5);
        AddSpacer(rtb);
    }

    private static void BuildChapter6(RichTextBlock rtb)
    {
        AddChapterTitle(rtb, SH.ViewGuide6Title);

        AddSectionTitle(rtb, SH.ViewGuide6_1Title);
        AddBulletList(rtb,
            SH.ViewGuide6_1Bullet1,
            SH.ViewGuide6_1Bullet2,
            SH.ViewGuide6_1Bullet3);

        AddSectionTitle(rtb, SH.ViewGuide6_2Title);
        AddBulletList(rtb,
            SH.ViewGuide6_2Bullet1,
            SH.ViewGuide6_2Bullet2,
            SH.ViewGuide6_2Bullet3,
            SH.ViewGuide6_2Bullet4,
            SH.ViewGuide6_2Bullet5);

        AddSectionTitle(rtb, SH.ViewGuide6_3Title);
        AddBulletList(rtb,
            SH.ViewGuide6_3Bullet1,
            SH.ViewGuide6_3Bullet2,
            SH.ViewGuide6_3Bullet3);
        AddSpacer(rtb);
    }

    private static void BuildChapter7(RichTextBlock rtb)
    {
        AddChapterTitle(rtb, SH.ViewGuide7Title);

        AddSectionTitle(rtb, SH.ViewGuide7_1Title);
        AddBulletList(rtb,
            SH.ViewGuide7_1Bullet1,
            SH.ViewGuide7_1Bullet2,
            SH.ViewGuide7_1Bullet3);

        AddSectionTitle(rtb, SH.ViewGuide7_2Title);
        AddBulletList(rtb,
            SH.ViewGuide7_2Bullet1,
            SH.ViewGuide7_2Bullet2);

        AddSectionTitle(rtb, SH.ViewGuide7_3Title);
        AddBulletList(rtb,
            SH.ViewGuide7_3Bullet1,
            SH.ViewGuide7_3Bullet2,
            SH.ViewGuide7_3Bullet3);

        AddSectionTitle(rtb, SH.ViewGuide7_4Title);
        AddText(rtb, SH.ViewGuide7_4Text);
        AddSpacer(rtb);
    }

    private static void BuildDeveloperDeclarations(RichTextBlock rtb)
    {
    }

    private static void AddTitle(RichTextBlock rtb, string text, double fontSize)
    {
        Paragraph p = new() { FontSize = fontSize, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 8) };
        p.Inlines.Add(new Run { Text = text });
        rtb.Blocks.Add(p);
    }

    private static void AddChapterTitle(RichTextBlock rtb, string text)
    {
        Paragraph p = new() { FontSize = 17, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 20, 0, 12) };
        p.Inlines.Add(new Run { Text = text });
        rtb.Blocks.Add(p);
    }

    private static void AddSectionTitle(RichTextBlock rtb, string text)
    {
        Paragraph p = new() { FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 12, 0, 6) };
        p.Inlines.Add(new Run { Text = text });
        rtb.Blocks.Add(p);
    }

    private static void AddText(RichTextBlock rtb, string text)
    {
        Paragraph p = new() { Margin = new Thickness(0, 0, 0, 8) };
        p.Inlines.Add(new Run { Text = text });
        rtb.Blocks.Add(p);
    }

    private static void AddWarning(RichTextBlock rtb, string text)
    {
        Paragraph p = new() { Margin = new Thickness(0, 0, 0, 8) };
        p.Inlines.Add(new Run
        {
            Text = text,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 220, 53, 69)),
            TextDecorations = Windows.UI.Text.TextDecorations.Underline,
        });
        rtb.Blocks.Add(p);
    }

    private static void AddBulletList(RichTextBlock rtb, params string[] items)
    {
        Paragraph p = new() { Margin = new Thickness(0, 0, 0, 10) };
        for (int i = 0; i < items.Length; i++)
        {
            if (i > 0)
            {
                p.Inlines.Add(new LineBreak());
            }

            p.Inlines.Add(new Run { Text = items[i] });
        }

        rtb.Blocks.Add(p);
    }

    private static void AddSpacer(RichTextBlock rtb)
    {
        Paragraph p = new() { Margin = new Thickness(0, 4, 0, 4) };
        p.Inlines.Add(new Run { Text = " ", FontSize = 4 });
        rtb.Blocks.Add(p);
    }

    private static void AddFooter(RichTextBlock rtb, string text)
    {
        Paragraph p = new() { Margin = new Thickness(0, 16, 0, 0), FontSize = 12 };
        string[] lines = text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            if (i > 0)
            {
                p.Inlines.Add(new LineBreak());
            }

            p.Inlines.Add(new Run { Text = lines[i] });
        }

        rtb.Blocks.Add(p);
    }
}