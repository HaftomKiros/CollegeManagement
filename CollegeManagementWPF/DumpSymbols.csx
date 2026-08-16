#!/usr/bin/env dotnet-script
#r "nuget: WPF-UI, 3.0.5"
foreach(var name in System.Enum.GetNames(typeof(Wpf.Ui.Controls.SymbolRegular)))
    System.Console.WriteLine(name);
