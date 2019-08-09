using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bismuth.Mvvm;

namespace Bismuth.Wpf.Demo.ViewModels
{
    public class DragDropTreeViewDemoViewModel : ObservableObject
    {
        public DragDropTreeViewDemoViewModel()
        {
            CreateTree();
        }

        private IList<NodeViewModel> _rootNodes;
        public IList<NodeViewModel> RootNodes
        {
            get { return _rootNodes; }
            set { Set(ref _rootNodes, value); }
        }

        private IList<NodeViewModel> _selectedNodes = new ObservableCollection<NodeViewModel>();
        public IList<NodeViewModel> SelectedNodes
        {
            get { return _selectedNodes; }
            set { Set(ref _selectedNodes, value); }
        }

        public void CreateTree()
        {
            var rootNodes = new[]
            {
                new DragDropNodeViewModel(GetName(),
                    new DragDropNodeViewModel(GetName(),
                        new DragDropNodeViewModel(GetName()),
                        new DragDropNodeViewModel(GetName()),
                        new DragDropNodeViewModel(GetName())),
                    new DragDropNodeViewModel(GetName(),
                        new DragDropNodeViewModel(GetName()),
                        new DragDropNodeViewModel(GetName()),
                        new DragDropNodeViewModel(GetName()))
                ),
                new DragDropNodeViewModel(GetName(),
                    new DragDropNodeViewModel(GetName(),
                        new DragDropNodeViewModel(GetName()),
                        new DragDropNodeViewModel(GetName()),
                        new DragDropNodeViewModel(GetName())))
            };

            RootNodes = new ObservableCollection<NodeViewModel>(rootNodes);

            RegisterRemoveCallbacks();
        }

        private string GetName()
        {
            return $"Node ({IdGenerator.GetNext()})";
        }

        private void RegisterRemoveCallbacks()
        {
            foreach (var rootNode in RootNodes)
                rootNode.RemoveCallback += () => RootNodes.Remove(rootNode);
        }
    }
}
