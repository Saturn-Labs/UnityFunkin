using System.Collections;
using System.Collections.Generic;
using Animator.SpriteAnimating;
using SpriteUtils;
using UnityEngine;

namespace TestScene
{
    public class TestBehaviour : MonoBehaviour
    {
        [Header("Sparrow Atlas Testing")]
        [SerializeField] 
        private SpriteAnimator? animator;
        [SerializeField]
        private Sprite? sprite;
        [SerializeField]
        private TextAsset? sparrow;
        
        private void Start()
        {
            if (animator == null || sprite == null || sparrow == null)
            {
                Debug.LogError("Animator, Sprite or Sparrow Atlas is not assigned.");
                return;
            }
            
            var atlas = SparrowAtlas.Deserialize(sparrow);
            var animations = atlas.CreateAnimations(sprite,
                new[]
                {
                    new AnimationDescriptor("idle", 24, new Vector2(0, 0), true, (descriptor, spriteAnimation, texture) => texture.Name.StartsWith("Nightmare SANS Idle instance ")),
                    new AnimationDescriptor("singLEFT", 24, new Vector2(109, -85), true, (descriptor, spriteAnimation, texture) => texture.Name.StartsWith("Leftt instance ")),
                    new AnimationDescriptor("singDOWN", 24, new Vector2(2, 10), true, (descriptor, spriteAnimation, texture) => texture.Name.StartsWith("DOWNN instance ")),
                    new AnimationDescriptor("singUP", 24, new Vector2(-85, 61), true, (descriptor, spriteAnimation, texture) => texture.Name.StartsWith("UPP instance ")),
                    new AnimationDescriptor("singRIGHT", 24, new Vector2(60, -93), true, (descriptor, spriteAnimation, texture) => texture.Name.StartsWith("Rightt instance ")),
                }
            );
            animator.AddRange(animations);
            animator.ShouldLoop = true;

            IEnumerator StartAnimating()
            {
                while (true)
                {
                    animator.Play("idle");
                    yield return new WaitForSeconds(1f);
                    animator.Play("singLEFT");
                    yield return new WaitForSeconds(1f);
                    animator.Play("singDOWN");
                    yield return new WaitForSeconds(1f);
                    animator.Play("singUP");
                    yield return new WaitForSeconds(1f);
                    animator.Play("singRIGHT");
                    yield return new WaitForSeconds(1f);
                }
            }
            StartCoroutine(StartAnimating());
        }
    }
}