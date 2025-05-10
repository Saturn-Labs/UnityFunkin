using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Animator.ForSprite
{
    [Serializable]
    public class SpriteAnimation : IAnimation<SpriteAnimationFrame>
    {
        [SerializeField]
        private List<SpriteAnimationFrame> _Frames = new();
        public IReadOnlyList<SpriteAnimationFrame> Frames => _Frames;
        
        [SerializeField]
        private string _Name = "Blank Animation";
        public string Name
        {
            get => _Name;
            set => _Name = value;
        }
        
        [SerializeField]
        private Vector2 _Offset = Vector2.zero;
        public Vector2 Offset
        {
            get => _Offset;
            set => _Offset = value;
        }
        
        [SerializeField]
        private int _FrameRate = 24;
        public int FrameRate
        {
            get => _FrameRate;
            set => _FrameRate = value;
        }
        
        [SerializeField]
        private bool _Reverse = false;
        public bool Reverse
        {
            get => _Reverse;
            set => _Reverse = value;
        }

        public SpriteAnimation() {}
        public SpriteAnimation(IEnumerable<SpriteAnimationFrame> frames) : this()
        {
            _Frames = frames.ToList();
        }

        public bool TryGetFrame(int index, out SpriteAnimationFrame? frame)
        {
            frame = null;
            if (!IsValidFrameIndex(index))
                return false;
            frame = _Frames[index];
            return true;
        }

        public SpriteAnimationFrame GetFrame(int index)
        {
            if (!IsValidFrameIndex(index))
                throw new ArgumentException($"Index was invalid when trying to get frame (Index: {index}, Count: {_Frames.Count}).");
            return _Frames[index];
        }

        public int GetFrameCount() => _Frames.Count;
        public IReadOnlyList<SpriteAnimationFrame> GetFrames() => Frames;

        public void AddFrame(SpriteAnimationFrame frame)
        {
            _Frames.Add(frame);
        }

        public bool RemoveFrame(int index)
        {
            if (!IsValidFrameIndex(index))
                return false;
            _Frames.RemoveAt(index);
            return true;
        }

        public bool RemoveFrame(SpriteAnimationFrame frame) => _Frames.Remove(frame);
        public bool IsValidFrameIndex(int index) => index >= 0 && index < _Frames.Count;
        public void Clear() => _Frames.Clear();
        public bool InsertFrame(int index, SpriteAnimationFrame frame)
        {
            if (index < 0 || index > _Frames.Count)
                return false;
            _Frames.Insert(index, frame);
            return true;
        }

        public void ReverseFrames() => _Frames.Reverse();
        public void SortFrames(Comparison<SpriteAnimationFrame> comparison) => _Frames.Sort(comparison);
        public void AddRange(IEnumerable<SpriteAnimationFrame> frames) => _Frames.AddRange(frames);
        public int AddRangeCount(IEnumerable<SpriteAnimationFrame> frames)
        {
            _Frames.AddRange(frames);
            return Frames.Count;
        }
    }
}