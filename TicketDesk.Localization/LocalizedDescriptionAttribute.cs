namespace TicketDesk.Localization
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.All)]
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private PropertyInfo _nameProperty;
        private Type _resourceType;

        public LocalizedDescriptionAttribute(string displayNameKey)
            : base(displayNameKey)
        {
        }

        public Type NameResourceType
        {
            get
            {
                return _resourceType;
            }

            set
            {
                _resourceType = value;

                _nameProperty = _resourceType.GetProperty(Description, BindingFlags.Static | BindingFlags.Public);
            }
        }

        public override string Description
        {
            get
            {
                // check if nameProperty is null and return original display name value
                if (_nameProperty == null)
                {
                    return base.Description;
                }

                return (string)_nameProperty.GetValue(_nameProperty.DeclaringType, null);
            }
        }
    }
}