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
        
        var leftAnimation = new SpriteAnimation()
        {
            Name = "KB_DanceLeft"
        };
        
        var rightAnimation = new SpriteAnimation()
        {
            Name = "KB_DanceRight"
        };
        
        foreach (var subTexture in atlas.SubTextures)
        {
            if (!subTexture.Name.StartsWith("KB_DanceLeft") && !subTexture.Name.StartsWith("KB_DanceRight"))
                continue;
            
            const int ppu = 100;
            var offX = (subTexture.Name.StartsWith("KB_DanceLeft") ? 148 : 107) / (float)ppu;
            var offY = (subTexture.Name.StartsWith("KB_DanceLeft") ? 103 : 112) / (float)ppu;
            //var fixedY = sprite.texture.height - subTexture.Y - subTexture.Height;
            //var sprite = Sprite.Create(texture, new Rect(subTexture.X, fixedY, subTexture.Width, subTexture.Height), new Vector2(0f, 1f), ppu);
            var offset = new Vector2((Math.Abs(subTexture.FrameX) / (float)ppu) - offX, (-Math.Abs(subTexture.FrameY) / (float)ppu) + offY);

            var uvOffset = new Vector2(subTexture.X / (float)sprite.texture.width, subTexture.Y / (float)sprite.texture.height);
            var uvSides = new Vector2(subTexture.Width / (float)sprite.texture.width, subTexture.Height / (float)sprite.texture.height);
            
            if (subTexture.Name.StartsWith("KB_DanceLeft"))
                leftAnimation.AddFrame(new SpriteAnimationFrame(subTexture.Name, sprite, offset, true, uvOffset, uvSides));
            else
                rightAnimation.AddFrame(new SpriteAnimationFrame(subTexture.Name, sprite, offset, true, uvOffset, uvSides));
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
