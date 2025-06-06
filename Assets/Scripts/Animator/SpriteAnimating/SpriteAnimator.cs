using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Animator.SpriteAnimating
{
    [RequireComponent(typeof(SpriteRenderer))]
    [Serializable]
    public class SpriteAnimator : AbstractAnimator<SpriteAnimation>
    {
        public delegate void OnAnimationStartedDelegate(SpriteAnimator animator, SpriteAnimation animation);
        public delegate void OnFrameChangedDelegate(SpriteAnimator animator, SpriteAnimation animation, SpriteAnimationFrame frame, int frameIndex);
        public delegate void OnAnimationFinishedDelegate(SpriteAnimator animator, SpriteAnimation animation);
        
        private SpriteRenderer? _Renderer;
        private SparrowRenderer? _SparrowRenderer;
        
        [SerializeField]
        private List<SpriteAnimation> _Animations = new();
        public override IReadOnlyList<SpriteAnimation> Animations => _Animations;

        [SerializeField]
        private SpriteAnimation? _CurrentAnimation;
        public override SpriteAnimation? CurrentAnimation => _CurrentAnimation;

        [SerializeField]
        private int _FrameRate = 24;
        public override int FrameRate
        {
            get => _FrameRate;
            set => _FrameRate = value;
        }

        [SerializeField]
        private int _LastFrame = -1;
        [SerializeField]
        private SpriteAnimation? _LastAnimation;

        public override int? CurrentFrame
        {
            get
            {
                var frameRate = UseAnimationFrameRate ? CurrentAnimation?.FrameRate : FrameRate;
                if (CurrentAnimation is null || CurrentAnimation.Frames.Count == 0 || frameRate is null || frameRate <= 0)
                    return null;
                var frameCount = CurrentAnimation.Frames.Count;
                var time = Time;
                var frame = Mathf.FloorToInt(time * frameRate.Value % frameCount);
                if (ShouldReverse || CurrentAnimation.Reverse)
                    frame = frameCount - frame - 1;
                return frame;
            }
        }

        [SerializeField]
        private float _Time;
        public override float Time => _Time;
        
        [SerializeField]
        private bool _IsPlaying;
        public override bool IsPlaying => _IsPlaying;
        
        [SerializeField]
        private bool _ShouldReverse;
        public override bool ShouldReverse
        {
            get => _ShouldReverse;
            set => _ShouldReverse = value;
        }
        
        [SerializeField]
        private bool _ShouldLoop;
        public override bool ShouldLoop
        {
            get => _ShouldLoop;
            set
            {
                var lastShouldLoop = _ShouldLoop;
                _ShouldLoop = value;
                if (_ShouldLoop != lastShouldLoop)
                    Resume();
            }
        }

        public bool StartOnFirstAnimation;
        public bool UseOffsets;
        public bool UseAnimationFrameRate = true;
        public bool UseSparrowAtlas = true;
        
        public bool IsValidSprite => _Renderer && _Renderer.sprite;
        
        public event OnAnimationStartedDelegate? OnAnimationStarted;
        public event OnFrameChangedDelegate? OnFrameChanged;
        public event OnAnimationFinishedDelegate? OnAnimationFinished;
        
        public override void AddAnimation(SpriteAnimation _animation) => _Animations.Add(_animation);
        public override bool RemoveAnimation(SpriteAnimation _animation) => _Animations.Remove(_animation);
        public override bool RemoveAnimation(int index)
        {
            if (!IsValidAnimationIndex(index))
                return false;
            _Animations.RemoveAt(index);
            return true;
        }

        public override bool InsertAnimation(int index, SpriteAnimation _animation)
        {
            if (index < 0 || index > _Animations.Count)
                return false;
            _Animations.Insert(index, _animation);
            return true;
        }

        public override bool IsValidAnimationIndex(int index) => index >= 0 && index < _Animations.Count;
        public override bool TryGetAnimation(int index, out SpriteAnimation? _animation)
        {
            _animation = null;
            if (!IsValidAnimationIndex(index))
                return false;
            _animation = _Animations[index];
            return true;
        }
        
        public override IReadOnlyList<SpriteAnimation> GetAnimations() => Animations;
        public override int GetAnimationCount() => Animations.Count;
        public override void Clear() => _Animations.Clear();
        public override void AddRange(IEnumerable<SpriteAnimation> animations) => _Animations.AddRange(animations);
        public override int AddRangeCount(IEnumerable<SpriteAnimation> animations)
        {
            AddRange(animations);
            return _Animations.Count;
        }
        public override void ReverseAnimations() => _Animations.Reverse();
        public override void SortAnimations(Comparison<SpriteAnimation> comparison) => _Animations.Sort(comparison);
        public override bool Play(string _name, bool resetTime = true)
        {
            var foundAnimation = Animations.FirstOrDefault(a => a.Name == _name);
            return foundAnimation is not null && Play(foundAnimation, resetTime);
        }

        public override bool Play(SpriteAnimation _animation, bool resetTime = true)
        {
            Pause();
            _CurrentAnimation = _animation;
            if (resetTime)
                ResetTime();
            Resume();
            OnAnimationStarted?.Invoke(this, _CurrentAnimation);
            return true;
        }

        public override void ResetTime()
        {
            _LastFrame = -1;
            _Time = 0f;
        }

        public override void Restart()
        {
            ResetTime();
            Resume();
        }

        public override void Pause()
        {
            _IsPlaying = false;
        }

        public override void Resume()
        {
            _IsPlaying = true;
        }

        private void Awake()
        {
            _Renderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            if (!StartOnFirstAnimation || Animations.Count <= 0) 
                return;
            _CurrentAnimation = Animations[0];
        }
        
        private void Update()
        {
            if (!_Renderer || !IsPlaying) 
                return;
            
            _Time += UnityEngine.Time.deltaTime;
            if (CurrentAnimation is null || CurrentFrame is null || CurrentFrame == _LastFrame) 
                return;
                
            var frame = CurrentAnimation.GetFrame(CurrentFrame.Value);
            var frameSprite = frame.GetValue();
            if (_Renderer.sprite != frameSprite)
                _Renderer.sprite = frameSprite;
            if (!frameSprite)
                return;
            
            if (frame is { SubTexture: { } subTexture } && GetSparrowRenderer() is { } sparrowRenderer)
            {
                sparrowRenderer.x = (uint)subTexture.X;
                sparrowRenderer.y = (uint)subTexture.Y;
                sparrowRenderer.width = (uint)subTexture.Width;
                sparrowRenderer.height = (uint)subTexture.Height;
                sparrowRenderer.frameX = subTexture.FrameX;
                sparrowRenderer.frameY = subTexture.FrameY;
                sparrowRenderer.frameWidth = (uint)subTexture.FrameWidth;
                sparrowRenderer.frameHeight = (uint)subTexture.FrameHeight;
            }

            _LastFrame = CurrentFrame.Value;
            OnFrameChanged?.Invoke(this, CurrentAnimation, frame, CurrentFrame.Value);
            if (CurrentFrame.Value == CurrentAnimation.Frames.Count - 1)
            {
                if (!_ShouldLoop)
                    Pause();
                OnAnimationFinished?.Invoke(this, CurrentAnimation);
            }
        }

        private void OnDestroy()
        {
            OnAnimationStarted = null;
            OnFrameChanged = null;
            OnAnimationFinished = null;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public SparrowRenderer? GetSparrowRenderer()
        {
            if (!UseSparrowAtlas)
            {
                _SparrowRenderer?.ResetValues();
                _SparrowRenderer = null;
                return null;
            }
            if (!_SparrowRenderer)
            {
                _SparrowRenderer = GetComponent<SparrowRenderer>();
                if (!_SparrowRenderer)
                    Debug.LogWarning("SparrowRenderer not found on the same GameObject as SpriteAnimator, but a frame is trying to use SubTextures.");
            }
            return _SparrowRenderer;
        }
    }
}