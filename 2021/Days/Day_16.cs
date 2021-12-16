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

        private (long versionSum, int length) ParsePacket(ReadOnlySpan<char> binaryString)
        {
            var header = GetHeader(binaryString);
            long versionSum = header.version;
            int readBits = 6;

            if (header.typeId == 4) // literal packet
            {
                while (true)
                {
                    var group = Bin2Int(binaryString[readBits..(readBits + 5)]);
                    readBits += 5;
                    if (group < 16) // Final
                        break;
                }
                return (versionSum, readBits);
            }
            else
            {
                var lengthTypeId = binaryString[6];
                readBits += 1;

                if (lengthTypeId == '0') // 15 bits are a number that represents the total length in bits
                {
                    var lengthInBits = Bin2Int(binaryString[7..22]);
                    readBits += 15;
                    while (readBits < 22 + lengthInBits)
                    {
                        var subpacket = ParsePacket(binaryString[readBits..]);
                        versionSum += subpacket.versionSum;
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
                        readBits += subpacket.length;
                    }
                }
            }
            return (versionSum, readBits);
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

        public override async ValueTask<string> Solve_2()
        {
            return "";
        }
    }
}
