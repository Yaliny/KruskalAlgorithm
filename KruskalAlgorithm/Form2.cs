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

        public static int numberOfVertices; //counter of vertices left to plot
        public static int numberOfRoots; //counter of roots
        public static int currentVertex; //vertex pointer
        public static int currentStep; //pointer of the step mode
        public static int treeWeight; //weight of the spanning tree       
        public static int minRoot;
        public static int maxRoot;

        static Pen ordinary = new Pen(Color.Black, 1);
        static Pen selected = new Pen(Color.Green, 4);

        List<Vertex> vertices; //list of all vertices
        List<Edge> edges; //list of all edges
        IEnumerable<Edge> edgesSorted; //list of all edges sorted ascending by weight
        IEnumerator<Edge> stepEdge; //pointer of the sorted edges list
        List<Image> image; //list of images for vertices
        List<Image> imageSelected; //list of images for vertices connected to the spanning tree
        List<PictureBox> pics; //list of image containers for vertices

        class Vertex
        {
            public int number;
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
                //a method to draw an ordinary edge
                e.Graphics.DrawLine(ordinary, A.location, B.location);
            }
            public void Select(PaintEventArgs e)
            {
                //a method to highlight the selected edge
                e.Graphics.DrawLine(selected, A.location, B.location);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            numberOfVertices = Form1.N;
            numberOfRoots = 0;
            currentVertex = 0;
            currentStep = 0;
            treeWeight = 0;
            label3.Text = numberOfVertices.ToString();
            vertices = new List<Vertex>();
            edges = new List<Edge>();
            edgesSorted = new List<Edge>();
            image = new List<Image>();
            imageSelected = new List<Image>();
            pics = new List<PictureBox>();
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
                //plotting vertex
                PictureBox pic = new PictureBox();
                pic.Location = new Point(e.Location.X - 17, e.Location.Y - 17);
                //e.Location - coordinates of the mouse click
                pic.Name = "pic" + numberOfRoots;
                pic.Size = new Size(35, 35);
                pic.Image = image[numberOfRoots];
                this.Controls.Add(pic);
                pics.Add(pic);

                //adding vertex to list
                numberOfRoots++;
                vertices.Add(new Vertex() { number = numberOfRoots, root = numberOfRoots, location = e.Location });
                numberOfVertices--;
                label3.Text = numberOfVertices.ToString();
            }

            else if (numberOfVertices == 1)
            {
                PictureBox pic = new PictureBox();
                pic.Location = new Point(e.Location.X - 17, e.Location.Y - 17);
                pic.Name = "pic" + numberOfRoots;
                pic.Size = new Size(35, 35);
                pic.Image = image[numberOfRoots];
                this.Controls.Add(pic);
                pics.Add(pic);
                
                numberOfRoots++;
                vertices.Add(new Vertex() { number = numberOfRoots, root = numberOfRoots, location = e.Location });
                numberOfVertices--;
                groupBox1.Text = "Minimum spanning tree";
                label2.Text = "Number of roots:";
                label3.Text = numberOfRoots.ToString();
                
                //creating list of edges using input matrix of weights from text file
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

                //sorting edges by weight ascending
                edgesSorted = edges.OrderBy(edge => edge.weight);

                //initializing the sorted list pointer, pointer place - before the first item of the list
                stepEdge = edgesSorted.GetEnumerator();

                //displaying edges weights in the table
                dataGridView1.ColumnCount = 2;
                dataGridView1.Columns[0].HeaderText = "Edge";
                dataGridView1.Columns[1].HeaderText = "Weight";
                foreach (Edge edge in edgesSorted)
                {
                    this.dataGridView1.Rows.Add(edge.A.number.ToString() + "-" + edge.B.number.ToString(), edge.weight);
                }

                //plotting edges
                foreach (Edge edge in edges)
                {
                    //service variables for drawing
                    Graphics g = this.CreateGraphics();
                    Rectangle r = new Rectangle();
                    PaintEventArgs p = new PaintEventArgs(g, r);

                    edge.Draw(p);
                }                
                button1.Enabled = !button1.Enabled;
                button2.Enabled = !button2.Enabled;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //step by step
            stepEdge.MoveNext();
            button2.Enabled = false;
            dataGridView1.CurrentCell = dataGridView1[0, currentStep];

            //if start and end vertices of the edge have different roots
            if (stepEdge.Current.A.root != stepEdge.Current.B.root)
            {
                //highlight the selected edge
                Graphics g = this.CreateGraphics();
                Rectangle r = new Rectangle();
                PaintEventArgs p = new PaintEventArgs(g, r);
                stepEdge.Current.Select(p);

                //add weight of the edge to sum of the weights of all selected edges
                treeWeight = treeWeight + stepEdge.Current.weight;

                //highlight the row in the table with green
                dataGridView1.CurrentRow.DefaultCellStyle.BackColor = Color.LightGreen;

                //change the start and end vertices images to highlighted
                pics[stepEdge.Current.A.number - 1].Image = imageSelected[stepEdge.Current.A.number - 1];
                pics[stepEdge.Current.B.number - 1].Image = imageSelected[stepEdge.Current.B.number - 1];

                //find minimum and maximum root between two vertices
                if (stepEdge.Current.A.root < stepEdge.Current.B.root)
                {
                    minRoot = stepEdge.Current.A.root;
                    maxRoot = stepEdge.Current.B.root;
                }
                else
                {
                    minRoot = stepEdge.Current.B.root;
                    maxRoot = stepEdge.Current.A.root;
                }

                //set minRoot value to all the vertices' roots equal to maxRoot
                foreach (Vertex v in vertices)
                {
                    if (v.root == maxRoot)
                        v.root = minRoot;
                }

                numberOfRoots--;
                currentStep++;
            }

            //if both vertices of the edge belong to same tree
            else
            {
                //omit the edge and highlight the row in table with red
                dataGridView1.CurrentRow.DefaultCellStyle.BackColor = Color.PaleVioletRed;
                currentStep++;
            }

            label3.Text = numberOfRoots.ToString();

            //when the minimum spanning tree found
            if (numberOfRoots == 1)
            {
                //highlight the rows of unused edges with gray
                foreach (DataGridViewRow row in dataGridView1.Rows)
                    if (row.DefaultCellStyle.BackColor == Color.Empty)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGray;
                    }

                button1.Visible = !button1.Visible;
                button2.Visible = !button2.Visible;
                button3.Visible = !button3.Visible;
                label1.Location = (new Point(80, 9));

                //display weight of minimum spanning tree
                label1.Text = "Minimum spanning tree. Total weight: " + treeWeight.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //final spanning tree

            foreach (Edge edge in edgesSorted)
            {
                dataGridView1.CurrentCell = dataGridView1[0, currentStep];

                //if start and end vertices of the edge have different roots
                if (edge.A.root != edge.B.root)
                {
                    //highlight the selected edge
                    Graphics g = this.CreateGraphics();
                    Rectangle r = new Rectangle();
                    PaintEventArgs p = new PaintEventArgs(g, r);                  
                    edge.Select(p);

                    //add weight of the edge to sum of the weights of all selected edges
                    treeWeight = treeWeight + edge.weight;

                    //highlight the row in the table with green
                    dataGridView1.CurrentRow.DefaultCellStyle.BackColor = Color.LightGreen;

                    //change the start and end vertices images to highlighted
                    pics[edge.A.number - 1].Image = imageSelected[edge.A.number - 1];
                    pics[edge.B.number - 1].Image = imageSelected[edge.B.number - 1];

                    //find minimum and maximum root between two vertices
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

                    //set minRoot value to all the vertices' roots equal to maxRoot
                    foreach (Vertex v in vertices)
                    {
                        if (v.root == maxRoot)
                            v.root = minRoot;
                    }

                    numberOfRoots--;
                    currentStep++;
                }
                else
                {
                    //omit the edge and highlight the row in table with red
                    dataGridView1.CurrentRow.DefaultCellStyle.BackColor = Color.PaleVioletRed;
                    currentStep++;
                }

                label3.Text = numberOfRoots.ToString();

                //when the minimum spanning tree found
                if (numberOfRoots == 1)
                {
                    //highlight the rows of unused edges with gray
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                        if (row.DefaultCellStyle.BackColor == Color.Empty)
                            row.DefaultCellStyle.BackColor = Color.LightGray;
                    button1.Visible = !button1.Visible;
                    button2.Visible = !button2.Visible;
                    button3.Visible = !button3.Visible;
                    label1.Location = (new Point(80,9));

                    //display weight of minimum spanning tree
                    label1.Text = "Minimum spanning tree. Total weight: " + treeWeight.ToString();

                    //stop the cycle
                    break;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //destroy form
            Dispose(true);
            this.Dispose();
        }
    }
}
