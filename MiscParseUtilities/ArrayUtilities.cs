using System;
using System.Collections.Generic;

namespace MiscParseUtilities
{
    public static class ArrayUtilities
    {
        // create a subset from a range of indices
        public static T[] RangeSubset<T>(this T[] array, int startIndex, int length)
        {
            T[] subset = new T[length];
            Array.Copy(array, startIndex, subset, 0, length);
            return subset;
        }

        // create a subset from a specific list of indices
        public static T[] Subset<T>(this T[] array, params int[] indices)
        {
            T[] subset = new T[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                subset[i] = array[indices[i]];
            }
            return subset;
        }

        //this exists so byte arrays can be used as keys in dictionaries
        //http://stackoverflow.com/questions/1440392/use-byte-as-key-in-dictionary
        public class ByteArrayComparer : IEqualityComparer<byte[]>
        {
            public bool Equals(byte[] left, byte[] right)
            {
                if (left == null || right == null)
                    return left == right;
                
                if (left.Length != right.Length)
                    return false;

                for (int i = 0; i < left.Length; i++)
                    if (left[i] != right[i])
                        return false;

                return true;
            }

            public int GetHashCode(byte[] key)
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                int sum = 0;
                foreach (byte cur in key)
                    sum += cur;
                return sum;
            }
        }
    }
}