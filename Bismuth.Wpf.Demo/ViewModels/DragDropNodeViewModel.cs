using System;
using System.Windows.Input;
using Bismuth.Mvvm;

namespace Bismuth.Wpf.Demo.ViewModels
{
    public class DragDropNodeViewModel : NodeViewModel
    {
        public DragDropNodeViewModel(string name, params NodeViewModel[] children) : base(name, children)
        {
        }

        public Func<object, bool> AcceptFunction { get; } = item => true;
        public Type[] AcceptTypes { get; } = new[] { typeof(object[]) };

        public ICommand DropCommand => new RelayCommand<DragDropNodeViewModel[]>(Drop);

        public void Drop(DragDropNodeViewModel[] nodes)
        {
        }

        public ICommand DropBeforeCommand => new RelayCommand<DragDropNodeViewModel[]>(DropBefore);

        public void DropBefore(DragDropNodeViewModel[] nodes)
        {
        }

        public ICommand DropAfterCommand => new RelayCommand<DragDropNodeViewModel[]>(DropAfter);

        public void DropAfter(DragDropNodeViewModel[] nodes)
        {
        }
    }
}
