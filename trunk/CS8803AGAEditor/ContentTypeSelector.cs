using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using MetroidAIGameLibrary;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace MetroidAIEditor
{
    public partial class ContentTypeSelector : Form
    {
        protected PropertyGrid m_pgActiveProperties;

        public ContentTypeSelector()
        {
            InitializeComponent();

            GenericCollectionEditor.ItemsChanged += new EventHandler(ItemsCollectionEditor_DestroyInst);

            // If you get rid of the CharacterInfo class, this line won't compile.
            // Just make sure to give it some type of class in the GameLibrary assembly
            //  so that it can easily retrieve the assembly.
            Assembly asm = Assembly.GetAssembly(typeof(CharacterInfo));

            foreach (Type type in asm.GetTypes())
            {
                m_lbTypes.Items.Add(type);
            }
        }

        private void ItemsCollectionEditor_DestroyInst(object sender, EventArgs e)
        {
            m_pgActiveProperties.Refresh();
        }

        private void m_lbTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_lbTypes.SelectedItem != null)
            {
                m_bCreateNew.Enabled = true;
            }
            else
            {
                m_bCreateNew.Enabled = false;
            }
        }

        private void createPropertyGridTab(object objectToEdit, string title)
        {
            createPropertyGridTab(objectToEdit, title, null);
        }

        private void createPropertyGridTab(object objectToEdit, string title, object propGridTag)
        {
            PropertyGrid pg = new PropertyGrid();
            pg.Dock = DockStyle.Fill;
            pg.SelectedObject = objectToEdit;
            pg.Tag = propGridTag;

            m_pgActiveProperties = pg;

            TabPage tp = new TabPage(title);
            m_newCounter++;
            tp.Controls.Add(pg);
            tp.Tag = pg; // I'm a bad person

            m_tcTabs.TabPages.Add(tp);
            m_tcTabs.SelectedTab = tp;
        }

        private static int m_newCounter = 0;

        private void m_bCreateNew_Click(object sender, EventArgs e)
        {
            try
            {
                Type type = (Type)m_lbTypes.SelectedItem;

                object[] args = new object[0];
                object o = Activator.CreateInstance(type, args);

                createPropertyGridTab(o, String.Format("New{0}", m_newCounter));
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}\n\n{1}",ex.Message,ex.StackTrace));
            }
        }

        private void m_bLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select an object";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (XmlReader reader = XmlReader.Create(ofd.FileName))
                {
                    object result = IntermediateSerializer.Deserialize<object>(reader, null);
                    createPropertyGridTab(result, ofd.SafeFileName, ofd.FileName);
                }
            }
        }

        private void m_bSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            PropertyGrid pg = (PropertyGrid)m_tcTabs.SelectedTab.Tag;
            if (pg.Tag != null)
            {
                String originalFilePath = (string)pg.Tag;
                sfd.InitialDirectory = originalFilePath.Substring(0, originalFilePath.LastIndexOf('\\'));
                sfd.FileName = originalFilePath.Substring(originalFilePath.LastIndexOf('\\')+1);
            }

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;

                using (XmlWriter writer = XmlWriter.Create(sfd.FileName, settings))
                {
                    IntermediateSerializer.Serialize(writer, pg.SelectedObject, null);
                }

                m_tcTabs.SelectedTab.Text = sfd.FileName.Substring(sfd.FileName.LastIndexOf('\\'));
                pg.Tag = sfd.FileName;
            }
        }

        private void m_bPicEditor_Click(object sender, EventArgs e)
        {
            if (m_pgActiveProperties != null &&
                m_pgActiveProperties.SelectedObject is AnimationSet)
            {
                SpriteSheetPreviewer ssp =
                    new SpriteSheetPreviewer(
                        (AnimationSet)m_pgActiveProperties.SelectedObject
                        );
                ssp.StartPosition = FormStartPosition.CenterParent;

                ssp.Show();
                ssp.AnimationExported += new AnimationExportEventHandler(HandleAnimExport);
            }
            else
            {
                SpriteSheetPreviewer ssp = new SpriteSheetPreviewer();
                ssp.StartPosition = FormStartPosition.CenterParent;

                ssp.Show();
                ssp.AnimationExported += new AnimationExportEventHandler(HandleAnimExport);
            }
        }

        private void HandleAnimExport(object sender, AnimationSet animSet)
        {
            this.createPropertyGridTab(animSet, "AnimSet");
        }

        private void m_tcTabs_Selected(object sender, TabControlEventArgs e)
        {
            PropertyGrid pg = (PropertyGrid)m_tcTabs.SelectedTab.Tag;
            m_pgActiveProperties = pg;
        }
    }
}
