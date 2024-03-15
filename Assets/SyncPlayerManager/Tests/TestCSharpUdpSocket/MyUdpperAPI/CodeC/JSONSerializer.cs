using System;
using System.Text;

using Newtonsoft.Json;

namespace GoldSprite.MyUdpperAPI
{
    public class JSONSerializer : ISerializer
    {
        public override byte SerializerAlgorithm => ISerializerAlgorithm.JSON;


        public override object Deserialize(byte[] bytes, Type type)
        {
            string json = Encoding.UTF8.GetString(bytes);
            object obj = JsonConvert.DeserializeObject(json, type);
            return obj;
        }


        public override byte[] Serialize(Object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            return bytes;
        }
    }
}