using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using raytracer.Utils;
using raytracer.Domain;
using System.Threading;
using System.Globalization;

namespace raytracer
{

    public partial class Form1 : Form
    {
        private Scene loadedScene;
        public Form1()
        {
            InitializeComponent();
            checkedListBox1.BackColor = Color.DarkGray;
            loadedScene = null;
            CultureInfo ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            numericUpDown2.Value = 1;
        }
        string pathToFile = "";//to save the location of the selected object
        private void Load_Click(object sender, EventArgs e)
        {
            loadedScene = new Scene();
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "TXT files|*.txt";
            theDialog.InitialDirectory = @"C:\Desktop";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                //MessageBox.Show(theDialog.FileName.ToString());
                pathToFile = theDialog.FileName;

            }

            textBox1.Text = Path.GetFileName(theDialog.FileName).ToString();


            if (File.Exists(pathToFile))
            {
                loadedScene = RaytracerFileReader.loadSceneIntoMemory(pathToFile);
                foreach(Object obj in loadedScene.getAllObjects3D())
                {
                    Object3D obj3D = (Object3D)obj;
                    obj3D.obtainRealPoints(loadedScene.camera.transformation);
                }
                foreach(Object obj in loadedScene.getAllLights())
                {
                    Light l = (Light)obj;
                    l.obtainRealPoints(loadedScene.camera.transformation);
                }
                numericUpDown3.Value = Convert.ToInt32(loadedScene.image.size.sizeX);
                numericUpDown4.Value = Convert.ToInt32(loadedScene.image.size.sizeY);
                Color backgroundColor = Color.FromArgb(Convert.ToInt32(255 * loadedScene.image.color3.red), Convert.ToInt32(255 * loadedScene.image.color3.green), Convert.ToInt32(255 * loadedScene.image.color3.blue));
                pictureBox2.BackColor = backgroundColor;
                loadedScene.image.size.sizeY = Convert.ToInt32(numericUpDown4.Value.ToString());
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private bool runningFlag;

        private async void Render_Click(object sender, EventArgs e)
        {

            if (loadedScene == null)
            {
                MessageBox.Show("Please Load the file first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
            {
                int nThreads = Convert.ToInt32(numericUpDown2.Value);
                int nRecursions = Convert.ToInt32(numericUpDown1.Value);
                int nSample = Convert.ToInt32(numericUpDown5.Value);
                bool shadow = false;
                bool ambient = false;
                bool diffuse = false;
                bool specular = false;
                bool refraction = false;
                bool antiAliasing = checkBox1.Checked;
                bool softShadow = false;
                runningFlag = true;
                int i = 0;


                if (nThreads < 1)
                {
                    MessageBox.Show("You must have at least one thread", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (checkedListBox1.CheckedItems.Contains("Shadows"))
                {
                    shadow = true;
                }
                if (checkedListBox1.CheckedItems.Contains("Ambient"))
                {

                    ambient = true;
                }
                if (checkedListBox1.CheckedItems.Contains("Diffuse"))
                {
                    diffuse = true;
                }
                if (checkedListBox1.CheckedItems.Contains("Specular"))
                {
                    specular = true;
                }
                if (checkedListBox1.CheckedItems.Contains("Refraction"))
                {
                    refraction = true;
                }
                if (checkedListBox1.CheckedItems.Contains("SoftShadow"))
                {
                    softShadow = true;
                }


                Task.Run(() => { updateImage();});


                await Task.Run(() => {
                    RayTracerGenerator.generatePrimaryRaysMultiThreaded(loadedScene, loadedScene.camera, loadedScene.image, nRecursions, nThreads, nSample, shadow, ambient, diffuse, specular, refraction, antiAliasing, softShadow);
                    //RayTracerGenerator.generatePrimaryRays(loadedScene.camera, loadedScene.image, 1);
                    pictureBox1.Image = loadedScene.image.convertMatrixToBitmap();
                    runningFlag = false;
                });

                MessageBox.Show("Finished rendering image.", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

        }
        private void updateImage()
        {
            while (runningFlag)
            {
                pictureBox1.Image = loadedScene.image.convertMatrixToBitmap();

                Thread.Sleep(250);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
           // double p1 = Convert.ToDouble(ponto1);
            Domain.Vector4 p1 = new Domain.Vector4(-30, -6,-6, 1);
            Domain.Vector4 p2 = new Domain.Vector4(-18, -6, -6, 1);
            Domain.Vector4 p3 = new Domain.Vector4(-24, 0, 6, 1);
            //double p2 = Convert.ToDouble(ponto2);
            Domain.Vector4 vector1 = RaytracerMath.createVectorA(p1, p2);
            Domain.Vector4 vector2 = RaytracerMath.createVectorA(p1, p3);
            Domain.Vector4 cross = RaytracerMath.crossVectors(vector1, vector2);
            Domain.Vector4 normal = RaytracerMath.Normalize(cross);

            MessageBox.Show(normal.ToString());
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void saveImage_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                pictureBox1.Image.Save(fs,System.Drawing.Imaging.ImageFormat.Jpeg);
                fs.Close();
            }
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            loadedScene.image.size.sizeX = Convert.ToInt32(numericUpDown3.Value.ToString());
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            loadedScene.image.size.sizeY = Convert.ToInt32(numericUpDown4.Value.ToString());
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            // Keeps the user from selecting a custom color.
            MyDialog.AllowFullOpen = true;
            // Allows the user to get help. (The default is false.)
            MyDialog.ShowHelp = true;

            // Update the text box color if the user clicks OK 
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.BackColor = MyDialog.Color;
            }
            double r = (double)MyDialog.Color.R / 255;
            double g = (double)MyDialog.Color.G / 255;
            double b = (double)MyDialog.Color.B / 255;

            loadedScene.image.color3.red = r;
            loadedScene.image.color3.green = g;
            loadedScene.image.color3.blue = b;
        }
    }
}
