using System;
using System.Collections;
using System.Collections.Generic;
using Animator.ForSprite;
using SpriteUtils;
using UnityEngine;

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
            new("singLEFT", 24, new Vector2(249, 244), true, (_, _, texture) => texture.Name.StartsWith("KB_Left")),
            new("singDOWN", 24, new Vector2(186, 33), true, (_, _, texture) => texture.Name.StartsWith("KB_Down")),
            new("singUP", 24, new Vector2(160, 265), true, (_, _, texture) => texture.Name.StartsWith("KB_Up")),
            new("singRIGHT", 24, new Vector2(-114, 50), true, (_, _, texture) => texture.Name.StartsWith("KB_Right")),
            new("idle-alt", 24, new Vector2(76, 90), true, (_, _, texture) => texture.Name.StartsWith("KB_idleTired")),
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
        
        //spriteAnimator.Play(animations[0]);
    }

    private float time;
    private float offsetTime;
    private bool left = true;
    void Update()
    {
        const float spb = 1f / (222f / 60f);
        time += Time.deltaTime;
        if (time > offsetTime + spb)
        {
            offsetTime += spb;
            spriteAnimator.Play(left ? "danceLeft" : "danceRight");
            left = !left;
        }
    }
}
