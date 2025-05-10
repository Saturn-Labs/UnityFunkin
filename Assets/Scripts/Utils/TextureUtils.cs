using UnityEngine;

namespace Utils
{
    public class TextureUtils
    {
        public static Vector2 CalculateLocalPositionFromFrame(
            float frameX, float frameY,
            float frameWidth, float frameHeight,
            float spriteWidth, float spriteHeight,
            Vector2 containerPivot, Vector2 spritePivot,
            float pixelsPerUnit)
        {
            float offsetX = (frameWidth * containerPivot.x - frameX - spriteWidth * spritePivot.x) / pixelsPerUnit;
            float offsetY = -(frameHeight * containerPivot.y - frameY - spriteHeight * spritePivot.y) / pixelsPerUnit;

            return new Vector2(offsetX, offsetY);
        }

    }
}