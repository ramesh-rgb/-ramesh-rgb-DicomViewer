using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FellowOakDicom
{
    public partial class DicomTagg : Form
    {
        List<string> str;
        public DicomTagg()
        {
            InitializeComponent();
        }


        public void SetString(ref List<string> strg)
        {
            str = strg;
            string s1, s4, s5, s11, s12;

            // Add items to the List View Control
            for (int i = 0; i < str.Count; ++i)
            {
                s1 = str[i];
                ExtractStrings(s1, out s4, out s5, out s11, out s12);
                ListViewItem lvi = new ListViewItem(s11);
                lvi.SubItems.Add(s12);
                lvi.SubItems.Add(s4);
                lvi.SubItems.Add(s5);
                listView.Items.Add(lvi);
            }
        }
        void ExtractStrings(string s1, out string s4, out string s5, out string s11, out string s12)
        {
            int ind;
            string s2, s3;
            ind = s1.IndexOf("//");
            s2 = s1.Substring(0, ind);
            s11 = s1.Substring(0, 4);
            s12 = s1.Substring(4, 4);
            s3 = s1.Substring(ind + 2);
            ind = s3.IndexOf(":");
            s4 = s3.Substring(0, ind);
            s5 = s3.Substring(ind + 1);
        }
    }
}
