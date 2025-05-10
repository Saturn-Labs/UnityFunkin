using System;
using Animator.ForSprite;
using SpriteUtils;
using UnityEngine;

public class DefaultStartScene : MonoBehaviour
{
    public float Time;
    public SpriteAnimator logoAnimator;
    public Sprite logoSprite;
    public TextAsset logoSparrowAtlas;
    
    
    public void Start()
    {
        var atlas = SparrowAtlas.Deserialize(logoSparrowAtlas);
        var animations = atlas.CreateAnimations(logoSprite, new AnimationDescriptor[]
        {
            new("logo", 24, new Vector2(0, 0), true, (_, _, texture) => texture.Name.StartsWith("logo bumpin"))
        });
        logoAnimator.AddRange(animations);
    }

    private float _nextLogoBump;
    private void Update()
    {
        const float bumpInterval = 1f / (222f / 60f);
        Time += UnityEngine.Time.deltaTime;
        if (Time >= _nextLogoBump + bumpInterval)
        {
            _nextLogoBump = Time + bumpInterval;
            logoAnimator.Play("logo");
        }
    }
}
