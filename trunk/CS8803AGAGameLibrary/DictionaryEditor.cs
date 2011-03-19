using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace MetroidAIGameLibrary
{
    class DictionaryEditor<K,V> : Form
    {
        private Dictionary<K, V> m_dictionary;

        private Button m_bAdd;
        private Button m_bRemove;
        private ListBox m_lbEntries;

        private PropertyGrid m_pgProperties;

        private class KVRef
        {
            [NotifyParentProperty(true)]
            public K Key {get; set;}

            [NotifyParentProperty(true)]
            public V Value {get; set;}

            public KVRef()
            {
                Key = default(K);
                Value = default(V);
            }

            public KVRef(K key, V val)
            {
                Key = key;
                Value = val;
            }

            public KVRef(KeyValuePair<K, V> pair)
            {
                Key = pair.Key;
                Value = pair.Value;
            }

            public override string ToString()
            {
                return String.Format("{0} : {1}", Key.ToString(), Value.ToString());
            }
        }

        public DictionaryEditor( Dictionary<K,V> value )
        {
            InitializeComponent();

            m_dictionary = value;
            //m_pgProperties.SelectedObject = new DictionaryPropertyGridAdapter(m_dictionary);
            initializeListBoxEntries();
        }

        private void initializeListBoxEntries()
        {
            m_lbEntries.Items.Clear();
            foreach (KeyValuePair<K, V> pair in m_dictionary.ToList())
            {
                m_lbEntries.Items.Add(new KVRef(pair));
            }

            if (m_lbEntries.Items.Count > 0)
            {
                m_lbEntries.SelectedIndex = 0;
            }
        }

        private void InitializeComponent()
        {
            this.m_pgProperties = new System.Windows.Forms.PropertyGrid();
            this.m_bAdd = new System.Windows.Forms.Button();
            this.m_bRemove = new System.Windows.Forms.Button();
            this.m_lbEntries = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // m_pgProperties
            // 
            this.m_pgProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_pgProperties.Location = new System.Drawing.Point(267, 12);
            this.m_pgProperties.Name = "m_pgProperties";
            this.m_pgProperties.Size = new System.Drawing.Size(285, 487);
            this.m_pgProperties.TabIndex = 0;
            this.m_pgProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.m_pgProperties_PropertyValueChanged);
            // 
            // m_bAdd
            // 
            this.m_bAdd.Location = new System.Drawing.Point(13, 13);
            this.m_bAdd.Name = "m_bAdd";
            this.m_bAdd.Size = new System.Drawing.Size(75, 23);
            this.m_bAdd.TabIndex = 1;
            this.m_bAdd.Text = "Add";
            this.m_bAdd.UseVisualStyleBackColor = true;
            this.m_bAdd.Click += new System.EventHandler(this.m_bAdd_Click);
            // 
            // m_bRemove
            // 
            this.m_bRemove.Location = new System.Drawing.Point(94, 13);
            this.m_bRemove.Name = "m_bRemove";
            this.m_bRemove.Size = new System.Drawing.Size(75, 23);
            this.m_bRemove.TabIndex = 2;
            this.m_bRemove.Text = "Remove";
            this.m_bRemove.UseVisualStyleBackColor = true;
            this.m_bRemove.Click += new System.EventHandler(this.m_bRemove_Click);
            // 
            // m_lbEntries
            // 
            this.m_lbEntries.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.m_lbEntries.FormattingEnabled = true;
            this.m_lbEntries.Location = new System.Drawing.Point(13, 43);
            this.m_lbEntries.Name = "m_lbEntries";
            this.m_lbEntries.Size = new System.Drawing.Size(248, 329);
            this.m_lbEntries.TabIndex = 3;
            this.m_lbEntries.SelectedIndexChanged += new System.EventHandler(this.m_lbEntries_SelectedIndexChanged);
            // 
            // DictionaryEditor
            // 
            this.ClientSize = new System.Drawing.Size(564, 511);
            this.Controls.Add(this.m_lbEntries);
            this.Controls.Add(this.m_bRemove);
            this.Controls.Add(this.m_bAdd);
            this.Controls.Add(this.m_pgProperties);
            this.Name = "DictionaryEditor";
            this.ResumeLayout(false);

        }

        private void m_bAdd_Click(object sender, EventArgs e)
        {
            object[] nullConstructorArgs = new object[0];
            Type[] nullConstructorType = new Type[0];

            K key;
            V val;
            if (typeof(K) == typeof(string))
            {
                key = (K)"".Clone();
            }
            else
            {
                key = (K)Activator.CreateInstance(typeof(K), nullConstructorArgs);
            }

            if (typeof(V) == typeof(string))
            {
                val = (V)"".Clone();
            }
            else
            {
                val = (V)Activator.CreateInstance(typeof(V), nullConstructorArgs);
            }

            m_dictionary.Add(key, val);

            KVRef copy = new KVRef(key, val);
            m_lbEntries.Items.Add(copy);
            m_lbEntries.SelectedItem = copy;
        }

        private void m_bRemove_Click(object sender, EventArgs e)
        {
            if (m_lbEntries.SelectedItem != null)
            {
                if (m_lbEntries.SelectedItem == m_pgProperties.SelectedObject)
                {
                    m_pgProperties.SelectedObject = null;
                }

                m_dictionary.Remove(((KVRef)m_lbEntries.SelectedItem).Key);
                m_lbEntries.Items.Remove(m_lbEntries.SelectedItem);
                initializeListBoxEntries();
            }
        }

        private void m_lbEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_lbEntries.SelectedItem != null)
            {
                m_pgProperties.SelectedObject = m_lbEntries.SelectedItem;
            }
        }

        private void m_pgProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.Label == "Key")
            {
                K oldKey = (K)e.OldValue;
                K newKey = (K)e.ChangedItem.Value;
                V val = (V)m_dictionary[oldKey];

                m_dictionary.Remove(oldKey);
                m_lbEntries.Items.Remove(m_lbEntries.SelectedItem);

                m_dictionary.Add(newKey, val);

                KVRef copy = new KVRef(newKey, val);
                m_lbEntries.Items.Add(copy);
                m_lbEntries.SelectedItem = copy;
            }

            if (e.ChangedItem.Label == "Value")
            {
                KVRef curItem = (KVRef)m_pgProperties.SelectedObject;
                m_dictionary[curItem.Key] = curItem.Value;
            }
        }
    }
    /*
    class DictionaryPropertyGridAdapter : ICustomTypeDescriptor
    {
        IDictionary m_dictionary;

        public DictionaryPropertyGridAdapter(IDictionary d)
        {
            m_dictionary = d;
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return m_dictionary;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        PropertyDescriptorCollection
            System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            ArrayList properties = new ArrayList();
            foreach (DictionaryEntry e in m_dictionary)
            {
                properties.Add(new DictionaryPropertyDescriptor(m_dictionary, e.Key));
            }

            PropertyDescriptor[] props =
                (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));

            return new PropertyDescriptorCollection(props);
        }
    }

    class DictionaryPropertyDescriptor : PropertyDescriptor
    {
        IDictionary m_dictionary;
        object m_key;

        internal DictionaryPropertyDescriptor(IDictionary d, object key)
            : base(key.ToString(), null)
        {
            m_dictionary = d;
            m_key = key;
        }

        public override Type PropertyType
        {
            get { return m_dictionary[m_key].GetType(); }
        }

        public override void SetValue(object component, object value)
        {
            m_dictionary[m_key] = value;
        }

        public override object GetValue(object component)
        {
            return m_dictionary[m_key];
        }
        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type ComponentType
        {
            get { return null; }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }*/
}
