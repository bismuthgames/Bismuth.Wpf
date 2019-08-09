using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Bismuth.Mvvm;

namespace Bismuth.Wpf.Demo.ViewModels
{
    public class NodeViewModel : ObservableObject
    {
        public NodeViewModel(string name, params NodeViewModel[] children)
        {
            if (children == null) throw new ArgumentNullException(nameof(children));

            Name = name ?? throw new ArgumentNullException(nameof(name));
            Children = new ObservableCollection<NodeViewModel>(children);

            RegisterRemoveCallbacks();
        }

        public event Action RemoveCallback;
        public string Name { get; }
        public IList<NodeViewModel> Children { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(ref _isSelected, value); }
        }

        private bool _isExpanded = true;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { Set(ref _isExpanded, value); }
        }

        public ICommand AddCommand => new RelayCommand(Add);

        public void Add()
        {
            var node = new NodeViewModel($"New Node ({IdGenerator.GetNext()})");
            node.RemoveCallback = () => Children.Remove(node);
            Children.Add(node);
        }

        public ICommand AddAsSelectedCommand => new RelayCommand(AddAsSelected);

        public void AddAsSelected()
        {
            var node = new NodeViewModel($"New Node ({IdGenerator.GetNext()})");
            node.IsSelected = true;
            node.RemoveCallback = () => Children.Remove(node);
            Children.Add(node);
        }

        public ICommand RemoveCommand => new RelayCommand(Remove);

        public void Remove()
        {
            RemoveCallback?.Invoke();
        }

        private void RegisterRemoveCallbacks()
        {
            foreach (var child in Children)
                child.RemoveCallback += () => Children.Remove(child);
        }

        public override string ToString()
        {
            return $"{GetType().Name}.Name: {Name}";
        }
    }
}
