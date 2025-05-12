using System;
using System.Collections;
using System.Collections.Generic;
using Animator.ForSprite;
using ModManagement.Models;
using SpriteUtils;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class TestScript : MonoBehaviour
{
    public TextAsset sparrow;
    public Sprite sprite;
    public SpriteAnimator[] spriteAnimators;
    
    public SpriteRenderer spriteRenderer;
    public TextAsset descriptor;
    
    void Start()
    {
        var atlas = SparrowAtlas.Deserialize(sparrow);
        var animations = atlas.CreateAnimations(sprite, new AnimationDescriptor[]
        {
            new("main", 24, new Vector2(148, 103), true, (_, _, texture) => true)
        });
        foreach (var animator in spriteAnimators)
        {
            animator.AddRange(animations);
            animator.ShouldLoop = true;
            animator.Play("main");
        }
        //left = Random.value > 0.5f;
        
        var assets = AssetBundle.LoadFromFile("Assets/AssetBundles/assets.bundle");
        var scenes = AssetBundle.LoadFromFile("Assets/AssetBundles/scenes.bundle");
        var assetPaths = assets.GetAllAssetNames();
        var scenePaths = scenes.GetAllScenePaths();
        foreach (var scenePath in scenePaths)
        {
            Debug.Log(scenePath);
        }
        
        foreach (var assetPath in assetPaths)
        {
            Debug.Log(assetPath);
        }

        var shader = assets.LoadAsset<Shader>("Assets/mod@ryd3v@custommod/Images/TestShader.shader");
        spriteRenderer.material = new Material(shader);
        
        var desc = ModDescriptor.FromJson(descriptor.text);
        Debug.Log(desc);
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
            //spriteAnimator.Play(left ? "danceLeft" : "danceRight");
        }
    }
}
