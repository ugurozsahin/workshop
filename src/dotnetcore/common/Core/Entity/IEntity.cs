namespace Core.Entity
{
    public interface IEntity<TId>
    {
        TId Id { get; set; }
    }
}
