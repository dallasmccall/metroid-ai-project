using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Collections.Specialized;
using CS8803AGAGameLibrary;

namespace CS8803AGAEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            TypeDescriptor.AddAttributes(
                typeof(Array),
                new EditorAttribute(typeof(SmartArrayEditor), typeof(UITypeEditor)));

            Application.Run(new ContentTypeSelector());
        }
    }
}

