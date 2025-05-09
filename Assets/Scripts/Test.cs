using System;
using System.Collections.Generic;
using Animator.ForSprite;
using TextureUtils;
using UnityEngine;

public class Test : MonoBehaviour
{
    public TextAsset sparrowText;
    public Sprite sprite;
    public SpriteAnimator spriteAnimator;
    
    void Start()
    {
        var atlas = SparrowAtlas.Deserialize(sparrowText);
        
        var leftAnimation = new SpriteAnimation
        {
            Name = "KB_DanceLeft",
            TreatOffsetAsPixels = true,
            Offset = new Vector2(148, 103)
        };
        //leftAnimation.Offset = new Vector2(0, leftAnimation.Offset.y);
        
        var rightAnimation = new SpriteAnimation
        {
            Name = "KB_DanceRight",
            TreatOffsetAsPixels = true,
            Offset = new Vector2(107, 112)
        };
        //rightAnimation.Offset = new Vector2(0, rightAnimation.Offset.y);
        
        foreach (var subTexture in atlas.SubTextures)
        {
            if (!subTexture.Name.StartsWith("KB_DanceLeft") && !subTexture.Name.StartsWith("KB_DanceRight"))
                continue;

            var offset = new Vector2(subTexture.FrameX, subTexture.FrameY)
            {
                //x = 0,
            };
            var frame = new SpriteAnimationFrame
            {
                Name = subTexture.Name,
                Sprite = sprite,
                TreatOffsetAsPixels = true,
                Offset = offset,
                UseSubTexture = true,
                SubTextureRect = new Rect(subTexture.X, subTexture.Y, subTexture.Width, subTexture.Height)
            };
            
            if (subTexture.Name.StartsWith("KB_DanceLeft"))
                leftAnimation.AddFrame(frame);
            else
                rightAnimation.AddFrame(frame);
        }
        
        spriteAnimator.AddAnimation(leftAnimation);
        spriteAnimator.AddAnimation(rightAnimation);
    }

    private float time;
    private float offsetTime;
    private bool left = true;
    void Update()
    {
        const float spb = 1f / (120f / 60f);
        time += Time.deltaTime;
        if (time > offsetTime + spb)
        {
            offsetTime += spb;
            spriteAnimator.Play(left ? "KB_DanceLeft" : "KB_DanceRight");
            left = !left;
        }
    }
}
