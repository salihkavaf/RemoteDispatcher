using System;
using System.Reflection;

namespace RemoteDispatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var versionString =
                    Assembly.GetEntryAssembly()
                        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                        .InformationalVersion
                        .ToString();

                Console.WriteLine($"salih-rd v{versionString}");
                Console.WriteLine("-------------");
                Console.WriteLine("\nUsage:");
                Console.WriteLine("  salih-rd <message>");
                return;
            }
        }
    }
}
