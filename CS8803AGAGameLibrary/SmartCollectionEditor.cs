using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;

namespace CS8803AGAGameLibrary
{
    /// <summary>
    /// A collection editor which, when used on Collections of types with
    /// subclasses, presents the user with choices of which subclass to
    /// instantiate upon pressing the Add button.
    /// 
    /// Usage: [Editor(typeof(SmartCollectionEditor), typeof(UITypeEditor)]
    ///        public List<Foo> foos;
    ///        Make sure to allocate the collection during owner construction.
    /// </summary>
    public class SmartCollectionEditor : CollectionEditor
    {
        public SmartCollectionEditor(Type t)
            : base(t)
        {
            // nch
        }

        /// <summary>
        /// Creates an instance of type contained in the collection.
        /// If subclasses of the type exist, let user choose via a form.
        /// </summary>
        protected override object CreateInstance(Type itemType)
        {
            if (this.CollectionType.IsArray)
            {
                throw new Exception("A SmartCollectionEditor has been assigned to an Array type or object, use SmartArrayEditor instead");
            }

            if (itemType == typeof(string) || itemType == typeof(String))
            {
                return "".Clone();
            }

            List<Type> creatableTypes = InstantiableSubclassManager.getInstantiableTypes(itemType);

            if (creatableTypes.Count == 1)
            {
                return base.CreateInstance(creatableTypes[0]);
            }
            if (creatableTypes.Count == 0)
            {
                // will probably err, as it should
                return base.CreateInstance(itemType);
            }

            BasicSelectionForm<Type> typeSelectionForm =
                new BasicSelectionForm<Type>(creatableTypes);

            typeSelectionForm.ShowDialog();

            return base.CreateInstance(typeSelectionForm.SelectedItem);
        }
    }

    /// <summary>
    /// An array editor which, when used on Arrays of types with
    /// subclasses, presents the user with choices of which subclass to
    /// instantiate upon pressing the Add button.
    /// 
    /// Usage: [Editor(typeof(SmartArrayEditor), typeof(UITypeEditor)]
    ///        public Foo[] foos;
    ///        You should try allocating the array to length 0 in the owner's
    ///        constructor, and if that doesn't work try not allocating it.
    /// </summary>
    public class SmartArrayEditor : ArrayEditor
    {
        public SmartArrayEditor(Type t)
            : base(t)
        {
            // nch
        }

        /// <summary>
        /// Creates an instance of type contained in the array.
        /// If subclasses of the type exist, let user choose via a form.
        /// </summary>
        protected override object CreateInstance(Type itemType)
        {
            if (this.CollectionType.IsArray)
            {
                if (this.CollectionType.HasElementType)
                {
                    itemType = this.CollectionType.GetElementType();
                }
            }
            else
            {
                throw new Exception("A SmartArrayEditor has been attached to a non-Array object or type");
            }

            if (itemType == typeof(string) || itemType == typeof(String))
            {
                return "".Clone();
            }

            List<Type> creatableTypes = InstantiableSubclassManager.getInstantiableTypes(itemType);

            if (creatableTypes.Count == 1)
            {
                return base.CreateInstance(creatableTypes[0]);
            }
            if (creatableTypes.Count == 0)
            {
                // will probably err, as it should
                return base.CreateInstance(itemType);
            }

            BasicSelectionForm<Type> typeSelectionForm =
                new BasicSelectionForm<Type>(creatableTypes);

            typeSelectionForm.ShowDialog();

            return base.CreateInstance(typeSelectionForm.SelectedItem);
        }
    }

    /// <summary>
    /// Static class for managing concrete subclasses of types.
    /// Uses reflection to find subclasses, then caches results.
    /// </summary>
    public static class InstantiableSubclassManager
    {
        private static Dictionary<Type, List<Type>> s_typeLookup =
            new Dictionary<Type, List<Type>>();

        /// <summary>
        /// Finds a list of all Types which are derived from the provided type
        /// and have arguments with null constructors, plus that type itself.
        /// </summary>
        /// <param name="itemType">Type which can be instantiated </param>
        /// <returns>List of all concrete subclasses of itemType, plus itemType if it is concrete.</returns>
        public static List<Type> getInstantiableTypes(Type itemType)
        {
            List<Type> creatableTypes;
            if (!s_typeLookup.TryGetValue(itemType, out creatableTypes))
            {
                creatableTypes = new List<Type>();

                if (canBeCreated(itemType))
                {
                    creatableTypes.Add(itemType);
                }

                Assembly assembly = Assembly.GetAssembly(itemType);
                foreach (Type testType in assembly.GetTypes())
                {
                    if (testType.IsSubclassOf(itemType) && canBeCreated(testType))
                    {
                        creatableTypes.Add(testType);
                    }
                }

                s_typeLookup[itemType] = creatableTypes;
            }

            return creatableTypes;
        }

        /// <summary>
        /// Whether a given Type is both concrete and has an empty ctor.
        /// </summary>
        /// <param name="type">Type for checking.</param>
        /// <returns>True if type can be instantiated with an empty ctor.</returns>
        public static bool canBeCreated(Type type)
        {
            Type[] nullConstructorArgs = new Type[] { };
            ConstructorInfo cons = type.GetConstructor(nullConstructorArgs);
            if (cons != null && !type.IsAbstract)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Create an instance of a type using its default ctor.
        /// </summary>
        /// <param name="type">Type to construct.</param>
        /// <returns>Instance of type if it can be created, null otherwise.</returns>
        public static object createInstance(Type type)
        {
            if (canBeCreated(type))
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }

    /// <summary>
    /// A basic form which forces a user to select from a list of objects.
    /// Upon clicking an item, it closes and the choice can be retrieved.
    /// </summary>
    /// <typeparam name="T">Type of objects to be included in the form.</typeparam>
    public class BasicSelectionForm<T> : Form
    {
        public T SelectedItem { get; private set; }

        private ListBox m_listBox;

        public BasicSelectionForm(List<T> items)
        {
            this.StartPosition = FormStartPosition.CenterParent;

            m_listBox = new ListBox();
            m_listBox.Dock = DockStyle.Fill;

            foreach (T item in items)
            {
                m_listBox.Items.Add(item);
            }

            m_listBox.SelectedIndexChanged += new EventHandler(ItemSelected);

            this.Controls.Add(m_listBox);
        }

        public void ItemSelected(object sender, EventArgs e)
        {
            SelectedItem = (T)m_listBox.SelectedItem;
            this.Close();
        }
    }

    # region Test Classes

    public class SmartCollectionEditorTestClass
    {
        [NotifyParentProperty(true)]
        [Editor(typeof(SmartCollectionEditor), typeof(UITypeEditor))]
        public List<Dummy> dummyCollection { get; set; }

        [NotifyParentProperty(true)]
        public Dummy[] dummyArray { get; set; }

        public String[] stringArray {get; set;}

        public SmartCollectionEditorTestClass()
        {
            dummyArray = new Dummy[0];
            dummyCollection = new List<Dummy>();
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Dummy
    {
        public string firstName { get; set; }
        public string lastName { get; set; }

        public Dummy()
        {
            // nch
        }
    }

    public class Dummy2 : Dummy
    {
        public string organization { get; set; }

        public Dummy2()
            : base()
        { }
    }
    public class Dummy3 : Dummy
    {
        public string title { get; set; }

        public Dummy3()
            : base()
        { }
    }

    #endregion
}
