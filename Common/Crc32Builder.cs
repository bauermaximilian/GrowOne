/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using System;
using System.IO;

namespace GrowOne.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// This class is based on the implementation provided by StackOverflow user "spludlow" under 
    /// the following URL: https://stackoverflow.com/a/61495753/13223033 (retrieved on 2022-07-23).
    /// </remarks>
    public class Crc32Builder
    {
        public const int HashLength = sizeof(uint);

        private const uint polynomial = 0xEDB88320;
        
        private readonly uint[] checksumTable;
        private readonly byte[] lastHash = new byte[HashLength];
        private bool lastHashIsUpToDate = false;
        private uint currentHashValue;

        public byte[] Hash
        {
            get
            {
                if (!lastHashIsUpToDate)
                {
                    byte[] hashReverse = BitConverter.GetBytes(~currentHashValue);
                    for (int i = hashReverse.Length - 1, j = 0; i >= 0; --i, j++)
                        lastHash[j] = hashReverse[i];
                    lastHashIsUpToDate = true;
                }

                return lastHash;
            }
        }

        public Crc32Builder()
        {
            checksumTable = new uint[0x100];

            for (uint index = 0; index < 0x100; ++index)
            {
                uint item = index;
                for (int bit = 0; bit < 8; ++bit)
                    item = ((item & 1) != 0) ? (polynomial ^ (item >> 1)) : (item >> 1);
                checksumTable[index] = item;
            }

            Reset();
        }

        public void Reset()
        {
            currentHashValue = 0xFFFFFFFF;
            lastHashIsUpToDate = false;
        }

        public void Add(byte[] values)
        {
            Add(values, 0, values.Length);
        }

        public void Add(byte[] values, int index, int count)
        {
            if (index < 0 || index > values.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0 || (index + count) > values.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            for (int i = index; i < count; ++i)
            {
                currentHashValue = checksumTable[(currentHashValue & 0xFF) ^ values[i]] ^
                    (currentHashValue >> 8);
            }
            lastHashIsUpToDate = false;
        }
    }
}
