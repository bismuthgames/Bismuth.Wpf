﻿using Bismuth.Mvvm;

namespace Bismuth.Wpf.Demo.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public MultiSelectTreeViewDemoViewModel MultiSelectTreeViewDemo { get; } = new MultiSelectTreeViewDemoViewModel();
        public DragDropDemoViewModel DragDropDemo { get; } = new DragDropDemoViewModel();
    }
}
