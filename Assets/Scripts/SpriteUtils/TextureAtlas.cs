using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Animator.SpriteAnimating;
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

        public SpriteAnimation CreateAnimationByIndices(Sprite sprite, string prefix, IEnumerable<int> indices, AnimationDescriptor descriptor)
        {
            var animation = new SpriteAnimation()
            {
                Name = descriptor.Name,
                Offset = descriptor.Offset,
                FrameRate = descriptor.FrameRate,
                Reverse = descriptor.Reverse
            };
            
            var subTextures = SubTextures
                .Where(s => s.Name.StartsWith(prefix))
                .ToArray();
            foreach (var index in indices)
            {
                var subTexture = subTextures.ElementAtOrDefault(index);
                if (subTexture != null)
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