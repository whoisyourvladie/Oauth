using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaaS.Common.Extensions;

namespace SaaS.Api.OauthClientGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var hash = "K4K1By1dpmVS2eP4e3WDIZ3fHZLPMP1OReo6bBD5YZI=".GetHash();


            Console.WriteLine(hash);
            Console.ReadLine();
        }
    }
}
