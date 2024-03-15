using System;

namespace GoldSprite.MyUdpperAPI
{
    public abstract class ISerializer
    {
        public readonly static ISerializer DEFAULT = new JSONSerializer();

        public abstract byte SerializerAlgorithm { get; }
        public abstract byte[] Serialize(Object obj);
        public abstract object Deserialize(byte[] bytes, Type type);
    }
}