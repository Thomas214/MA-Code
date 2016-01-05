using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Foggy
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public string distance { get; set; }
        

        private void button1_Click(object sender, EventArgs e)
        {
            distance = txt_distance.Text;
            this.Close();
        }
    }
}
