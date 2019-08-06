using System.Windows.Controls;
using Bismuth.Wpf.Controls;
using Bismuth.Wpf.Demo.ViewModels;

namespace Bismuth.Wpf.Demo.Views
{
    public partial class DragDropDemoView : UserControl
    {
        public DragDropDemoView()
        {
            InitializeComponent();
        }

        private void DropTarget_ItemDropped(object sender, ItemDroppedEventArgs e)
        {
            ((DragDropDemoViewModel)DataContext).DropOnA((DragDropItemViewModel)e.Item);
        }
    }
}
