using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecodeROM
{
    class Program
    {
        static byte[] byteOrder = null;
        static byte[] addressOrder = null;

        static void Main(string[] args)
        {            
            Console.WriteLine("DecodeROM - Command line tool for decoding ROM images with data or address bits out of order");
            Console.WriteLine("(C) Brendan Alford 2017 (brendanalford@eircom.net)\n");
            
            bool debug = System.Diagnostics.Debugger.IsAttached;

            if (args.Length < 2 && !debug)
            {
                DisplayUsage();
                return;
            }

  //          try
            {
                string fileName = ParseCommandLine(args);

                Decoder d = new Decoder();
                d.DecodeFile(fileName, addressOrder, byteOrder);
            }
            //catch (Exception e)
            //{
            //    Console.WriteLine("Fatal error: " + e.Message);
            //}
        }

        static string ParseCommandLine(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i][0] == '-')
                {
                    string param = args[i].Substring(1).ToLower();
                    if (param.StartsWith("a") || param.StartsWith("address"))
                    {
                        ParseAddressLines(StripParamName(param));
                    }
                    if (param.StartsWith("d") || param.StartsWith("data"))
                    {
                        ParseDataLines(StripParamName(param));
                    }

                }
                else
                {
                    if (i + 1 < args.Length)
                    {
                        Console.Write("Too many parameters!");
                        DisplayUsage();
                        Environment.Exit(0);
                    }
                }
            }
            return args[args.Length - 1].ToLower();
        }

        static string StripParamName(string param)
        {
            return param.Substring(param.IndexOf('=') + 1);
        }

        static void ParseDataLines(string data)
        {
            try
            {
                string[] dataBits = data.Split(',');
                if (dataBits.Length != 8)
                {
                    throw new Exception("Need to specify 8 data bits when using -d or -data");
                }
                byteOrder = new byte[dataBits.Length];
                for (int i = 0; i < dataBits.Length; i++)
                {
                    byteOrder[i] = byte.Parse(dataBits[i]);
                }
            }
            catch (FormatException)
            {
                throw new Exception(String.Format("Could not parse data lines: {0}", data));
            }
        }

        static void ParseAddressLines(string address)
        {
            try
            {
                string[] addressBits = address.Split(',');
                addressOrder = new byte[addressBits.Length];
                for (int i = 0; i < addressBits.Length; i++)
                {
                    addressOrder[i] = byte.Parse(addressBits[i]);
                }
            }
            catch (FormatException)
            {
                throw new Exception(String.Format("Could not parse address lines: {0}", address));
            }
        }

        static void DisplayUsage()
        {
            Console.WriteLine("\nUsage: DecodeROM <options> <filename>\n");
            Console.WriteLine("Valid options are:");
            Console.WriteLine("-d, -data     : Specify data line ordering to use, e.g.");
            Console.WriteLine("                -d=1,3,5,7,0,2,4,6");
            Console.WriteLine("                Requires mapping for all 8 bits in data byte.");
            Console.WriteLine("-a, -address  : Specify address line ordering to use, e.g.");
            Console.WriteLine("                -a=1,0,3,2,5,4,7,6,8,7,10,9");
            Console.WriteLine("                Enough bits must be specified to cover address space");
            Console.WriteLine("                for the ROM image given.\n");
            Console.WriteLine("Decoded ROM image will be written to <filename>.out");
        }
    }
}
