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

using HandyControl.Tools;
using System;
using System.Windows;

namespace ChatGptApiClientV2;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public App()
    {
        AppDomain.CurrentDomain.UnhandledException += AppExceptionHandler;
        DispatcherUnhandledException += UiExceptionHandler;
        ConfigHelper.Instance.SetLang("zh-CN");
    }

    private static void UiExceptionHandler(object sender,
        System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        HandyControl.Controls.MessageBox.Show(e.Exception.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }

    private static void AppExceptionHandler(object sender, UnhandledExceptionEventArgs e)
    {
        // dump to crash.log
        var ex = (Exception)e.ExceptionObject;
        System.IO.File.WriteAllText("crash.log", ex.ToString());
    }
}