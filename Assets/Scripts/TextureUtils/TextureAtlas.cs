using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TextureUtils
{
    [Serializable]
    [XmlRoot("TextureAtlas")]
    public class TextureAtlas
    {
        [XmlElement("SubTexture")]
        public List<SubTexture> SubTextures = new();
        
        [XmlAttribute("imagePath")]
        public string ImagePath { get; set; } = string.Empty;
    }
}