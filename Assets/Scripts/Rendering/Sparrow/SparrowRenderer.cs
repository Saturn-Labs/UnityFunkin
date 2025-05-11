using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class SparrowRenderer : MonoBehaviour
{
    private static Material? _SparrowMaterial;
    private SpriteRenderer _SpriteRenderer = null!;
    private MaterialPropertyBlock _MaterialPropertyBlock = null!;
    
    [Header("SubTexture Parameters")]
    public uint x;
    public uint y;
    public uint width;
    public uint height;
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
        
        if (_SpriteRenderer.drawMode != SpriteDrawMode.Sliced)
            _SpriteRenderer.drawMode = SpriteDrawMode.Sliced;
        _SpriteRenderer.size = new Vector2(
            frameWidth / pixelsPerUnit,
            frameHeight / pixelsPerUnit
        );
        
        _MaterialPropertyBlock.SetVector("_SubTexture", new Vector4(x, y, width, height));
        _MaterialPropertyBlock.SetVector("_SubTextureFrame", new Vector4(frameX, frameY, frameWidth, frameHeight));
        _MaterialPropertyBlock.SetFloat("_PixelsPerUnit", pixelsPerUnit);
        _SpriteRenderer.SetPropertyBlock(_MaterialPropertyBlock);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!_SpriteRenderer || !_SpriteRenderer.sprite || !texture || !sprite)
            return;
        var oldColor = Gizmos.color;
        
        // Draw frame rectangle
        Gizmos.color = Color.red;
        var frameOffset = new Vector2(
            (0.5f - sprite.pivot.x / texture.width) * (frameWidth / pixelsPerUnit),
            (0.5f - sprite.pivot.y / texture.height) * (frameHeight / pixelsPerUnit)
        );
        Gizmos.DrawWireCube(transform.position + (Vector3)frameOffset, new Vector3(frameWidth / pixelsPerUnit * transform.lossyScale.x, frameHeight / pixelsPerUnit * transform.lossyScale.y, 0));
        
        // Draw sub-texture rectangle
        Gizmos.color = Color.cyan;
        var subTextureOffset = new Vector2(
            (0.5f - sprite.pivot.x / texture.width) * (width / pixelsPerUnit) - frameX / pixelsPerUnit - (frameWidth - width) / pixelsPerUnit / 2f,
            (0.5f - sprite.pivot.y / texture.height) * (height / pixelsPerUnit) + frameY / pixelsPerUnit + (frameHeight - height) / pixelsPerUnit / 2f
        );
        Gizmos.DrawWireCube(transform.position + (Vector3)subTextureOffset, new Vector3(width / pixelsPerUnit * transform.lossyScale.x, height / pixelsPerUnit * transform.lossyScale.y, 0));
        Gizmos.color = oldColor;
    }
}
