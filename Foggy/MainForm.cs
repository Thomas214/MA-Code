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

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
//using Emgu.CV.CvInvoke;


namespace Foggy
{
    public partial class mainForm : Form
    {

        private Mat matOriginal;
        private Mat matDepthmap;
        private Image<Bgr, Byte> imageOriginal;
        private Image<Bgr, Byte> imageFog;
        private Image<Gray, Byte> imageGray;
        private Image<Gray, Byte> imageNoise;
        private Image<Gray, Byte> imageDepthmap;
        private Image<Bgr, Byte> imageSuperpixels;
        private Image<Bgr, Byte> imageRoadsigns;
        private Image<Bgr, Byte> imageRectangles;
        private Image<Bgr, Byte> imageEnhanced;

        private Dictionary<string, Image<Bgr, Byte>> images;
        int imageNr;

        int imageHeight;
        int imageWidth;

        private double scaleX = 1;
        private double scaleY = 1;

        private int visibility = int.MaxValue;
        private double skyLevel = 1;
        private int[,] depthMatrix;
        bool depthMatrixCreated = false;

        bool checkSingleImage = false;

        double horizonDistance = 0;


        //private LineSegment2D horizon = new LineSegment2D();
        private Point skypoint = new Point();

        //private Point recStart;
        //private Rectangle rectangle = new Rectangle();

        private bool setHorizon = false;
        private bool showSkylevelEllipse = false;
        private bool drawRectangle = false;

        private bool kNoise = false;
        private bool skyNoise = false;


        private static Brush brushRed = new SolidBrush(Color.FromArgb(200, 200, 0, 0));
        private static Brush brushGreen = new SolidBrush(Color.FromArgb(200, 0, 200, 0));
        private Pen penGreen = new Pen(brushGreen, 3);
        private Pen penRed = new Pen(brushRed, 3);

        private int[,] kNoiseMap;
        private int[,] skyNoiseMap;

        private Rectangle[] centerRecs;
        private bool drawCenters = false;

        private Superpixels superpixels;
        private bool selectVerticals = false;
        private int oldRegionNr = -1;

        verticalObject newObject;
        private List<verticalObject> verticalObjects;

        private bool mouseDown = false;

        ColorBasedDetection colorBasedDetection;
        Enhancement enhancement;

        string currentFileName;

        List<string> groundTruthList;
        List<Rectangle> groundTruthRecs = new List<Rectangle>();
        List<Rectangle> foundRecs = new List<Rectangle>();
        List<Rectangle> detectedSigns = new List<Rectangle>();
        List<Rectangle> missedSigns = new List<Rectangle>();

        Results results;

        // ----- Kontruktor -----
        public mainForm()
        {
            InitializeComponent();
        }

        // ----- Form geladen -----
        private void Form1_Load(object sender, EventArgs e)
        {
            // Elemente deaktivieren
            btn_loadimage.Enabled = false;
            btn_loadMultipleImages.Enabled = false;
            btn_loadDepthmap.Enabled = false;
            btn_setVision.Enabled = false;
            btn_setHorizon.Enabled = false;
            btn_horizonDistance.Enabled = false;
            btn_setSkylevel.Enabled = false;
            btn_addNoise.Enabled = false;
            trackBar1.Enabled = false;
            checkBoxK.Enabled = false;
            checkBoxSky.Enabled = false;
            btn_clearFog.Enabled = false;
            btn_superpixels.Enabled = false;
            btn_newObject.Enabled = false;
            btn_objectsDone.Enabled = false;
            btn_saveObject.Enabled = false;
            cBox_colorBased.Enabled = false;
            btn_signDetection.Enabled = false;
            btn_multipleSignDetection.Enabled = false;
            cBox_enhancement.Enabled = false;
            btn_enhancement.Enabled = false;
            btn_undoEnhancement.Enabled = false;
            btn_compareImages.Enabled = false;
            btn_next.Enabled = false;
            btn_previous.Enabled = false;

            // ComboBoxen initialisieren
            cBox_colorBased.Items.Add("RGB - Benallal");
            cBox_colorBased.Items.Add("RGB - Varun");
            cBox_colorBased.Items.Add("RGB - Gomez-Moreno");
            cBox_colorBased.Items.Add("RGB - Zaklouta");

            cBox_colorBased.Items.Add("HSV - Wang");
            cBox_colorBased.Items.Add("HSV - Chen");

            cBox_colorBased.Items.Add("HSI - De la Escalera");
            cBox_colorBased.Items.Add("HSI - Kuo");
            cBox_colorBased.Items.Add("HSI - Xu Qingsong");

            cBox_colorBased.SelectedIndex = 0;

            cBox_enhancement.Items.Add("Broggi (RGB Enhance)");
            cBox_enhancement.Items.Add("Ruta (RGB Enhance)");
            cBox_enhancement.Items.Add("Greyworld");
            cBox_enhancement.SelectedIndex = 0;

        }

        // ========================================================================
        // ==========================   Dateien öffnen   ==========================
        // ========================================================================

        // ----- Ground Truth Datei laden -----
        private void btn_loadGroundTruth_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog();
            Openfile.Filter = "txt files (*.txt)|*.txt";
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                // Datei zeilenweise auslesen
                groundTruthList = System.IO.File.ReadAllLines(Openfile.FileName).ToList();
            }

