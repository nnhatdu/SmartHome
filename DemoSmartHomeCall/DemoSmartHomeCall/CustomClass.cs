using System;
using System.Collections;
using System.ComponentModel;

namespace DemoSmartHomeCall
{
    /// <summary>
    ///     CustomClass (Which is binding to property grid)
    /// </summary>
    public class CustomClass : CollectionBase, ICustomTypeDescriptor
    {
        /// <summary>
        ///     Indexer
        /// </summary>
        public CustomProperty this[int index]
        {
            get => (CustomProperty) List[index];
            set => List[index] = value;
        }

        /// <summary>
        ///     Add CustomProperty to Collectionbase List
        /// </summary>
        /// <param name="Value"></param>
        public void Add(CustomProperty Value)
        {
            List.Add(Value);
        }

        /// <summary>
        ///     Remove item from List
        /// </summary>
        /// <param name="Name"></param>
        public void Remove(string Name)
        {
            foreach (CustomProperty prop in List)
                if (prop.Name == Name)
                {
                    List.Remove(prop);
                    return;
                }
        }


        #region "TypeDescriptor Implementation"

        /// <summary>
        ///     Get Class Name
        /// </summary>
        /// <returns>String</returns>
        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        ///     GetAttributes
        /// </summary>
        /// <returns>AttributeCollection</returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        ///     GetComponentName
        /// </summary>
        /// <returns>String</returns>
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        /// <summary>
        ///     GetConverter
        /// </summary>
        /// <returns>TypeConverter</returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        /// <summary>
        ///     GetDefaultEvent
        /// </summary>
        /// <returns>EventDescriptor</returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        /// <summary>
        ///     GetDefaultProperty
        /// </summary>
        /// <returns>PropertyDescriptor</returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        /// <summary>
        ///     GetEditor
        /// </summary>
        /// <param name="editorBaseType">editorBaseType</param>
        /// <returns>object</returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var newProps = new PropertyDescriptor[Count];
            for (var i = 0; i < Count; i++)
            {
                var prop = this[i];
                newProps[i] = new CustomPropertyDescriptor(ref prop, attributes);
            }

            return new PropertyDescriptorCollection(newProps);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }

    /// <summary>
    ///     Custom property class
    /// </summary>
    public class CustomProperty
    {
        public CustomProperty(string sName, object value, Type type, bool bReadOnly, bool bVisible)
        {
            Name = sName;
            Value = value;
            Type = type;
            ReadOnly = bReadOnly;
            Visible = bVisible;
        }

        public Type Type { get; }

        public bool ReadOnly { get; }

        public string Name { get; } = string.Empty;

        public bool Visible { get; } = true;

        public object Value { get; set; }
    }


    /// <summary>
    ///     Custom PropertyDescriptor
    /// </summary>
    public class CustomPropertyDescriptor : PropertyDescriptor
    {
        private readonly CustomProperty m_Property;

        public CustomPropertyDescriptor(ref CustomProperty myProperty, Attribute[] attrs) : base(myProperty.Name, attrs)
        {
            m_Property = myProperty;
        }

        #region PropertyDescriptor specific

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType => null;

        public override object GetValue(object component)
        {
            return m_Property.Value;
        }

        public override string Description => m_Property.Name;

        public override string Category => string.Empty;

        public override string DisplayName => m_Property.Name;

        public override bool IsReadOnly => m_Property.ReadOnly;

        public override void ResetValue(object component)
        {
            //Have to implement
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override void SetValue(object component, object value)
        {
            m_Property.Value = value;
        }

        public override Type PropertyType => m_Property.Type;

        #endregion
    }
}