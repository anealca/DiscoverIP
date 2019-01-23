using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPServer
{
    public partial class KeyPad : Form
    {
        double value = 0;
        string sValue = "";

        public KeyPad() {
            InitializeComponent();
            tValue.Text = "0";
        }

        private void bZero_Click(object sender, EventArgs e) {
            if (value > 0) {
                sValue += '0';
                tValue.Text = sValue;
            }
        }
        private void bOne_Click(object sender, EventArgs e) {
            sValue += '1';
            tValue.Text = sValue;
        }
        private void bTwo_Click(object sender, EventArgs e) {
            sValue += '2';
            tValue.Text = sValue;
        }
        private void bThree_Click(object sender, EventArgs e) {
            sValue += '3';
            tValue.Text = sValue;
        }
        private void bFour_Click(object sender, EventArgs e) {
            sValue += '4';
            tValue.Text = sValue;
        }
        private void bFive_Click(object sender, EventArgs e) {
            sValue += '5';
            tValue.Text = sValue;
        }
        private void bSix_Click(object sender, EventArgs e) {
            sValue += '6';
            tValue.Text = sValue;
        }
        private void bSeven_Click(object sender, EventArgs e) {
            sValue += '7';
            tValue.Text = sValue;
        }
        private void bEight_Click(object sender, EventArgs e) {
            sValue += '8';
            tValue.Text = sValue;
        }
        private void bNine_Click(object sender, EventArgs e) {
            sValue += '9';
            tValue.Text = sValue;
        }
        private void bClear_Click(object sender, EventArgs e) {
            sValue = "";
            tValue.Text = "0";
        }
        private void bBack_Click(object sender, EventArgs e) {
            sValue = sValue.Substring(0, sValue.Length - 1);
            tValue.Text = sValue;
        }
        private void bDecimal_Click(object sender, EventArgs e) {
            sValue += '.';
            tValue.Text = sValue;
        }

        private void bSave_Click(object sender, EventArgs e) {
            try {
                double value = double.Parse(sValue);
                // TODO: How to update the text box in the parent form
                //UDPServer.UpdateKeypadText(value);
                //keypadValue = value;
                //UpdateKeypadText(value);
            }
            catch (Exception) { }
            this.Hide();
        }
    }
}
