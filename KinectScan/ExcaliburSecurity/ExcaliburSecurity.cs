using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Management;

namespace ExcaliburSecurity
{
    public class SecurityClient
    {
        public bool PermissionToRun
        {
            get
            {
                return true;
            }
        }
        public SecurityClient()
        {
            //GetManagementInfo("Win32_Processor", new string[] {"Name", "ProcessorId"});
        }

        //private ulong IdCPU
        //{
        //    get
        //    {

        //    }
        //}

        private object[] GetManagementInfo(string key, string[] properties)
        {
            ManagementObjectSearcher MOS = new ManagementObjectSearcher("SELECT * FROM " + key);
            ManagementObjectCollection MOC = MOS.Get();
            object[][] results = new object[MOC.Count][];
            int i = 0;
            foreach (ManagementObject MO in MOS.Get())
            {
                results[i] = new object[properties.Length];
                for (int j = 0; j < properties.Length; j++)
                {
                    results[i][j]
                        = MO.Properties[properties[j]].Value;
                }
            }
            return results;
        }

        private ulong[] GetHardwareID()
        {
            NetworkInterface[] NIs = NetworkInterface.GetAllNetworkInterfaces();
            List<ulong> IDs = new List<ulong>(NIs.Length);
            byte[] buffer = new byte[8];
            byte[] phy;
            ulong id;
            for (int i = 0; i < NIs.Length; i++)
            {
                phy = NIs[i].GetPhysicalAddress().GetAddressBytes();
                if (phy.Length > 0 && NIs[i].OperationalStatus == OperationalStatus.Up)
                {
                    for (int j = 0; j < phy.Length; j++) buffer[j] = phy[j];
                    for (int j = phy.Length; j < 8; j++) buffer[j] = 0;
                    id = BitConverter.ToUInt64(buffer, 0);
                    for (int j = 0; j < IDs.Count; j++)
                    {
                        while (IDs.Count > j && IDs[j] == id)
                        {
                            IDs.RemoveAt(j);
                        }
                    }
                    IDs.Add(id);
                }
            }
            return IDs.ToArray();
        }
    }
}
