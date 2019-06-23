using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bismuth.Mvvm
{
    public abstract class ObservableObject : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanging([CallerMemberName] string propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            RaisePropertyChanging(propertyName);
            field = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
