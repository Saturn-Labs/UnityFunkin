namespace Animator
{
    public interface IAnimationFrame
    {
    }
    
    public interface IAnimationFrame<out T> : IAnimationFrame
    {
        T? GetValue();
    }
}