using Core;
using Core.Combinatorics;
using MoreLinq.Extensions;
using System.IO;
using System.Linq;

namespace AoC_2021.Days
{
    public class Day_16 : BaseDay
    {
        private string _input;

        public Day_16()
        {
            _input = File.ReadAllText(InputFilePath);
        }

        public override ValueTask<string> Solve_1()
        {
            var bin = HexToBin(_input).AsSpan();
            var first = ParsePacket(bin);

            return ValueTask.FromResult(first.versionSum.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            var bin = HexToBin(_input).AsSpan();
            var first = ParsePacket(bin);

            return ValueTask.FromResult(first.value.ToString());
        }

        private (long versionSum, long value, int length) ParsePacket(ReadOnlySpan<char> binaryString)
        {
            var header = GetHeader(binaryString);
            long versionSum = header.version;
            int readBits = 6;

            if (header.typeId == 4) // literal packet
            {
                long value = 0;
                while (true)
                {
                    var group = Bin2Int(binaryString[readBits..(readBits + 5)]);
                    readBits += 5;
                    value = (value << 4) | (group & 15);
                    if (group < 16) // Final
                        break;
                }
                return (versionSum, value, readBits);
            }
            else
            {
                var lengthTypeId = binaryString[6];
                readBits += 1;
                var subValues = new List<long>();

                if (lengthTypeId == '0') // 15 bits are a number that represents the total length in bits
                {
                    var lengthInBits = Bin2Int(binaryString[7..22]);
                    readBits += 15;
                    while (readBits < 22 + lengthInBits)
                    {
                        var subpacket = ParsePacket(binaryString[readBits..]);
                        versionSum += subpacket.versionSum;
                        subValues.Add(subpacket.value);
                        readBits += subpacket.length;
                    }
                }
                else // 11 bits are a number that represents the number of sub-packets immediately contained 
                {
                    var subPacketCount = Bin2Int(binaryString[7..18]);
                    readBits += 11;
                    for (int i = 0; i < subPacketCount; i++)
                    {
                        var subpacket = ParsePacket(binaryString[readBits..]);
                        versionSum += subpacket.versionSum;
                        subValues.Add(subpacket.value);
                        readBits += subpacket.length;
                    }
                }
                long value = 0;
                switch (header.typeId)
                {
                    case 0:
                        value = subValues.Sum(); 
                        break;
                    case 1:
                        value = subValues.Product();
                        break;
                    case 2:
                        value = subValues.Min();
                        break;
                    case 3:
                        value = subValues.Max();
                        break;
                    case 5:
                        value = subValues[0] > subValues[1] ? 1 : 0;
                        break;
                    case 6:
                        value = subValues[0] < subValues[1] ? 1 : 0;
                        break;
                    case 7:
                        value = subValues[0] == subValues[1] ? 1 : 0;
                        break;
                }
                return (versionSum, value, readBits);
            }
        }

        private (int version, int typeId) GetHeader(ReadOnlySpan<char> bin)
        {
            var version = Bin2Int(bin[..3]);
            var packetTypeId = Bin2Int(bin[3..6]);
            return (version, packetTypeId);
        }

        private string HexToBin(string v)
        {
            return string.Join(string.Empty,
              v.Select(
                c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
              )
            );
        }
        private static int Bin2Int(ReadOnlySpan<char> v)
        {
            return Convert.ToInt32(v.ToString(), 2);
        }
    }
}
