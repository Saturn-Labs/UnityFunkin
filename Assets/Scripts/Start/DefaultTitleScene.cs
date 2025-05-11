using System;
using Animator.ForSprite;
using SpriteUtils;
using UnityEngine;

public class DefaultTitleScene : MonoBehaviour
{
    public float Time;
    
    [Header("Logo")]
    public SpriteAnimator logoAnimator;
    public Sprite logoSprite;
    public TextAsset logoSparrowAtlas;
    
    [Header("GF Title")]
    public SpriteAnimator gfAnimator;
    public Sprite gfSprite;
    public TextAsset gfSparrowAtlas;
    
    public void Start()
    {
        var logoAtlas = SparrowAtlas.Deserialize(logoSparrowAtlas);
        var logoAnimations = logoAtlas.CreateAnimations(logoSprite, new AnimationDescriptor[]
        {
            new("logo", 24, new Vector2(0, 0), true, (_, _, texture) => texture.Name.StartsWith("logo bumpin"))
        });
        logoAnimator.AddRange(logoAnimations);
        logoAnimator.ShouldLoop = true;
        
        var titleGfAtlas = SparrowAtlas.Deserialize(gfSparrowAtlas);
        var titleGfAnimations = titleGfAtlas.CreateAnimations(gfSprite, new AnimationDescriptor[]
        {
            new("gfDance", 24, new Vector2(0, 0), true, (_, _, texture) => texture.Name.StartsWith("gfDance"))
        });
        gfAnimator.AddRange(titleGfAnimations);
        gfAnimator.ShouldLoop = true;
        
        
        logoAnimator.Play("logo");
        gfAnimator.Play("gfDance");
    }

    private float _nextBeat;
    private void Update()
    {
        const float beatInterval = 1f / (222f / 60f);
        Time += UnityEngine.Time.deltaTime;
        if (Time >= _nextBeat + beatInterval)
        {
            _nextBeat = Time + beatInterval;
            //logoAnimator.Play("logo");
            
        }
    }
}
