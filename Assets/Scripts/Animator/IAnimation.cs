using System;
using System.Collections.Generic;

namespace Animator
{
    public interface IAnimation
    {
    }
    
    public interface IAnimation<T> : IAnimation
        where T : IAnimationFrame
    {
        string Name { get; set; }
        IReadOnlyList<T> Frames { get; }
        bool TryGetFrame(int index, out T? frame);
        T GetFrame(int index);
        int GetFrameCount();
        IReadOnlyList<T> GetFrames();
        void AddFrame(T frame);
        bool RemoveFrame(int index);
        bool RemoveFrame(T frame);
        bool IsValidFrameIndex(int index);
        void Clear();
        bool InsertFrame(int index, T frame);
        void ReverseFrames();
        void SortFrames(Comparison<T> comparison);
        void AddRange(IEnumerable<T> frames);
        int AddRangeCount(IEnumerable<T> frames);
    }
}