namespace CacheAside
{
    using StackExchange.Redis;

    class Program
    {
        static void Main(string[] args)
        {
            var store = new Store<MyEntity>();
            store.Add(new MyEntity() { Id = 1 });

            string cacheConnectionString = "";

            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(cacheConnectionString);

            var repo = new CacheAsideRepository<MyEntity>(store, connection.GetDatabase());
            var entity = repo.GetById(1);
            entity = repo.GetById(1);
            repo.Update(entity);
            entity = repo.GetById(1);
        }
    }
}

