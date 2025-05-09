using System;
using System.Collections.Generic;
using UnityEngine;

namespace Animator
{
    public abstract class AbstractAnimator<T> : MonoBehaviour
        where T : IAnimation
    {
        public abstract IReadOnlyList<T> Animations { get; }
        public abstract T? CurrentAnimation { get; }
        public abstract int FrameRate { get; set; }
        public abstract int? CurrentFrame { get; }
        public abstract float Time { get; }
        public abstract bool IsPlaying { get; }
        public abstract bool ShouldReverse { get; set; }
        public abstract bool ShouldLoop { get; set; }
        
        public abstract void AddAnimation(T animation);
        public abstract bool RemoveAnimation(T animation);
        public abstract bool RemoveAnimation(int index);
        public abstract bool InsertAnimation(int index, T animation);
        public abstract bool IsValidAnimationIndex(int index);
        public abstract bool TryGetAnimation(int index, out T? animation);
        public abstract IReadOnlyList<T> GetAnimations();
        public abstract int GetAnimationCount();
        public abstract void Clear();
        public abstract void AddRange(IEnumerable<T> animations);
        public abstract int AddRangeCount(IEnumerable<T> animations);
        public abstract void ReverseAnimations();
        public abstract void SortAnimations(Comparison<T> comparison);

        public abstract bool Play(string name, bool resetTime = true);
        public abstract bool Play(T animation, bool resetTime = true);
        public abstract void Pause();
        public abstract void Resume();
        public abstract void ResetTime();
        public abstract void Restart();
    }
}