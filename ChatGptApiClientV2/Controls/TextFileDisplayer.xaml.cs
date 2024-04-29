/*
    ChatGPT Client V2: A GUI client for the OpenAI ChatGPT API (and also Anthropic Claude API) based on WPF.
    Copyright (C) 2024 Lone Wolf Akela

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System.Windows;

namespace ChatGptApiClientV2.Controls;

/// <summary>
/// TextFileDisplayer.xaml 的交互逻辑
/// </summary>
public partial class TextFileDisplayer
{
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded),
        typeof(bool),
        typeof(TextFileDisplayer),
        new PropertyMetadata(false)
    );

    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(
        nameof(FileName),
        typeof(string),
        typeof(TextFileDisplayer),
        new PropertyMetadata("")
    );

    public static readonly DependencyProperty TextContentProperty = DependencyProperty.Register(
        nameof(TextContent),
        typeof(string),
        typeof(TextFileDisplayer),
        new PropertyMetadata("")
    );

    public static readonly DependencyProperty TextContentMaxHeightProperty = DependencyProperty.Register(
        nameof(TextContentMaxHeight),
        typeof(double),
        typeof(TextFileDisplayer),
        new PropertyMetadata(300.0)
    );

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        init => SetValue(IsExpandedProperty, value);
    }

    public string FileName
    {
        get => (string)GetValue(FileNameProperty);
        init => SetValue(FileNameProperty, value);
    }

    public string TextContent
    {
        get => (string)GetValue(TextContentProperty);
        init => SetValue(TextContentProperty, value);
    }

    public double TextContentMaxHeight
    {
        get => (double)GetValue(TextContentMaxHeightProperty);
        init => SetValue(TextContentMaxHeightProperty, value);
    }

    public TextFileDisplayer()
    {
        InitializeComponent();
    }
}