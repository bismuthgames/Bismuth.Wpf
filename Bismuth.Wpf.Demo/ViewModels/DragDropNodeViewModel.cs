using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bismuth.Wpf.Demo.ViewModels
{
    public class DragDropNodeViewModel : NodeViewModel
    {
        public DragDropNodeViewModel(string name, params NodeViewModel[] children) : base(name, children)
        {
        }
    }
}
