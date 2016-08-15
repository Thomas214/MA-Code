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
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;



namespace Foggy
{
    public partial class mainForm : Form
    {
        // benötigte Bilder
        private Image<Bgr, Byte> imageOriginal;
        private Image<Bgr, Byte> imageFog;
        private Image<Gray, Byte> imageGray;
        private Image<Gray, Byte> imageNoise;
        private Image<Gray, Byte> imageDepthmap;
        private Image<Bgr, Byte> imageRoadsigns;
        private Image<Bgr, Byte> imageRectangles;

        // Liste für Bilder
        private Dictionary<string, Image<Bgr, Byte>> images;
        private int imageNr;

        // Bildgröße
        private int imageHeight;
        private int imageWidth;
        private double scaleX = 1;
        private double scaleY = 1;

        // aktuelle Parameter
        private int visibility = int.MaxValue;
        private double skyLevel = 1;
        private string currentFileName;

        // Tiefenmatrix
        private int[,] depthMatrix;

        // Noise Typen setzen
        private bool kNoise = false;
        private bool skyNoise = false;

        // Noisematrizen
        private int[,] kNoiseMap;
        private int[,] skyNoiseMap;

        // Zeichentools
        private static Brush brushRed = new SolidBrush(Color.FromArgb(200, 200, 0, 0));
        private static Brush brushGreen = new SolidBrush(Color.FromArgb(200, 0, 200, 0));
        private Pen penGreen = new Pen(brushGreen, 3);
        private Pen penRed = new Pen(brushRed, 3);

        // Farberkennungsobjekt
        private ColorSegmentation colorBasedDetection;

        // Schilderkennung für ein oder mehrere Bilder
        private bool checkSingleImage = false;

        // Listen mit gefundenen und nicht gefundenen Schildern
        private List<string> groundTruthList;
        private List<Rectangle> groundTruthRecs = new List<Rectangle>();
        private List<Rectangle> foundRecs = new List<Rectangle>();
        private List<Rectangle> detectedSigns = new List<Rectangle>();
        private List<Rectangle> missedSigns = new List<Rectangle>();

        // Ergebnisobjekt
        private Results results;


        // ----- Kontruktor -----
        public mainForm()
        {
            InitializeComponent();
        }

        // ----- Form geladen -----
        private void Form1_Load(object sender, EventArgs e)
        {
            // Elemente deaktivieren
            btn_loadMultipleImages.Enabled = false;
            trackBar1.Enabled = false;
            checkBoxK.Enabled = false;
            checkBoxSky.Enabled = false;
            btn_clearFog.Enabled = false;
            cBox_colorBased.Enabled = false;
            btn_signDetection.Enabled = false;
            btn_multipleSignDetection.Enabled = false;
            btn_next.Enabled = false;
            btn_previous.Enabled = false;

            // ComboBox initialisieren
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

                cBox_colorBased.Enabled = true;
                btn_signDetection.Enabled = true;
                btn_multipleSignDetection.Enabled = true;

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


        // ----- Initialize Image -----
        private void initializeNewImage(KeyValuePair<string, Image<Bgr, Byte>> image)
        {

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
            //imageGray.Save("../../gray.jpg");

            // Bildgröße
            imageHeight = imageOriginal.Height;
            imageWidth = imageOriginal.Width;

            // Nebelbild erstellen
            imageFog = new Image<Bgr, Byte>(imageWidth, imageHeight);
            imageFog = imageOriginal.Clone();

            // Bildobjekte erstellen
            imageNoise = new Image<Gray, Byte>(imageWidth, imageHeight);
            imageDepthmap = new Image<Gray, Byte>(imageWidth, imageHeight);
            imageRectangles = new Image<Bgr, Byte>(imageWidth, imageHeight);
            imageRoadsigns = new Image<Bgr, Byte>(imageWidth, imageHeight);

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
        }


        // ==========================================================================
        // =======================   Sichtweite einstellen    =======================
        // ==========================================================================
        

        // ----- Sichtweite setzen -----
        public void setVision(int visionDistance)
        {
            // Sichtweite setzen
            visibility = visionDistance;
            // Text updaten

            if (visibility == int.MaxValue)
            {
                trackBar1.Value = trackBar1.Maximum;
                txt_vision.Text = "\u221E";

                imageBox.Image = imageOriginal;

                //Console.WriteLine("Visibility = infinity" );
            }
            else
            {
                txt_vision.Text = visibility.ToString();
                trackBar1.Value = visibility;

                //Console.WriteLine("Visibility = " + visibility + "m");
            }

            txt_vision.Refresh();
            trackBar1.Refresh();

            // Bild updaten
            updateFog();
        }


        // Schieberegler bewegen
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // Textfeld updaten
            txt_vision.Text = trackBar1.Value.ToString();
        }

