using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UGDR
{
    public partial class Main : MetroFramework.Forms.MetroForm
    {
        public Main()
        {
            InitializeComponent();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {

            Thread td1 = new Thread(() => testtest());
            td1.Start();

        }
        void testtest()
        {
            Suzip sz1 = new Suzip();
            for (int test = 4743104; test <= 4743297; test++)
            {
                sz1.GetB(test);
            }
            
        }

    }
}
