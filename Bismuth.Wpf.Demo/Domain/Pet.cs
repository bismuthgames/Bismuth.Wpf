using System.ComponentModel;

namespace Bismuth.Wpf.Demo.Domain
{
    public abstract class Pet
    {
        public string Name { get; set; }
        public int Age { get; set; }
        [DisplayName("Registration Number")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string RegistrationNumber { get; set; }
    }

    public class Cat : Pet
    {

    }

    public class Dog : Pet
    {

    }
}
