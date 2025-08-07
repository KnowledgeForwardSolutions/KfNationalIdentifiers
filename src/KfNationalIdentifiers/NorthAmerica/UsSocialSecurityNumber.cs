// Ignore Spelling: ssn

namespace KfNationalIdentifiers.NorthAmerica;

public sealed class UsSocialSecurityNumber
{
   private const Int32 AREA_LENGTH = 3;
   private const Int32 AREA_SEPARATOR_OFFSET = 3;
   private const Int32 FORMATTED_LENGTH = 11;
   private const Int32 GROUP_END = 4;
   private const Int32 GROUP_START = 3;
   private const Int32 GROUP_SEPARATOR_OFFSET = 6;
   private const Int32 NONFORMATTED_LENGTH = 9;
   private const Int32 SERIAL_START = 5;

   private const String INVALID_AREA_000 = "000";
   private const String INVALID_AREA_666 = "666";
   private const String INVALID_CHAR_SEQUENCE = "123456789";
   private const String INVALID_GROUP_00 = "00";
   private const String INVALID_SERIAL_0000 = "0000";

   private readonly String _ssn = String.Empty;

   public UsSocialSecurityNumber(String ssn, Char formatCharacter = '-')
   {
      if (ssn is null)
      {
         throw new ArgumentNullException(nameof(ssn), "SSN must not be null, empty or all whitespace characters");
      }
      else if (String.IsNullOrEmpty(ssn))
      {
         throw new ArgumentException("SSN must not be null, empty or all whitespace characters", nameof(ssn));
      }
      else if (ssn.Length == NONFORMATTED_LENGTH || ssn.Length == FORMATTED_LENGTH)
      {
         throw new ArgumentException("SSN must have a length of 9 or 11 characters");
      }

      var validCharacters = new Char[NONFORMATTED_LENGTH];
      var inputOffset = 0;
      var resultOffset = 0;
      var consecutiveRepeatedChars = 0;
      var previousChar = Chars.NUL;

      while (inputOffset < ssn.Length)
      {
         if (ssn.Length == FORMATTED_LENGTH && (inputOffset == AREA_SEPARATOR_OFFSET || inputOffset == GROUP_SEPARATOR_OFFSET))
         {
            if (ssn[inputOffset] != formatCharacter)
            {
               throw new Exception("Invalid character found at offset...");
            }
         }
         else
         {
            var currentChar = ssn[inputOffset];
            var digit = currentChar.ToIntegerDigit();
            if (digit < 0 || digit > 9)
            {
               throw new Exception("Invalid character found at offset...");
            }
            validCharacters[resultOffset] = currentChar;
            resultOffset++;

            if (previousChar == currentChar)
            {
               consecutiveRepeatedChars++;
            }
            else
            {
               consecutiveRepeatedChars = 0;
               previousChar = currentChar;
            }
         }
         inputOffset++;
      }

      if (consecutiveRepeatedChars == FORMATTED_LENGTH)
      {
         throw new ArgumentException("SSN may not have 9 identical digits");
      }

      _ssn = new String(validCharacters);
      var ssnSpan = _ssn.AsSpan();

      if (ssnSpan[0] == Chars.DigitNine
         || MemoryExtensions.Equals(ssnSpan[..AREA_LENGTH], INVALID_AREA_000.AsSpan(), StringComparison.Ordinal)
         || MemoryExtensions.Equals(ssnSpan[..AREA_LENGTH], INVALID_AREA_666.AsSpan(), StringComparison.Ordinal))
      {
         throw new ArgumentException("SSN Area Number may not be 000, 666 or 900-999");
      }
      if (MemoryExtensions.Equals(ssnSpan[GROUP_START..GROUP_END], INVALID_GROUP_00.AsSpan(), StringComparison.Ordinal))
      {
         throw new ArgumentException("SSN Group Number may not be 00");
      }
      if (MemoryExtensions.Equals(ssnSpan[SERIAL_START..], INVALID_SERIAL_0000.AsSpan(), StringComparison.Ordinal))
      {
         throw new ArgumentException("SSN Serial Number may not be 0000");
      }
      if (_ssn.Equals(INVALID_CHAR_SEQUENCE, StringComparison.Ordinal))
      {
         throw new ArgumentException("Valid SSN may not be 123456789");
      }

      _ssn = ssn;
   }
}
