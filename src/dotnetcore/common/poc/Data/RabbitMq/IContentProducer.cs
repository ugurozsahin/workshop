using Entity;

namespace Data.RabbitMq
{
    public interface IContentProducer
    {
        void Produce(Content content);
    }
}