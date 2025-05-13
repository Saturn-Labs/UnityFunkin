using System;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class SparrowRenderer : MonoBehaviour
{
    private static readonly int _SubTextureId = Shader.PropertyToID("_SubTexture");
    private static readonly int _SubTextureFrameId = Shader.PropertyToID("_SubTextureFrame");
    private static Material SparrowMaterial => StaticResources.SparrowMaterial;
    
    private SpriteRenderer _SpriteRenderer = null!;
    private MaterialPropertyBlock _MaterialPropertyBlock = null!;
    
    [Header("SubTexture Parameters")]
    public uint x;
    public uint y;

    [SerializeField]
    private uint _Width;
    [SerializeField]
    private uint _Height;

    public uint width
    {
        get => _Width > 0 ? _Width : _FrameWidth;
        set => _Width = value;
    }

    public uint height
    {
        get => _Height > 0 ? _Height : _FrameHeight;
        set => _Height = value;
    }
    
    [Header("Rectangle Parameters")]
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
    public Sprite? sprite => _SpriteRenderer.sprite;
    public float pixelsPerUnit => _SpriteRenderer.sprite?.pixelsPerUnit ?? 100f;
    
    private void OnEnable()
    {
        _SpriteRenderer = GetComponent<SpriteRenderer>();
        _SpriteRenderer.sharedMaterial = SparrowMaterial;
        _MaterialPropertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (!texture)
            return;
        
        if (_SpriteRenderer.drawMode != SpriteDrawMode.Sliced)
            _SpriteRenderer.drawMode = SpriteDrawMode.Sliced;
        _SpriteRenderer.size = new Vector2(
            frameWidth / pixelsPerUnit,
            frameHeight / pixelsPerUnit
        );
        
        _MaterialPropertyBlock.SetVector(_SubTextureId, new Vector4(x, y, width, height));
        _MaterialPropertyBlock.SetVector(_SubTextureFrameId, new Vector4(frameX, frameY, frameWidth, frameHeight));
        _SpriteRenderer.SetPropertyBlock(_MaterialPropertyBlock);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!_SpriteRenderer || !_SpriteRenderer.sprite || !texture || !sprite)
            return;
        var oldColor = Gizmos.color;
        
        var offsetToFrame = new Vector2(
            (int)frameWidth - (int)width,
            (int)frameHeight - (int)height
        ) / pixelsPerUnit;
        
        Gizmos.color = Color.red;
        var frameOffset = new Vector2(
            (0.5f - sprite.pivot.x / texture.width) * (frameWidth / pixelsPerUnit),
            (0.5f - sprite.pivot.y / texture.height) * (frameHeight / pixelsPerUnit)
        );
        Gizmos.DrawWireCube(transform.position + (Vector3)frameOffset, new Vector3(frameWidth / pixelsPerUnit * transform.lossyScale.x, frameHeight / pixelsPerUnit * transform.lossyScale.y, 0));
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + (Vector3)frameOffset + new Vector3(-offsetToFrame.x, offsetToFrame.y) / 2 + new Vector3(-frameX / pixelsPerUnit, frameY / pixelsPerUnit), new Vector3(frameWidth / pixelsPerUnit * transform.lossyScale.x, frameHeight / pixelsPerUnit * transform.lossyScale.y, 0) - (Vector3)offsetToFrame);
        Gizmos.color = oldColor;
    }

    public void ResetValues()
    {
        x = 0;
        y = 0;
        width = (uint?)texture?.width ?? 100u;
        height = (uint?)texture?.height ?? 100u;
        frameX = 0;
        frameY = 0;
        frameWidth = width;
        frameHeight = height;
        _MaterialPropertyBlock = new MaterialPropertyBlock();
    }
}
