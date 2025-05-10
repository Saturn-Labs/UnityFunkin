using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Animator.ForSprite;
using UnityEngine;

namespace SpriteUtils
{
    [Serializable]
    [XmlRoot("TextureAtlas")]
    public class TextureAtlas
    {
        [XmlElement("SubTexture")]
        public List<SubTexture> SubTextures = new();
        
        [XmlAttribute("imagePath")]
        public string ImagePath { get; set; } = string.Empty;

        public SpriteAnimation CreateAnimation(Sprite sprite, AnimationDescriptor descriptor)
        {
            var animation = new SpriteAnimation()
            {
                Name = descriptor.Name,
                Offset = descriptor.Offset,
                FrameRate = descriptor.FrameRate,
                Reverse = descriptor.Reverse
            };
                
            foreach (var subTexture in SubTextures)
            {
                if (descriptor.FrameSelector(descriptor, animation, subTexture))
                {
                    animation.AddFrame(new SpriteAnimationFrame
                    {
                        Name = subTexture.Name,
                        Sprite = sprite,
                        SubTexture = subTexture
                    });
                }
            }
                
            return animation;
        }

        public List<SpriteAnimation> CreateAnimations(Sprite sprite, IEnumerable<AnimationDescriptor> descriptors)
        {
            List<SpriteAnimation> animations = new();
            foreach (var descriptor in descriptors)
            {
                animations.Add(CreateAnimation(sprite, descriptor));
            }
            return animations;
        }
    }
}