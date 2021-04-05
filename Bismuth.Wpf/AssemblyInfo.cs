using System.Windows;
using System.Windows.Markup;

[assembly: XmlnsPrefix("http://schemas.bismuth.dk/winfx/xaml", "bm")]
[assembly: XmlnsDefinition("http://schemas.bismuth.dk/winfx/xaml", "Bismuth.Wpf")]
[assembly: XmlnsDefinition("http://schemas.bismuth.dk/winfx/xaml", "Bismuth.Wpf.Controls")]
[assembly: XmlnsDefinition("http://schemas.bismuth.dk/winfx/xaml", "Bismuth.Wpf.Converters")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]
