using System.Collections.Generic;
using Bismuth.Mvvm;
using Bismuth.Wpf.Demo.Domain;

namespace Bismuth.Wpf.Demo.ViewModels
{
    public class PropertyInspectorDemoViewModel : ObservableObject
    {
        public IList<Pet> Pets { get; } = new Pet[]
        {
            new Cat { Name = "Kobra", Age = 8},
            new Cat { Name = "Panda", Age = 4},
            new Dog { Name = "Dino", Age = 11}
        };
    }
}
