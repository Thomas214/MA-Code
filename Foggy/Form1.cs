using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
//using Emgu.CV.CvInvoke;


namespace Foggy
{
    public partial class Form1 : Form
    {

        // ========== Variablen ==========

        private Mat matOriginal;
        private Image<Bgr, Byte> imageOriginal;
        private Image<Bgr, Byte> imageFog;
        private Image<Gray, Byte> imageGray;
        private Image<Gray, Byte> imageNoise;
        private Image<Bgr, Byte> imageSuperpixels;
        private Image<Bgr, Byte> imageTrafficsigns;

        private double scaleX = 1;
        private double scaleY = 1;

        private double vision = 0;
        private double skyLevel = 0;
        private double[,] depthMap;

        private LineSegment2D horizon = new LineSegment2D();
        private Point skypoint = new Point();

        private Point recStart;
        private Rectangle rectangle = new Rectangle();

        private bool setHorizon = false;
        private bool setSkylevel = false;
        private bool drawRectangle = false;

        private bool noise = false;

        private static Brush brush = new SolidBrush(Color.FromArgb(128, 200, 0, 0));
        private Pen pen = new Pen(brush, 2);

        private byte[,] noiseMap;


        private Rectangle[] centerRecs;
        private bool drawCenters = false;

        private Superpixels superpixels;
        private bool selectVerticals = false;
        private int oldRegionNr = -1;

        verticalObject newObject;
        private List<verticalObject> verticalObjects;

        private bool mouseDown = false;



        // ========== Funktionen ==========

        // ----- Kontruktor -----
        public Form1()
        {
            InitializeComponent();
        }

        // ----- Form geladen -----
        private void Form1_Load(object sender, EventArgs e)
        {
            // Buttons initialisieren
            enableButtons(false);
            btn_loadimage.Enabled = true;

            // ComboBox initialisieren
            cBox_colorBased.Items.Add("Benallal & Meunier");
            cBox_colorBased.Items.Add("Estevez & Mehtarnavaz");
            cBox_colorBased.Items.Add("Varun");
            cBox_colorBased.Items.Add("Kuo & Lin");
            cBox_colorBased.SelectedIndex = 0;
        }
        

        // ----- Button: Load Image -----
        private void btn_loadimage_Click(object sender, EventArgs e)
        {
            OpenFileDialog Openfile = new OpenFileDialog();
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                // Bild öffnen
                matOriginal = CvInvoke.Imread(Openfile.FileName, LoadImageType.AnyColor);

                // Originalbild erstellen
                imageOriginal = matOriginal.ToImage<Bgr, Byte>();

                // Nebelbild erstellen
                imageFog = new Image<Bgr, Byte>(matOriginal.Width, matOriginal.Height);

                // Noisebild erstellen
                imageNoise = new Image<Gray, Byte>(matOriginal.Width, matOriginal.Height);

                // Grauwertbild erstellen
                imageGray = matOriginal.ToImage<Gray, Byte>();

                // Superpixelbild erstellen
                imageSuperpixels = new Image<Bgr, Byte>(matOriginal.Width, matOriginal.Height);

                // Trafficsigns Bild erstellen
                imageTrafficsigns = new Image<Bgr, Byte>(matOriginal.Width, matOriginal.Height);

                // Bild anzeigen
                ib_fog.Image = imageOriginal;

                // Skalierungsfaktor des Bildes berechnen
                scaleX = Convert.ToDouble(matOriginal.Width) / Convert.ToDouble(ib_fog.Width);
                scaleY = Convert.ToDouble(matOriginal.Height) / Convert.ToDouble(ib_fog.Height);

                // Tiefenmatrix und Noisematrix mit 0 initializieren
                depthMap = new double[matOriginal.Height, matOriginal.Width];
                noiseMap = new byte[matOriginal.Height, matOriginal.Width];
                for (int h = 0; h < matOriginal.Height; h++)
                {
                    for (int w = 0; w < matOriginal.Width; w++)
                    {
                        depthMap[h, w] = 0;
                        noiseMap[h, w] = 0;
                    }
                }

                // Buttons initialisieren
                enableButtons(false);
                btn_loadimage.Enabled = true;
                btn_setVision.Enabled = true;
                btn_superpixels.Enabled = true;
                btn_signDetection.Enabled = true;
                cBox_colorBased.Enabled = true;

                // Regionen und Verticals zurücksetzen
                verticalObjects = new List<verticalObject>();
                selectVerticals = false;
                oldRegionNr = -1;

            }
        }


        


