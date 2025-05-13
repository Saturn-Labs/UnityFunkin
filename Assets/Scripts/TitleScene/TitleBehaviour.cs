using System;
using Animator.ForSprite;
using SpriteUtils;
using UnityEngine;

namespace TitleScene
{
    public class TitleBehaviour : MonoBehaviour
    {
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

        [Header("Press Enter")]
        [SerializeField]
        private SpriteAnimator? pressEnterAnimator;
        [SerializeField]
        private Sprite? pressEnterSprite;
        [SerializeField]
        private TextAsset? pressEnterSparrow;

        [Header("Main")]
        [SerializeField]
        private bool enterWasPressed = false;
        
        public void Start()
        {
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
            logoAnimator.ShouldLoop = true;
        
            if (girlfriendAnimator == null || girlfriendSprite == null || girlfriendSparrow == null)
            {
                Debug.LogError("Girlfriend animator, sprite or sparrow atlas is not set.");
                return;
            }
        
            var titleGfAtlas = SparrowAtlas.Deserialize(girlfriendSparrow);
            var titleGfAnimations = titleGfAtlas.CreateAnimations(girlfriendSprite,
                new AnimationDescriptor[]
                {
                    new("gfDance", 24, new Vector2(0, 0), true, (_, _, texture) => texture.Name.StartsWith("gfDance"))
                }
            );
            girlfriendAnimator.AddRange(titleGfAnimations);
            girlfriendAnimator.ShouldLoop = true;

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
            
            logoAnimator.Play("logo");
            girlfriendAnimator.Play("gfDance");
            pressEnterAnimator.Play("press");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) && !enterWasPressed && pressEnterAnimator)
            {
                enterWasPressed = true;
                pressEnterAnimator.Play("pressed");
            }
        }
    }
}