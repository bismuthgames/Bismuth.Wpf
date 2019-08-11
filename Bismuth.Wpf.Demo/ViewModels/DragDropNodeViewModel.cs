using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Bismuth.Mvvm;

namespace Bismuth.Wpf.Demo.ViewModels
{
    public class DragDropNodeViewModel : NodeViewModel
    {
        public DragDropNodeViewModel(string name, params NodeViewModel[] children) : base(name, children)
        {
        }

        public Func<object, bool> AcceptFunction { get; } = item => item is IList<object> list && list.All(x => x is DragDropNodeViewModel);
        public Type[] AcceptTypes { get; } = new[] { typeof(IList) };

        public ICommand DropCommand => new RelayCommand<IList>(Drop);

        public void Drop(IList nodes)
        {
        }

        public ICommand DropBeforeCommand => new RelayCommand<IList>(DropBefore);

        public void DropBefore(IList nodes)
        {
        }

        public ICommand DropAfterCommand => new RelayCommand<IList>(DropAfter);

        public void DropAfter(IList nodes)
        {
        }
    }
}