        // Schieberegler loslassen
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
            //Console.WriteLine("Update Fog");

            // k berechnen
            double twenty = 20;
            double log = Math.Log(twenty);
            double k = log / visibility;

            double distance = 0;

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

                    // Extinction Noise hinzufügen
                    if (kNoise)
                    {
                        // Noisewert auf 0.5..1.5 normalisieren
                        noiseValue = (double)(kNoiseMap[r, c]) / 255 * 1.0 + 0.5;
                        // k berechnen
                        newK = k * noiseValue;
                    }

                    // Skylevel Noise hinzufügen
                    if (skyNoise)
                    {
                        // Noisewert auf 0.8..1.0 normalisieren
                        noiseValue = (double)(skyNoiseMap[r, c]) / 255 * 0.15 + 0.85;

                        // skyLevel berechnen
                        newSkyLevel = skyLevel * noiseValue;
                    }

                    // BGR Farbkanäle
                    double blue = Convert.ToDouble(imageOriginal.Data[r, c, 0]) / 255;
                    double green = Convert.ToDouble(imageOriginal.Data[r, c, 1]) / 255;
                    double red = Convert.ToDouble(imageOriginal.Data[r, c, 2]) / 255;
                    
                    // neue Werte berechnen
                    double newB = blue * Math.Exp(-newK * distance) + newSkyLevel * (1 - Math.Exp(-newK * distance));
                    double newG = green * Math.Exp(-newK * distance) + newSkyLevel * (1 - Math.Exp(-newK * distance));
                    double newR = red * Math.Exp(-newK * distance) + newSkyLevel * (1 - Math.Exp(-newK * distance));

