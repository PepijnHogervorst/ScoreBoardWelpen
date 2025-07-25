using System.Text;

namespace WelpenScoreboard.Domain.Common.Extensions;
public static class StringExtensions
{
    private static readonly char[] _digitsAndWhitespace = [' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

    public static string FirstCharToUpper(this string input)
    {
        return input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
        };
    }

    /// <summary>
    /// Trims whitespace and digits from end of the string
    /// </summary>
    public static string TrimEndDigitsAndWhitespace(this string input)
    {
        return input.TrimEnd(_digitsAndWhitespace);
    }

    /// <summary>
    /// Trims found digits from end of the string and returns the number
    /// </summary>
    public static string TrimEndDigits(this string input, out int valueAtEnd)
    {
        valueAtEnd = 0;
        string trimmedInput = input.TrimEnd();
        int pointer = trimmedInput.Length;
        while (pointer > 0)
        {
            char charAtPointer = trimmedInput[pointer - 1];
            if (!char.IsDigit(charAtPointer)) break;

            // add digit to count
            valueAtEnd += (int)char.GetNumericValue(charAtPointer) * (int)Math.Pow(10, trimmedInput.Length - pointer);
            pointer--;
        }

        return trimmedInput[..pointer];
    }

    /// <summary>
    /// Removes ALL whitespace from the string
    /// </summary>
    public static string RemoveWhitespace(this string input)
        => new([.. input.Where(c => !char.IsWhiteSpace(c))]);

    /// <summary>
    /// Limits string to given length. 
    /// If too long cuts off string -2 of length and adds ... 
    /// E.g.: 'My string should be shortened' with length 10 => 'My string..'
    /// </summary>
    public static string LimitLength(this string input, int length)
    {
        if (input.Length > length) return $"{input[..(length - 2)]}..";
        return input;
    }

    /// <summary>
    /// Offsets the string with given offset
    /// If a number is present at end of string add offset to that
    /// Also add space if not yet present
    /// E.g.: MyCount w. offset 2 => MyCount 2
    /// </summary>
    public static string AddOffset(this string input, int offset)
    {
        if (offset == 0) return input;

        StringBuilder sb = new(input.TrimEndDigits(out int number));

        if (offset > 0 || number > 0)
        {
            if (sb.Length > 0 && sb[^1] != ' ' && number == 0) sb.Append(' ');
            sb.Append(number + offset);
        }

        return sb.ToString();
    }
}
