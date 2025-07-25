namespace WelpenScoreboard.Domain.Common;

public class BcdConverters
{
    public static int BcdToInt(byte[] data)
    {
        int result = 0;
        for (int i = 0; i < data.Length; i++)
        {
            result *= 100;
            result += BcdPartToInt(data[i]);
        }
        return result;
    }

    public static uint BcdToUint(byte[] data) => (uint)BcdToInt(data);

    public static string BcdToString(byte[] data)
    {
        int buf;
        string result = string.Empty;

        for (int i = 0; i < data.Length; i++)
        {
            buf = BcdPartToInt(data[i]);
            result += buf.ToString("D2");
        }

        return result.TrimStart('0');
    }

    public static byte[] IntToBcd(int val, int arrayLength)
    {
        int bcd = 0;
        byte[] retData;
        for (int digit = 0; digit < arrayLength * 2; ++digit)
        {
            int nibble = val % 10;
            bcd |= nibble << (digit * 4);
            val /= 10;
        }
        retData = new byte[arrayLength];
        for (int i = 0; i < retData.Length; i++)
        {
            retData[i] = (byte)((bcd >> (i * 8)) & 0xff);
        }
        return retData;
    }

    public static byte[] UIntToBcd(uint val, int arrayLength)
    {
        uint bcd = 0;
        byte[] retData;
        for (int digit = 0; digit < arrayLength * 2; ++digit)
        {
            uint nibble = val % 10;
            bcd |= nibble << (digit * 4);
            val /= 10;
        }
        retData = new byte[arrayLength];
        for (int i = 0; i < retData.Length; i++)
        {
            retData[arrayLength - i - 1] = (byte)((bcd >> (i * 8)) & 0xff);
        }
        return retData;
    }

    public static int BcdPartToInt(byte data) => (10 * (data >> 4)) + (data & 0x0f);
}
