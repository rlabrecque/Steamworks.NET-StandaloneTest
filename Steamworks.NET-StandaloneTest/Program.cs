using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Steamworks;

namespace SteamworksNET_StandaloneTest
{
    class Program
    {
        static void Main(string[] args)
        {
            bool ret = SteamAPI.Init();
            if(!ret)
            {
                Console.WriteLine("SteamAPI.Init() failed!");
                return;
            }

            ret = Packsize.Test();
            Console.WriteLine("Packsize.Test() returned: " + ret);

            while (!Console.KeyAvailable)
            {
                SteamAPI.RunCallbacks();
                Thread.Sleep(100);
            }

            SteamAPI.Shutdown();
        }
    }
}
