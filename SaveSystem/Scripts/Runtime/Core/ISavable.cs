namespace SaveSystem.Scripts.Runtime.Core
{
    public interface ISavable<T>
    {
        T data { get; }
        void Load(T data);
    }
}