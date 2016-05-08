using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KruskalAlgorithm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static int[][] weights;
        public static int N;

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                weights = File.ReadAllLines(textBox1.Text).Select(l => l.Split(' ').Select(i => int.Parse(i)).ToArray()).ToArray();
                N = weights.Length;
                Form2 f2 = new Form2();
                f2.Show();
            }
            catch (Exception) //specify exceptions
            {
                MessageBox.Show("Source file is invalid or does not exist. Check and try again.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
