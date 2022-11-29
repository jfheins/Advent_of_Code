using Core;

namespace AoC_2021.Days
{
    public class Day_16 : BaseDay
    {
        private long _versionSum;
        private long _value;

        public Day_16()
        {
            var input = File.ReadAllText(InputFilePath);
            (_versionSum, _value) = ParseSinglePacket(HexToBin(input));
        }

        public override async ValueTask<string> Solve_1()
        {
            return _versionSum.ToString();
        }

        public override async ValueTask<string> Solve_2()
        {
            return _value.ToString();
        }

        private (long versionSum, long value) ParseSinglePacket(string binaryString)
        {
            var (versionSum, value, _) = ParsePacket(binaryString.AsSpan());
            return (versionSum, value);
        }

        private (long versionSum, long value, int length) ParsePacket(ReadOnlySpan<char> binaryString)
        {
            var (version, typeId) = GetHeader(binaryString);
            long versionSum = version;
            int processedBits = 6;

            if (typeId == 4) // literal packet
            {
                long literalValue = 0;
                while (ReadNextGroup(binaryString, ref literalValue)) { }
                return (versionSum, literalValue, processedBits);
            }

            var subValues = new List<long>();
            var lengthTypeId = ReadBits(binaryString, 1);

            if (lengthTypeId == 0) // 15 bits are a number that represents the total length in bits
            {
                var lengthInBits = ReadBits(binaryString, 15);
                var endOffset = processedBits + lengthInBits;
                while (processedBits < endOffset)
                    ParseSubPacket(binaryString);
            }
            else // 11 bits are a number that represents the number of sub-packets immediately contained 
            {
                var subPacketCount = ReadBits(binaryString, 11);
                for (int i = 0; i < subPacketCount; i++)
                    ParseSubPacket(binaryString);
            }
            var operatorValue = CalculateValue(typeId, subValues);
            return (versionSum, operatorValue, processedBits);

            // Local helper methods
            uint ReadBits(ReadOnlySpan<char> str, int count)
            {
                var value = Bin2UInt(str.Slice(processedBits, count));
                processedBits += count;
                return value;
            }

            bool ReadNextGroup(ReadOnlySpan<char> str, ref long value)
            {
                var group = ReadBits(str, 5);
                value = (value << 4) | (group & 15);
                return group > 15;
            }

            void ParseSubPacket(ReadOnlySpan<char> str)
            {
                var (vSum, val, len) = ParsePacket(str[processedBits..]);
                versionSum += vSum;
                subValues?.Add(val);
                processedBits += len;
            }
        }

        private static long CalculateValue(uint typeId, IReadOnlyList<long> subValues) => typeId switch
        {
            0 => subValues.Sum(),
            1 => subValues.Product(),
            2 => subValues.Min(),
            3 => subValues.Max(),
            5 => subValues[0] > subValues[1] ? 1 : 0,
            6 => subValues[0] < subValues[1] ? 1 : 0,
            7 => subValues[0] == subValues[1] ? 1 : 0,
            _ => throw new NotImplementedException(),
        };

        private (uint version, uint typeId) GetHeader(ReadOnlySpan<char> bin)
        {
            var version = Bin2UInt(bin[..3]);
            var packetTypeId = Bin2UInt(bin[3..6]);
            return (version, packetTypeId);
        }

        private static string HexToBin(string s)
            => string.Concat(s.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));

        private static uint Bin2UInt(ReadOnlySpan<char> str)
        {
            return Convert.ToUInt32(str.ToString(), 2);
        }
    }
}
