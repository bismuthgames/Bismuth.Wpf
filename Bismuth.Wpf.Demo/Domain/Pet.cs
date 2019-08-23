namespace Bismuth.Wpf.Demo.Domain
{
    public abstract class Pet
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Cat : Pet
    {

    }

    public class Dog : Pet
    {

    }
}
