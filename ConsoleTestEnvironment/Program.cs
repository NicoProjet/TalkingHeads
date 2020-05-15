using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkingHeads.BodyParts;
using TalkingHeads.UnitTest.BodyParts;
using System.Drawing.Imaging;

namespace ConsoleTestEnvironment
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Supplementary tests
                DateTime start = DateTime.Now;
                Console.WriteLine("\njpg: ");
                Eyes.FindForms(Memory.LoadImageToBmp(Configuration.PathImages + "0.jpg"), true, true);
                Console.WriteLine("Time used: " + (DateTime.Now - start));

                DateTime start0 = DateTime.Now;
                Console.WriteLine("\nbmp: ");
                Eyes.FindForms(Memory.LoadImageToBmp(Configuration.PathImages + "0.bmp"), true, true);
                Console.WriteLine("Time used: " + (DateTime.Now - start0));

                // Unit tests
                Console.WriteLine("\nTests started. ");
                UnitTestMemory.RunAll(Configuration.PathImages + "0.jpg");
                UnitTestEyes.RunAll();
                Console.WriteLine("Time used: " + (DateTime.Now - start));
            }
            finally
            {
                Console.WriteLine("\n ----------------");
                Console.WriteLine("Tests ended.");
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }
        }
    }
}
