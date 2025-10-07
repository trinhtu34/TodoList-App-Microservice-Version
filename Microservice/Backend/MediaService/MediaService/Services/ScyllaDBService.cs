using Cassandra;
using DotNetEnv;

namespace MediaService.Services
{
    public class ScyllaDBService : IDisposable
    {

        private readonly Cluster _cluster;
        private readonly Cassandra.ISession _session;

        public Cassandra.ISession Session => _session;

        public ScyllaDBService()
        {
            Env.Load(); // đọc file .env nếu có

            var host = Env.GetString("SCYLLADB_HOST");
            var port = Env.GetInt("SCYLLADB_PORT");
            var keyspace = Env.GetString("SCYLLADB_KEYSPACE");

            _cluster = Cluster.Builder()
                .AddContactPoint(host)
                .WithPort(port)
                .Build();

            _session = _cluster.Connect(keyspace);
        }

        public RowSet GetMessages(Guid channelId)
        {
            var query = "SELECT * FROM messages WHERE channel_id = ?";
            var stmt = _session.Prepare(query);
            return _session.Execute(stmt.Bind(channelId));
        }

        public void InsertMessage(Guid channelId, Guid userId, string content)
        {
            var query = "INSERT INTO messages (channel_id, message_id, user_id, content, created_at) VALUES (?, now(), ?, ?, toTimestamp(now()))";
            var stmt = _session.Prepare(query);
            _session.Execute(stmt.Bind(channelId, userId, content));
        }

        public void Dispose()
        {
            _session?.Dispose();
            _cluster?.Dispose();
        }
    }
}
