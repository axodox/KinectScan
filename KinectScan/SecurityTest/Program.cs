using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcaliburSecurity;
namespace SecurityTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SecurityClient SC = new SecurityClient();
            
            SC.Activate("asd");
            Console.WriteLine(SC.HasPermissionToRun);
            Console.ReadLine();
        }
    }
}
