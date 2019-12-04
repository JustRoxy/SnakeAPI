namespace SnakeAPI.Model.Auth.DataHolder
{
    public interface IHolder<in T>
    {
        void Create(T token, Snapshot snapshot);
        void Edit(T token, Snapshot snapshot);
        void Delete(T token);
        Snapshot Get(T token);
    }
}