        // ----- Sichtweite setzen -----
        private void btn_setVision_Click(object sender, EventArgs e)
        {
            // Form1 deaktivieren
            this.Enabled = false;
            // Dialog anzeigen
            Form2 form2 = new Form2();
            form2.ShowDialog();
            // Sichtweite setzen
            vision = Convert.ToDouble(form2.distance);
            // Text updaten
            txt_vision.Text = form2.distance;
            // Bild updaten
            updateFog();
            // Form1 deaktivieren
            this.Enabled = true;
            // Button aktivieren
            enableButtons(true);
        }



        // ----- Depthmap updaten (Rechteck) -----
        private void updateDepthmapRectangle(Rectangle rec, double objectDistance)
        {
            // Matrixwerte updaten
            for (int h = Convert.ToInt32(rec.Location.Y * scaleY); h <= (rec.Location.Y + rec.Height) * scaleY; h++)
            {
                for (int w = Convert.ToInt32(rec.Location.X * scaleX); w <= (rec.Location.X + rec.Width) * scaleX; w++)
                {
                    depthMap[h, w] = objectDistance;
                }
            }
        }


        // ----- Depthmap updaten (Horizont) -----
        private void updateDepthmapHorizon(int horizonLevel, double horizonDistance)
        {
            // Schrittweiten pro Zeile
            double aboveHeight = horizonLevel;
            double belowHeight = matOriginal.Height - horizonLevel;

            // Linear
            //double aboveStep = horizonDistance / aboveHeight;
            //double belowStep = horizonDistance / belowHeight;

            // Quadratisch
            double aboveStep = Math.Sqrt(horizonDistance) / aboveHeight;
            double belowStep = Math.Sqrt(horizonDistance) / belowHeight;

            // Matrixwerte updaten
            for (int h = 0; h < matOriginal.Height; h++)
            {
                for (int w = 0; w < matOriginal.Width; w++)
                {

                    // Approximation der Tiefenwerte, ober/unterhalb Horizont quadratisch, rechts/links Mitte linear

                    // oberhalb Horizont
                    if (h < horizonLevel)
                    {
                        // links von der Mitte
                        if (w < matOriginal.Width/2)
                        {
                            depthMap[h, w] = ((aboveStep * h) * (aboveStep * h)) * ((double)w / ((double)matOriginal.Width / 2));
                        }
                        // rechts von Mitte
                        else
                        {
                            depthMap[h, w] = ((aboveStep * h) * (aboveStep * h)) * (-(double)(w - matOriginal.Width) / ((double)matOriginal.Width / 2));
                        }

                        //depthMap[h, w] = aboveStep * h;
                        //depthMap[h, w] = -(horizonDistance / (aboveHeight * aboveHeight)) * (h) * (h) + horizonDistance;      // ============== nicht korrekte Tiefenberechnung ==================
                    }



                    // unterhalb Horizont
                    else
                    {

                        // links von der Mitte
                        if (w < matOriginal.Width/2)
                        {
                            depthMap[h, w] = (belowStep * (matOriginal.Height - h)) * (belowStep * (matOriginal.Height - h)) *((double)w / ((double)matOriginal.Width / 2));
                        }
                        // rechts von Mitte
                        else
                        {
                            depthMap[h, w] = (belowStep * (matOriginal.Height - h)) * (belowStep * (matOriginal.Height - h))  *(-(double)(w - matOriginal.Width) / ((double)matOriginal.Width / 2));
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


        // ----- Nebelbild updaten -----
        private void updateFog()
        {
            // Konstanten einlesen
            //double distance = Convert.ToDouble(txt_distance.Text);
            //double vision = Convert.ToDouble(txt_vision.Text);
            //double grayLevelSky = Convert.ToDouble(txt_graylevel.Text);

            // k berechnen
            double twenty = 20;
            double log = Math.Log(twenty);
            double k = log / vision;

            double distance = 0;

            // Alle Pixel durchlaufen
            for (int r = 0; r < matOriginal.Height; r++)
            {
                for (int c = 0; c < matOriginal.Width; c++)
                {

                    //Pixeldistanz aus depthmap auslesen
                    distance = depthMap[r, c];

                    // aktueller Noisewert
                    double noiseValue = 1;
                    if (noise)
                    {
                        // Noisewert prozentual berechnen (ca 75 - 125 %)
                        double noiseStrength = 0.015; // 0.01 - 0.03
                        noiseValue = 1 + (imageNoise.Data[r, c, 0] - 25) * noiseStrength;
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

                    double newB = blue * Math.Exp(-k * distance * noiseValue) + skyLevel * (1 - Math.Exp(-k * distance * noiseValue));
                    double newG = green * Math.Exp(-k * distance * noiseValue) + skyLevel * (1 - Math.Exp(-k * distance * noiseValue));
                    double newR = red * Math.Exp(-k * distance * noiseValue) + skyLevel * (1 - Math.Exp(-k * distance * noiseValue));

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


            ib_fog.Image = imageFog;


            //ib_fog.SetZoomScale(0.5, new Point(0, 0));
        }


        // ----- Horizontlinie festlegen -----
        private void btn_setHorizon_Click(object sender, EventArgs e)
        {
            enableButtons(false);
            setHorizon = true;
        }


        // ----- Himmel Helligkeit festlegen ----
        private void btn_setSkylevel_Click(object sender, EventArgs e)
        {
            enableButtons(false);
            setSkylevel = true;
        }


        // ----- Nebel zurücksetzen -----
        private void btn_clearFog_Click(object sender, EventArgs e)
        {
            // Tiefenmatrix mit 0 füllen
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
        }




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
            if (setSkylevel && !drawRectangle && !setHorizon && !selectVerticals)
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
                for (int h = Convert.ToInt32(e.Y * scaleY) - l; h <= Convert.ToInt32(e.Y * scaleY) + l; h++)
                {
                    for (int w = Convert.ToInt32(e.X * scaleX) - l; w <= Convert.ToInt32(e.X * scaleX) + l; w++)
                    {
                        skyLevel += Convert.ToDouble(imageGray.Data[h, w, 0]) / 255;
                    }
                }
                skyLevel = skyLevel / (n*n);
                //skyLevel = hsv.Value;

                // Textfeld updaten
                txt_skylevel.Text = skyLevel.ToString("0.00");
                // Bild updaten
                updateFog();
                // Buttons aktivieren
                enableButtons(true);

                setSkylevel = false;
            }


            // --- Horizont ---
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


            // --- Regionen ---
            if (!setHorizon && !drawRectangle && !setSkylevel && selectVerticals)
            {
                // Koordinaten
                int x = Convert.ToInt32(e.X * scaleX);
                int y = Convert.ToInt32(e.Y * scaleY);

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
                ib_fog.Image = imageSuperpixels;

            }
        }



        // ----- Maus Bewegung -----
        private void ib_fog_MouseMove(object sender, MouseEventArgs e)
        {
            // --- Horizont ---
            if (setHorizon){
                horizon.P1 = new Point(0, e.Location.Y);
                horizon.P2 = new Point(matOriginal.Width, e.Location.Y);
                ((PictureBox)sender).Invalidate();
            }

            // --- Skylevel ---
            if (setSkylevel)
            {
                skypoint = e.Location;
                ((PictureBox)sender).Invalidate();
            }

            // --- Rechteck ---
            if (drawRectangle)
            {
                Point recEnd = e.Location;
                rectangle.Location = new Point(Math.Min(recStart.X, recEnd.X), Math.Min(recStart.Y, recEnd.Y));
                rectangle.Size = new Size(Math.Abs(recStart.X - recEnd.X), Math.Abs(recStart.Y - recEnd.Y));
                ((PictureBox)sender).Invalidate();
            }

            // --- Regionen ---
            if (selectVerticals)
            {
                // Koordinaten
                int x = Convert.ToInt32(e.X * scaleX);
                int y = Convert.ToInt32(e.Y * scaleY);

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
                ib_fog.Image = imageSuperpixels;
            }

        }


        // ----- Zeichnen -----
        private void ib_fog_Paint(object sender, PaintEventArgs e)
        {
            // --- Horizont ---
            if (ib_fog.Image != null && setHorizon)
            {
                e.Graphics.DrawLine(pen, horizon.P1, horizon.P2);
            }

            // --- Skylevel ---
            if (ib_fog.Image != null && setSkylevel)
            {
                Rectangle skyrectangle = new Rectangle(new Point(skypoint.X - 10, skypoint.Y - 10), new Size(20, 20));
                e.Graphics.DrawEllipse(pen, skyrectangle);
            }

            // --- Rechteck ---
            if (ib_fog.Image != null && drawRectangle)
            {
                e.Graphics.DrawRectangle(pen, rectangle);
            }

            // --- Center ---
            if (ib_fog.Image != null && drawCenters)
            {
                foreach (Rectangle r in centerRecs)
                {
                    e.Graphics.DrawRectangle(pen, r);
                }
            }
        }


        // ----- Button Klick Add Noise -----
        private void btn_addNoise_Click(object sender, EventArgs e)
        {
            int initialSize = 128;
            int size = initialSize;

            Random rnd = new Random();

            // Bilder erstellen
            imageNoise = new Image<Gray, Byte>(matOriginal.Width, matOriginal.Height);
            Image<Gray, Byte> imageTemp = new Image<Gray, Byte>(matOriginal.Width, matOriginal.Height);

            // Zähler für Loop
            int loop = 1;
            // Mehrere Noise Bilder erstellen
            while (size >= 1)
            {
                loop++;
                for (int h = 0; h < matOriginal.Height; h++)
                {
                    for (int w = 0; w < matOriginal.Width; w++)
                    {
                        // Zufallszahl 0-255 erzeugen
                        noiseMap[h, w] = Convert.ToByte(rnd.Next(256));
                        // Grauwert berechnen
                        imageTemp.Data[h, w, 0] = Convert.ToByte(noiseMap[h / size, w / size] / loop);
                    }
                }

                // Noise-Bild glätten
                imageTemp = imageTemp.SmoothBlur(size, size);
                //imageTemp.Save("C:/Users/Thomas/Desktop/Noises/" + size + ".jpg");
                //Console.WriteLine(Math.Log(initialSize, 2) + 1);


                // Bild zum Gesamt-Noise addieren
                imageNoise += imageTemp / (Math.Log(initialSize, 2) + 1);

                // Pixelgröße für nächsten Schleifendurchlauf halbieren
                size /= 2;
            }

            //imageNoise.Save("C:/Users/Thomas/Desktop/sum.jpg");
            //ib_fog.Image = imageNoise;

            
            // Noise auf depthMap anwenden
            noise = true;
            updateFog();
            
        }




        // ----- Buttons aktivieren / deaktivieren -----
        private void enableButtons(bool enable)
        {
            btn_loadimage.Enabled = enable;
            btn_setVision.Enabled = enable;
            btn_setHorizon.Enabled = enable;
            btn_setSkylevel.Enabled = enable;
            btn_addNoise.Enabled = enable;
            btn_clearFog.Enabled = enable;
            btn_superpixels.Enabled = enable;
            btn_newObject.Enabled = enable;
            btn_objectsDone.Enabled = enable;
            btn_saveObject.Enabled = enable;
            btn_signDetection.Enabled = enable;
            cBox_colorBased.Enabled = enable;
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
                r.Location = new Point(Convert.ToInt32(clusterList.ElementAt(i).currentCenter.x / scaleX - 3), Convert.ToInt32(clusterList.ElementAt(i).currentCenter.y / scaleY - 3));
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
            ib_fog.Image = imageSuperpixels;

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
            newObject.depthValue = depthMap[objectY, objectX];

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
            List <ClusterRegion> clusterRegions = superpixels.getRegionList();

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
                            depthMap[y, x] = o.depthValue;
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
            for (int y = 0; y < matOriginal.Height; y++){
                for (int x = 0; x < matOriginal.Width; x++){

                    double value = depthMap[y,x];
                    double newValue = value;
                    int valueCount = 1;

                    // Tiefenwert oben, unten, links, rechts auf harte Kante überprüfen
                    if (y - 1 >= 0 && Math.Abs(value - depthMap[y - 1, x]) > diff)
                    {
                        newValue += depthMap[y - 1, x];
                        valueCount++;
                    }
                    if (y + 1 < matOriginal.Height && Math.Abs(value - depthMap[y + 1, x]) > diff)
                    {
                        newValue += depthMap[y + 1, x];
                        valueCount++;
                    }
                    if (x - 1 >= 0 && Math.Abs(value - depthMap[y, x - 1]) > diff)
                    {
                        newValue += depthMap[y, x - 1];
                        valueCount++;
                    }
                    if (x + 1 < matOriginal.Width && Math.Abs(value - depthMap[y, x + 1]) > diff)
                    {
                        newValue += depthMap[y, x + 1];
                        valueCount++;
                    }

                    // Durchschnittswert berechnen
                    newValue = newValue / valueCount;
                    depthMap[y, x] = newValue;
                }
            }
        }



        // ----- Mausbutton losgelassen -----
        private void ib_fog_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }








        // ----- Schilderkennung starten -----
        private void btn_signDetection_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Traffic Sign Detection");

            // aktuelles Bild auslesen
            Image<Bgr, Byte> image = (Image<Bgr, Byte>)ib_fog.Image.Clone();

            // Objekt anlegen
            ColorBasedDetection colorBasedRecognition = new ColorBasedDetection(image);

            // ausgewählten Erkennungs-Algorithmus ausführen
            colorBasedRecognition.detectSigns(cBox_colorBased.SelectedIndex);

            // Bild mit erkannten Schildern zurückgeben
            imageTrafficsigns = colorBasedRecognition.getImage();

            // Bild anzeigen
            ib_fog.Image = imageTrafficsigns;
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
