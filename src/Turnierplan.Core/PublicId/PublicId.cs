using System.Security.Cryptography;

namespace Turnierplan.Core.PublicId;

public readonly record struct PublicId
{
    public static readonly PublicId Empty = 0;

    private static readonly List<char> __encodingAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-".ToList();

    /// <summary>
    /// Creates a new public ID instance from the specified value.
    /// </summary>
    public PublicId(ulong value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new public ID instance from the specified value.
    /// </summary>
    public PublicId(long value)
    {
        Value = unchecked((ulong)value);
    }

    /// <summary>
    /// Creates a new public ID instance and parses the specified string representation. The string representation is
    /// a base-64 encoded string with a custom format which contains the 8 bytes of data for the public ID.
    /// </summary>
    public PublicId(string representation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(representation);

        if (representation.Length != 11)
        {
            throw new ArgumentException("String representation must be 11 characters long.", nameof(representation));
        }

        Value = 0;

        for (var i = 0; i < representation.Length; i++)
        {
            var j = __encodingAlphabet.IndexOf(representation[i]);

            if (j == -1 || (i == 0 && j > 0b001111))
            {
                throw new ArgumentException($"String representation contains invalid character at index {i}.", nameof(representation));
            }

            Value |= (ulong)j << ((10 - i) * 6);
        }
    }

    /// <summary>
    /// Creates a new public ID instance using the specified eight bytes.
    /// </summary>
    public PublicId(byte[] bytes)
    {
        if (bytes.Length != 8)
        {
            throw new ArgumentException("Byte array must be 8 bytes long.", nameof(bytes));
        }

        Value = ConvertFromBytes(bytes);
    }

    /// <summary>
    /// Creates a new public ID instance with a pseudo-random value.
    /// </summary>
    public PublicId()
    {
        Value = ConvertFromBytes(RandomNumberGenerator.GetBytes(8));
    }

    /// <summary>
    /// The value of this public ID instance.
    /// </summary>
    public ulong Value { get; }

    /// <summary>
    /// Returns the string representation of this public ID which contains the 8-byte integer in a custom base-64 format.
    /// </summary>
    public override string ToString()
    {
        var characters = new[]
        {
            __encodingAlphabet[(int)((Value >> 60) & 0b001111)],
            __encodingAlphabet[(int)((Value >> 54) & 0b111111)],
            __encodingAlphabet[(int)((Value >> 48) & 0b111111)],
            __encodingAlphabet[(int)((Value >> 42) & 0b111111)],
            __encodingAlphabet[(int)((Value >> 36) & 0b111111)],
            __encodingAlphabet[(int)((Value >> 30) & 0b111111)],
            __encodingAlphabet[(int)((Value >> 24) & 0b111111)],
            __encodingAlphabet[(int)((Value >> 18) & 0b111111)],
            __encodingAlphabet[(int)((Value >> 12) & 0b111111)],
            __encodingAlphabet[(int)((Value >> 06) & 0b111111)],
            __encodingAlphabet[(int)((Value >> 00) & 0b111111)]
        };

        return new string(characters);
    }

    /// <summary>
    /// Returns the numeric value of this public ID instance as a signed long instead of unsigned (<see cref="Value"/>).
    /// This is achieved using an unchecked explicit cast.
    /// </summary>
    public long ToSignedInt64()
    {
        return unchecked((long)Value);
    }

    public static bool TryParse(string? representation, out PublicId publicId)
    {
        if (string.IsNullOrWhiteSpace(representation))
        {
            publicId = Empty;
            return false;
        }

        try
        {
            publicId = new PublicId(representation);
            return true;
        }
        catch (ArgumentException)
        {
            publicId = Empty;
            return false;
        }
    }

    private static ulong ConvertFromBytes(byte[] bytes)
    {
        return bytes[0]
               | ((ulong)bytes[1] << 8)
               | ((ulong)bytes[2] << 16)
               | ((ulong)bytes[3] << 24)
               | ((ulong)bytes[4] << 32)
               | ((ulong)bytes[5] << 40)
               | ((ulong)bytes[6] << 48)
               | ((ulong)bytes[7] << 56);
    }

    public static implicit operator PublicId(ulong value) => new(value);
}
