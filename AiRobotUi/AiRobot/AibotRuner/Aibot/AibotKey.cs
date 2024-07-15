using System;
using System.Diagnostics;

namespace Aibot
{
    public enum AibotKeyType
    {
        Boolean,
        Integer,
        Double,
        String,
        List,
        Object
    }

    [DebuggerDisplay("{Name}: {Type}")]
    public readonly struct AibotKey : IEquatable<AibotKey>
    {
        public static AibotKey Invalid { get; } = new AibotKey();

        public AibotKey(string name, AibotKeyType type)
        {
            Name = name ?? throw new ArgumentException(nameof(name));
            Type = type;
        }

        public AibotKey(string name) : this(name, AibotKeyType.Object)
        {
        }

        public readonly string Name;
        public readonly AibotKeyType Type;

        public static implicit operator AibotKey(string name)
            => new AibotKey(name);

        public static implicit operator string(AibotKey key)
            => key.Name;

        public override bool Equals(object? obj)
            => obj is AibotKey bk && bk.Equals(this);

        public override int GetHashCode()
            => Name?.GetHashCode() ?? -1;

        public bool Equals(AibotKey other)
            => other.Name == Name;

        public static bool operator ==(AibotKey left, AibotKey right)
            => left.Equals(right);

        public static bool operator !=(AibotKey left, AibotKey right)
            => !(left == right);
    }
}
