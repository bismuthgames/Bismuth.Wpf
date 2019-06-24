using Bismuth.Mvvm;

namespace Bismuth.Wpf.Demo.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public MultiSelectTreeViewDemoViewModel MultiSelectTreeViewDemo { get; } = new MultiSelectTreeViewDemoViewModel();
    }
}
