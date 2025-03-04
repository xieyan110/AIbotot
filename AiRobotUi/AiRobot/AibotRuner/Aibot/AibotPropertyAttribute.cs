using System;

namespace Aibot
{
    public enum AibotKeyUsage
    {
        Input,
        Output
    }

    /// <summary>
    /// Properties decorated with this attribute must always be of type <see cref="AibotProperty"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class AibotPropertyAttribute : Attribute
    {
        /// <summary>
        /// Properties decorated with this attribute must always be of type <see cref="AibotProperty"/>.
        /// </summary>
        /// <param name="name">The display name of the key.</param>
        /// <param name="type">The data type of the value that the key refers to.</param>
        public AibotPropertyAttribute(string? name, AibotKeyType type = AibotKeyType.Object)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Properties decorated with this attribute must always be of type <see cref="AibotProperty"/>.
        /// </summary>
        /// <param name="type">The data type of the value that the key refers to.</param>
        public AibotPropertyAttribute(AibotKeyType type = AibotKeyType.Object) : this(null, type)
        {

        }

        public string? Name { get; }

        public bool IsRoot { get; set; } = false;
        public AibotKeyType Type { get; }
        public AibotKeyUsage Usage { get; set; }
        public bool CanChangeType { get; set; }
    }
}
