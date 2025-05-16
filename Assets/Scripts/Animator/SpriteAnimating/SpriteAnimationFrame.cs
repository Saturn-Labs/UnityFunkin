using System;
using SpriteUtils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Animator.SpriteAnimating
{
    [Serializable]
    public class SpriteAnimationFrame : IAnimationFrame<Sprite>
    {
        public string Name = "Unnamed Frame";
        public Sprite? Sprite = null;
        public SubTexture? SubTexture = null;
        
        public Sprite? GetValue()
        {
            return Sprite;
        }
    }
}