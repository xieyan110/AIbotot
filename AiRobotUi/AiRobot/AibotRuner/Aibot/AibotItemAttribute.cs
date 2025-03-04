using System;

namespace Aibot
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class AibotItemAttribute : Attribute
    {
        public AibotItemAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        public string DisplayName { get; }

        public ActionType ActionType { get; set; }

        public ActionViewType ActionViewType { get; set; }
    }
}
