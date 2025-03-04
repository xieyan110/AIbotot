using Nodify;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace Aibot
{
    public static class Lock
    {
        private static ConcurrentDictionary<string, SemaphoreSlim> Locks = new ConcurrentDictionary<string, SemaphoreSlim>();

        public static SemaphoreSlim GetOrCreate(string lockName)
        {
            return Locks.GetOrAdd(lockName, _ => new SemaphoreSlim(1, 1));
        }

        public static bool Exists(string lockName)
        {
            return Locks.ContainsKey(lockName);
        }

        public static bool IsLocked(string lockName)
        {
            if (Locks.TryGetValue(lockName, out var semaphore))
            {
                return semaphore.CurrentCount == 0;
            }
            return false;
        }
    }



}