                    // Werte in Bild eintragen
                    imageFog.Data[r, c, 0] = Convert.ToByte(newB * 255);
                    imageFog.Data[r, c, 1] = Convert.ToByte(newG * 255);
                    imageFog.Data[r, c, 2] = Convert.ToByte(newR * 255);

                }
            }

            imageBox.Image = imageFog;
            imageBox.Refresh();

            // Nebelbild speichern
            /*
            if (visibility != int.MaxValue)
            {
                imageFog.Save("../../FoggyImages/" + currentFileName + "_" + visibility + ".jpg");
            }
            */
        }

        // ----- Nebel zurücksetzen -----
        private void btn_clearFog_Click(object sender, EventArgs e)
        {
            setVision(int.MaxValue);
        }


        // ==================================================================
        // ==========================   Zeichnen   ==========================
        // ==================================================================

        // ----- Zeichnen -----
        private void ib_fog_Paint(object sender, PaintEventArgs e)
        {

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

        // ----- Status der Extinction Noise Box geändert -----
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

        // ----- Status der Sky Noise Box geändert -----
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
            //Console.WriteLine("Add Noise");

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

                        // Grauwert berechnen
                        currentNoiseImage.Data[h, w, 0] = Convert.ToByte(currentNoiseMap[h / size, w / size] / loop);
                    }
                }

                // Noise-Bild glätten
                currentNoiseImage = currentNoiseImage.SmoothBlur(size, size);
                //currentNoiseImage.Save("../../noise" + size + ".png");

                // Werte addieren
                for (int h = 0; h < imageHeight; h++)
                {
                    for (int w = 0; w < imageWidth; w++)
                    {
                        finalNoiseMap[h, w] += currentNoiseImage.Data[h, w, 0];
                    }
                }

                // Pixelgröße für nächsten Schleifendurchlauf halbieren
                size /= 2;
            }

            int min = 9999;
            int max = 0;
            for (int h = 0; h < imageHeight; h++)
            {
                for (int w = 0; w < imageWidth; w++)
                {
                    finalNoiseMap[h, w] /= Convert.ToInt32(Math.Log(initialSize, 2) + 1);

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

            //imageNoise.Save("../../noiseResult.png");

            // noiseMap zurückgeben
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

            // UI deaktivieren
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

                // Noisemaps erstellen
                if (kNoise)
                {
                    kNoiseMap = createNoisemap();
                }
                if (skyNoise)
                {
                    skyNoiseMap = createNoisemap();
                }

                // Alle Sichtdistanzen durchlaufen
                foreach (int v in visibilities)
                {
                    // Sichtweite setzen
                    setVision(v);

                    // Schilder suchen
                    detectSigns();

                    // aktuelles Bild speichern
                    /*
                    if (v != int.MaxValue)
                    {
                        imageBox.Image.Save("../../Images/" + currentFileName + "_" + v + ".jpg");
                    }
                    */
                }

            }

            results.saveResults();

            // UI aktivieren
            this.Enabled = true;
        }

        // ----- Schilderkennung durchführen -----
        public void detectSigns()
        {
            //Console.WriteLine("Traffic Sign Detection");

            // Rechtecke entfernen
            detectedSigns.Clear();
            missedSigns.Clear();

            // aktuelles Bild
            Image<Bgr, Byte> image = (Image<Bgr, Byte>)imageBox.Image.Clone();

            // Objekt anlegen
            colorBasedDetection = new ColorSegmentation(image);

            // ausgewählten Erkennungs-Algorithmus ausführen
            colorBasedDetection.detectSigns(cBox_colorBased.SelectedIndex);

            // Bild mit erkannten Schildern zurückgeben
            imageRoadsigns = colorBasedDetection.getRoadsignImage();
            
            // Binärbild speichern
            //imageRoadsigns.Save("../../binary.jpg");

            // Bild mit Rechtecken um erkannte Schilder zurückgeben
            imageRectangles = colorBasedDetection.getRectangleImage();

            // gefundene Rechtecke zurückgeben
            foundRecs = colorBasedDetection.getFoundRecs();

            // gefundene Rechtecke mit Ground Truth vergleichen
            compareGroundTruthWithFoundSigns();

            // Zeichnen
            imageBox.Invalidate();
        }

        // ----- ground Truth mit gefundenen Schildern vergleichen -----
        public void compareGroundTruthWithFoundSigns()
        {
            //Console.WriteLine("Compare Ground Truth with found Signs");

            // Werte, auf die gerundet wird
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

            // Alle Ground Truth Rechtecke durchlaufen
            foreach (Rectangle groundTruthRec in groundTruthRecs)
            {
                //Console.WriteLine("---------------------");

                // Mittelpunkt berechnen
                int truthCenterX = (groundTruthRec.Left + groundTruthRec.Right) / 2;
                int truthCenterY = (groundTruthRec.Top + groundTruthRec.Bottom) / 2;

                // Mittelpunkt darf nur 10% der Schildgröße abweichen
                double toleranceX = (groundTruthRec.Right - groundTruthRec.Left) * 0.1;
                double toleranceY = (groundTruthRec.Bottom - groundTruthRec.Top) * 0.1;

                // mit allen gefundenen Rechtecken vergleichen
                foreach (Rectangle foundRec in foundRecs)
                {
                    // Mittelpunkt berechnen
                    int foundCenterX = (foundRec.Left + foundRec.Right) / 2;
                    int foundCenterY = (foundRec.Top + foundRec.Bottom) / 2;

                    // Wenn innerhalb des angegebenen Bereichs
                    if (foundCenterX >= truthCenterX - toleranceX && foundCenterX <= truthCenterX + toleranceX && foundCenterY >= truthCenterY - toleranceY && foundCenterY <= truthCenterY + toleranceY)
                    {
                        // Schild in Liste der gefundenen Schilder einfügen
                        detectedSigns.Add(groundTruthRec);

                        // Schild aus Liste der nicht gefundenen Schilder entfernen
                        missedSigns.Remove(groundTruthRec);

                        // Distanz aus Tiefenmatrix entnehmen
                        int signDistance = depthMatrix[foundCenterY, foundCenterX];

                        // Distanz auf nächsten Wert im Round-Array runden
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
                        
                        //Console.WriteLine(currentFileName + " - sign found (" + signDistance + "m)");

                        // Ergebnis "detected" festhalten (nur wenn Algorithmus nicht auf einzelnes Bild angewendet wird)
                        if (signDistance <= 100 && !checkSingleImage)
                        {
                            results.data[visibility][signDistance].Add(true);
                        }
                    }
                }
            }

            // Ergebnis "not detected" festhalten (nur wenn Algorithmus nicht auf einzelnes Bild angewendet wird)
            if (!checkSingleImage)
            {
                foreach (Rectangle rec in missedSigns)
                {
                    // Mittelpunkt berechnen
                    int foundCenterX = (rec.Left + rec.Right) / 2;
                    int foundCenterY = (rec.Top + rec.Bottom) / 2;

                    // Distanz aus Tiefenmatrix entnehmen
                    int signDistance = depthMatrix[foundCenterY, foundCenterX];

                    // Distanz auf nächsten Wert im Round-Array runden
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

                    // Ergebnis "not detected" festhalten
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

    }
}
