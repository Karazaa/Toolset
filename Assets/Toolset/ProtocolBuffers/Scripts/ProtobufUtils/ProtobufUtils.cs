using System;
using System.IO;
using ProtoBuf;

namespace Toolset.ProtocolBuffers
{
    /// <summary>
    /// Static utility class for using Protocol Buffers
    /// </summary>
    public static class ProtoBufUtils
    {
        /// <summary>
        /// Gets whether or not the passed in type is ProtoBuf serializable.
        /// </summary>
        /// <param name="T">The type to check.</param>
        /// <returns>Whether or not the passed in type is serializable.</returns>
        public static bool IsSerializableProtoBuf(Type T)
        {
            if (T == null)
                return false;

            return T is IExtensible || T.IsDefined(typeof(ProtoContractAttribute), true);
        }

        /// <summary>
        /// Uses a MemoryStream to serialize a ProtoBuf serializable object to 
        /// a byte array.
        /// </summary>
        /// <typeparam name="T">The type of object being serialized</typeparam>
        /// <param name="instance">The object instance to be serialized.</param>
        /// <returns>A byte array that contains the serialized data of the passed instance.</returns>
        public static byte[] Serialize<T>(T instance) where T : class
        {
            if (instance == null || !IsSerializableProtoBuf(typeof(T)))
                return null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, instance);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Uses a MemoryStream and a passed byte array to deserialize
        /// bytes into a C# class instance.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize</typeparam>
        /// <param name="data">The byte data to Desetialize from.</param>
        /// <returns>A new instance of the class T.</returns>
        public static T Deserialize<T>(byte[] data) where T : class
        {
            if (data == null || !IsSerializableProtoBuf(typeof(T)))
                return null;

            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(memoryStream);
            }
        }
        
        
    }
}

