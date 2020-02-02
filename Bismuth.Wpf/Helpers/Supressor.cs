using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bismuth.Wpf.Helpers
{
    public class Supressor : IDisposable
    {
        public bool Is { get; private set; }

        public static implicit operator bool(Supressor x)
        {
            return x.Is;
        }

        public Supressor Begin()
        {
            Is = true;
            return this;
        }

        public void Dispose()
        {
            Is = false;
        }
    }
}
