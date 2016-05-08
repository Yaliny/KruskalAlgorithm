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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public static int numberOfVertices = Form1.N;
        public static int numberOfRoots = 0;
        public static int currentVertex = 0;
        public static int minRoot;
        public static int maxRoot;
        static Pen ordinary = new Pen(Color.Black, 1);
        static Pen selected = new Pen(Color.Green, 4);

        class Vertex
        {
            public int name;
            public int root;
            public Point location;
        }

        class Edge
        {
            public int weight;
            public Vertex A;
            public Vertex B;
            public void Draw(PaintEventArgs e)
            {
                e.Graphics.DrawLine(ordinary, A.location, B.location);
            }
            public void Select(PaintEventArgs e)
            {
                e.Graphics.DrawLine(selected, A.location, B.location);
            }
        }

        List<Vertex> vertices = new List<Vertex>();
        List<Edge> edges = new List<Edge>();
        IEnumerable<Edge> edgesSorted = new List<Edge>();
        List<Image> image = new List<Image>();
        List<Image> imageSelected = new List<Image>();
        List<PictureBox> pics = new List<PictureBox>();

        private void Form2_Load(object sender, EventArgs e)
        {
            label3.Text = numberOfVertices.ToString();
            string directory = @".\img\simple\";
            foreach (string myFile in Directory.GetFiles(directory, "*.png", SearchOption.TopDirectoryOnly))
            {
                image.Add(Image.FromFile(myFile));
            }
            directory = @".\img\selected\";
            foreach (string myFile in Directory.GetFiles(directory, "*.png", SearchOption.TopDirectoryOnly))
            {
                imageSelected.Add(Image.FromFile(myFile));
            }
        }

        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            if (numberOfVertices > 1)
            {
                numberOfRoots++;
                vertices.Add(new Vertex() { name = numberOfRoots, root = numberOfRoots, location = e.Location });
                numberOfVertices--;
                label3.Text = numberOfVertices.ToString();
                PictureBox pic = new PictureBox();
                pic.Location = new Point(e.Location.X - 17, e.Location.Y - 17);
                pic.Name = "pic" + numberOfRoots;
                pic.Size = new Size(35, 35);
                pic.Image = image[numberOfVertices];
                this.Controls.Add(pic);
                pics.Add(pic);
            }

            else if (numberOfVertices == 1)
            {
                numberOfRoots++;
                vertices.Add(new Vertex() { name = numberOfRoots, root = numberOfRoots, location = e.Location });
                numberOfVertices--;
                button1.Visible = !button1.Visible;
                label1.Visible = !label1.Visible;
                label2.Text = "Number of roots:";
                label3.Text = numberOfRoots.ToString();
                PictureBox pic = new PictureBox();
                pic.Location = new Point(e.Location.X - 17, e.Location.Y - 17);
                pic.Name = "pic" + numberOfRoots;
                pic.Size = new Size(35, 35);
                pic.Image = image[numberOfVertices];
                this.Controls.Add(pic);
                pics.Add(pic);
                foreach (Vertex v in vertices)
                {
                    for (int i = 0; i < Form1.N; i++)
                    {
                        if (Form1.weights[currentVertex][i] != 0)
                        {
                            edges.Add(new Edge() { A = v, B = vertices[i], weight = Form1.weights[currentVertex][i] });
                        }
                    }
                    currentVertex++;
                }
                edgesSorted = edges.OrderBy(edge => edge.weight);
                foreach (Edge edge in edges)
                {
                    Graphics g = this.CreateGraphics();
                    Rectangle r = new Rectangle();
                    PaintEventArgs p = new PaintEventArgs(g, r);
                    edge.Draw(p);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //step by step
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //final spanning tree
            foreach (Edge edge in edgesSorted)
            {
                if (edge.A.root != edge.B.root)
                {
                    Graphics g = this.CreateGraphics();
                    Rectangle r = new Rectangle();
                    PaintEventArgs p = new PaintEventArgs(g, r);
                    edge.Select(p);

                    pics[edge.A.name - 1].Image = imageSelected[edge.A.name - 1];
                    pics[edge.B.name - 1].Image = imageSelected[edge.B.name - 1];

                    if (edge.A.root < edge.B.root)
                    {
                        minRoot = edge.A.root;
                        maxRoot = edge.B.root;
                    }
                    else
                    {
                        minRoot = edge.B.root;
                        maxRoot = edge.A.root;
                    }
                    foreach (Vertex v in vertices)
                    {
                        if (v.root == maxRoot)
                            v.root = minRoot;
                    }
                    numberOfRoots--;
                }
                if (numberOfRoots == 0)
                    break;
            }
        }
    }
}
