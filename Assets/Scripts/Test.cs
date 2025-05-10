using System;
using System.Collections;
using System.Collections.Generic;
using Animator.ForSprite;
using SpriteUtils;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour
{
    public TextAsset sparrowText;
    public Sprite sprite;
    public SpriteAnimator spriteAnimator;
    
    void Start()
    {
        var atlas = SparrowAtlas.Deserialize(sparrowText);
        var animations = atlas.CreateAnimations(sprite, new AnimationDescriptor[]
        {
            new("danceLeft", 24, new Vector2(148, 103), true, (_, _, texture) => texture.Name.StartsWith("KB_DanceLeft")),
            new("danceRight", 24, new Vector2(107, 112), true, (_, _, texture) => texture.Name.StartsWith("KB_DanceRight")),
        });
        spriteAnimator.AddRange(animations);
        
         // var current = 0;
         // var willPlay = false;
         // IEnumerator PlayAnim(SpriteAnimator animator)
         // {
         //     willPlay = true;
         //     yield return new WaitForSeconds(0.5f);
         //     var anim = animator.Animations[++current % animator.Animations.Count];
         //     animator.Play(anim);
         //     willPlay = false;
         // }
         //
         // spriteAnimator.OnAnimationFinished += (animator, _) =>
         // {
         //     if (willPlay)
         //         return;
         //     StartCoroutine(PlayAnim(spriteAnimator));
         // };
         
        left = Random.value > 0.5f;
    }

    private double rand;
    private double time;
    private double nextDance;
    private bool left;
    private void Update()
    {
        double danceInterval = 1d / (222d / 60d) / 2d + rand;
        time += Time.deltaTime;
        if (time >= nextDance + danceInterval)
        {
            nextDance = time + danceInterval;
            left = !left;
            spriteAnimator.Play(left ? "danceLeft" : "danceRight");
        }
    }
}
