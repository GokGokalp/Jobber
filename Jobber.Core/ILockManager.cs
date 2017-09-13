using System;

namespace Jobber.Core
{
    interface ILockManager
    {
        bool IsLocked(string key);
        bool IsLockedForThisInstance(string key, string value);
        bool GetLock(string key, string value, TimeSpan expiryTime);
        void ReleaseLock(string key, string value);
        void ExtendLock(string key, string value, TimeSpan expiryTime);
    }
}