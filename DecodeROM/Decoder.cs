using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DecodeROM
{
    public class Decoder
    {
        public void DecodeFile(string fileName, byte[] addressBitOrder, byte[] dataBitOrder)
        {
            FileInfo fInfo = new FileInfo(fileName);
            FileStream fs = new FileStream(fInfo.FullName, FileMode.Open);
            byte[] data = new byte[fInfo.Length];
            fs.Read(data, 0, (int)fInfo.Length);
            fs.Close();

            Console.WriteLine("Decoding " + fileName);
            byte[] output = Decode(data, addressBitOrder, dataBitOrder);

            FileStream ofs = new FileStream(fInfo.FullName + ".out", FileMode.Create);
            ofs.Write(output, 0, output.Length);
        }

        public byte[] Decode(byte[] input, byte[] addressBitOrder, byte[] dataBitOrder)
        {
            // Check that address bit order, if present, has the space available to 
            // map the file contents (2^abo.length - 1 >= input.length)

            if (addressBitOrder != null && ((Math.Pow(2,addressBitOrder.Length)) - 1 >= input.Length))
            {
                throw new Exception(String.Format("Not enough bits in address line ordering to map full address space\n(Can map {0}, but input is {1}",
                    (Math.Pow(2,addressBitOrder.Length)) - 1, input.Length));
            }
            byte[] output = new byte[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                int address = (addressBitOrder != null) ? ConstructDestinationAddr(i, addressBitOrder) : i;
                byte value = (dataBitOrder != null) ? ConstructByte(input[i], dataBitOrder) : input[i];
                output[address] = value;
            }
            return output;
        }

        int ConstructDestinationAddr(int addr, byte[] abo)
        {
            int newAddr = 0;
            int curBit;
            for (int i = 0; i < abo.Length; i++)
            {
                curBit = (addr % 2) << abo[i];
                newAddr |= curBit;
                addr >>= 1;
            }
            return newAddr;
        }

        byte ConstructByte(byte value, byte[] dbo)
        {
            byte newByte = 0;
            byte curBit;
            for (int i = 0; i < dbo.Length; i++)
            {
                curBit = (byte)((value % 2) << dbo[i]);
                newByte |= curBit;
                value >>= 1;
            }
            return newByte;
        }
    }
}
