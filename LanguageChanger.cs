using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace HBYS
{

    class LanguageChanger
    {
        public static String language;
        public static void ChangeLanguage(String lang)
        {
            CultureInfo cultureInfo = new CultureInfo(lang);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            language = lang;

            if (language.Equals(ConfigurationManager.AppSettings["language"]))
                return;

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            foreach (XmlElement xmlElement in xmlDoc.DocumentElement)
                if (xmlElement.Name.Equals("appSettings"))
                    foreach (XmlNode xmlNode in xmlElement.ChildNodes)
                        if (xmlNode.Attributes[0].Value.Equals("language"))
                            xmlNode.Attributes[1].Value = lang;

            ConfigurationManager.RefreshSection("appSettings");

            xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            Size oldSize;

            List<FormSize> formList = new List<FormSize>();
            foreach (Form form in Application.OpenForms)
                formList.Add(new FormSize(form));

            for ( int i = 0; i < formList.Count; i ++)
            {
                LocalizeForm( formList[i].form);
                formList[i].form.Size = formList[i].size;
                if (formList[i].isMaximized)
                {
                    formList[i].form.WindowState = FormWindowState.Normal;
                    formList[i].form.WindowState = FormWindowState.Maximized;
                }
            }
        }

        private static void LocalizeForm(Form form)
        {
            ComponentResourceManager manager = new ComponentResourceManager(form.GetType());
            manager.ApplyResources(form, "$this");
            ApplyControls(manager, form.Controls);
        }

        private static void ApplyControls(ComponentResourceManager manager, Control.ControlCollection ctrls)
        {
            foreach (Control ctrl in ctrls)
            {
                if (ctrl is ToolStrip)
                {
                    ApplyToolStripItems(manager, ((ToolStrip)ctrl).Items);
                    manager.ApplyResources(ctrl, ctrl.Name);
                }
                else if (ctrl is DataGridView)
                {
                    ApplyDataGridColumns(manager, ((DataGridView)ctrl).Columns);
                    manager.ApplyResources(ctrl, ctrl.Name);
                }
                else
                {
                    ApplyControls(manager, ctrl.Controls);
                    manager.ApplyResources(ctrl, ctrl.Name);
                }
            }
        }

        private static void ApplyToolStripItems(ComponentResourceManager manager, ToolStripItemCollection tstripitmes)
        {
            foreach (ToolStripItem tstripitem in tstripitmes)
            {
                manager.ApplyResources(tstripitem, tstripitem.Name);
                if ( tstripitem is ToolStripMenuItem)
                    ApplyToolStripDropDownItems(manager, ((ToolStripMenuItem)tstripitem).DropDownItems);
            }
        }
        private static void ApplyToolStripDropDownItems(ComponentResourceManager manager, ToolStripItemCollection dropdownitems)
        {
            foreach (ToolStripItem dropdown in dropdownitems)
                if (dropdown is ToolStripDropDownItem)
                {
                    if ( dropdown.Text.StartsWith("&"))
                        Console.WriteLine(dropdown);
                    manager.ApplyResources(dropdown, dropdown.Name);
                    ApplyToolStripDropDownItems(manager, ((ToolStripDropDownItem)dropdown).DropDownItems);
                }
        }

        private static void ApplyDataGridColumns(ComponentResourceManager manager, DataGridViewColumnCollection columns)
        {
            foreach (DataGridViewColumn column in columns)
            {
                manager.ApplyResources(column, column.Name);
            }
        }

        class FormSize
        {
            public bool isMaximized;
            public Size size;
            public Form form;

            public FormSize( Form form)
            {
                this.form = form;
                size = form.Size;
                isMaximized = form.WindowState == FormWindowState.Maximized;
            }
        }

    }
}
