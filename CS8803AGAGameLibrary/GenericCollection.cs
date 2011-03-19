using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing.Design;

// http://www.codeproject.com/KB/tabs/customizingcollectiondata.aspx

namespace CS8803AGAGameLibrary
{
    public class TestClass
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [RefreshProperties(RefreshProperties.All)]
        public GenericCollection<Int32> ints { get; set; }

        public TestClass()
        {
            ints = new GenericCollection<Int32>();
            ints.Add(42);
            ints.Add(73);
            ints.Add(69);
        }
    }

    public class TestClass2
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        //[RefreshProperties(RefreshProperties.All)]
        [NotifyParentProperty(true)]
        public GenericCollection<Dummy> ints { get; set; }

        public TestClass2()
        {
            ints = new GenericCollection<Dummy>();
            //ints.Add(42);
            //ints.Add(73);
            //ints.Add(69);
        }
    }

    /// <summary>
    /// Don't use this, it doesn't let you Add/Remove correctly.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Editor(typeof(GenericCollectionEditor), typeof(UITypeEditor))]
    public class GenericCollection<T> : CollectionBase, ICustomTypeDescriptor
    {
        public override string ToString()
        {
            return this.List.Count.ToString();
        }

        public void Add(T t)
        {
            this.List.Add(t);
        }

        public void Remove(T t)
        {
            this.List.Remove(t);
        }

        public T this[int index]
        {
            get
            {
                return (T)this.List[index];
            }
        }

        #region ICustomTypeDescriptor Members

        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

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

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            // Create a new collection object PropertyDescriptorCollection

            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            // Iterate the list of items

            for (int i = 0; i < this.List.Count; i++)
            {
                // For each item create a property descriptor 
                // and add it to the 
                // PropertyDescriptorCollection instance

                CollectionPropertyDescriptor<T> pd = new
                              CollectionPropertyDescriptor<T>(this, (T)this.List[i]);
                pds.Add(pd);
            }
            return pds;
        }


        #endregion
    }

    public class CollectionPropertyDescriptor<T> : PropertyDescriptor
    {
        private GenericCollection<T> collection = null;
        private T t = default(T);

        public CollectionPropertyDescriptor(GenericCollection<T> coll, T t)
            : base( t.ToString(), null )
        {
            this.collection = coll;
            this.t = t;
        } 

        public override AttributeCollection Attributes
        {
            get 
            { 
                return new AttributeCollection(null);
            }
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get 
            {
                return this.collection.GetType();
            }
        }

        public override string DisplayName
        {
            get 
            {
                return t.ToString();
            }
        }

        public override string Description
        {
            get
            {
                return this.DisplayName;
            }
        }

        public override object GetValue(object component)
        {
            return t;
        }

        public override bool IsReadOnly
        {
            get { return true;  }
        }

        public override string Name
        {
            get { return t.ToString(); }
        }

        public override Type PropertyType
        {
            get { return t.GetType(); }
        }

        public override void ResetValue(object component) {}

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override void SetValue(object component, object value)
        {
            
        }
    }

    public class GenericCollectionEditor : CollectionEditor
    {
        private static Dictionary<Type, List<Type>> s_typeLookup =
            new Dictionary<Type, List<Type>>();

        public static event EventHandler ItemsChanged;
        protected static void OnItemsChanged(GenericCollectionEditor sender)
        {
            if (ItemsChanged != null)
            {
                ItemsChanged(sender, EventArgs.Empty);
            }
        }

        public GenericCollectionEditor(Type t)
            : base(t)
        {
            // nch
        }

        /*protected override object CreateInstance(Type itemType)
        {
            if (!s_typeLookup.ContainsKey(itemType))
            {

            }
            return base.CreateInstance(itemType);
        }*/

        protected override void DestroyInstance(object instance)
        {
            base.DestroyInstance(instance);
            this.Context.OnComponentChanged();
            GenericCollectionEditor.OnItemsChanged(this);
        }
    }
}
