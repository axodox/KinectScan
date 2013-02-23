using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Modules
{
    public class ModuleManager
    {
        public List<ModuleBase> Modules { get; private set; }
        public object Context { get; private set; }
        private object[] CreationArgs;
        public ModuleManager(object context)
        {
            Modules = new List<ModuleBase>();
            Context = context;
            CreationArgs = new object[] { Context };
        }

        public bool Load(string path)
        {
            if (!File.Exists(path)) return false;
            try
            {
                Assembly A = Assembly.UnsafeLoadFrom(path);
                ModuleBase MB = (ModuleBase)A.CreateInstance("KinectScan.KinectScanModule", false, BindingFlags.Default, null, CreationArgs, null, null);
                if (MB == null) return false;
                Modules.Add(MB);
                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
