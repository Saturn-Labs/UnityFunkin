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
        private static readonly int SubTextureID = Shader.PropertyToID("_SubTexture");
        public static Material? SparrowAtlasMaterial { get; private set; }

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

        private void Awake()
        {
            if (!SparrowAtlasMaterial)
            {
                SparrowAtlasMaterial = Resources.Load<Material>("Materials/SparrowAtlas");
            }
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
                    var frameSprite = frame.GetValue();
                    if (!frameSprite)
                        frameSprite = _Renderer.sprite;
                    if (_Renderer.sprite != frameSprite)
                        _Renderer.sprite = frameSprite;
                    if (!frameSprite)
                        return;
                    var ppu = frameSprite.pixelsPerUnit;
                    var targetObj = _Renderer.gameObject.transform;
                    if (UseOffsets)
                    {
                        if (!Mathf.Approximately(frameSprite.pivot.x / frameSprite.texture.width, 0) || !Mathf.Approximately(frameSprite.pivot.y / frameSprite.texture.height, 1))
                            Debug.LogWarning($"SpriteAnimator: Sprite pivot is not (0, 1) / 'Top Left'. This may cause unexpected behavior on offsetting with 'frameX' and 'frameY'.\n X: {frameSprite.pivot.x}, Y: {frameSprite.pivot.y}");
                        
                        var animationOffset = new Vector2(
                            CurrentAnimation.TreatOffsetAsPixels ? -CurrentAnimation.Offset.x / ppu : -CurrentAnimation.Offset.x,
                            CurrentAnimation.TreatOffsetAsPixels ? CurrentAnimation.Offset.y / ppu : CurrentAnimation.Offset.y
                        );
                        
                        var frameOffset = new Vector2(
                            frame.TreatOffsetAsPixels ? -frame.Offset.x / ppu : -frame.Offset.x,
                            frame.TreatOffsetAsPixels ? frame.Offset.y / ppu : frame.Offset.y
                        );

                        var offset = animationOffset + frameOffset;
                        if (OffsetOnLocalPosition)
                            targetObj.localPosition = new Vector3(offset.x, offset.y, targetObj.localPosition.z);
                        else
                            targetObj.position = new Vector3(offset.x, offset.y, targetObj.position.z);;
                    }
                    
                    if (frameSprite && frame is { UseSubTexture: true, SubTextureRect: { } rect })
                    {
                        if (_Renderer.material.shader.name != "Custom/SparrowAtlas")
                        {
                            _Renderer.material = new Material(SparrowAtlasMaterial);
                        }
                        _Renderer.material.SetVector(SubTextureID, new Vector4(rect.x, rect.y, rect.width, rect.height));
                        targetObj.transform.localScale = new Vector3(rect.width / frameSprite.texture.width, rect.height / frameSprite.texture.height, _Renderer.gameObject.transform.localScale.z);
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