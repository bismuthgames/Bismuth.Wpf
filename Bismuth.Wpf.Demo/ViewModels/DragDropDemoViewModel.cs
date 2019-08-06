using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Bismuth.Mvvm;

namespace Bismuth.Wpf.Demo.ViewModels
{
    public class DragDropDemoViewModel : ObservableObject
    {
        public DragDropDemoViewModel()
        {
            ItemsA = new ObservableCollection<DragDropItemViewModel>
            {
                new DragDropItemViewModel("Drag me"),
                new DragDropItemViewModel("No! Drag me!!"),
                new DragDropItemViewModel("Leave me alone!"),
                new DragDropItemViewModel("Not accepted")
            };
        }

        public IList<DragDropItemViewModel> ItemsA { get; }
        public IList<DragDropItemViewModel> ItemsB { get; } = new ObservableCollection<DragDropItemViewModel>();

        public Func<object, bool> AcceptFunction { get; } = item => item is DragDropItemViewModel dragDropItem && dragDropItem.Text != "Not accepted";

        public ICommand DropOnACommand => new RelayCommand<DragDropItemViewModel>(DropOnA);

        public void DropOnA(DragDropItemViewModel item)
        {
            ItemsA.Remove(item);
            ItemsB.Remove(item);
            ItemsA.Add(item);
        }

        public ICommand DropOnBCommand => new RelayCommand<DragDropItemViewModel>(DropOnB);

        public void DropOnB(DragDropItemViewModel item)
        {
            ItemsA.Remove(item);
            ItemsB.Remove(item);
            ItemsB.Add(item);
        }
    }
}
