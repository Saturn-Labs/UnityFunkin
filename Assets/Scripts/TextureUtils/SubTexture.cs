using System;
using System.Xml.Serialization;

namespace TextureUtils
{
    [Serializable]
    public class SubTexture
    {
        [XmlAttribute("name")]
        public string Name { get; set; } = string.Empty;
    
        [XmlAttribute("x")]
        public int X { get; set; }
    
        [XmlAttribute("y")]
        public int Y { get; set; }
    
        [XmlAttribute("width")]
        public int Width { get; set; }
    
        [XmlAttribute("height")]
        public int Height { get; set; }
    
        [XmlAttribute("frameX")]
        public int FrameX { get; set; }
    
        [XmlAttribute("frameY")]
        public int FrameY { get; set; }
    
        [XmlAttribute("frameWidth")]
        public int FrameWidth { get; set; }
    
        [XmlAttribute("frameHeight")]
        public int FrameHeight { get; set; }
    }
}