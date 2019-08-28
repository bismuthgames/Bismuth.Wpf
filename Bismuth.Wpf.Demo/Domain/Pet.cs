using System.ComponentModel;

namespace Bismuth.Wpf.Demo.Domain
{
    public enum Gender
    {
        Male,
        Female
    }

    public abstract class Pet
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
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
