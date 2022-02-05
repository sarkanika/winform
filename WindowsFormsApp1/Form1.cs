using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
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
            imageDiffPairs = new Dictionary<ImageData, Double>();
            foreach(ImageData imageData in imageDatas)
            {
                if(!data.Equals(imageData))
                {
                    Double diff = HistogramDifference(data, imageData, colorCode);
                    imageDiffPairs.Add(imageData, diff);
                }
                
            }
            imageDiffPairs = imageDiffPairs.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            ResultsWindow(imageDiffPairs, colourCodeMethod.Text);
            
        }

        //event button to perform image query using colour intensity method
        private void IntensityMethod_Click(object sender, EventArgs e)
        {
            imageDiffPairs = new Dictionary<ImageData, Double>();
            foreach (ImageData imageData in imageDatas)
            {
                if (!data.Equals(imageData))
                {
                    Double diff = HistogramDifference(data, imageData, colorIntensity);
                    imageDiffPairs.Add(imageData, diff);
                }

            }
            imageDiffPairs = imageDiffPairs.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            ResultsWindow(imageDiffPairs, intensityMethod.Text);
        }

        private void ColorCodeIntensityMethod_Click(object sender, EventArgs e)
        {
            featureMatrix = GetFeatureMatrix();
            imageDiffPairs = new Dictionary<ImageData, Double>();
            double[] avg = GetFeaturesAvg(featureMatrix);
            double[] stdev = GetFeatureStdev(featureMatrix, avg);
            double[] weights = new double[featureMatrix.Values.First().Length];
            double minstdev = 0.5 * stdev.Where(sd => sd != 0).Min();
            for (int i = 0; i < avg.Length; i++)
            {
                if(stdev[i] == 0 && avg[i] != 0)
                {
                    stdev[i] = minstdev;
                    weights[i] = 1 / avg.Length;
                } else if (stdev[i] != 0 && avg[i] != 0)
                {
                    weights[i] = (double) 1 / (double)avg.Length;
                }
                featureMatrix.Values.ToList().ForEach(x => x[i] = stdev[i] != 0 ? (x[i] - avg[i])/stdev[i] : 0);
            }

            foreach(KeyValuePair<ImageData, double[]> pair in featureMatrix)
            {
                if (!data.Equals(pair.Key))
                {
                    double diff = GetDifference(featureMatrix[data], pair.Value, weights);
                    imageDiffPairs.Add(pair.Key, diff);
                }
            }

            imageDiffPairs = imageDiffPairs.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            ResultsWindow(imageDiffPairs, colorCodeIntensityMethod.Text);
        }

        private double GetDifference(double[] value1, double[] value2, double[] weights)
        {
            double diff = 0.0;
            for (int i = 0; i < weights.Length; i++)
            {
                diff += Math.Abs(value1[i] - value2[i]) * weights[i];
            }
            return diff;
        }

        private double[] GetFeaturesAvg(Dictionary<ImageData, double[]> featureMatrix)
        {
            double[] avg = new double[featureMatrix.Values.First().Length];
            for (int i = 0; i < avg.Length; i++)
            {
                avg[i] = featureMatrix.Average(fm => fm.Value[i]);
            }
            return avg;
        }

        private double[] GetFeatureStdev(Dictionary<ImageData, double[]> featureMatrix, double[] avg)
        {
            double[] stdev = new double[featureMatrix.Values.First().Length];
            for (int i = 0; i < stdev.Length; i++)
            {
                stdev[i] = 0.0;
                stdev[i] = featureMatrix.Sum(fm => Math.Pow(fm.Value[i] - avg[i], 2));
                stdev[i] = Math.Sqrt(stdev[i] / (featureMatrix.Count - 1));
            }
            return stdev;
        }

        private void Relevance_CheckedChanged(object sender, EventArgs e)
        {
            if (relevance.Checked)
            {
                relFeedback = true;
                if (relFeedback && checkBoxes.Where(cb => cb.Checked == true).Any())
                {
                    search.Enabled = true;
                }
            } else
            {
                relFeedback = false;
                search.Enabled = false;
                if (checkBoxes.Any()
                    && !checkBoxes.All(cb => cb.Checked == false))
                {
                    checkBoxes.Where(cb => cb.Checked == true).ToList().ForEach(cb => cb.Checked = false);
                }
            }
            splitContainer.Panel2.Controls.Clear();
            Panel2Load(label);
        }

        //this will load the results window
        private void ResultsWindow(Dictionary<ImageData, Double> imageDiffPairs, string text)
        {
            this.Controls.Clear();
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
            splitContainer.AutoSize = true;
            splitContainer.Location = new Point(0, 0);
            splitContainer.Name = "splitContainer1";
            splitContainer.SplitterDistance = 400;

            displayBox.Location = new Point(12, 12);
            displayBox.Image = new Bitmap(data.fileLocation);
            displayBox.SizeMode = PictureBoxSizeMode.AutoSize;
            splitContainer.Panel1.Controls.Add(displayBox);

            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.Dock = DockStyle.Left;
            tableLayoutPanel1.Size = splitContainer.Panel1.Size;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.Location = new Point(0, 400);
            splitContainer.Panel1.Controls.Add(tableLayoutPanel1);

            relevance = new CheckBox();
            relevance.Name = "Relevance";
            relevance.Text = "Relevance";
            relevance.Checked = relFeedback;
            relevance.Enabled = true;
            relevance.Location = new Point(12, 12);
            relevance.Size = new Size(150, 30);
            relevance.CheckedChanged += Relevance_CheckedChanged;
            tableLayoutPanel1.Controls.Add(relevance, 1, 0);

            reset = new Button();
            reset.Location = new Point(12, 12);
            reset.Size = new Size(150, 50);
            reset.Text = "Return to Main Page";
            reset.Click += Reset_Click;
            tableLayoutPanel1.Controls.Add(reset, 0, 0);

            search = new Button();
            search.Location = new Point(0, 0);
            search.Size = new Size(150, 50);
            search.Text = "Search";
            search.Enabled = relFeedback && rlvntSet.Any();
            search.Click += Search_Click;
            tableLayoutPanel1.Controls.Add(search, 0, 1);

            this.Controls.Add(splitContainer);
            
            Panel2Load(text);
        }

        private void Panel2Load(String text)
        {
            groupBox = new GroupBox();
            groupBox.Name = "Results";
            groupBox.Size = splitContainer.Panel2.Size;
            groupBox.Padding = new Padding(3);
            groupBox.AutoSize = true;
            groupBox.Dock = DockStyle.Fill;
            groupBox.Text = relFeedback && !text.Contains(" with user relevance feedback") ? text + " with user relevance feedback"
                : text.Contains(" with user relevance feedback") ? text.Replace(" with user relevance feedback", "") : text;
            label = groupBox.Text;
            splitContainer.Panel2.Controls.Add(groupBox);

            tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(firstImage_x, firstImage_y);
            tableLayoutPanel2.Size = splitContainer.Panel2.Size;
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.AutoSize = true;
            tableLayoutPanel2.ColumnCount = 5;
            tableLayoutPanel2.AutoScroll = true;
            tableLayoutPanel2.VerticalScroll.Enabled = true;
            tableLayoutPanel2.HorizontalScroll.Enabled = false;
            tableLayoutPanel2.VerticalScroll.Visible = true;
            tableLayoutPanel2.HorizontalScroll.Visible = false;
            groupBox.Controls.Add(tableLayoutPanel2);

            resultBoxes = new PictureBox[imageDiffPairs.Count];

            int i = 0;
            int col = 0;
            int row = 0;
            if (relFeedback)
            {
                checkBoxes.Clear();
                foreach (ImageData image in imageDiffPairs.Keys)
                {
                    resultBoxes[i] = GenerateResultBox(image);
                    
                    CheckBox cb = new CheckBox();
                    cb.Name = image.fileLocation;
                    cb.Text = "Relevant";
                    cb.Location = new Point(0, 0);
                    cb.Checked = rlvntSet.Contains(image);
                    cb.CheckedChanged += Cb_CheckedChanged;

                    TableLayoutPanel tbl = new TableLayoutPanel();
                    tbl.Location = new Point(0, 0);
                    tbl.ColumnCount = 1;
                    tbl.AutoSize = true;
                    tbl.Controls.Add(resultBoxes[i], 0, 0);
                    tbl.Controls.Add(cb, 0, 1);
                    checkBoxes.Add(cb);

                    tableLayoutPanel2.Controls.Add(tbl, col, row);
                    i++;
                    col++;
                    if (col == 5)
                    {
                        col = 0;
                        row++;
                    }
                }
            }
            else
            {
                foreach (ImageData image in imageDiffPairs.Keys)
                {
                    resultBoxes[i] = GenerateResultBox(image);
                    tableLayoutPanel2.Controls.Add(resultBoxes[i], col, row);
                    i++;
                    col++;
                    if (col == 5)
                    {
                        col = 0;
                        row++;
                    }
                }
            }
        }

        private PictureBox GenerateResultBox(ImageData image)
        {
            PictureBox pb = new PictureBox();
            pb.Name = image.fileLocation;
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.Margin = new Padding(3);
            Bitmap bmp = new Bitmap(image.fileLocation);
            pb.Image = bmp;
            pb.Size = new Size(bmp.Width /4, bmp.Height /4);
            pb.Location = new Point(10, 10);
            return pb;
        }

        private void Search_Click(object sender, EventArgs e)
        {
            if(featureMatrix == null || !featureMatrix.Any())
            {
                featureMatrix = GetFeatureMatrix();
                double[] avg = GetFeaturesAvg(featureMatrix);
                double[] stdev = GetFeatureStdev(featureMatrix, avg);
                double[] weights = new double[featureMatrix.Values.First().Length];
                double minstdev = 0.5 * stdev.Where(sd => sd != 0).Min();
                for (int i = 0; i < avg.Length; i++)
                {
                    if (stdev[i] == 0 && avg[i] != 0)
                    {
                        stdev[i] = minstdev;
                        weights[i] = 1 / avg.Length;
                    }
                    else if (stdev[i] == 0 && avg[i] == 0)
                    {
                        weights[i] = 0.0;
                    }
                    else
                    {
                        weights[i] = 1 / avg.Length;
                    }
                    featureMatrix.Values.ToList().ForEach(x => x[i] = stdev[i] != 0 ? (x[i] - avg[i]) / stdev[i] : 0);
                }
            }
            rlvntSet.Add(data);
            Dictionary<ImageData, double[]> relMatrix = featureMatrix.Where(fm => rlvntSet.Contains(fm.Key)).ToDictionary(fm => fm.Key, fm => fm.Value);
            imageDiffPairs.Clear();
            double[] rlvtAvg = GetFeaturesAvg(relMatrix);
            double[] rlvntStdev = GetFeatureStdev(relMatrix, rlvtAvg);
            double[] rlvntWeights = new double[rlvntStdev.Length];
            double minRlvntStdev = 0.5 * rlvntStdev.Where(std => std != 0).ToList().Min();
            for (int i = 0; i < rlvntStdev.Length; i++)
            {
                if (rlvntStdev[i] == 0 && rlvtAvg[i] !=0)
                {
                    rlvntStdev[i] = minRlvntStdev;
                    rlvntWeights[i] = (double)1 / rlvntStdev[i];
                } else if (rlvntStdev[i] != 0 && rlvtAvg[i] != 0)
                {
                    rlvntWeights[i] = (double)1 / rlvntStdev[i];
                }
            }
            double rlvntWtSum = rlvntWeights.Sum();
            rlvntWeights.ToList().ForEach(wt => wt /= rlvntWtSum);

            foreach (KeyValuePair<ImageData, double[]> pair in featureMatrix)
            {
                if (!data.Equals(pair.Key))
                {
                    double diff = GetDifference(featureMatrix[data], pair.Value, rlvntWeights);
                    imageDiffPairs.Add(pair.Key, diff);
                }
            }

            imageDiffPairs = imageDiffPairs.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            ResultsWindow(imageDiffPairs, colorCodeIntensityMethod.Text);
        }

        private void Cb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.Checked)
            {
                rlvntSet.Add(imageDatas.Where(im => im.fileLocation.Equals(cb.Name, StringComparison.OrdinalIgnoreCase)).First());
                if(search.Enabled == false)
                {
                    search.Enabled = true;
                }
            } else 
            {
                if(rlvntSet.Any() 
                    && rlvntSet.Where(im => im.fileLocation.Equals(cb.Name, StringComparison.OrdinalIgnoreCase)).Any()) {
                    rlvntSet.RemoveWhere(im => im.fileLocation.Equals(cb.Name, StringComparison.OrdinalIgnoreCase));
                }
                if(!rlvntSet.Any() || (rlvntSet.Count() == 1 && rlvntSet.Contains(data)))
                {
                    search.Enabled = false;
                }
            }
        }

        //this clears the window and reloads the previous page
        private void Reset_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            relFeedback = false;
            rlvntSet.Clear();
            this.InitializeComponent();
        }

        private Double HistogramDifference(ImageData data1, ImageData data2, string type)
        {
            Double totaldiff = 0;
            Bitmap bmp1 = new Bitmap(data1.fileLocation);
            Bitmap bmp2 = new Bitmap(data2.fileLocation);
            Double bmp1area = (Double)bmp1.Width * (Double)bmp1.Height;
            Double bmp2area = (Double)bmp2.Width * (Double)bmp2.Height;
            if (type.Equals(colorCode))
            {
                for (int i = 0; i < data1.colorCode.Length; i++)
                {
                    Double diff = Math.Abs(((Double)data1.colorCode[i] / bmp1area) - ((Double)data2.colorCode[i] / bmp2area));
                    totaldiff += diff;
                }
            } else if (type.Equals(colorIntensity))
            {
                for (int i = 0; i < data1.colorIntensity.Length; i++)
                {
                    Double diff = Math.Abs(((Double)data1.colorIntensity[i] / bmp1area) - ((Double)data2.colorIntensity[i] / bmp2area));
                    totaldiff += diff;
                }
            }
            return totaldiff;
        }

        private Dictionary<ImageData, double[]> GetFeatureMatrix()
        {
            Dictionary<ImageData, double[]> matrix = new Dictionary<ImageData,double[]>();
            foreach(ImageData image in imageDatas)
            {
                double[] features = new double[image.features.Length];
                Bitmap bitmap = new Bitmap(image.fileLocation);
                int pixels = bitmap.Width * bitmap.Height;
                int i = 0;
                foreach (int f in image.features)
                {
                    features[i++] = (double)f / (double)pixels;
                }
                matrix.Add(image, features);
            }
            return matrix;
        }

        private ImageData data;
        private HashSet<ImageData> rlvntSet = new HashSet<ImageData>();
        private List<CheckBox> checkBoxes = new List<CheckBox>();

        private const string colorIntensity = "colorIntensity";
        private const string colorCode = "colorCode";
        private bool relFeedback = false;
        private Dictionary<ImageData, Double> imageDiffPairs;
        private Dictionary<ImageData, double[]> featureMatrix;
        string label;
    }
}
