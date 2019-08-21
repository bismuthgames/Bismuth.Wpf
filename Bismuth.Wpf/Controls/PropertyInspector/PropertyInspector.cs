using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bismuth.Wpf.Controls
{
    public class PropertyInspector : ItemsControl
    {
        static PropertyInspector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyInspector), new FrameworkPropertyMetadata(typeof(PropertyInspector)));
        }
    }
}
