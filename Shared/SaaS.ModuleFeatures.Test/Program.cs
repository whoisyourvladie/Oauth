using System;

namespace SaaS.ModuleFeatures.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"D:\oauth.sodapdf.com\branches\spdf-3427(trunk)\Web.Api\SaaS.Api\App_GlobalResources\lulu-moduleFeatures.json";
            var map = Map.Load(path);

            Write(map, new Version(9, 0));
            Write(map, new Version(10, 0));
            //Write(map, new Version(11, 0));
           //Write(map, new Version(12, 0));
           
            Console.ReadKey();
        }

        private static void Write(Map map, Version license)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("------------------{0}-----------------", license);
            var modules = map.GetModules(MapStrategy.PerpetualLicense, license);

            foreach (var module in modules)
            {
                uint index = 0;
                foreach (var feature in module.Features)
                {
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.Write(feature.Version ?? new Version());
                    if (feature.IsHidden)
                        Console.ForegroundColor = ConsoleColor.Gray;
                    else
                    {
                        if (feature.IsUpgradable)
                            Console.ForegroundColor = ConsoleColor.Red;
                        else
                            Console.ForegroundColor = ConsoleColor.Green;
                    }
                    Console.WriteLine("*");
                    ++index;

                   //if (index > 7) break;
                }
                //break;
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
