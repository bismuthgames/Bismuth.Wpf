using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bismuth.Mvvm;

namespace Bismuth.Wpf.Demo.ViewModels
{
    public class MultiSelectTreeViewDemoViewModel : ObservableObject
    {
        public MultiSelectTreeViewDemoViewModel()
        {
            var rootNodes = new[]
            {
                new NodeViewModel(GetName(),
                    new NodeViewModel(GetName(),
                        new NodeViewModel(GetName()),
                        new NodeViewModel(GetName()),
                        new NodeViewModel(GetName())),
                    new NodeViewModel(GetName(),
                        new NodeViewModel(GetName()),
                        new NodeViewModel(GetName()),
                        new NodeViewModel(GetName()))
                ),
                new NodeViewModel(GetName(),
                    new NodeViewModel(GetName(),
                        new NodeViewModel(GetName()),
                        new NodeViewModel(GetName()),
                        new NodeViewModel(GetName())))
            };

            RootNodes = new ObservableCollection<NodeViewModel>(rootNodes);

            RegisterRemoveCallbacks();
        }

        public IList<NodeViewModel> RootNodes { get; }
        public IList<NodeViewModel> SelectedNodes { get; } = new ObservableCollection<NodeViewModel>();

        private int _idCounter = 0;
        private string GetName()
        {
            return $"Node ({++_idCounter})";
        }

        private void RegisterRemoveCallbacks()
        {
            foreach (var rootNode in RootNodes)
                rootNode.RemoveCallback += () => RootNodes.Remove(rootNode);
        }
    }
}
