/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using System;
using Windows.Storage;
using Windows.Storage.Streams;

namespace GrowOne.Core
{
    public class InternalStorage
    {
        private const string TemporaryWriteFileBufferName = "tempfile.tmp";

        public static void Save(object instance, string fileName, bool includeChecksum = true)
        {
            StorageFile instanceFile;
            byte[] instanceBuffer;
            try
            {
                instanceFile = GetInternalStorageFolder().CreateFile(TemporaryWriteFileBufferName,
                    CreationCollisionOption.ReplaceExisting);
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

            try
            {
                if (Exists(fileName, out var existingFile))
                {
                    existingFile.Delete();
                    instanceFile.Rename(fileName);
                }
            }
            catch (Exception exc)
            {
                throw new Exception("The changes couldn't be applied to the file system.", exc);
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
