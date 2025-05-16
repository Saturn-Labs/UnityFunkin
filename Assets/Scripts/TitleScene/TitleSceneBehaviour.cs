using System;
using Animator.SpriteAnimating;
using SpriteUtils;
using TransitionManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace TitleScene
{
    public class TitleSceneBehaviour : MonoBehaviour
    {
        public const float MusicBPM = 102f;
        public static float SecondsPerMusicBeat => 60f / MusicBPM;
        public static readonly int[] GirlfriendDanceLeftIndices = new[] { 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 };
        public static readonly int[] GirlfriendDanceRightIndices = new[] { 30, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
        
        [Header("Title Logo")]
        [SerializeField] 
        private SpriteAnimator? logoAnimator;
        [SerializeField] 
        private Sprite? logoSprite;
        [SerializeField] 
        private TextAsset? logoSparrow;
    
        [Header("Title Girlfriend")]
        [SerializeField] 
        private SpriteAnimator? girlfriendAnimator;
        [SerializeField] 
        private Sprite? girlfriendSprite;
        [SerializeField] 
        private TextAsset? girlfriendSparrow;
        [SerializeField]
        private bool girlfriendDanceLeft = true;

        [Header("Press Enter")]
        [SerializeField]
        private SpriteAnimator? pressEnterAnimator;
        [SerializeField]
        private Sprite? pressEnterSprite;
        [SerializeField]
        private TextAsset? pressEnterSparrow;
        
        [Header("Main")]
        [SerializeField]
        private bool inIntroText = true;
        [SerializeField]
        private bool enterWasPressed = false;
        [SerializeField]
        private int curBeat = 0;
        [SerializeField]
        private float nextBeatTime = 0f;
        [SerializeField]
        private float time = 0f;
        [SerializeField]
        private AudioClip? freakyMenu;
        private AudioSource? freakyMenuSource;
        
        public void Start()
        {
            var freakyMenuSourceObj = GameObject.FindGameObjectWithTag("FreakyMenuMusic");
            if (freakyMenuSourceObj == null)
            {
                freakyMenuSourceObj = new GameObject("FreakyMenu", typeof(AudioSource))
                {
                    tag = "FreakyMenuMusic"
                };
                freakyMenuSource = freakyMenuSourceObj.GetComponent<AudioSource>();
                freakyMenuSource.clip = freakyMenu;
                freakyMenuSource.loop = true;
                freakyMenuSource.volume = 0.15f;
                freakyMenuSource.Play();
            }
            else
            {
                freakyMenuSource = freakyMenuSourceObj.GetComponent<AudioSource>();
            }
            DontDestroyOnLoad(freakyMenuSourceObj);
            
            if (logoAnimator == null || logoSprite == null || logoSparrow == null)
            {
                Debug.LogError("Logo animator, sprite or sparrow atlas is not set.");
                return;
            }
        
            var logoAtlas = SparrowAtlas.Deserialize(logoSparrow);
            var logoAnimations = logoAtlas.CreateAnimations(logoSprite,
                new AnimationDescriptor[]
                {
                    new("logo", 24, new Vector2(0, 0), true, (_, _, texture) => texture.Name.StartsWith("logo bumpin"))
                }
            );
            logoAnimator.AddRange(logoAnimations);
        
            if (girlfriendAnimator == null || girlfriendSprite == null || girlfriendSparrow == null)
            {
                Debug.LogError("Girlfriend animator, sprite or sparrow atlas is not set.");
                return;
            }
        
            var titleGfAtlas = SparrowAtlas.Deserialize(girlfriendSparrow);
            girlfriendAnimator.AddAnimation(
                titleGfAtlas.CreateAnimationByIndices(
                    girlfriendSprite,
                    "gfDance",
                    GirlfriendDanceLeftIndices,
                    new AnimationDescriptor(
                        "danceLeft",
                        24,
                        new Vector2(0, 0),
                        true,
                        (_, _, _) => true
                    )
                )
            );
            girlfriendAnimator.AddAnimation(
                titleGfAtlas.CreateAnimationByIndices(
                    girlfriendSprite,
                    "gfDance",
                    GirlfriendDanceRightIndices,
                    new AnimationDescriptor(
                        "danceRight",
                        24,
                        new Vector2(0, 0),
                        true,
                        (_, _, _) => true
                    )
                )
            );
            girlfriendAnimator.ShouldLoop = false;

            if (pressEnterAnimator == null || pressEnterSprite == null || pressEnterSparrow == null)
            {
                Debug.LogError("Press enter animator, sprite or sparrow atlas is not set.");
                return;
            }
            
            var pressEnterAtlas = SparrowAtlas.Deserialize(pressEnterSparrow);
            var pressEnterAnimations = pressEnterAtlas.CreateAnimations(pressEnterSprite,
                new AnimationDescriptor[]
                {
                    new("press", 24, new Vector2(0, 0), true, (_, _, texture) => texture.Name.StartsWith("Press Enter to Begin")),
                    new("pressed", 24, new Vector2(0, 0), true, (_, _, texture) => texture.Name.StartsWith("ENTER PRESSED"))
                }
            );
            pressEnterAnimator.AddRange(pressEnterAnimations);
            pressEnterAnimator.ShouldLoop = true;
            if (freakyMenuSource && !freakyMenuSource.isPlaying)
            {
                freakyMenuSource.Play();
            }
            logoAnimator.Play("logo");
            girlfriendAnimator.Play("danceLeft");
            pressEnterAnimator.Play("press");
            SkipIntroText();
        }

        private float beforeSample = 1;
        private void Update()
        {
            time += Time.deltaTime;
            if (freakyMenuSource)
            {
                if (beforeSample < freakyMenuSource.timeSamples)
                {
                    beforeSample = freakyMenuSource.timeSamples - 1f;
                }
                else if (beforeSample > freakyMenuSource.timeSamples)
                {
                    beforeSample = freakyMenuSource.timeSamples - 1f;
                    nextBeatTime = 0;
                    curBeat = 0;
                }
                
                if (freakyMenuSource.time > nextBeatTime)
                {
                    curBeat++;
                    nextBeatTime = freakyMenuSource.time + SecondsPerMusicBeat;
                    logoAnimator?.Play("logo");
                    girlfriendDanceLeft = !girlfriendDanceLeft;
                    girlfriendAnimator?.Play(girlfriendDanceLeft ? "danceLeft" : "danceRight");
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Return) && !enterWasPressed && !inIntroText && pressEnterAnimator)
            {
                enterWasPressed = true;
                pressEnterAnimator.Play("pressed");
                TransitionManager.Main.LoadScene("StartupScene");
            }
        }

        public void SkipIntroText()
        {
            if (!inIntroText)
                return;
            inIntroText = false;
            
        }
    }
}