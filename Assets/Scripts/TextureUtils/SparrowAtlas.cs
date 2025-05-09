using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace TextureUtils
{
    public static class SparrowAtlas
    {
        public static TextureAtlas Deserialize(TextAsset textAsset)
        {
            return Deserialize(textAsset.text);
        }
        
        public static TextureAtlas Deserialize(string text)
        {
            var serializer = new XmlSerializer(typeof(TextureAtlas));
            using var reader = new StringReader(text);
            return (TextureAtlas)serializer.Deserialize(reader);
        }
    }
}