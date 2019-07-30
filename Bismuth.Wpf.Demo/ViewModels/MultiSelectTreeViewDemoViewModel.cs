using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
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

        private IList<NodeViewModel> _selectedNodes = new ObservableCollection<NodeViewModel>();
        public IList<NodeViewModel> SelectedNodes
        {
            get { return _selectedNodes; }
            set { Set(ref _selectedNodes, value); }
        }

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

        public ICommand FixedSizeTest1Command => new RelayCommand(FixedSizeTest1);

        public void FixedSizeTest1()
        {
            SelectedNodes = new[]
            {
                RootNodes[0].Children[1],
                RootNodes[0].Children[1].Children[0],
                RootNodes[0].Children[1].Children[1],
            };
        }

        public ICommand FixedSizeTest2Command => new RelayCommand(FixedSizeTest2);

        public void FixedSizeTest2()
        {
            SelectedNodes = new[]
            {
                RootNodes[1].Children[0],
                RootNodes[1].Children[0].Children[1],
                RootNodes[1].Children[0].Children[2],
            };
        }

        public ICommand ResetCommand => new RelayCommand(Reset);

        public void Reset()
        {
            SelectedNodes = new ObservableCollection<NodeViewModel>();
        }

        public ICommand ClearCommand => new RelayCommand(Clear);

        public void Clear()
        {
            SelectedNodes.Clear();
        }

        public ICommand SelectCommand => new RelayCommand<NodeViewModel>(Select);

        public void Select(NodeViewModel node)
        {
            SelectedNodes.Add(node);
        }

        public ICommand UnselectCommand => new RelayCommand<NodeViewModel>(Unselect);

        public void Unselect(NodeViewModel node)
        {
            SelectedNodes.Remove(node);
        }
    }
}
