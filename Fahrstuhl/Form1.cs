using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fahrstuhl
{
    public partial class Form1 : Form
    {

        private Dictionary<int, CheckBox> anforderungen = new Dictionary<int, CheckBox>();
        private Dictionary<int, RadioButton> etagen = new Dictionary<int, RadioButton>();

        private Fahrstuhl fahrstuhl;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            foreach (Control control in checkboxes.Controls)
            {
                if (control is CheckBox request)
                {
                    anforderungen.Add(GetEtage(request), request);
                }
            }

            foreach (Control control in radiobuttons.Controls)
            {
                if (control is RadioButton current)
                {
                    etagen.Add(GetEtage(current), current);
                }
            }

            fahrstuhl = new Fahrstuhl(1000, new Fahrstuhl.FahrstuhlAngekommenListener(Arrived), new Fahrstuhl.FahrstuhlPauseListener(Paused));
        }

        private void button_Click(object sender, EventArgs e)
        {
            int etage = GetEtage((Button)sender);

            if (anforderungen[etage].Checked = !anforderungen[etage].Checked)
            {
                fahrstuhl.Anforderung(etage);
            }
            else
            {
                fahrstuhl.Abbruch(etage);
            }
        }


        private void Arrived(int floor)
        {
            Invoke((MethodInvoker)delegate { etagen[floor].Checked = true; });
        }

        private void Paused(int floor)
        {
            Invoke((MethodInvoker)delegate { anforderungen[floor].Checked = false; });
        }

        private int GetEtage(Control control)
        {
            return int.Parse(control.Text.Replace("E", ""));
        }
    }
}
