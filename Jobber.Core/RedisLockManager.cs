using System;
using System.Net;
using StackExchange.Redis;

namespace Jobber.Core
{
    class RedisLockManager : ILockManager
    {
        private static readonly Lazy<RedisLockManager> _Instance = new Lazy<RedisLockManager>(() => new RedisLockManager());

        private RedisLockManager()
        {
        }

        public static RedisLockManager Instance => _Instance.Value;

        private static IDatabase _database;

        public RedisLockManager Initialize(EndPoint[] redisEndPoints, string password = null, int database = 0)
        {
            _database = RedisConnectionFactory.Instance.SetEndPoints(redisEndPoints).
                                                        GetConnection(password).
                                                        GetDatabase(database);

            return this;
        }

        public bool IsLocked(string key)
        {
            var result = _database.LockQuery(key);

            return result.HasValue;
        }

        public bool IsLockedForThisInstance(string key, string value)
        {
            var result = _database.LockQuery(key);

            if (result.HasValue && result == value)
            {
                return true;
            }

            return false;
        }

        public bool GetLock(string key, string value, TimeSpan expiryTime)
        {
            bool result = _database.LockTake(key, value, expiryTime);
            return result;
        }

        public void ReleaseLock(string key, string value)
        {
            _database.LockRelease(key, value);
        }

        public void ExtendLock(string key, string value, TimeSpan expiryTime)
        {
            _database.LockExtend(key, value, expiryTime);
        }
    }
}