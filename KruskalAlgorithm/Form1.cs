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
                //read the text file and store it to array of lines, each line as an array of integers
                weights = File.ReadAllLines(textBox1.Text).Select(l => l.Split(' ').Select(i => int.Parse(i)).ToArray()).ToArray();
                N = weights.Length; //matrix dimentions size
                if (N > 21) //set maximum number of vertices
                    throw new IndexOutOfRangeException();
                foreach (int[] weight in weights)
                {
                    //size of array of each line must be equal to the number of lines (square matrix)
                    if (weight.Length != N)
                        throw new InvalidDataException();

                    //lower triangle can't contain anything except zeros
                    for (int i = 0; i <= Array.IndexOf(weights, weight); i++)
                    {
                        if (weight[i] != 0)
                            throw new InvalidDataException();
                    }

                    //all the values of the upper triangle must be positive
                    for (int i = Array.IndexOf(weights, weight); i < N; i++)
                    {
                        if (weight[i] < 0)
                            throw new InvalidDataException();
                    }
                }

                Form2 f2 = new Form2();
                f2.Show();

            }
            catch (Exception ex)           
            {
                if (ex is ArgumentException || ex is DirectoryNotFoundException)
                    MessageBox.Show("Invalid file path. Check the path and try again.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (ex is FileNotFoundException)
                    MessageBox.Show("File not found. Check the path and try again.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (ex is IndexOutOfRangeException)
                    MessageBox.Show("Graph is too big. Maximum number of vertices is 21.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (ex is InvalidDataException)
                    MessageBox.Show("Invalid graph data. Check data file and try again.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("An error ocured while reading the file. Check path and file data and try again.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
