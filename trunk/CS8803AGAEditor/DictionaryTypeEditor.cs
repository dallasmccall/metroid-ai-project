using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace CS8803AGAGameLibrary
{
    class DictionaryTypeEditor<K, V> : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value.GetType() != typeof(Dictionary<K, V>))
            {
                MessageBox.Show(
                    String.Format("Trying to use a Dictionary<{0},{1}> custom UITypeEditor for something which isn't one", typeof(K).ToString(), typeof(V).ToString()));
                return value;
            }

            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if( edSvc != null )
            {
                Dictionary<K,V> valueAsDict = (Dictionary<K,V>)value;

                edSvc.ShowDialog( new DictionaryEditor<K,V>(valueAsDict) );

                return valueAsDict;
            }
            return value;
        }
    }
}
