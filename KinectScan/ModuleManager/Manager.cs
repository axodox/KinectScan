using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

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
            AvailableModules = new List<ModuleInfo>();
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

        public class ModuleInfo
        {
            public string Name { get; private set; }
            public string FileName { get; private set; }
            public string ID { get; private set; }
            private ModuleInfo() { }
            public static ModuleInfo FromFile(string pluginFile)
            {
                try
                {
                    ModuleInfo MI = new ModuleInfo();
                    MI.ID = Path.GetFileNameWithoutExtension(pluginFile).ToUpper();
                    string[] infoLines = File.ReadAllLines(pluginFile);
                    string[] infoLine;
                    foreach (string info in infoLines)
                    {
                        infoLine = info.Split('|');
                        switch (infoLine[0])
                        {
                            case "file":
                                MI.FileName = Path.GetDirectoryName(pluginFile)+"\\"+infoLine[1];
                                break;
                            case "name":
                                if (infoLine[1] == "default" || infoLine[1] == Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower())
                                    MI.Name = infoLine[2];
                                break;
                        }
                    }
                    return MI;
                }
                catch
                {
                    return null;
                }
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public List<ModuleInfo> AvailableModules { get; private set; }
        public void LoadAvailableModules()
        {
            try
            {
                AvailableModules.Clear();
                string[] plugins = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.plugin");
                ModuleInfo MI;
                foreach (string plugin in plugins)
                {
                    MI = ModuleInfo.FromFile(plugin);
                    if (MI != null)
                        AvailableModules.Add(MI);
                }
            }
            catch
            {

            }
        }
    }
}
