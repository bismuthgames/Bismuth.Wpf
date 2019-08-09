namespace Bismuth.Wpf.Demo.ViewModels
{
    public static class IdGenerator
    {
        private static int _idCounter = 0;

        public static int GetNext()
        {
            return ++_idCounter;
        }
    }
}
