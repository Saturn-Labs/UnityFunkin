using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Animator.ForSprite
{
    [Serializable]
    public class SpriteAnimationFrame : IAnimationFrame<Sprite>
    {
        [SerializeField]
        private string _Name;
        public string Name => _Name;
        
        [SerializeField]
        private Sprite _Sprite;
        public Sprite Sprite => _Sprite;
        
        [SerializeField]
        private Vector2 _Offset;
        public Vector2 Offset => _Offset;

        [SerializeField]
        private bool _ShouldUseUv;
        public bool ShouldUseUv => _ShouldUseUv;
        
        [SerializeField]
        private Vector2 _UvOffset;
        public Vector2 UvOffset => _UvOffset;
        
        [SerializeField]
        private Vector2 _UvSides;
        public Vector2 UvSides => _UvSides;
        
        public SpriteAnimationFrame(string name, Sprite sprite, Vector2 offset, bool useUvOffsetAndUvSides = false, Vector2? uvOffset = null, Vector2? uvSides = null)
        {
            _Name = name;
            _Sprite = sprite;
            _Offset = offset;
            _ShouldUseUv = useUvOffsetAndUvSides;
            _UvOffset = uvOffset ?? Vector2.zero;
            _UvSides = uvSides ?? Vector2.zero;
        }
        
        public Sprite GetValue()
        {
            return Sprite;
        }
    }
}