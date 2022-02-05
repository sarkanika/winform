using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            string currentPath = Application.StartupPath.Replace("bin\\Debug", "Resources"); //reading image files from Resources folder
            
            //
            //tabControl
            //
            tabControl = new TabControl();
            tabControl.Location = new Point(12, 12);
            tabControl.Size = new Size(500, 450);
            tabControl.TabIndex = 0;
            tabControl.Margin = new Padding(3, 3, 3, 3);
            tabControl.Name = "PictureTabs";
            tabControl.SelectedIndex = 0;

            string[] images = Directory.GetFiles(@currentPath);
            
            pictureBoxes = new PictureBox[images.Length];
            int tabPageSize = images.Length / 20 + ((images.Length % 20 != 0) ? 1 : 0); //determining number of tabpages based on the number of pages
                                                                                        //we will be printing 20 images per page
            //
            //tabPages
            //
            tabPages = new TabPage[tabPageSize];
            for (int i = 0; i < tabPageSize; i++)
            {
                tabPages[i] = new TabPage();
                tabPages[i].Name = "Page" + (i + 1);
                tabPages[i].Size = new Size(500, 450);
                tabPages[i].Text = "Page" + (i + 1);
                tabPages[i].TabIndex = i;
                tabPages[i].Padding = new Padding(3);
                tabPages[i].UseVisualStyleBackColor = true;
            }
            tabControl.Controls.AddRange(tabPages);
            int image_x = firstImage_x;
            int image_y = firstImage_y;
            int label_x = firstLabel_x;
            int label_y = firstLabel_y;
            int k = 0;
            //
            //pictureboxes and labels
            //
            for (int i = 0; i < images.Length; i++) //creating and processing each picture into pictureboxes
            {
                pictureBoxes[i] = new PictureBox();
                string imgName = Path.GetFileName(images[i]);
                pictureBoxes[i].Name = images[i];
                pictureBoxes[i].TabIndex = i;
                Bitmap bmp = new Bitmap(images[i]);
                pictureBoxes[i].Image = bmp;
                pictureBoxes[i].SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBoxes[i].Size = new Size(100, 50);
                pictureBoxes[i].Margin = new Padding(3, 3, 3, 3);
                pictureBoxes[i].Location = new Point(image_x, image_y);
                pictureBoxes[i].Click += pictureBox_Click;
                tabPages[k].Controls.Add(pictureBoxes[i]);

                Label label = new Label();
                label.Text = imgName;
                label.Location = new Point(label_x, label_y);
                label.Name = imgName;
                label.Size = new Size(50, 20);
                label.Padding = new Padding(2);
                label.Visible = true;
                tabPages[k].Controls.Add(label);
                if (i % 4 != 3 && i % 20 != 19) //modifying locations of each image on the window after it has been added to the tab page
                {
                    image_x += x_incre;
                    label_x += x_incre;
                }
                else if (i % 20 == 19)
                {
                    k++;
                    image_x = firstImage_x;
                    image_y = firstImage_y;
                    label_x = firstLabel_x;
                    label_y = firstLabel_y;
                }
                else
                {
                    image_x = firstImage_x;
                    label_x = firstLabel_x;
                    image_y += y_incre;
                    label_y += y_incre;
                }
            }

            if(imageDatas == null || imageDatas.All(im => im == null))
            {
                int i = 0;
                imageDatas = new ImageData[images.Length];
                foreach(string img in images)
                {
                    imageDatas[i++] = new ImageData(img);
                }
            }

            //
            //displayBox
            //
            displayBox = new PictureBox();
            displayBox.Name = "displayBox";
            displayBox.BackColor = Color.White;
            displayBox.Size = new Size(400, 300);
            displayBox.Location = new Point(580, 30);
            displayBox.SizeMode = PictureBoxSizeMode.AutoSize;
            displayBox.Margin = new Padding(3);
            
            //
            //intensityMethod (Button)
            //
            intensityMethod = new Button();
            intensityMethod.Name = "intensityMethod";
            intensityMethod.Text = "Intensity Method";
            intensityMethod.Location = new Point(12,12);
            intensityMethod.Size = new Size(150, 30);
            intensityMethod.Padding = new Padding(3);
            intensityMethod.BackColor = Color.Empty;
            intensityMethod.Click += IntensityMethod_Click;

            //
            //colourCodeMethod (Button)
            //
            colourCodeMethod = new Button();
            colourCodeMethod.Name = "colourCodeMethod";
            colourCodeMethod.Text = "Colour Code";
            colourCodeMethod.Location = new Point(12,12);
            colourCodeMethod.Size = new Size(150, 30);
            colourCodeMethod.Padding = new Padding(3);
            colourCodeMethod.BackColor = Color.Empty;
            colourCodeMethod.Click += ColourCodeMethod_Click;

            //
            //colorCodeIntensityMethod (Button)
            //
            colorCodeIntensityMethod = new Button();
            colorCodeIntensityMethod.Name = "colorCodeIntensityMethod";
            colorCodeIntensityMethod.Text = "Color Code + Intensity";
            colorCodeIntensityMethod.Location = new Point(12,12);
            colorCodeIntensityMethod.Size = new Size(150, 30);
            colorCodeIntensityMethod.Padding = new Padding(3);
            colorCodeIntensityMethod.BackColor = Color.Empty;
            colorCodeIntensityMethod.Click += ColorCodeIntensityMethod_Click;            

            //
            //tableLayoutPanel1 
            //
            tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.Location = new Point(650, 450);
            tableLayoutPanel2.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            tableLayoutPanel2.AutoSize = true;
            tableLayoutPanel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel2.Controls.Add(intensityMethod, 0, 0);
            tableLayoutPanel2.Controls.Add(colourCodeMethod, 0, 1);
            tableLayoutPanel2.Controls.Add(colorCodeIntensityMethod, 0, 2);

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(displayBox);
            this.Controls.Add(tableLayoutPanel2);
            this.Name = "Image Query";
            this.Text = "Image Query";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ImageData[] imageDatas;
        private TabControl tabControl;
        private TabPage[] tabPages;
        private PictureBox[] pictureBoxes;
        private PictureBox displayBox;
        private Button intensityMethod;
        private Button colourCodeMethod;
        private Button colorCodeIntensityMethod;
        private PictureBox[] resultBoxes;
        private SplitContainer splitContainer;
        private Button reset;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private CheckBox relevance;
        private GroupBox groupBox;
        private Button search;


        private const int firstImage_x = 12;
        private const int firstImage_y = 12;
        private const int firstLabel_x = 40;
        private const int firstLabel_y = 60;
        private const int x_incre = 106;
        private const int y_incre = 75;
    }
}

