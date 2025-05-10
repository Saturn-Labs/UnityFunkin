using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class SparrowRenderer : MonoBehaviour
{
    private static Material? _SparrowMaterial;
    private SpriteRenderer _SpriteRenderer = null!;
    private MaterialPropertyBlock _MaterialPropertyBlock = null!;

    public Vector3 Scale = new Vector3(1f, 1f, 1f);
    public uint x;
    public uint y;
    public uint width;
    public uint height;
    public int frameX;
    public int frameY;
    
    [SerializeField]
    private uint _FrameWidth;
    [SerializeField]
    private uint _FrameHeight;

    public uint frameWidth
    {
        get => _FrameWidth > 0 ? _FrameWidth : width;
        set => _FrameWidth = value;
    }
    
    public uint frameHeight
    {
        get => _FrameHeight > 0 ? _FrameHeight : height;
        set => _FrameHeight = value;
    }

    public Texture? texture => _SpriteRenderer.sprite?.texture;
    
    void OnEnable()
    {
        _SparrowMaterial ??= Resources.Load<Material>("Materials/SparrowAtlas");
        _SpriteRenderer = GetComponent<SpriteRenderer>();
        _SpriteRenderer.sharedMaterial = _SparrowMaterial;
        _MaterialPropertyBlock = new MaterialPropertyBlock();
    }

    void OnDisable()
    {
    }

    void Update()
    {
        if (!texture)
            return;
        
        transform.localScale = new Vector3(
            frameWidth / (float)texture.width * Scale.x,
            frameHeight / (float)texture.height * Scale.y,
            transform.localScale.z * Scale.z
        );
        
        
        
        _MaterialPropertyBlock.SetVector("_SubTexture", new Vector4(x, y, width, height));
        _MaterialPropertyBlock.SetVector("_SubTextureFrame", new Vector4(frameX, frameY, frameWidth, frameHeight));
        _MaterialPropertyBlock.SetFloat("_PixelsPerUnit", _SpriteRenderer.sprite.pixelsPerUnit);
        _SpriteRenderer.SetPropertyBlock(_MaterialPropertyBlock);
    }

    private void OnDrawGizmosSelected()
    {
        if (!_SpriteRenderer)
            return;
        
    }
}
