using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Animator.ForSprite
{
    [RequireComponent(typeof(SpriteRenderer))]
    [Serializable]
    public class SpriteAnimator : AbstractAnimator<SpriteAnimation>
    {
        private static readonly int UVOffset = Shader.PropertyToID("_UVOffset");
        private static readonly int UVSides = Shader.PropertyToID("_UVSides");

        public delegate void OnAnimationStartedDelegate(SpriteAnimator animator, SpriteAnimation animation);
        public delegate void OnFrameChangedDelegate(SpriteAnimator animator, SpriteAnimation animation, SpriteAnimationFrame frame, int frameIndex);
        public delegate void OnAnimationFinishedDelegate(SpriteAnimator animator, SpriteAnimation animation);
        
        private SpriteRenderer? _Renderer;

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
        
        public override int? CurrentFrame =>
            CurrentAnimation?.Frames.Count is { } frameCount && CurrentAnimation?.Frames.Count != 0 && FrameRate > 0
                ? !_ShouldReverse ? 
                    Mathf.FloorToInt(Time * FrameRate % frameCount) : 
                    frameCount - Mathf.FloorToInt(Time * FrameRate % frameCount) - 1
                : null;

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
        public bool OffsetOnLocalPosition = true;
        
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

        private void Start()
        {
            _Renderer = GetComponent<SpriteRenderer>();
            if (StartOnFirstAnimation)
            {
                if (Animations.Count > 0)
                    _CurrentAnimation = Animations[0];
            }
        }

        private void Update()
        {
            if (_Renderer && IsPlaying)
            {
                _Time += UnityEngine.Time.deltaTime;
                if (CurrentAnimation is not null && CurrentFrame is not null && CurrentFrame != _LastFrame)
                {
                    var frame = CurrentAnimation.GetFrame(CurrentFrame.Value);
                    if (UseOffsets)
                    {
                        var obj = _Renderer.gameObject.transform;
                        if (OffsetOnLocalPosition)
                            obj.localPosition = new Vector3(frame.Offset.x, frame.Offset.y, obj.localPosition.z);
                        else
                            obj.position = new Vector3(frame.Offset.x, frame.Offset.y, obj.position.z);
                    }

                    _Renderer.sprite = frame.GetValue();
                    if (frame.ShouldUseUv)
                    {
                        _Renderer.material.SetVector(UVOffset, new Vector4(frame.UvOffset.x, -frame.UvOffset.y, 1f, 1f));
                        _Renderer.material.SetVector(UVSides, new Vector4(0f, 1 - frame.UvSides.x, 1 - frame.UvSides.y, 0f));
                        _Renderer.gameObject.transform.localScale = new Vector3(frame.UvSides.x, frame.UvSides.y, _Renderer.gameObject.transform.localScale.z);
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
            }
        }

        private void OnDestroy()
        {
            OnAnimationStarted = null;
            OnFrameChanged = null;
            OnAnimationFinished = null;
        }
    }
}