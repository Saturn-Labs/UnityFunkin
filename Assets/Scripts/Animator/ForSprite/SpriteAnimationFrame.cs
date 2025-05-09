using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Animator.ForSprite
{
    [Serializable]
    public class SpriteAnimationFrame : IAnimationFrame<Sprite>
    {
        public string Name = "Unnamed Frame";
        public Sprite? Sprite = null;
        public bool TreatOffsetAsPixels = true;
        public Vector2 Offset = Vector2.zero;
        public bool UseSubTexture = false;
        public Rect? SubTextureRect = null;
        
        public Sprite? GetValue()
        {
            return Sprite;
        }
    }
}