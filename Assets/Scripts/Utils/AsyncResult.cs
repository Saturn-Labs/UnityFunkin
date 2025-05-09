namespace Utils
{
    public class AsyncResult
    {
        public delegate void OnCompleteDelegate(AsyncResult result);
        
        public bool IsCompleted { get; private set; } = false;
        public event OnCompleteDelegate? OnComplete;

        private void SetCompleted()
        {
            if (IsCompleted)
                return;
            IsCompleted = true;
            OnComplete?.Invoke(this);
        }
    }
}