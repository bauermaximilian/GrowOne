/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using System;
using Windows.Storage;
using Windows.Storage.Streams;

namespace GrowOne.Common
{
    public class InternalStorage
    {
        public static void Save(object instance, string fileName, bool overwrite, 
            bool includeChecksum = true)
        {
            StorageFile instanceFile;
            byte[] instanceBuffer;
            try
            {
                instanceFile = GetInternalStorageFolder().CreateFile(fileName, overwrite ?
                    CreationCollisionOption.ReplaceExisting : CreationCollisionOption.FailIfExists);
            }
            catch (Exception exc)
            {
                throw new Exception("The file couldn't be created.", exc);
            }

            try
            {
                instanceBuffer = ByteSerializer.Serialize(instance, includeChecksum);
            }
            catch (Exception exc)
            {
                throw new Exception("The object instance couldn't be serialized.", exc);
            }

            try
            {
                FileIO.WriteBytes(instanceFile, instanceBuffer);
            }
            catch (Exception exc)
            {
                throw new Exception("The serialized object instance couldn't be saved " +
                    "to the file.", exc);
            }
        }

        public static bool Exists(string fileName, out StorageFile file)
        {
            file = null!;
            StorageFile[] files = GetInternalStorageFolder().GetFiles();
            foreach (StorageFile fileCandidate in files)
            {
                if (fileCandidate.Name == fileName)
                {
                    file = fileCandidate;
                    return true;
                }
            }
            return false;
        }

        public static object Load(string fileName, Type targetType, bool expectChecksum = true)
        {
            if (Exists(fileName, out StorageFile instanceFile))
            {
                byte[] instanceBuffer;
                try
                {
                    var instanceFileBuffer = FileIO.ReadBuffer(instanceFile);
                    instanceBuffer = new byte[instanceFileBuffer.Length];
                    DataReader.FromBuffer(instanceFileBuffer).ReadBytes(instanceBuffer);
                }
                catch (Exception exc)
                {
                    throw new Exception("The file couldn't be read.", exc);
                }

                try
                {
                    int index = 0;
                    return ByteSerializer.Deserialize(instanceBuffer, ref index, targetType, 
                        expectChecksum);
                }
                catch (Exception exc)
                {
                    throw new Exception("The file couldn't be deserialized.", exc);
                }
            }
            else
            {
                throw new Exception("The specified file couldn't be found.");
            }
        }

        private static StorageFolder GetInternalStorageFolder()
        {
            StorageFolder[] folders = KnownFolders.InternalDevices.GetFolders();
            if (folders.Length > 0)
            {
                return folders[0];
            }
            else
            {
                throw new InvalidOperationException("The internal storage isn't available.");
            }
        }
    }
}
