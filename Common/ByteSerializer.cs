/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using System;
using System.Collections;
using System.Reflection;
using System.Text;
using BC = System.BitConverter;

namespace GrowOne.Common
{
    public static class ByteSerializer
    {
        public static byte[] Serialize(object instance, bool includeChecksum = true)
        {
            ArrayList buffers = new();
            int totalBufferSize = includeChecksum ? Crc32Builder.HashLength : 0;

            Type instanceType = instance.GetType();
            foreach (var method in instanceType.GetMethods())
            {
                string methodName = method.Name;
                if (TryGetPropertyType(method, out Type propertyType, out bool isSetter) &&
                    !isSetter)
                {
                    object propertyValue = method.Invoke(instance, null);
                    if (TrySerializeValue(propertyValue, propertyType, out byte[] buffer))
                    {
                        buffers.Add(buffer);
                        totalBufferSize += buffer.Length;
                    }
                    else
                    {
                        buffer = Serialize(propertyValue, false);
                        buffers.Add(buffer);
                        totalBufferSize += buffer.Length;
                    }
                }
            }

            foreach (var field in instanceType.GetFields())
            {
                object propertyValue = field.GetValue(instance);
                if (TrySerializeValue(propertyValue, field.FieldType, out byte[] buffer))
                {
                    buffers.Add(buffer);
                    totalBufferSize += buffer.Length;
                }
                else
                {
                    buffer = Serialize(propertyValue, false);
                    buffers.Add(buffer);
                    totalBufferSize += buffer.Length;
                }
            }

            Crc32Builder? crcBuilder = includeChecksum ? new() : null;
            byte[] combinedBuffer = new byte[totalBufferSize];
            int index = 0; 
            foreach (byte[] buffer in buffers)
            {
                Array.Copy(buffer, 0, combinedBuffer, index, buffer.Length);
                crcBuilder?.Add(buffer);
                index += buffer.Length;
            }

            if (crcBuilder != null)
            {
                Array.Copy(crcBuilder.Hash, 0, combinedBuffer, index, Crc32Builder.HashLength);
            }

            return combinedBuffer;
        }

        public static object Deserialize(byte[] buffer, ref int index, Type instanceType,
            bool expectChecksum = true)
        {
            if (expectChecksum)
            {
                int hashStartIndex = buffer.Length - Crc32Builder.HashLength;
                Crc32Builder crc32Builder = new();
                crc32Builder.Add(buffer, 0, hashStartIndex);
                for (int i = hashStartIndex, j = 0; i < buffer.Length; i++, j++)
                {
                    if (buffer[i] != crc32Builder.Hash[j])
                    {
                        throw new Exception("The checksum of the file was invalid.");
                    }
                }
            }            

            object instance = instanceType.GetConstructor(new Type[0]).Invoke(null);
            
            foreach(var method in instanceType.GetMethods())
            {
                if (TryGetPropertyType(method, out Type propertyType, out bool isSetter) && 
                    isSetter)
                {
                    if (TryDeserializeValue(propertyType, buffer, ref index, out var value))
                    {
                        method.Invoke(instance, new object[] { value! });
                    }
                    else
                    {
                        value = Deserialize(buffer, ref index, propertyType, false);
                        method.Invoke(instance, new object[] { value });
                    }
                }
            }

            foreach (var field in instanceType.GetFields())
            {
                if (TryDeserializeValue(field.FieldType, buffer, ref index, out var value))
                {
                    field.SetValue(instance, new object[] { value! });
                }
                else
                {
                    value = Deserialize(buffer, ref index, field.FieldType, false);
                    field.SetValue(instance, new object[] { value });
                }
            }



            return instance;
        }

        private static bool TrySerializeValue(object value, Type valueType, out byte[] buffer)
        {
            if (value == null)
            {
                buffer = BC.GetBytes(-1);
            }
            else if (valueType == typeof(bool))
            {
                buffer = BC.GetBytes((bool)value);
            }
            else if (valueType == typeof(byte))
            {
                buffer = BC.GetBytes((byte)value);
            }
            else if (valueType == typeof(sbyte))
            {
                buffer = BC.GetBytes((sbyte)value);
            }
            else if (valueType == typeof(char))
            {
                buffer = BC.GetBytes((char)value);
            }
            else if (valueType == typeof(double))
            {
                buffer = BC.GetBytes((double)value);
            }
            else if (valueType == typeof(float))
            {
                buffer = BC.GetBytes((float)value);
            }
            else if (valueType == typeof(int))
            {
                buffer = BC.GetBytes((int)value);
            }
            else if (valueType == typeof(uint))
            {
                buffer = BC.GetBytes((uint)value);
            }
            else if (valueType == typeof(long))
            {
                buffer = BC.GetBytes((long)value);
            }
            else if (valueType == typeof(ulong))
            {
                buffer = BC.GetBytes((ulong)value);
            }
            else if (valueType == typeof(short))
            {
                buffer = BC.GetBytes((short)value);
            }
            else if (valueType == typeof(ushort))
            {
                buffer = BC.GetBytes((ushort)value);
            }
            else if (valueType == typeof(byte[]))
            {
                byte[] valueBuffer = (byte[])value;
                buffer = new byte[sizeof(int) + valueBuffer.Length];
                byte[] lengthPrefixBuffer = BC.GetBytes(valueBuffer.Length);
                Array.Copy(lengthPrefixBuffer, 0, buffer, 0, lengthPrefixBuffer.Length);
                Array.Copy(valueBuffer, 0, buffer, lengthPrefixBuffer.Length, valueBuffer.Length);
            }
            else if (valueType == typeof(string))
            {
                byte[] valueBuffer = Encoding.UTF8.GetBytes((string)value);
                buffer = new byte[sizeof(int) + valueBuffer.Length];
                byte[] lengthPrefixBuffer = BC.GetBytes(valueBuffer.Length);
                Array.Copy(lengthPrefixBuffer, 0, buffer, 0, lengthPrefixBuffer.Length);
                Array.Copy(valueBuffer, 0, buffer, lengthPrefixBuffer.Length, valueBuffer.Length);
            }
            else
            {
                buffer = null!;
                return false;
            }

            return true;
        }

