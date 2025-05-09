using Animator.ForSprite;
using UnityEngine;

namespace SpriteUtils
{
    public class AnimationDescriptor
    {
        public delegate bool FrameSelectorDelegate(AnimationDescriptor descriptor, SpriteAnimation animation, SubTexture subTexture);
        
        public string Name;
        public int FrameRate = 24;
        public Vector2 Offset = Vector2.zero;
        public bool TreatOffsetAsPixels = true;
        public bool Reverse;
        public readonly FrameSelectorDelegate FrameSelector;

        public AnimationDescriptor(string name, FrameSelectorDelegate frameSelector)
        {
            Name = name;
            FrameSelector = frameSelector;
        }
        
        public AnimationDescriptor(string name, int frameRate, Vector2 offset, bool treatOffsetAsPixels, FrameSelectorDelegate frameSelector, bool reverse = false) : 
            this(name, frameSelector)
        {
            FrameRate = frameRate;
            Offset = offset;
            TreatOffsetAsPixels = treatOffsetAsPixels;
            Reverse = reverse;
        }
    }
}