using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CS8803AGAGameLibrary
{
    public class SmartExpandableConverter<T> : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                if ((string)value == "")
                {
                    return null;
                }

                List<Type> types =
                    InstantiableSubclassManager.getInstantiableTypes(typeof(T));
                if (types.Count == 0)
                {
                    return base.ConvertFrom(context, culture, value);
                }
                if (types.Count == 1)
                {
                    return InstantiableSubclassManager.createInstance(types[0]);
                }
                BasicSelectionForm<Type> typeForm = new BasicSelectionForm<Type>(types);
                typeForm.ShowDialog();
                return InstantiableSubclassManager.createInstance(typeForm.SelectedItem);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