        private static bool TryDeserializeValue(Type type, byte[] buffer, ref int index, 
            out object? value)
        {
            if (type == typeof(bool))
            {
                value = BC.ToBoolean(buffer, index);
                index += sizeof(bool);
            }
            else if (type == typeof(byte))
            {
                value = buffer[index];
                index += sizeof(byte);
            }
            else if (type == typeof(sbyte))
            {
                value = (sbyte)buffer[index];
                index += sizeof(sbyte);
            }
            else if (type == typeof(char))
            {
                value = BC.ToChar(buffer, index);
                index += sizeof(char);
            }
            else if (type == typeof(double))
            {
                value = BC.ToDouble(buffer, index);
                index += sizeof(double);
            }
            else if (type == typeof(float))
            {
                value = BC.ToSingle(buffer, index);
                index += sizeof(float);
            }
            else if (type == typeof(int))
            {
                value = BC.ToInt32(buffer, index);
                index += sizeof(int);
            }
            else if (type == typeof(uint))
            {
                value = BC.ToUInt32(buffer, index);
                index += sizeof(uint);
            }
            else if (type == typeof(long))
            {
                value = BC.ToInt64(buffer, index);
                index += sizeof(long);
            }
            else if (type == typeof(ulong))
            {
                value = BC.ToUInt64(buffer, index);
                index += sizeof(ulong);
            }
            else if (type == typeof(short))
            {
                value = BC.ToInt16(buffer, index);
                index += sizeof(short);
            }
            else if (type == typeof(ushort))
            {
                value = BC.ToUInt16(buffer, index);
                index += sizeof(ushort);
            }
            else if (type == typeof(byte[]))
            {
                int valueLength = BC.ToInt32(buffer, index);
                if (valueLength < 0)
                {
                    value = null;
                }
                else
                {
                    index += sizeof(int);
                    byte[] valueData = new byte[valueLength];
                    Array.Copy(buffer, index, valueData, 0, valueLength);
                    index += valueLength;
                    value = valueData;
                }
            }
            else if (type == typeof(string))
            {
                int valueLength = BC.ToInt32(buffer, index);
                if (valueLength < 0)
                {
                    value = null;
                }
                else
                {
                    index += sizeof(int);
                    string valueData = Encoding.UTF8.GetString(buffer, index, valueLength);
                    index += valueLength;
                    value = valueData;
                }
            }
            else
            {
                value = null!;
                return false;
            }

            return true;
        }

        private static bool TryGetPropertyType(MethodInfo propertyGetterOrSetter, 
            out Type type, out bool isSetter)
        {
            const string GetterPrefix = "get_";
            const string SetterPrefix = "set_";

            type = null!;
            isSetter = false;
            if (propertyGetterOrSetter != null)
            {
                if (propertyGetterOrSetter.Name.StartsWith(GetterPrefix))
                {
                    var matchingSetter =
                        propertyGetterOrSetter.DeclaringType.GetMethod(SetterPrefix +
                        propertyGetterOrSetter.Name.Substring(GetterPrefix.Length));
                    
                    if (matchingSetter != null)
                    {
                        type = propertyGetterOrSetter.ReturnType;
                        isSetter = false;
                    }
                }
                else if (propertyGetterOrSetter.Name.StartsWith(SetterPrefix))
                {
                    var matchingGetter =
                        propertyGetterOrSetter.DeclaringType.GetMethod(GetterPrefix +
                        propertyGetterOrSetter.Name.Substring(SetterPrefix.Length));

                    if (matchingGetter != null)
                    {
                        var parameters = propertyGetterOrSetter.GetParameters();
                        if (parameters.Length == 1)
                        {
                            type = parameters[0].ParameterType;
                            isSetter = true;
                        }
                    }
                }
            }

            if (type == null) return false;
            else return true;
        }
    }
}
