using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace api.Utilities
{
    public static class StringUtil
    {
        public static string RemoveAllButLettersAndNumbers(this string input)
        {
            char[] arr = input.Where(c => (char.IsLetterOrDigit(c) ||
                             char.IsWhiteSpace(c))).ToArray();

            return new string(arr);
        }

        public static int TryParseInt(this string input, int defaultValue = 0)
        {
            int x = defaultValue;
            int.TryParse(input, out x);
            return x;
        }

        public static bool IsNullOrEmpty(this string input)
        {
            return string.IsNullOrEmpty(input);
        }

        public static bool IsNullOrWhiteSpace(this string input)
        {
            return string.IsNullOrWhiteSpace(input);
        }

        public static bool IsNotEmpty(this string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

        public static string TakeFirst(this string input, int chars)
        {
            return input.Substring(0, Math.Min(chars, input.Length));
        }

        public static string Sha512Hash(this string input)
        {
            var sha512 = new SHA512Managed();
            sha512.Initialize();
            var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(input));
            var hashedPasword = Encoding.UTF8.GetString(hash);
            return hashedPasword;
        }

        public static string RandomString(int length, string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(allowedChars).ToArray();

            // Guid.NewGuid and System.Random are not particularly random. By using a
            // cryptographically-secure random number generator, the caller is always
            // protected, regardless of use.
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < length)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < length; ++i)
                    {
                        // Divide the byte into allowedCharSet-sized groups. If the
                        // random value falls into the last group and the last group is
                        // too small to choose from the entire allowedCharSet, ignore
                        // the value in order to avoid biasing the result.
                        var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                        if (outOfRangeStart <= buf[i]) continue;
                        result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                    }
                }
                return result.ToString();
            }
        }

        public static string ToString(this Stream stream)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public static long GetMinDistance(this string source, string destination)
        {
            var distances = new EditDistanceCostsMap<long>(1, 1, 1);
            if (source == destination)
                return 0;

            // Dynamic Programming 3D Table
            long[,] dynamicTable = new long[source.Length + 1, destination.Length + 1];

            // Initialize table
            for (int i = 0; i <= source.Length; ++i)
                dynamicTable[i, 0] = i;

            for (int i = 0; i <= destination.Length; ++i)
                dynamicTable[0, i] = i;

            // Compute min edit distance cost
            for (int i = 1; i <= source.Length; ++i)
            {
                for (int j = 1; j <= destination.Length; ++j)
                {
                    if (source[i - 1] == destination[j - 1])
                    {
                        dynamicTable[i, j] = dynamicTable[i - 1, j - 1];
                    }
                    else
                    {
                        long insert = dynamicTable[i, j - 1] + distances.InsertionCost;
                        long delete = dynamicTable[i - 1, j] + distances.DeletionCost;
                        long substitute = dynamicTable[i - 1, j - 1] + distances.SubstitutionCost;

                        dynamicTable[i, j] = Math.Min(insert, Math.Min(delete, substitute));
                    }
                }
            }

            // Get min edit distance cost
            return dynamicTable[source.Length, destination.Length];
        }

    }

    class EditDistanceCostsMap<TCost> where TCost : IComparable<TCost>, IEquatable<TCost>
    {
        public TCost DeletionCost { get; set; }
        public TCost InsertionCost { get; set; }
        public TCost SubstitutionCost { get; set; }

        public EditDistanceCostsMap(TCost insertionCost, TCost deletionCost, TCost substitutionCost)
        {
            DeletionCost = deletionCost;
            InsertionCost = insertionCost;
            SubstitutionCost = substitutionCost;
        }
    }
}
