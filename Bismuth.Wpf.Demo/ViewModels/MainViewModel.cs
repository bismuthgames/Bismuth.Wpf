using Bismuth.Mvvm;

namespace Bismuth.Wpf.Demo.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public MultiSelectTreeViewDemoViewModel MultiSelectTreeViewDemo { get; } = new MultiSelectTreeViewDemoViewModel();
        public DragDropDemoViewModel DragDropDemo { get; } = new DragDropDemoViewModel();
        public DragDropTreeViewDemoViewModel DragDropTreeViewDemo { get; } = new DragDropTreeViewDemoViewModel();
        public PropertyInspectorDemoViewModel PropertyInspectorDemo { get; } = new PropertyInspectorDemoViewModel();
    }
}
