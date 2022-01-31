using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        
        //clicking each image in the first page will display on a
        //new displaybox and store it for image query
        private void pictureBox_Click(object sender, EventArgs e)
        {
            PictureBox picture = null;
            for(int i = 0; i < pictureBoxes.Length; i++)
            {
                if (sender == pictureBoxes[i])
                {
                    displayBox.Image = pictureBoxes[i].Image;
                    picture = pictureBoxes[i];
                }
            }
            for(int i = 0; i < imageDatas.Length; i++)
            {
                if(picture.Name.Equals(imageDatas[i].fileLocation))
                {
                    data = imageDatas[i];
                }
            }
        }

        //event button to perform image query using colour code method
        private void ColourCodeMethod_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            SortedDictionary<Double, ImageData> imageDiffPairs = new SortedDictionary<Double, ImageData>();

            foreach(ImageData imageData in imageDatas)
            {
                if(!data.Equals(imageData))
                {
                    Double diff = codeHistogramDifference(data, imageData);
                    while (imageDiffPairs.ContainsKey(diff))
                    {
                        diff += 0.0000001;
                    }
                    imageDiffPairs.Add(diff, imageData);
                }
                
            }
            SortedDictionary<Double, ImageData>.ValueCollection values = imageDiffPairs.Values;
            resultsWindow(values);
            
        }

        //event button to perform image query using colour intensity method
        private void IntensityMethod_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            SortedDictionary<Double, ImageData> imageDiffPairs = new SortedDictionary<Double, ImageData>();
            foreach (ImageData imageData in imageDatas)
            {
                if (!data.Equals(imageData))
                {
                    Double diff = intensityhistogramDifference(data, imageData);
                    while (imageDiffPairs.ContainsKey(diff))
                    {
                        diff += 0.0000001;
                    }
                    imageDiffPairs.Add(diff, imageData);
                }

            }
            SortedDictionary<Double, ImageData>.ValueCollection values = imageDiffPairs.Values;
            resultsWindow(values);
        }

        private void Vscroll_Scroll(object sender, ScrollEventArgs e)
        {
            splitContainer.Panel2.VerticalScroll.Value = vscroll.Value;
        }

        //this will load the results window
        private void resultsWindow(SortedDictionary<Double, ImageData>.ValueCollection values)
        {
            splitContainer = new SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(splitContainer)).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            splitContainer.VerticalScroll.Enabled = true;
            splitContainer.Size = new Size(1000, 600);
            splitContainer.Enabled = true;
            splitContainer.SplitterWidth = 4;
            splitContainer.SplitterIncrement = 1;
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Location = new Point(0, 0);
            splitContainer.Name = "splitContainer1";
            splitContainer.SplitterDistance = 400;
            splitContainer.Panel2.AutoScroll = true;

            vscroll = new VScrollBar();
            vscroll.Dock = DockStyle.Right;
            vscroll.Enabled = true;
            vscroll.Visible = true;
            vscroll.Size = new Size(17, 600);
            vscroll.Location = new Point(580, 0);
            vscroll.Scroll += Vscroll_Scroll;
            vscroll.Focus();
            splitContainer.Panel2.Controls.Add(vscroll);

            reset = new Button();
            reset.Location = new Point(100, 450);
            reset.Size = new Size(200, 50);
            reset.Text = "Return to Main Page";
            reset.Click += Reset_Click;
            splitContainer.Panel1.Controls.Add(reset);

            displayBox.Location = new Point(12, 12);
            splitContainer.Panel1.Controls.Add(displayBox);

            this.Controls.Add(splitContainer);

            resultBoxes = new PictureBox[values.Count];

            int i = 0;
            int point_x = firstImage_x;
            int point_y = firstImage_y;
            int label_x = firstLabel_x;
            int label_y = firstLabel_y;
            foreach (ImageData image in values)
            {
                resultBoxes[i] = new PictureBox();
                resultBoxes[i].Name = image.imageName;
                resultBoxes[i].SizeMode = PictureBoxSizeMode.StretchImage;
                resultBoxes[i].Size = new Size(100, 50);
                resultBoxes[i].Margin = new Padding(3, 3, 3, 3);
                resultBoxes[i].Image = new Bitmap(image.fileLocation);
                resultBoxes[i].Location = new Point(point_x, point_y);
                splitContainer.Panel2.Controls.Add(resultBoxes[i]);

                Label label = new Label();
                label.Text = image.imageName;
                label.Location = new Point(label_x, label_y);
                label.Size = new Size(50, 20);
                label.Padding = new Padding(2);
                label.Visible = true;
                splitContainer.Panel2.Controls.Add(label);
                i++;
                if (i % 5 != 0)
                {
                    point_x += x_incre;
                    label_x += x_incre;
                }
                else
                {
                    point_x = firstImage_x;
                    point_y += y_incre;
                    label_x = firstLabel_x;
                    label_y += y_incre;
                }


            }
        }

        //this clears the window and reloads the previous page
        private void Reset_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            this.InitializeComponent();
        }

        private ImageData data;
    }
}