            btn_loadimage.Enabled = true;
            btn_loadMultipleImages.Enabled = true;

        }

        // ----- Button: Load Multiple Images -----

        private void btn_loadMultipleImages_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog();
            Openfile.Multiselect = true;
            //Openfile.Filter = "Images (*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|";

            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                images = new Dictionary<string, Image<Bgr, Byte>>();

                // alle Bilder in Liste einfügen
                foreach (String filePath in Openfile.FileNames)
                {
                    // Bild öffnen
                    Image<Bgr, Byte> image = CvInvoke.Imread(filePath, LoadImageType.AnyColor).ToImage<Bgr, Byte>();

                    // Bild mit Pfadnamen in Liste einfügen
                    images.Add(filePath, image);
                }

                // Elemente aktivieren
                trackBar1.Enabled = true;
                checkBoxK.Enabled = true;
                checkBoxSky.Enabled = true;
                btn_clearFog.Enabled = true;

                btn_loadDepthmap.Enabled = true;

                cBox_colorBased.Enabled = true;
                btn_signDetection.Enabled = true;
                btn_multipleSignDetection.Enabled = true;

                cBox_enhancement.Enabled = true;
                btn_enhancement.Enabled = true;
                btn_undoEnhancement.Enabled = true;

                btn_horizonDistance.Enabled = true;

                btn_compareImages.Enabled = true;

                btn_setVision.Enabled = true;

                btn_next.Enabled = true;
                btn_previous.Enabled = true;

                // erstes Bild anzeigen
                imageNr = 0;
                label_imageNrLeft.Text = (imageNr + 1).ToString();
                label_imageNrRight.Text = images.Count.ToString();

                // aktuelles Bild initialisieren
                initializeNewImage(images.ElementAt(imageNr));

                // Bild anzeigen
                imageBox.Image = imageOriginal;
                imageBox.Refresh();
            }

            
        }


        // ----- Button: Load Image -----
        private void btn_loadimage_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog();

            images = new Dictionary<string, Image<Bgr, Byte>>();
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                // Bild öffnen
                Image<Bgr, Byte> image = CvInvoke.Imread(Openfile.FileName, LoadImageType.AnyColor).ToImage<Bgr, Byte>();
                
                // Bild mit Pfadnamen in Liste einfügen
                images.Add(Openfile.FileName, image);
            }

            // Elemente aktivieren
            btn_loadDepthmap.Enabled = true;

            cBox_colorBased.Enabled = true;
            btn_signDetection.Enabled = true;
            btn_multipleSignDetection.Enabled = true;

            cBox_enhancement.Enabled = true;
            btn_enhancement.Enabled = true;
            btn_undoEnhancement.Enabled = true;

            btn_horizonDistance.Enabled = true;

            btn_compareImages.Enabled = true;
        }



        // ----- Initialize Image -----
        private void initializeNewImage(KeyValuePair<string, Image<Bgr, Byte>> image)
        {

            // Original-Mat
            //matOriginal = image.Value;

            // Bildnummer anzeigen
            label_imageNrLeft.Text = (imageNr + 1).ToString();
            label_imageNrLeft.Refresh();

            //Pfadname
            string currentFilePath = image.Key;

            // Dateiname
            currentFileName = Path.GetFileNameWithoutExtension(image.Key);

            // Originalbild erstellen
            imageOriginal = image.Value;

            // Grauwertbild erstellen
            imageGray = imageOriginal.Convert<Gray, byte>();

            // Bildgröße
            imageHeight = imageOriginal.Height;
            imageWidth = imageOriginal.Width;

            // Nebelbild erstellen
            imageFog = new Image<Bgr, Byte>(imageWidth, imageHeight);
            imageFog = imageOriginal.Clone();

            // Noisebild zurücksetzen
            //imageNoise.SetZero();

            // Bildobjekte erstellen
            imageNoise = new Image<Gray, Byte>(imageWidth, imageHeight);
            imageDepthmap = new Image<Gray, Byte>(imageWidth, imageHeight);
            imageRectangles = new Image<Bgr, Byte>(imageWidth, imageHeight);
            imageRoadsigns = new Image<Bgr, Byte>(imageWidth, imageHeight);


            // Depthmap zurücksetzen
            //imageDepthmap.SetZero();

            // Grauwertbild erstellen
            //imageGray = imageOriginal.ToImage<Gray, Byte>();

            // Superpixelbild erstellen
            //imageSuperpixels = new Image<Bgr, Byte>(imageWidth, imageHeight);

            // Trafficsigns Bild zurücksetzen
            //imageRoadsigns.SetZero();

            // Trafficsigns Rectangles zurücksetzen
            //imageRectangles.SetZero();

            // Bild anzeigen
            imageBox.Image = imageOriginal;

            // Skalierungsfaktor des Bildes berechnen
            scaleX = Convert.ToDouble(imageBox.Width) / Convert.ToDouble(imageWidth);
            scaleY = Convert.ToDouble(imageBox.Height) / Convert.ToDouble(imageHeight);

            // Tiefenmatrix und Noisematrix erstellen
            depthMatrix = new int[imageHeight, imageWidth];
            if (kNoiseMap == null)
            {
                kNoiseMap = new int[imageHeight, imageWidth];
            }

            //noiseMap = new int[imageHeight, imageWidth];
            //noise = false;

            /*
            for (int h = 0; h < imageHeight; h++)
            {
                for (int w = 0; w < imageWidth; w++)
                {
                    depthMap[h, w] = 0;
                    noiseMap[h, w] = 0;
                }
            } */

            // Listen mit Rechtecken zurücksetzen
            groundTruthRecs.Clear();
            detectedSigns.Clear();
            missedSigns.Clear();

            // Depthmap Bild erstellen
            imageDepthmap = CvInvoke.Imread(Path.Combine(Path.GetDirectoryName(currentFilePath), currentFileName) + "depth.png", LoadImageType.Grayscale).ToImage<Gray, Byte>();

            // Distanzen in Tiefenmatrix setzen und skylevel berechnen
            double counter = 0;
            for (int r = 0; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    //double minDistance = 4;
                    //depthMap[r, c] = (minDistance - horizonDistance) / 255 * imageDepthmap.Data[r, c, 0] + horizonDistance;

                    // 1 Schritt in Farbskala = 1 Meter
                    depthMatrix[r, c] = 255 - imageDepthmap.Data[r, c, 0];

                    // Skylevel berechnen
                    if (imageDepthmap.Data[r, c, 0] == 0)
                    {
                        skyLevel += imageGray.Data[r, c, 0];
                        counter++;
                    }
                }
            }
            skyLevel = (skyLevel / counter) / 255;

            //imageGray.Save("../../gray.jpg");

            //Console.WriteLine("Skylevel = " + skyLevel);

            // default depthmap laden
            //matDepthmap = CvInvoke.Imread(Path.Combine(Path.GetDirectoryName(currentFilePath), "defaultdepth.png"), LoadImageType.Grayscale);


            // Horizont Distanz aus Datei setzen
            /*
            List<string> horizonDistances = System.IO.File.ReadAllLines(Path.Combine(Path.GetDirectoryName(currentFilePath), "horizonDistances.txt")).ToList();
            foreach (string line in horizonDistances)
            {
                char delimiterChar = ';';
                string[] lineData = line.Split(delimiterChar);
                if (lineData[0] == currentFileName)
                {
                    horizonDistance = Convert.ToInt32(lineData[1]) - 50;
                }
            }*/

            //default Horizont distance
            //horizonDistance = 100;

            // Regionen und Verticals zurücksetzen
            //verticalObjects = new List<verticalObject>();
            //selectVerticals = false;
            //oldRegionNr = -1;

            depthMatrixCreated = false;
        }


        // ----- Button: Load Depthmap -----
        private void btn_loadDepthmap_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog();
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                // Bild öffnen
                matDepthmap = CvInvoke.Imread(Openfile.FileName, LoadImageType.Grayscale);

                // Depthmap Bild erstellen
                imageDepthmap = matDepthmap.ToImage<Gray, Byte>();
            }

            // nächsten Button aktivieren
            btn_horizonDistance.Enabled = true;
        }


        // ==========================================================================
        // ==========================   Parameter setzen   ==========================
        // ==========================================================================

        // ----- Horizont Entfernung setzen -----
        private void btn_horizonDistance_Click(object sender, EventArgs e)
        {
            // Form1 deaktivieren
            this.Enabled = false;
            // Dialog anzeigen
            distanceForm form2 = new distanceForm();
            form2.ShowDialog();
            // Horizontdistanz setzen
            horizonDistance = Convert.ToDouble(form2.distance);

            // Textbox updaten
            txt_horizon.Text = form2.distance;

            // Depthmap berechnen
            // Matrixwerte updaten
            for (int r = 0; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    double minDistance = 5;
                    depthMatrix[r, c] = Convert.ToInt32((minDistance - horizonDistance) / 255 * imageDepthmap.Data[r, c, 0] + horizonDistance);
                }
            }

            // Bild updaten
            updateFog();

            // Form1 aktivieren
            this.Enabled = true;

            // nächsten Button aktivieren
            btn_setSkylevel.Enabled = true;
        }

        // ----- Sichtweite über Button setzen -----
        private void btn_setVision_Click(object sender, EventArgs e)
        {
            // Form1 deaktivieren
            this.Enabled = false;
            // Dialog anzeigen
            distanceForm form2 = new distanceForm();
            form2.ShowDialog();
            // Sichtweite setzen
            setVision(Convert.ToInt32(form2.distance));
            // Form1 deaktivieren
            this.Enabled = true;
            // nächste Button aktivieren
            btn_addNoise.Enabled = true;
            btn_clearFog.Enabled = true;
        }
        

        // ----- Sichtweite setzen -----
        public void setVision(int visionDistance)
        {
            // Sichtweite setzen
            visibility = visionDistance;
            // Text updaten

            if (visibility == int.MaxValue)
            {
                trackBar1.Value = trackBar1.Maximum;
                //checkBoxK.Checked = false;
                //checkBoxSky.Checked = false;

                txt_vision.Text = "\u221E";

                imageBox.Image = imageOriginal;

                Console.WriteLine("--> Visibility = infinity" );
            }
            else
            {
                txt_vision.Text = visibility.ToString();
                trackBar1.Value = visibility;

                Console.WriteLine("--> Visibility = " + visibility + "m");
            }

            txt_vision.Refresh();
            trackBar1.Refresh();

            

            // Bild updaten
            updateFog();
        }



        // Schieberegler
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // Textfeld updaten
            txt_vision.Text = trackBar1.Value.ToString();
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            // Nebel updaten
            visibility = trackBar1.Value;
            updateFog();
        }


        // ===========================================================================
        // ==========================   Nebelbild updaten   ==========================
        // ===========================================================================

        // ----- Nebelbild updaten -----
        private void updateFog()
        {
            // Konstanten einlesen
            //double distance = Convert.ToDouble(txt_distance.Text);
            //double vision = Convert.ToDouble(txt_vision.Text);
            //double grayLevelSky = Convert.ToDouble(txt_graylevel.Text);

            Console.WriteLine("Update Fog");

            // k berechnen
            double twenty = 20;
            double log = Math.Log(twenty);
            double k = log / visibility;

            double distance = 0;

            //double max = 0;
            //double min = 9999;

            // Alle Pixel durchlaufen
            for (int r = 0; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {

                    //Pixeldistanz aus Tiefenmatrix auslesen
                    distance = depthMatrix[r, c];

                    double newK = k;
                    double newSkyLevel = skyLevel;

                    // aktueller Noisewert
                    double noiseValue = 1;

                    if (kNoise)
                    {
                        // Noisewert auf 0.5..1.5 normalisieren
                        noiseValue = (double)(kNoiseMap[r, c]) / 255 * 1.0 + 0.5;
                        // k berechnen
                        newK = k * noiseValue;

                        // Noisewert prozentual berechnen (ca 75 - 125 %)
                        //double noiseStrength = 0.015; // 0.01 - 0.03
                        //noiseValue = 1 + (imageNoise.Data[r, c, 0] - 25) * noiseStrength;
                        //Console.WriteLine(noiseValue);

                        //if (min > noiseValue) { min = noiseValue; }
                        //if (max < noiseValue) { max = noiseValue; }
                    }

                    if (skyNoise)
                    {
                        // Noisewert auf 0.8..1.0 normalisieren
                        noiseValue = (double)(skyNoiseMap[r, c]) / 255 * 0.15 + 0.85;

                        //Console.WriteLine(noiseValue);
                        // skyLevel berechnen
                        newSkyLevel = skyLevel * noiseValue;

                        //Console.WriteLine(skyLevel + "   " + newSkyLevel);
                    }

                    // BGR Farbkanäle
                    double blue = Convert.ToDouble(imageOriginal.Data[r, c, 0]) / 255;
                    double green = Convert.ToDouble(imageOriginal.Data[r, c, 1]) / 255;
                    double red = Convert.ToDouble(imageOriginal.Data[r, c, 2]) / 255;
                    //Bgr bgr = new Bgr(b, g, r);

                    // Umrechnen in HSV
                    // Hsv hsv = BGRtoHSV(bgr);

                    // Helligkeitswert ändern
                    //Hsv hsv2 = hsv;
                    //hsv2.Value = hsv.Value * Math.Exp(-k * distance) + skyLevel * (1 - Math.Exp(-k * distance));

                    //Umrechnen in BGR
                    //Bgr bgr2 = HSVtoBGR(hsv2);

                    //double newB = blue * Math.Exp(-k * distance * noiseValue) + skyLevel * (1 - Math.Exp(-k * distance * noiseValue));
                    //double newG = green * Math.Exp(-k * distance * noiseValue) + skyLevel * (1 - Math.Exp(-k * distance * noiseValue));
                    //double newR = red * Math.Exp(-k * distance * noiseValue) + skyLevel * (1 - Math.Exp(-k * distance * noiseValue));

                    

                    double newB = blue * Math.Exp(-newK * distance) + newSkyLevel * (1 - Math.Exp(-newK * distance));
                    double newG = green * Math.Exp(-newK * distance) + newSkyLevel * (1 - Math.Exp(-newK * distance));
                    double newR = red * Math.Exp(-newK * distance) + newSkyLevel * (1 - Math.Exp(-newK * distance));

                    imageFog.Data[r, c, 0] = Convert.ToByte(newB * 255);
                    imageFog.Data[r, c, 1] = Convert.ToByte(newG * 255);
                    imageFog.Data[r, c, 2] = Convert.ToByte(newR * 255);

                    //imageFog[h, w] = new Bgr(0, 1, 0);


                    //imageFog.Data[h, w, 0] = Convert.ToByte(bgr2.Blue);
                    //imageFog.Data[h, w, 1] = Convert.ToByte(bgr2.Green);
                    //imageFog.Data[h, w, 2] = Convert.ToByte(bgr2.Red);


                    //double gr = oldImageGray.Data[h, w, 0];

                    //byte hue = oldImage.Data[h, w, 0];
                    //byte sat = oldImage.Data[h, w, 1];
                    //byte val = oldImage.Data[h, w, 2];

                    // Bgr color = new Bgr(oldImage.Data[h, w, 0], oldImage.Data[h, w, 1], oldImage.Data[h, w, 2]);

                    // V Helligkeitskanal
                    //oldLuminance = oldImage.Data[h, w, 2];

                    // Helligkeitswert
                    //newLuminance = oldLuminance * Math.Exp(-k * distance) + grayLevelSky * (1 - Math.Exp(k * distance));

                    /*
                    Console.WriteLine("oldImageHsv: " + hue + "   oldImageBgr: " + b + "   ColorFromHSV: " + color.);
                    Console.WriteLine("oldImageHsv: " + sat + "   oldImageBg: " + g + "   ColorFromHSV: " + color.G);
                    Console.WriteLine("oldImageHsv: " + val + "   oldImageBgr: " + r + "   ColorFromHSV: " + color.R);
                    Console.WriteLine("");
                    */


                    //if (b == 0) { b = 1; }
                    //if (g == 0) { g = 1; }
                    //if (r == 0) { r = 1; }



                    //if (newB < 0) { newB = 0; }
                    //if (newG < 0) { newG = 0; }
                    //if (newR < 0) { newR = 0; }



                    //newImageGray.Data[h, w, 0] = Convert.ToByte(gr * Math.Exp(-k * distance) + grayLevelSky * (1 - Math.Exp(k * distance)));

                    /*
                    Bgr bgr = new Bgr(b, g, r);
                    Hsv hsv = bgr2hsv(bgr);

                    Console.WriteLine("b: " + b + " g: " + g + " r: " + r);
                    Console.WriteLine("hue: " + hsv.Hue + " sat: " + hsv.Satuation + " val: " + hsv.Value);
                    Console.WriteLine("");
                    */

                    //newImage.Data[h, w, 2] = Convert.ToByte(newLuminance);

                    //newImage.Data[h, w, 0] = (byte)(r * Math.Exp(-k * distance) + grayLevelSky * (1 - Math.Exp(k * distance)));
                    //newImage.Data[h, w, 1] = (byte)(g * Math.Exp(-k * distance) + grayLevelSky * (1 - Math.Exp(k * distance)));
                    //newImage.Data[h, w, 2] = (byte)(b * Math.Exp(-k * distance) + grayLevelSky * (1 - Math.Exp(k * distance)));

                }
                //Console.WriteLine("old: " + oldLuminance + "   new: " + newLuminance);
            }

            //Console.WriteLine("min = " + min);
            //Console.WriteLine("max = " + max);

            imageBox.Image = imageFog;
            imageBox.Refresh();

            //ib_fog.SetZoomScale(0.5, new Point(0, 0));
        }

        // ----- Nebel zurücksetzen -----
        private void btn_clearFog_Click(object sender, EventArgs e)
        {
            // Tiefenmatrix mit 0 füllen
            /*
            depthMap = new double[matOriginal.Height, matOriginal.Width];
            for (int h = 0; h < matOriginal.Height; h++)
            {
                for (int w = 0; w < matOriginal.Width; w++)
                {
                    depthMap[h, w] = 0;
                    noiseMap[h, w] = 0;
                }
            }
            noise = false;
            updateFog();
            */

            setVision(int.MaxValue);

        }


        // =======================================================================
        // ==========================   Maus-Aktionen   ==========================
        // =======================================================================

        // ----- Maus Klick -----
        private void ib_fog_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;

            // --- Rechteck ---
            /*if (!drawRectangle && !setHorizon && !setSkylevel)
            {
                // Buttons deaktivieren
                enableButtons(false);
                // Rechteck starten
                drawRectangle = true;
                rectangle.Location = e.Location;
                recStart = e.Location;
            }
            else if (drawRectangle)
            {
                // Form1 deaktivieren
                this.Enabled = false;
                // Dialog anzeigen
                Form2 form2 = new Form2();
                form2.ShowDialog();
                // Distanzwerte updaten
                updateDepthmapRectangle(rectangle, Convert.ToDouble(form2.distance));
                // Bild updaten
                updateFog();
                // Rechteck beendet
                drawRectangle = false;
                // Form1 aktivieren
                this.Enabled = true;
                // Buttons aktivieren
                enableButtons(true);
            }*/

            // --- Skylevel ---
            if (showSkylevelEllipse && !drawRectangle && !setHorizon && !selectVerticals)
            {

                // Bgr Wert auslesen
                //Bgr bgr = new Bgr(imageOriginal.Data[e.Y, e.X, 0], imageOriginal.Data[e.Y, e.X, 1], imageOriginal.Data[e.Y, e.X, 2]);
                // Helligkeit berechnen
                //Hsv hsv = BGRtoHSV(bgr);


                // Skylevel aus 121 Umgebungspixeln berechnen

                // Nachbarschaft Kantenlänge (immer ungerade)
                int n = 11;
                int l = (n - 1) / 2;

                skyLevel = 0;
                for (int h = Convert.ToInt32(e.Y / scaleY) - l; h <= Convert.ToInt32(e.Y / scaleY) + l; h++)
                {
                    for (int w = Convert.ToInt32(e.X / scaleX) - l; w <= Convert.ToInt32(e.X / scaleX) + l; w++)
                    {
                        skyLevel += Convert.ToDouble(imageGray.Data[h, w, 0]) / 255;
                    }
                }
                skyLevel = skyLevel / (n*n);
                //skyLevel = hsv.Value;

                // Textfeld updaten
                txt_skylevel.Text = skyLevel.ToString("0.00");


                // Buttons aktivieren
                btn_loadimage.Enabled = true;
                btn_loadDepthmap.Enabled = true;
                btn_horizonDistance.Enabled = true;
                btn_setVision.Enabled = true;
                btn_setHorizon.Enabled = true;
                btn_setSkylevel.Enabled = true;
                btn_addNoise.Enabled = true;
                btn_superpixels.Enabled = true;
                btn_newObject.Enabled = true;
                btn_objectsDone.Enabled = true;
                btn_saveObject.Enabled = true;
                cBox_colorBased.Enabled = true;
                btn_signDetection.Enabled = true;

                cBox_enhancement.Enabled = true;
                btn_enhancement.Enabled = true;

                btn_undoEnhancement.Enabled = false;

                btn_compareImages.Enabled = true;

                showSkylevelEllipse = false;

                // Bild updaten
                updateFog();
            }


            // --- Horizont ---
            /*
            if (setHorizon && !drawRectangle && !setSkylevel && !selectVerticals)
            {
                // Form1 deaktivieren
                this.Enabled = false;
                // Dialog anzeigen
                Form2 form2 = new Form2();
                form2.ShowDialog();
                // Horizontdistanz setzen
                double horizonDistance = Convert.ToDouble(form2.distance);
                // Horizontlevel updaten
                int horizonLevel = e.Location.Y * 2;
                // setHorizon false
                setHorizon = false;
                // Distanzwerte updaten
                updateDepthmapHorizon(horizonLevel, horizonDistance);
                // Textbox updaten
                txt_horizon.Text = form2.distance;
                // Bild updaten
                updateFog();
                // Form1 aktivieren
                this.Enabled = true;
                // Buttons aktivieren
                enableButtons(true);
            }
            */

            // --- Regionen ---
            /*
            if (!setHorizon && !drawRectangle && !setSkylevel && selectVerticals)
            {
                // Koordinaten
                int x = Convert.ToInt32(e.X / scaleX);
                int y = Convert.ToInt32(e.Y / scaleY);

                // Region rausfinden
                Pixel[,] pixels = superpixels.getPixelArray();
                int clusterNr = pixels[x, y].clusterNr;
                int regionNr = superpixels.getClusterList().ElementAt(clusterNr).regionNr;
                ClusterRegion region = superpixels.getRegionList().ElementAt(regionNr);

                // wenn Region schon ausgewählt war
                if (region.selected)
                {
                    // Region hellrot färben
                    colorRegion(region, new Bgr(150, 150, 255));

                    // selected Flag setzen
                    region.selected = false;

                    // aus selected Liste entfernen
                    newObject.regions.Remove(region);
                }
                // wenn Region noch nicht ausgewählt war
                else
                {
                    // Region dunkelrot färben
                    colorRegion(region, new Bgr(30, 30, 150));

                    // selected Flag setzen
                    region.selected = true;

                    // in selected Liste einfügen
                    newObject.regions.Add(region);
                }

                // Superpixel-Bild anzeigen
                imageBox.Image = imageSuperpixels;

            }
            */
        }



        // ----- Maus Bewegung -----
        private void ib_fog_MouseMove(object sender, MouseEventArgs e)
        {
            // --- Horizont ---
            /*
            if (setHorizon){
                horizon.P1 = new Point(0, e.Location.Y);
                horizon.P2 = new Point(matOriginal.Width, e.Location.Y);
                ((PictureBox)sender).Invalidate();
            }
            */

            // --- Skylevel ---
            if (showSkylevelEllipse)
            {
                skypoint = e.Location;
                ((PictureBox)sender).Invalidate();
            }

            // --- Rechteck ---
            /*
            if (drawRectangle)
            {
                Point recEnd = e.Location;
                rectangle.Location = new Point(Math.Min(recStart.X, recEnd.X), Math.Min(recStart.Y, recEnd.Y));
                rectangle.Size = new Size(Math.Abs(recStart.X - recEnd.X), Math.Abs(recStart.Y - recEnd.Y));
                ((PictureBox)sender).Invalidate();
            }
            */

            // --- Regionen ---
            /*
            if (selectVerticals)
            {
                // Koordinaten
                int x = Convert.ToInt32(e.X / scaleX);
                int y = Convert.ToInt32(e.Y / scaleY);

                // Region rausfinden
                Pixel[,] pixels = superpixels.getPixelArray();
                int clusterNr = pixels[x, y].clusterNr;
                int newRegionNr = superpixels.getClusterList().ElementAt(clusterNr).regionNr;
                
                // wenn Maus sich in neue Region bewegt
                if (newRegionNr != oldRegionNr)
                {
                    // bei nicht gedrückter Maustaste
                    if (!mouseDown)
                    {
                        // alte Region zurückfärben
                        if (oldRegionNr != -1)
                        {
                            ClusterRegion oldRegion = superpixels.getRegionList().ElementAt(oldRegionNr);
                            if (!oldRegion.selected)
                            {
                                colorRegion(oldRegion, new Bgr(oldRegion.color.Blue, oldRegion.color.Green, oldRegion.color.Red));
                            }
                        }

                        // neue Region hellrot färben
                        ClusterRegion newRegion = superpixels.getRegionList().ElementAt(newRegionNr);
                        if (!newRegion.selected)
                        {
                            colorRegion(newRegion, new Bgr(150, 150, 255));
                        }
                    }
                    // bei gedrückter Maustaste
                    else
                    {
                        // neue Region dunkelrot färben
                        ClusterRegion newRegion = superpixels.getRegionList().ElementAt(newRegionNr);
                        colorRegion(newRegion, new Bgr(30, 30, 150));

                        // selected Flag setzen
                        newRegion.selected = true;

                        // in selected Liste einfügen
                        newObject.regions.Add(newRegion);
                    }
                    // aktuelle Nummer merken
                    oldRegionNr = newRegionNr;
                }

                // Bild anzeigen
                imageBox.Image = imageSuperpixels;
            }
            */
        }

        // ==================================================================
        // ==========================   Zeichnen   ==========================
        // ==================================================================

        // ----- Zeichnen -----
        private void ib_fog_Paint(object sender, PaintEventArgs e)
        {
            // --- Horizont ---
            /*
            if (imageBox.Image != null && setHorizon)
            {
                e.Graphics.DrawLine(pen, horizon.P1, horizon.P2);
            }
            */

            // --- Skylevel ---
            if (showSkylevelEllipse)
            {
                Rectangle skyrectangle = new Rectangle(new Point(skypoint.X - 10, skypoint.Y - 10), new Size(20, 20));
                e.Graphics.DrawEllipse(penRed, skyrectangle);
            }

            /*
            // --- Rechteck ---
            if (imageBox.Image != null && drawRectangle)
            {
                e.Graphics.DrawRectangle(pen, rectangle);
            }
            */

            /*
            // --- Center ---
            if (imageBox.Image != null && drawCenters)
            {
                foreach (Rectangle r in centerRecs)
                {
                    e.Graphics.DrawRectangle(pen, r);
                }
            }
            */

            // --- grünes Rechteck für richtig erkannte Schilder ---
            foreach (Rectangle rec in detectedSigns)
            {
                Point scaledLocation = new Point(Convert.ToInt32(rec.Location.X * scaleX) - 1, Convert.ToInt32(rec.Location.Y * scaleY) - 1);
                Size scaledSize = new Size(Convert.ToInt32(rec.Size.Width * scaleX), Convert.ToInt32(rec.Size.Height * scaleY));

                Rectangle scaledRec = new Rectangle(scaledLocation, scaledSize);

                e.Graphics.DrawRectangle(penGreen, scaledRec);
            }

            // --- rotes Kreuz für nicht erkannte Schilder ---
            foreach (Rectangle rec in missedSigns)
            {
                int scaledLeft = Convert.ToInt32(rec.Left * scaleX) - 1;
                int scaledRight = Convert.ToInt32(rec.Right * scaleX) - 1;
                int scaledTop = Convert.ToInt32(rec.Top * scaleX) - 1;
                int scaledBottom = Convert.ToInt32(rec.Bottom * scaleX) - 1;

                Point lefttop = new Point(scaledLeft, scaledTop);
                Point righttop = new Point(scaledRight, scaledTop);
                Point leftbottom = new Point(scaledLeft, scaledBottom);
                Point rightbottom = new Point(scaledRight, scaledBottom);

                e.Graphics.DrawLine(penRed, lefttop, rightbottom);
                e.Graphics.DrawLine(penRed, leftbottom, righttop);
            }
        }


        // ===============================================================
        // ==========================   Noise   ==========================
        // ===============================================================


        // ----- Button Klick Add Noise -----
        private void btn_addNoise_Click(object sender, EventArgs e)
        {
            kNoiseMap = createNoisemap();

            // Noise auf depthMap anwenden
            //noise = true;
            updateFog();
        }



        // Status der k Noise Box geändert
        private void checkBoxK_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxK.Checked)
            {
                // neue Noisemap erstellen
                kNoiseMap = createNoisemap();

                kNoise = true;
            }
            else
            {
                kNoise = false;
            }
            // Nebel updaten
            updateFog();
        }

        // Status der Sky Noise Box geändert
        private void checkBoxSky_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSky.Checked)
            {
                // neue Noisemap erstellen
                skyNoiseMap = createNoisemap();

                skyNoise = true;
            }
            else
            {
                skyNoise = false;
            }
            // Nebel updaten
            updateFog();
        }


        // ----- Noisemap erzeugen -----
        private int[,] createNoisemap()
        {
            Console.WriteLine("Add Noise");

            int initialSize = 128;
            int size = initialSize;

            Random rnd = new Random();

            // Bilder erstellen
            imageNoise = new Image<Gray, Byte>(imageWidth, imageHeight);
            Image<Gray, Byte> imageTemp = new Image<Gray, Byte>(imageWidth, imageHeight);

            int[,] currentNoiseMap = new int[imageHeight, imageWidth];
            Image<Gray, Byte> currentNoiseImage = new Image<Gray, Byte>(imageWidth, imageHeight);

            int[,] finalNoiseMap = new int[imageHeight, imageWidth];
            Image<Gray, Byte> finalNoiseImage = new Image<Gray, Byte>(imageWidth, imageHeight);


            // Zähler für Loop
            int loop = 0;
            // Mehrere Noise Bilder erstellen
            while (size >= 1)
            {
                loop++;

                for (int h = 0; h < imageHeight; h++)
                {
                    for (int w = 0; w < imageWidth; w++)
                    {
                        // Zufallszahl 0-255 erzeugen
                        currentNoiseMap[h, w] = Convert.ToByte(rnd.Next(256));
                        
                        //Console.WriteLine(noiseMap[h, w]);

                        // Grauwert berechnen
                        //imageTemp.Data[h, w, 0] = Convert.ToByte(noiseMap[h / size, w / size] / loop);
                        currentNoiseImage.Data[h, w, 0] = Convert.ToByte(currentNoiseMap[h / size, w / size] / loop);
                    }
                }

                // Noise-Bild glätten
                currentNoiseImage = currentNoiseImage.SmoothBlur(size, size);
                //currentNoiseImage.Save("../../noise" + size + ".png");
                //Console.WriteLine(Math.Log(initialSize, 2) + 1);

                // Werte addieren
                for (int h = 0; h < imageHeight; h++)
                {
                    for (int w = 0; w < imageWidth; w++)
                    {
                        finalNoiseMap[h, w] += currentNoiseImage.Data[h, w, 0];
                    }
                }

                // Bild zum Gesamt-Noise addieren
                //imageNoise += imageTemp / (Math.Log(initialSize, 2) + 1);

                //imageNoise += imageTemp / (Math.Log(initialSize, 2) + 1);

                // Pixelgröße für nächsten Schleifendurchlauf halbieren
                size /= 2;
            }

            //
            int min = 999;
            int max = 0;
            for (int h = 0; h < imageHeight; h++)
            {
                for (int w = 0; w < imageWidth; w++)
                {
                    finalNoiseMap[h, w] /= Convert.ToInt32(Math.Log(initialSize, 2) + 1);
                    //imageNoise.Data[h, w, 0] = (byte)finalNoiseMap[h, w];
                    //noiseMap[h, w] = finalNoiseMap[h, w];

                    if (max < finalNoiseMap[h, w]) { max = finalNoiseMap[h, w]; }
                    if (min > finalNoiseMap[h, w]) { min = finalNoiseMap[h, w]; }

                }
            }

            // Werte für auf 0..255 normalisieren und speichern
            double range = max - min;
            for (int h = 0; h < imageHeight; h++)
            {
                for (int w = 0; w < imageWidth; w++)
                {
                    // Normalisieren
                    finalNoiseMap[h, w] = Convert.ToInt32((double)(finalNoiseMap[h, w] - min) / range * 255);
                    // Bild erstellen
                    imageNoise.Data[h, w, 0] = (byte)finalNoiseMap[h, w];
                }
            }

            //Console.WriteLine(min + " " + max);

            //imageNoise.Save("noise.jpg");
            //imageNoise.Save("../../noiseResult.png");
            //ib_fog.Image = imageNoise;

            return finalNoiseMap;
        }



        // =========================================================================
        // ==========================   Schilderkennung   ==========================
        // =========================================================================

        // ----- einzelne Schilderkennung mit aktueller Sichtweite starten -----
        private void btn_signDetection_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            checkSingleImage = true;
            detectSigns();
            checkSingleImage = false;
            this.Enabled = true;
        }


        // ----- mehrere Schilderkennungen (400m - 100m Sichtweite) starten -----
        private void btn_multipleSignDetection_Click(object sender, EventArgs e)
        {
            // Sichtweiten
            int[] visibilities = new int[] { int.MaxValue, 400, 300, 200, 100 };

            this.Enabled = false;

            // Bildnummer zurücksetzen
            imageNr = -1;

            // Ergebnis Objekt für gewählten Algorithmus erstellen
            results = new Results(cBox_colorBased.Text);

            // Alle Bilder durchlaufen
            foreach (KeyValuePair<string, Image<Bgr, Byte>> img in images)
            {
                // Bildnummer erhöhen
                imageNr++;

                // aktuelles Bild initialisieren
                initializeNewImage(img);

                // Schilder ohne Nebel erkennen
                //detectSigns();

                // Alle Sichtdistanzen durchlaufen
                foreach (int v in visibilities)
                {
                    // kein noise
                    //noise = false;

                    // Sichtweite setzen
                    setVision(v);

                    // Noise hinzufügen
                    // Noisematrix erstellen
                    kNoiseMap = createNoisemap();
                    // Noise auf depthMap anwenden
                    //noise = true;
                    updateFog();

                    // Nebelbild speichern
                    //if (v != int.MaxValue)
                    //{
                    //    imageBox.Image.Save("../../Images/" + currentFileName + "_" + v + ".jpg");
                    //}

                    // Schilder suchen
                    detectSigns();
                }

            }

            //results.showResults();
            results.saveResults();

            this.Enabled = true;
        }

        // ----- Schilderkennung durchführen -----
        public void detectSigns()
        {
            Console.WriteLine("Traffic Sign Detection");

            // Rechtecke entfernen
            detectedSigns.Clear();
            missedSigns.Clear();

            // aktuelles Bild auslesen
            //Image<Bgr, Byte> image = (Image<Bgr, Byte>)ib_fog.Image.Clone();

            // aktuelles Bild
            Image<Bgr, Byte> image = (Image<Bgr, Byte>)imageBox.Image.Clone();

            // Objekt anlegen
            colorBasedDetection = new ColorBasedDetection(image);

            // ausgewählten Erkennungs-Algorithmus ausführen
            colorBasedDetection.detectSigns(cBox_colorBased.SelectedIndex);

            // Bild mit erkannten Schildern zurückgeben
            imageRoadsigns = colorBasedDetection.getRoadsignImage();
            //imageRoadsigns.Save("red.jpg");


            // Bild mit Rechtecken um erkannte Schilder zurückgeben
            imageRectangles = colorBasedDetection.getRectangleImage();

            // gefundene Rechtecke zurückgeben
            foundRecs = colorBasedDetection.getFoundRecs();

            // Escalera Bild zurückgeben
            //imageRoadsigns = colorBasedDetection.getEscaleraImage();

            // Bild anzeigen
            //imageBox.Image = imageRoadsigns;
            //imageBox.Image = imageRectangles;

            // gefundene Rechtecke mit Ground Truth vergleichen
            compareGroundTruthWithFoundSigns();

            // Zeichnen
            imageBox.Invalidate();
        }

        // ----- ground Truth mit gefundenen Schildern vergleichen -----
        public void compareGroundTruthWithFoundSigns()
        {
            Console.WriteLine("Compare Ground Truth with found Signs");

            //int[] roundArray = new int[] { 1, 10, 20, 30, 40, 50 };
            //int[] roundArray = new int[] { 1, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50 };
            //int[] roundArray = new int[] { 1, 2, 4, 8, 16, 32, 42, 64 };
            //int[] roundArray = new int[] { 1, 2, 3, 5, 8, 13, 21, 34, 55 };

            int[] roundArray = new int[] { 1, 3, 6, 10, 15, 21, 28, 36, 45};

            // Listen löschen
            groundTruthRecs.Clear();
            detectedSigns.Clear();
            missedSigns.Clear();

            // Groundtruth Rechtecke erstellen
            List<string> lines = groundTruthList.FindAll(findLines);
            foreach (string line in lines)
            {
                char delimiterChar = ';';
                string[] lineData = line.Split(delimiterChar);

                int type = Convert.ToInt32(lineData[5]);

                // nur rote Schilder, blaue und weiße ausschließen
                if (type < 32 && type != 6 && type != 12)
                {
                    int left = Convert.ToInt32(lineData[1]);
                    int top = Convert.ToInt32(lineData[2]);
                    int right = Convert.ToInt32(lineData[3]);
                    int bottom = Convert.ToInt32(lineData[4]);

                    Rectangle rec = new Rectangle(new Point(left, top), new Size(right - left, bottom - top));
                    groundTruthRecs.Add(rec);
                }
            }
            // Alle Schilder als nicht gefunden definieren
            missedSigns = missedSigns.Concat(groundTruthRecs).ToList();

            //Console.WriteLine("-------- RESULTS ---------------------");

            // Alle Ground Truth Rechtecke durchlaufen
            foreach (Rectangle groundTruthRec in groundTruthRecs)
            {
                Console.WriteLine("---------------------");

                // Mittelpunkt berechnen
                int truthCenterX = (groundTruthRec.Left + groundTruthRec.Right) / 2;
                int truthCenterY = (groundTruthRec.Top + groundTruthRec.Bottom) / 2;

                // Mittelpunkt darf nur 10% der Schildgröße abweichen
                double toleranceX = (groundTruthRec.Right - groundTruthRec.Left) * 0.1;
                double toleranceY = (groundTruthRec.Bottom - groundTruthRec.Top) * 0.1;

                //Console.WriteLine("toleranceX = " + toleranceX);
                //Console.WriteLine("toleranceY = " + toleranceY);

                //Console.WriteLine("truthCenter: " + truthCenterX + "," + truthCenterY);


                // mit allen gefundenen Rechtecken vergleichen
                foreach (Rectangle foundRec in foundRecs)
                {
                    // Mittelpunkt berechnen
                    int foundCenterX = (foundRec.Left + foundRec.Right) / 2;
                    int foundCenterY = (foundRec.Top + foundRec.Bottom) / 2;

                    //Console.WriteLine("foundCenter: " + foundCenterX + "," + foundCenterY);

                    // Wenn innerhalb des angegebenen Bereichs
                    if (foundCenterX >= truthCenterX - toleranceX && foundCenterX <= truthCenterX + toleranceX && foundCenterY >= truthCenterY - toleranceY && foundCenterY <= truthCenterY + toleranceY)
                    {
                        // Schild in Liste der gefundenen Schilder einfügen
                        detectedSigns.Add(groundTruthRec);

                        // Schild aus Liste der nicht gefundenen Schilder entfernen
                        missedSigns.Remove(groundTruthRec);

                        // Entfernung des Schilds berechnen (gerundet, max 50)

                        // Distanz auf nächsten Wert im Round-Array runden
                        
                        int signDistance = depthMatrix[foundCenterY, foundCenterX];
                        
                        //Console.WriteLine("signDistance = " + signDistance);
                        for (int i = 0; i < roundArray.Length; i++)
                        {
                            if (signDistance != 0 && signDistance < roundArray[i])
                            {
                                int smaller = roundArray[i - 1];
                                int greater = roundArray[i];
                                if (signDistance - smaller < greater - signDistance)
                                {
                                    signDistance = smaller;
                                    i = 99999;
                                }
                                else
                                {
                                    signDistance = greater;
                                    i = 99999;
                                }
                            }
                        }
                        if (signDistance > 45) { signDistance = 45; }
                        
                        //Console.WriteLine("rounded signDistance = " + signDistance);


                        //int distance = (int)(((int)Math.Round(depthMatrix[foundCenterY, foundCenterX] / round)) * round);
                        //if (distance > 50) { distance = 50; }

                        //int distance = Convert.ToInt32(depthMatrix[foundCenterY, foundCenterX]);
                        Console.WriteLine(currentFileName + " - sign found (" + signDistance + "m)");

                        // Ergebnis "detected" festhalten (nur wenn Algorithmus nicht auf einzelnes Bild angewendet)
                        if (signDistance <= 100 && !checkSingleImage)
                        {
                            results.data[visibility][signDistance].Add(true);
                        }
                    }
                }
            }
            //Console.WriteLine(detectedSigns.Count + " of " + groundTruthRecs.Count + " signs detected");
            //Console.WriteLine("--------------------------------------");

            // Ergebnis "not detected" festhalten (nur wenn Algorithmus nicht auf einzelnes Bild angewendet)
            if (!checkSingleImage)
            {
                foreach (Rectangle rec in missedSigns)
                {
                    // Mittelpunkt berechnen
                    int foundCenterX = (rec.Left + rec.Right) / 2;
                    int foundCenterY = (rec.Top + rec.Bottom) / 2;

                    // Distanz auf nächsten Wert im Round-Array runden
                    int signDistance = depthMatrix[foundCenterY, foundCenterX];
                    
                    //Console.WriteLine("signDistance = " + signDistance);
                    for (int i = 0; i < roundArray.Length; i++)
                    {
                        if (signDistance != 0 && signDistance < roundArray[i])
                        {
                            int smaller = roundArray[i - 1];
                            int greater = roundArray[i];
                            if (signDistance - smaller < greater - signDistance)
                            {
                                signDistance = smaller;
                                i = 99999;
                            }
                            else
                            {
                                signDistance = greater;
                                i = 99999;
                            }
                        }
                    }
                    if (signDistance > 45) { signDistance = 45; }
                    
                    //Console.WriteLine("rounded signDistance = " + signDistance);


                    // Entfernung des Schilds berechnen (auf 10m gerundet)
                    //int distance = (int)(((int)Math.Round(depthMatrix[foundCenterY, foundCenterX] / round)) * round);
                    //if (distance > 50) { distance = 50; }

                    // Ergebnis "detected" festhalten
                    if (signDistance <= 100)
                    {
                        results.data[visibility][signDistance].Add(false);
                    }
                }
            }
            
        }


        // ----- Prädikat zum Finden der Ground Truth Zeilen -----
        private bool findLines(string line)
        {
            if (line.Substring(0, 5) == currentFileName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        // ----- Ergebnisse anzeigen -----
        private void btn_showResults_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog Openfile = new OpenFileDialog();
            Openfile.Filter = "txt files (*.txt)|*.txt";

            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                string[] lines;

                // Datei zeilenweise auslesen
                lines = System.IO.File.ReadAllLines(Openfile.FileName);

                // Dialog anzeigen
                resultsForm resultsForm = new resultsForm();
                resultsForm.loadData(lines, Path.GetFileNameWithoutExtension(Openfile.FileName));

                // anzeigen
                resultsForm.ShowDialog();
            }


        }




        // =======================================================================
        // ==========================   Bildvergleich   ==========================
        // =======================================================================


        // ----- aktuelles Bild mit Ergebnisbild vergleichen (ÄHNLICH WIE IM PAPER MIT DEHAZE!) -----
        private void btn_compareImages_Click(object sender, EventArgs e)
        {
            Image<Bgr, Byte> currentImage = (Image<Bgr, Byte>)imageBox.Image.Clone();

            double values = 0;
            double difference = 0;

            for (int r = 0; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    difference += Math.Abs(imageOriginal.Data[r, c, 0] - currentImage.Data[r, c, 0]);
                    difference += Math.Abs(imageOriginal.Data[r, c, 1] - currentImage.Data[r, c, 1]);
                    difference += Math.Abs(imageOriginal.Data[r, c, 2] - currentImage.Data[r, c, 2]);

                    values += 3;
                }
            }

            difference = difference / values;
            difference = 100 - difference * 100 / 255;

            txt_compare.Text = difference.ToString("0.00");
        }



        // ========================================================================
        // ==========================   Verbesserungen   ==========================
        // ========================================================================


        // ----- Bildverbesserung starten -----
        private void btn_enhancement_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Image Enhancement");

            // aktuelles Bild auslesen
            //Image<Bgr, Byte> image = (Image<Bgr, Byte>)ib_fog.Image.Clone();

            // aktuelles Bild
            Image<Bgr, Byte> image = (Image<Bgr, Byte>)imageBox.Image.Clone();

            // Enhancement Objekt anlegen
            enhancement = new Enhancement(image);

            // ausgewählten Erkennungs-Algorithmus ausführen
            enhancement.performEnhancement(cBox_enhancement.SelectedIndex);

            // verbessertes Bild zurückgeben
            imageEnhanced = enhancement.getImage();

            // Bild anzeigen
            imageBox.Image = imageEnhanced;

            //Button
            btn_undoEnhancement.Enabled = true;

        }


        // ----- Bildverbesserung rückgängig machen -----
        private void btn_undoEnhancement_Click(object sender, EventArgs e)
        {
            // Urpsrungsbild anzeigen
            imageBox.Image = imageFog;

            // Button
            btn_undoEnhancement.Enabled = false;
        }



        // ========================================================================
        // ==========================   Bildsteuerung   ==========================
        // ========================================================================


        // ----- nächstes Bild anzeigen -----
        private void btn_next_Click(object sender, EventArgs e)
        {
            if (imageNr < images.Count - 1)
            {
                imageNr++;
            }
            else
            {
                imageNr = 0;
            }

            // aktuelles Bild initialisieren
            initializeNewImage(images.ElementAt(imageNr));

            // Bild anzeigen
            imageBox.Image = imageOriginal;
            imageBox.Refresh();

            updateFog();
        }



        // ----- vorheriges Bild anzeigen -----
        private void btn_previous_Click(object sender, EventArgs e)
        {
            if (imageNr > 0)
            {
                imageNr--;
            }
            else
            {
                imageNr = images.Count - 1;
            }

            // aktuelles Bild initialisieren
            initializeNewImage(images.ElementAt(imageNr));

            // Bild anzeigen
            imageBox.Image = imageOriginal;
            imageBox.Refresh();

            updateFog();
        }



        // ==================================================================================================================================
        // ==================================================================================================================================
        // ====================   ALTE FUNKTIONEN   =========================================================================================
        // ==================================================================================================================================
        // ==================================================================================================================================


        // ----- Buttons aktivieren / deaktivieren -----
        /*
        private void enableButtons(bool enable)
        {
            btn_loadimage.Enabled = enable;
            btn_loadDepthmap.Enabled = enable;
            btn_setVision.Enabled = enable;
            btn_setHorizon.Enabled = enable;
            btn_setSkylevel.Enabled = enable;
            btn_addNoise.Enabled = enable;
            btn_clearFog.Enabled = enable;
            btn_superpixels.Enabled = enable;
            btn_newObject.Enabled = enable;
            btn_objectsDone.Enabled = enable;
            btn_saveObject.Enabled = enable;

            cBox_colorBased.Enabled = enable;
            btn_signDetection.Enabled = enable;
            btn_Back.Enabled = enable;

            cBox_enhancement.Enabled = enable;
            btn_enhancement.Enabled = enable;
            btn_undoEnhancement.Enabled = enable;

            btn_compareImages.Enabled = enable;
        }
        */


        // ----- Himmel Helligkeit festlegen ----
        private void btn_setSkylevel_Click(object sender, EventArgs e)
        {
            // Elemente deaktivieren
            btn_loadimage.Enabled = false;
            btn_loadDepthmap.Enabled = false;
            btn_horizonDistance.Enabled = false;
            btn_setVision.Enabled = false;
            btn_setHorizon.Enabled = false;
            btn_setSkylevel.Enabled = false;
            btn_addNoise.Enabled = false;
            trackBar1.Enabled = false;
            checkBoxK.Enabled = false;
            checkBoxSky.Enabled = false;
            btn_clearFog.Enabled = false;
            btn_superpixels.Enabled = false;
            btn_newObject.Enabled = false;
            btn_objectsDone.Enabled = false;
            btn_saveObject.Enabled = false;
            cBox_colorBased.Enabled = false;
            btn_signDetection.Enabled = false;
            cBox_enhancement.Enabled = false;
            btn_enhancement.Enabled = false;
            btn_undoEnhancement.Enabled = false;
            btn_compareImages.Enabled = false;

            // Skylevel Modus aktivieren
            showSkylevelEllipse = true;
        }


        // ----- Mausbutton losgelassen -----
        private void ib_fog_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }


        // ----- Depthmap updaten (Rechteck) -----
        private void updateDepthmapRectangle(Rectangle rec, double objectDistance)
        {
            // Matrixwerte updaten
            for (int h = Convert.ToInt32(rec.Location.Y / scaleY); h <= (rec.Location.Y + rec.Height) / scaleY; h++)
            {
                for (int w = Convert.ToInt32(rec.Location.X / scaleX); w <= (rec.Location.X + rec.Width) / scaleX; w++)
                {
                    depthMatrix[h, w] = (int)objectDistance;
                }
            }
        }


        // ----- Horizontlinie festlegen -----
        private void btn_setHorizon_Click(object sender, EventArgs e)
        {
            //enableButtons(false);
            setHorizon = true;
        }


        // ----- Depthmap updaten (Horizont) -----
        private void updateDepthmapHorizon(int horizonLevel, double horizonDistance)
        {
            // Schrittweiten pro Zeile
            double aboveHeight = horizonLevel;
            double belowHeight = imageHeight - horizonLevel;

            // Linear
            //double aboveStep = horizonDistance / aboveHeight;
            //double belowStep = horizonDistance / belowHeight;

            // Quadratisch
            double aboveStep = Math.Sqrt(horizonDistance) / aboveHeight;
            double belowStep = Math.Sqrt(horizonDistance) / belowHeight;

            // Matrixwerte updaten
            for (int h = 0; h < imageHeight; h++)
            {
                for (int w = 0; w < imageWidth; w++)
                {

                    // Approximation der Tiefenwerte, ober/unterhalb Horizont quadratisch, rechts/links Mitte linear

                    // oberhalb Horizont
                    if (h < horizonLevel)
                    {
                        // links von der Mitte
                        if (w < imageWidth / 2)
                        {
                            depthMatrix[h, w] = (int)(((aboveStep * h) * (aboveStep * h)) * ((double)w / ((double)imageWidth / 2)));
                        }
                        // rechts von Mitte
                        else
                        {
                            depthMatrix[h, w] = (int)(((aboveStep * h) * (aboveStep * h)) * (-(double)(w - imageWidth) / ((double)imageWidth / 2)));
                        }

                        //depthMap[h, w] = aboveStep * h;
                        //depthMap[h, w] = -(horizonDistance / (aboveHeight * aboveHeight)) * (h) * (h) + horizonDistance;      // ============== nicht korrekte Tiefenberechnung ==================
                    }



                    // unterhalb Horizont
                    else
                    {

                        // links von der Mitte
                        if (w < imageWidth / 2)
                        {
                            depthMatrix[h, w] = (int)((belowStep * (imageHeight - h)) * (belowStep * (imageHeight - h)) * ((double)w / ((double)imageWidth / 2)));
                        }
                        // rechts von Mitte
                        else
                        {
                            depthMatrix[h, w] = (int)((belowStep * (imageHeight - h)) * (belowStep * (imageHeight - h)) * (-(double)(w - imageWidth) / ((double)imageWidth / 2)));
                        }


                        //double x = (matOriginal.Height - h);
                        //depthMap[h, w] = 0.0372 * x * x + 0.628 * x;                                                            // ============== nicht korrekte Tiefenberechnung ==================


                        //depthMap[h, w] = belowStep * (matOriginal.Height - h);
                    }
                }
            }
            /*
            Console.WriteLine("Reihe Oben = " + depthMap[0, 300]);
            Console.WriteLine("Reihe Viertel = " + depthMap[horizonLevel / 4, 300]);
            Console.WriteLine("Reihe Hälfte = " + depthMap[horizonLevel/2, 300]);
            Console.WriteLine("Reihe 3-Viertel = " + depthMap[horizonLevel / 4 * 3, 300]);
            Console.WriteLine("Reihe Horizont-1 = " + depthMap[horizonLevel-1, 300]);
            Console.WriteLine("Reihe Horizont = " + depthMap[horizonLevel, 300]);
            */
        }

        // ------ Superpixels berechnen ------
        private void superpixels_Click(object sender, EventArgs e)
        {
            // Anzahl Superpixel
            int k = 960;    // zB: 6, 12, 28, 66, 84, 112, 252, 416, 608, 960

            // Rechteck Array erstellen
            centerRecs = new Rectangle[k];

            // Superpixelobjekt erstellen
            superpixels = new Superpixels(imageOriginal, k);

            // Superpixel berechnen
            superpixels.computeSuperpixels();

            // Cluster zurückgeben
            List<Cluster> clusterList = superpixels.getClusterList();



            // Rechtecke erstellen für Clustercenter zeichnen
            /*
            for (int i = 0; i < clusterList.Count(); i++){
                
                Rectangle r = new Rectangle();
                r.Location = new Point(Convert.ToInt32(clusterList.ElementAt(i).currentCenter.x * scaleX - 3), Convert.ToInt32(clusterList.ElementAt(i).currentCenter.y * scaleY - 3));
                r.Size = new Size(6, 6);

                centerRecs[i] = r;
            }
            */


            // Zufallsfarben erstellen
            /*
            Bgr[] colors = new Bgr[k];
            
            Random rnd = new Random();
            for (int i = 0; i < k; i++)
            {
                int b = rnd.Next(256);
                int g = rnd.Next(256);
                int r = rnd.Next(256);
                colors[i] = new Bgr(b, g, r);
            }
            */

            // Cluster durchlaufen und Pixel neu einfärben
            /*
            foreach (Cluster c in clusterList)
            {
                Dictionary<Pixel, int> pixels = c.pixels;

                foreach (KeyValuePair<Pixel, int> p in pixels)
                {
                    //imageOriginal.Data[p.Key.vector.y, p.Key.vector.x, 0] = Convert.ToByte(colors[i].Blue);
                    //imageOriginal.Data[p.Key.vector.y, p.Key.vector.x, 1] = Convert.ToByte(colors[i].Green);
                    //imageOriginal.Data[p.Key.vector.y, p.Key.vector.x, 2] = Convert.ToByte(colors[i].Red);

                    imageSuperpixels.Data[p.Key.vector.y, p.Key.vector.x, 0] = Convert.ToByte(c.color.Blue);
                    imageSuperpixels.Data[p.Key.vector.y, p.Key.vector.x, 1] = Convert.ToByte(c.color.Green);
                    imageSuperpixels.Data[p.Key.vector.y, p.Key.vector.x, 2] = Convert.ToByte(c.color.Red);
                }
                //Console.WriteLine("Center[{0}] = {1} pixels", i, clusters[i].getPixelCount());
                //Console.WriteLine("Center[{0}] = {1} pixels", i, clusters[i].getPixelCount());
            }
            */


            // Cluster einfärben
            //colorAllClusters();

            // Regionen einfärben
            colorAllRegions();

            //Console.WriteLine("Center[{0}] = {1} pixels", i, clusters[i].getPixelCount());
            //Console.WriteLine("Center[{0}] = {1} pixels", i, clusters[i].getPixelCount());

            // Center Zeichnen?
            drawCenters = false;

            // Superpixel-Bild anzeigen
            imageBox.Image = imageSuperpixels;

            // Buttons aktivieren/deaktivieren
            btn_newObject.Enabled = true;
            btn_objectsDone.Enabled = true;
            btn_superpixels.Enabled = false;
        }



        // ------ Cluster einfärben ------
        private void colorAllClusters()
        {
            List<Cluster> clusterList = superpixels.getClusterList();
            // Cluster durchlaufen und Pixel neu einfärben
            foreach (Cluster c in clusterList)
            {
                Dictionary<Pixel, int> pixels = c.pixels;
                foreach (KeyValuePair<Pixel, int> p in pixels)
                {
                    //imageOriginal.Data[p.Key.vector.y, p.Key.vector.x, 0] = Convert.ToByte(colors[i].Blue);
                    //imageOriginal.Data[p.Key.vector.y, p.Key.vector.x, 1] = Convert.ToByte(colors[i].Green);
                    //imageOriginal.Data[p.Key.vector.y, p.Key.vector.x, 2] = Convert.ToByte(colors[i].Red);

                    imageSuperpixels.Data[p.Key.vector.y, p.Key.vector.x, 0] = Convert.ToByte(c.color.Blue);
                    imageSuperpixels.Data[p.Key.vector.y, p.Key.vector.x, 1] = Convert.ToByte(c.color.Green);
                    imageSuperpixels.Data[p.Key.vector.y, p.Key.vector.x, 2] = Convert.ToByte(c.color.Red);
                }
            }
        }



        // ------ Regionen einfärben ------
        private void colorAllRegions()
        {
            List<ClusterRegion> regionList = superpixels.getRegionList();
            // Cluster durchlaufen und Pixel neu einfärben
            foreach (ClusterRegion r in regionList)
            {
                foreach (KeyValuePair<Cluster, int> c in r.clusters)
                {
                    Dictionary<Pixel, int> pixels = c.Key.pixels;
                    foreach (KeyValuePair<Pixel, int> p in pixels)
                    {
                        //imageOriginal.Data[p.Key.vector.y, p.Key.vector.x, 0] = Convert.ToByte(colors[i].Blue);
                        //imageOriginal.Data[p.Key.vector.y, p.Key.vector.x, 1] = Convert.ToByte(colors[i].Green);
                        //imageOriginal.Data[p.Key.vector.y, p.Key.vector.x, 2] = Convert.ToByte(colors[i].Red);

                        imageSuperpixels.Data[p.Key.vector.y, p.Key.vector.x, 0] = Convert.ToByte(r.color.Blue);
                        imageSuperpixels.Data[p.Key.vector.y, p.Key.vector.x, 1] = Convert.ToByte(r.color.Green);
                        imageSuperpixels.Data[p.Key.vector.y, p.Key.vector.x, 2] = Convert.ToByte(r.color.Red);
                    }
                }
            }
        }


        // ------ eine Region einfärben ------
        private void colorRegion(ClusterRegion region, Bgr color)
        {
            foreach (KeyValuePair<Cluster, int> c in region.clusters)
            {
                Dictionary<Pixel, int> pixels = c.Key.pixels;
                foreach (KeyValuePair<Pixel, int> p in pixels)
                {
                    imageSuperpixels.Data[p.Key.vector.y, p.Key.vector.x, 0] = Convert.ToByte(color.Blue);
                    imageSuperpixels.Data[p.Key.vector.y, p.Key.vector.x, 1] = Convert.ToByte(color.Green);
                    imageSuperpixels.Data[p.Key.vector.y, p.Key.vector.x, 2] = Convert.ToByte(color.Red);
                }
            }
        }



        // ----- Button neues Objekt markieren -----
        private void btn_newObject_Click(object sender, EventArgs e)
        {
            // Buttons aktivieren/deaktivieren
            btn_newObject.Enabled = false;
            btn_objectsDone.Enabled = false;
            btn_saveObject.Enabled = true;

            // neues Objekt anlegen
            newObject = new verticalObject();

            // Regionen-Auswahl aktivieren
            selectVerticals = true;
        }




        // ----- Button Objekt speichern -----
        private void btn_saveObject_Click(object sender, EventArgs e)
        {
            // Buttons aktivieren/deaktivieren
            btn_newObject.Enabled = true;
            btn_objectsDone.Enabled = true;
            btn_saveObject.Enabled = false;

            // Regionen-Auswahl deaktivieren
            selectVerticals = false;

            // Koordinaten für Tiefenwert bestimmen
            int objectX = newObject.getX();
            int objectY = newObject.getY();

            // Tiefenwert in Objekt definieren
            newObject.depthValue = depthMatrix[objectY, objectX];

            // Objekt in Liste aufnehmen
            verticalObjects.Add(newObject);
        }


        // ----- Button Objekte bestätigen -----
        private void btn_objectsDone_Click(object sender, EventArgs e)
        {
            // Buttons aktivieren/deaktivieren
            btn_newObject.Enabled = true;
            btn_objectsDone.Enabled = true;
            btn_saveObject.Enabled = false;

            // Regionen-Auswahl deaktivieren
            selectVerticals = false;

            // Depthmap für jedes Objekt aktualisieren
            updateDepthmapVerticals();

            // Nebel updaten
            updateFog();
        }

        
        // ----- Depthmap updaten (Verticals) -----
        private void updateDepthmapVerticals()
        {
            List<ClusterRegion> clusterRegions = superpixels.getRegionList();

            // Objekte durchlaufen
            foreach (verticalObject o in verticalObjects)
            {
                // Regionen durchlaufen
                foreach (ClusterRegion r in o.regions)
                {
                    // Cluster durchlaufen
                    foreach (KeyValuePair<Cluster, int> c in r.clusters)
                    {
                        // Pixel durchlaufen
                        foreach (KeyValuePair<Pixel, int> p in c.Key.pixels)
                        {
                            // Koordinaten
                            int x = p.Key.vector.x;
                            int y = p.Key.vector.y;

                            // Tiefenwert updaten
                            depthMatrix[y, x] = (int)o.depthValue;
                        }
                    }
                }
            }

            // Harte Kanten in Depthmap eliminieren
            smoothDepthmap();
        }



        // ----- Weiche Kanten in Depthmap -----
        private void smoothDepthmap()
        {
            // minimale Differenz der Tiefenwerte
            int diff = 5;

            // Tiefenwerte durchlaufen
            for (int y = 0; y < imageHeight; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {

                    double value = depthMatrix[y, x];
                    double newValue = value;
                    int valueCount = 1;

                    // Tiefenwert oben, unten, links, rechts auf harte Kante überprüfen
                    if (y - 1 >= 0 && Math.Abs(value - depthMatrix[y - 1, x]) > diff)
                    {
                        newValue += depthMatrix[y - 1, x];
                        valueCount++;
                    }
                    if (y + 1 < imageHeight && Math.Abs(value - depthMatrix[y + 1, x]) > diff)
                    {
                        newValue += depthMatrix[y + 1, x];
                        valueCount++;
                    }
                    if (x - 1 >= 0 && Math.Abs(value - depthMatrix[y, x - 1]) > diff)
                    {
                        newValue += depthMatrix[y, x - 1];
                        valueCount++;
                    }
                    if (x + 1 < matOriginal.Width && Math.Abs(value - depthMatrix[y, x + 1]) > diff)
                    {
                        newValue += depthMatrix[y, x + 1];
                        valueCount++;
                    }

                    // Durchschnittswert berechnen
                    newValue = newValue / valueCount;
                    depthMatrix[y, x] = (int)newValue;
                }
            }
        }




        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {

        }


        















        
        /*
        
        // ----- RGB to HSV -----
        private Hsv BGRtoHSV(Bgr bgr)
        {
            //Console.WriteLine("Bgr = " + bgr);
            Rgb rgb = new Rgb(bgr.Red, bgr.Green, bgr.Blue);
            //Console.WriteLine("Rgb = " + rgb);

            double red = bgr.Red / 255;
            double green = bgr.Green / 255;
            double blue = bgr.Blue / 255;

            double cmax = Math.Max(red, Math.Max(green, blue));
            double cmin = Math.Min(red, Math.Min(green, blue));

            double delta = cmax - cmin;

            // Hue
            double hue = 0;
            if (delta == 0)
            {
                hue = 0;
            }
            else if (cmax == red)
            {
                hue = 60 * (((green - blue) / delta) % 6.0);
            }
            else if (cmax == green)
            {
                hue = 60 * (((blue - red) / delta) + 2);
            }
            else if (cmax == blue)
            {
                hue = 60 * (((red - green) / delta) + 4);
            }

            if (hue < 0)
            {
                hue = 360 + hue;
            }

            // Saturation
            double satuation;
            if (cmax == 0)
            {
                satuation = 0;
            }
            else
            {
                satuation = delta / cmax;
            }

            // Value
            double value = cmax;

            Hsv hsv = new Hsv(hue, satuation, value);

            //Console.WriteLine("Hsv = " + hsv);
            //Console.WriteLine("");

            return hsv;
        }
        
        

        // ----- HSV to BGR -----
        private Bgr HSVtoBGR(Hsv hsv)
        {

            //Console.WriteLine("Hsv = " + hsv);

            double hue = hsv.Hue;
            double satuation = hsv.Satuation;
            double value = hsv.Value;

            double c = value * satuation;
            double x = c * (1 - Math.Abs((hue / 60) % 2 - 1));
            double m = value - c;

            Rgb rgb = new Rgb(0, 0, 0);

            if (0 <= hue && hue < 60)
            {
                rgb = new Rgb(c, x, 0);
            }
            else if (60 <= hue && hue < 120)
            {
                rgb = new Rgb(x, c, 0);
            }
            else if (120 <= hue && hue < 180)
            {
                rgb = new Rgb(0, c, x);
            }
            else if (180 <= hue && hue < 240)
            {
                rgb = new Rgb(0, x, c);
            }
            else if (240 <= hue && hue < 300)
            {
                rgb = new Rgb(x, 0, c);
            }
            else if (300 <= hue && hue < 360)
            {
                rgb = new Rgb(c, 0, x);
            }

            rgb = new Rgb((rgb.Red + m) * 255, (rgb.Green + m) * 255, (rgb.Blue + m) * 255);

            //Console.WriteLine("Rgb = " + rgb);

            Bgr bgr = new Bgr(rgb.Blue, rgb.Green, rgb.Red);

            //Console.WriteLine("Bgr = " + bgr);
            //Console.WriteLine("");

            return bgr;
        }
        */


    }
}
