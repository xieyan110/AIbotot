using System.Collections.Generic;

namespace Aibot
{
    public class AibotV
    {

        public OperationViewModel? Node { get; set; }

        private readonly Dictionary<AibotKey, object?> _objects = new Dictionary<AibotKey, object?>();

        public virtual IReadOnlyCollection<AibotKey> Keys
            => _objects.Keys;

        public virtual T? GetValue<T>(AibotKey key)
            where T : struct
        {
            if (_objects.TryGetValue(key, out var value) && value is T result)
            {
                return result;
            }

            return default;
        }

        public virtual T? GetObject<T>(AibotKey key)
            where T : class
        {
            if (_objects.TryGetValue(key, out var value))
            {
                return value as T;
            }

            return default;
        }

        public virtual object? GetObject(AibotKey key)
        {
            if (_objects.TryGetValue(key, out var value))
            {
                return value;
            }

            return default;
        }

        public virtual void Set(AibotKey key, object? value)
            => _objects[key] = value;

        public virtual bool HasKey(AibotKey key)
            => _objects.ContainsKey(key);

        public virtual void Remove(AibotKey key)
            => _objects.Remove(key);

        public virtual void Clear()
            => _objects.Clear();

        public void CopyTo(AibotV newAibot)
        {
            foreach (var kvp in _objects)
            {
                newAibot.Set(kvp.Key, kvp.Value);
            }
        }

        public object? this[AibotKey key]
        {
            get => GetObject(key);
            set => Set(key, value);
        }

        public T? GetValue<T>(AibotProperty value) where T : struct
            => value.IsValue ? value.GetValue<T>() : GetValue<T>(value.Key);

        public T? GetObject<T>(AibotProperty value) where T : class
            => value.IsValue ? value.GetObject<T>() : GetObject<T>(value.Key);

        public object? GetObject(AibotProperty value)
            => value.IsValue ? value.Value : GetObject(value.Key);
    }

    public static class AibotExtensions
    {
        public static bool IsValid(this AibotKey key)
            => key != AibotKey.Invalid;

        public static bool IsValid(this AibotProperty action)
            => action != AibotProperty.Invalid;
    }
}
