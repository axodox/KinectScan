using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modules
{
    public abstract class ModuleBase
    {
        public abstract string Name { get; }
        public override string ToString()
        {
            return Name;
        }
    }
}
