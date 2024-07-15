using System;
using System.Diagnostics;

namespace Aibot
{
    [DebuggerDisplay("{IsKey ? Key : Value}")]
    public struct AibotProperty : IEquatable<AibotProperty>
    {
        public static AibotProperty Invalid { get; } = new AibotProperty();

        public AibotProperty(AibotKey key)
        {
            Key = key;
            Value = default;
        }

        public AibotProperty(object? value)
        {
            Key = AibotKey.Invalid;
            Value = value;
        }

        public AibotKey Key { get; }
        public object? Value { get; }

        public bool IsKey => Key.IsValid();
        public bool IsValue => !IsKey;

        public static implicit operator AibotKey(AibotProperty action)
            => action.Key;

        public override bool Equals(object? obj)
            => obj is AibotProperty action && action.Equals(this);

        public override int GetHashCode()
            => IsKey ? Key.GetHashCode() : Value?.GetHashCode() ?? -1;

        public bool Equals(AibotProperty other)
            => IsKey == other.IsKey && IsValue == other.IsValue && Key == other.Key && Value == other.Value;

        public static bool operator ==(AibotProperty left, AibotProperty right)
            => left.Equals(right);

        public static bool operator !=(AibotProperty left, AibotProperty right)
            => !(left == right);

        public T? GetValue<T>() where T : struct
            => Value is T result ? result : default;

        public T? GetObject<T>() where T : class
            => Value as T;
    }
}
