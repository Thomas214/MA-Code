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

namespace Foggy
{
    class ColorBasedDetection
    {

        // Bilder
        Image<Bgr, Byte> imageInput;
        Image<Bgr, Byte> imageRoadsigns;
        Image<Bgr, Byte> imageRectangles;
        Image<Bgr, Byte> imageEscalera;

        // gefundene Schilder
        List<Rectangle> foundRecs = new List<Rectangle>();

        // Arrays
        RoadsignPixel[,] pixels;

        // Bildgröße
        int imageHeight;
        int imageWidth;

        // Konstruktor
        public ColorBasedDetection(Image<Bgr, Byte> image)
        {
            imageHeight = image.Height;
            imageWidth = image.Width;

            imageInput = image.Copy();
            //imageTrafficsigns = imageBgr.Copy();
            imageRoadsigns = new Image<Bgr, Byte>(imageWidth, imageHeight);
            imageEscalera = new Image<Bgr, Byte>(imageWidth, imageHeight);
            imageRectangles = imageInput.Copy();

            // Pixelarray anlegen
            pixels = new RoadsignPixel[imageHeight, imageWidth];
            for (int r = 0; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    pixels[r, c] = new RoadsignPixel(r, c);
                }
            }
        }

        // Schilder finden
        public void detectSigns(int method){

            //imageTrafficsigns = imageOriginal;

            switch (method)
            {
                case 0:
                    benallal();
                    break;
                case 1:
                    estevez();
                    break;
                case 2:
                    varun();
                    break;
                case 3:
                    kuo();
                    break;
                case 4:
                    piccioli();
                    break;
                case 5:
                    paclik();
                    break;
                case 6:
                    escalera();
                    break;
            }

            // Regionen finden und zu kleine löschen
            removeSmallRegions();

            // Bild erzeugen
            createImage();
        }


        // ---------- RGB Thresholding - Benallal and Meunier ----------
        public void benallal()
        {
            // Bild durchlaufen
            for (int r = 0; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    double blue = imageInput.Data[r, c, 0];
                    double green = imageInput.Data[r, c, 1];
                    double red = imageInput.Data[r, c, 2];

                    double thresholdRG = 100;
                    double thresholdRB = 100;
                    double thresholdGB = 100;

                    // Wenn Pixel rot
                    if (red > green && ((red - green) >= thresholdRG) && ((red - blue) >= thresholdRB))
                    {
                        // Pixel rot färben
                        pixels[r, c].setRed();
                    }
                    // Wenn Pixel blau
                    else if (blue > green && ((blue - green) >= thresholdGB) && ((blue - red) >= thresholdRB))
                    {
                        // Pixel blau färben
                        pixels[r, c].setBlue();
                    }
                }
            }
        }

        // ---------- RGB Thresholding - Estevez and Mehtarnavaz ----------
        public void estevez()
        {
            // Bild durchlaufen
            for (int r = 0; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    double blue = imageInput.Data[r, c, 0];
                    double green = imageInput.Data[r, c, 1];
                    double red = imageInput.Data[r, c, 2];

                    double alpha = 2;

                    double redness = red - Math.Max(green, blue) - alpha * Math.Abs(green - blue);
                    // Wenn Pixel rot
                    if (redness > 0)
                    {
                        // Pixel rot färben
                        pixels[r, c].setRed();
                    }
                }
            }
        }


        // ---------- RGB Thresholding - Varun ----------
        public void varun()
        {
            // Bild durchlaufen
            for (int r = 0; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    double blue = imageInput.Data[r, c, 0];
                    double green = imageInput.Data[r, c, 1];
                    double red = imageInput.Data[r, c, 2];

                    // Wenn Pixel rot
                    if (1.5 * red > green + blue)
                    {
                        // Pixel rot färben
                        pixels[r, c].setRed();
                    }
                }
            }
        }



        // ---------- HSI Thresholding - Kuo & Lin ----------
        public void kuo()
        {
            // Bild durchlaufen
            for (int r = 0; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    // HSI Wert berechnen
                    Bgr bgr = new Bgr(imageInput.Data[r, c, 0], imageInput.Data[r, c, 1], imageInput.Data[r, c, 2]);
                    Hsv hsi = BGRtoHSI(bgr);

                    double hue = hsi.Hue;
                    double sat = hsi.Satuation;
                    double inten = hsi.Value;

                    // Falls Blauwert größer als Grünwert
                    if (imageInput.Data[r, c, 0] > imageInput.Data[r, c, 1])
                    {
                        // Hue in Grad umrechnen, von 360 abziehen und zurückrechnen
                        hue = hue * 57.2958;
                        hue = 360 - hue;
                        hue = hue / 57.2958;
                    }

                    // Wenn Pixel rot
                    if ((hue >= 0 && hue < 0.111 * Math.PI) || (hue >= 1.8 * Math.PI && hue < 2 * Math.PI))
                    {
                        if (sat > 0.1 && sat <= 1)
                        {
                            if (inten > 0.12 && inten < 0.8)
                            {
                                // Pixel rot färben
                                pixels[r, c].setRed();
                            }
                        }
                    }

                    // Wenn Pixel blau
                    if (hue >= 1.066 * Math.PI && hue <= 1.555 * Math.PI)
                    {
                        if (sat > 0.28 && sat <= 1)
                        {
                            if (inten > 0.22 && inten < 0.5)
                            {
                                // Pixel blau färben
                                pixels[r, c].setBlue();
                            }
                        }
                    }
                }
            }
        }


        // ---------- HSI Thresholding - Piccioli ----------
        public void piccioli()
        {
            // Bild durchlaufen
            for (int r = 0; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    // HSI Wert berechnen
                    Bgr bgr = new Bgr(imageInput.Data[r, c, 0], imageInput.Data[r, c, 1], imageInput.Data[r, c, 2]);
                    Hsv hsi = BGRtoHSI(bgr);

                    double hue = hsi.Hue;
                    double sat = hsi.Satuation;
                    double inten = hsi.Value;

                    // hue in Grad umrechnen
                    hue = hue * 57.2958;

                    // Wenn Pixel rot
                    if (hue > -30 && hue <= 30 && sat >= 0.2)
                    {
                       // Console.WriteLine("hue = " + hue);

                        // Pixel rot färben
                        pixels[r, c].setRed();
                    }
                }
            }
        }


        // ---------- HSV Thresholding - Paclik ----------
        public void paclik()
        {
            // Bild durchlaufen
            for (int r = 0; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    // HSV Wert berechnen
                    Bgr bgr = new Bgr(imageInput.Data[r, c, 0], imageInput.Data[r, c, 1], imageInput.Data[r, c, 2]);
                    Hsv hsv = BGRtoHSV(bgr);

                    double hue = hsv.Hue;
                    double sat = hsv.Satuation;
                    double val = hsv.Value;

                    // Wenn Pixel nicht zu schwarz oder zu weiß ist
                    if (sat >= 0.2 && val >= 0.2)
                    {
                        double range = 25;

                        // Wenn Pixel rot
                        if (hue <= 0 + range || hue >= 360 - range)
                        {
                            // Pixel rot färben
                            pixels[r, c].setRed();
                        }

                        // Wenn Pixel blau
                        if (hue <= 240 + range && hue >= 240 - range)
                        {
                            // Pixel rot färben
                            pixels[r, c].setBlue();
                        }

                        

                        // Wenn Pixel gelb
                        /*
                        if (hue <= 60 + range && hue >= 60 - range)
                        {
                            // Pixel rot färben
                            pixels[r, c].setYellow();
                        }
                        */
                    }
                }
            }
        }
        

        // ---------- HSI Thresholding - Escalera ----------
        public void escalera()
        {
            /*
            // Look Up Tables erstellen
            double[] hueLUT = new double[256];
            double[] satLUT = new double[256];

            for (int i = 0; i <= 255; i++)
            {
                // hue table
                if (i >= 0 && i < iMin)
                {
                    hueLUT[i] = 255 * ((iMin - i) / iMin);
                }
                else if (i >= iMin && i < iMax)
                {
                    hueLUT[i] = 0;
                }
                if (i >= iMax && i <= 255)
                {
                    hueLUT[i] = 255 * ((i - iMax) / iMax);
                }

                // sat table
                if (i >= 0 && i < 200)
                {
                    satLUT[i] = i;
                }
                else if (i >= 200 && i <= 255)
                {
                    satLUT[i] = 255;
                }
            }
            */

            int iMin = 40;
            int iMax = 160;

            double maxValue = 0;

            double[,] values = new double[imageHeight, imageWidth];



            // Bild durchlaufen
            for (int r = 4; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    // HSI Wert berechnen
                    Bgr bgr = new Bgr(imageInput.Data[r, c, 0], imageInput.Data[r, c, 1], imageInput.Data[r, c, 2]);
                    Hsv hsi = BGRtoHSI(bgr);

                    double hue = hsi.Hue;
                    double sat = hsi.Satuation;
                    double inten = hsi.Value;

                    // hue in Grad und dann in 0-255 umrechnen
                    hue = hue * 57.2958;
                    hue = hue * 255 / 360;

                    // Sat in 0-255 umrechnen
                    sat = sat * 255;

                    // neuer Hue Wert
                    double newHue = 0;
                    if (hue >= 0 && hue <= iMin)
                    {
                        newHue = 255 * ((iMin - hue) / iMin);
                    }
                    else if (hue >= iMin && hue <= iMax)
                    {
                        newHue = 0;
                    }
                    if (hue >= iMax && hue <= 255)
                    {
                        newHue = 255 * ((hue - iMax) / iMax);
                    }

                    // neuer Sat Wert
                    double newSat = 0;
                    if (sat >= 0 && sat < 190)
                    {
                        newSat = sat;
                    }
                    else if (sat >= 190 && sat <= 255)
                    {
                        newSat = 255;
                    }

                    // Multiplikation der beiden Werte
                    double finalValue = newHue * newSat;

                    values[r, c] = finalValue;

                    // ggf Maximalwert neu setzen
                    if (finalValue > maxValue)
                    {
                        maxValue = finalValue;
                    }              
                }
            }


            // Matrix durchlaufen, Werte normalisieren und prüfen
            for (int r = 4; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    values[r, c] = values[r, c] / maxValue * 255;

                    imageEscalera.Data[r, c, 2] = Convert.ToByte(values[r, c]);

                    // Wenn das Ergebnis über 50 liegt
                    if (values[r, c] >= 50)
                    {
                        // Pixel rot färben
                        pixels[r, c].setRed();
                    }    
                }
            }
        }



        // Regionen zusammenfassen und zu kleine löschen
        public void removeSmallRegions()
        {
            Console.WriteLine("Remove Small Regions");

            int minRegionSize = 200;
            double minRatio = 0.7;

            int currentLabel = 0;

            foundRecs.Clear();

            // Pixel durchlaufen
            for (int r = 0; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    // Wenn der Pixel als rot/blau/etc markiert wurde und noch nicht gelabelt ist
                    if (pixels[r, c].foreground && pixels[r, c].label == -1)
                    {
                        // Erster Pixel einer neuen Region
                        RoadsignPixel firstPixel = pixels[r, c];

                        // neue Listen für neue Region erstellen
                        List<RoadsignPixel> currentPixels = new List<RoadsignPixel>();
                        List<RoadsignPixel> allPixels = new List<RoadsignPixel>();

                        // ersten Pixel der neuen Region labeln
                        firstPixel.label = currentLabel;

                        // ersten Pixel Listen hinzufügen
                        currentPixels.Add(firstPixel);
                        allPixels.Add(firstPixel);

                        // Einen Pixel nach dem anderen aus aktueller Liste bearbeiten
                        do
                        {
                            // ersten Pixel aus der Liste nehmen und als aktuellen Pixel definieren
                            RoadsignPixel currentPixel = currentPixels.ElementAt(0);
                            //Console.WriteLine("ListCount = " + currentPixels.Count());
                           // Console.WriteLine("erstes entfernen");
                            currentPixels.RemoveAt(0);
                            //Console.WriteLine("ListCount = " + currentPixels.Count());

                            int x = currentPixel.x;
                            int y = currentPixel.y;

                            // 4er Nachbarschaft checken, Pixel labeln und Listen hinzufügen
                            if (y - 1 >= 0 && pixels[y - 1, x].foreground && pixels[y - 1, x].label == -1)
                            {
                                //Console.WriteLine("oberen Nachbarn hinzufügen");
                                pixels[y - 1, x].label = currentLabel;
                                currentPixels.Add(pixels[y - 1, x]);
                                allPixels.Add(pixels[y - 1, x]);
                            }
                            if (y + 1 < imageHeight && pixels[y + 1, x].foreground && pixels[y + 1, x].label == -1)
                            {
                                //Console.WriteLine("unteren Nachbarn hinzufügen");
                                pixels[y + 1, x].label = currentLabel;
                                currentPixels.Add(pixels[y + 1, x]);
                                allPixels.Add(pixels[y + 1, x]);
                            }
                            if (x - 1 >= 0 && pixels[y, x - 1].foreground && pixels[y, x - 1].label == -1)
                            {
                                //Console.WriteLine("linken Nachbarn hinzufügen");
                                pixels[y, x - 1].label = currentLabel;
                                currentPixels.Add(pixels[y, x - 1]);
                                allPixels.Add(pixels[y, x - 1]);
                            }
                            if (x + 1 < imageWidth && pixels[y, x + 1].foreground && pixels[1, x + 1].label == -1)
                            {
                                //Console.WriteLine("rechten Nachbarn hinzufügen");
                                pixels[y, x + 1].label = currentLabel;
                                currentPixels.Add(pixels[y, x + 1]);
                                allPixels.Add(pixels[y, x + 1]);
                            }

                        // solange noch Pixel in der Liste sind
                        } while (currentPixels.Count() > 0);

                        //Console.WriteLine("allPixelsCount = " + allPixels.Count());

                        // Regionsgrenzen
                        int left = 99999, right = 0, top = 99999, bottom = 0;

                        // Wenn Regionsgröße zu klein als Hintergrund definieren
                        if (allPixels.Count() < minRegionSize)
                        {
                            foreach (RoadsignPixel p in allPixels)
                            {
                                pixels[p.y, p.x].setGray();
                            }
                        }
                        // Wenn Region groß genug ist
                        else
                        {
                            foreach (RoadsignPixel p in allPixels)
                            {
                                if (p.x < left) { left = p.x; }
                                if (p.x > right) { right = p.x; }
                                if (p.y < top) { top = p.y; }
                                if (p.y > bottom) { bottom = p.y; }
                            }

                            // Seitenverhältnis bestimmen
                            double width = right - left;
                            double height = bottom - top;

                            double ratio = 0;
                            if (width > height)
                            {
                                ratio = height / width;
                            }
                            else
                            {
                                ratio = width / height;
                            }

                            // Wenn nicht Seitenverhältnis annähernd quadratisch
                            if (ratio < minRatio)
                            {
                                foreach (RoadsignPixel p in allPixels)
                                {
                                    pixels[p.y, p.x].setWhite();
                                }
                            }
                            // Wenn Seitenverhältnis quadratisch
                            else
                            {
                                // Rechteck für gefundenes Schild erstellen
                                Rectangle rec = new Rectangle(new Point(left, top), new Size(right - left, bottom - top));

                                foundRecs.Add(rec);

                                // Rechtecklinien zeichnen
                                /*
                                for (int x = left; x <= right; x++)
                                {
                                    imageRectangles.Data[top, x, 0] = 0;
                                    imageRectangles.Data[top, x, 1] = 0;
                                    imageRectangles.Data[top, x, 2] = 255;

                                    imageRectangles.Data[top + 1, x, 0] = 0;
                                    imageRectangles.Data[top + 1, x, 1] = 0;
                                    imageRectangles.Data[top + 1, x, 2] = 255;

                                    imageRectangles.Data[bottom - 1, x, 0] = 0;
                                    imageRectangles.Data[bottom - 1, x, 1] = 0;
                                    imageRectangles.Data[bottom - 1, x, 2] = 255;

                                    imageRectangles.Data[bottom, x, 0] = 0;
                                    imageRectangles.Data[bottom, x, 1] = 0;
                                    imageRectangles.Data[bottom, x, 2] = 255;
                                }

                                for (int y = top; y <= bottom; y++)
                                {
                                    imageRectangles.Data[y, left, 0] = 0;
                                    imageRectangles.Data[y, left, 1] = 0;
                                    imageRectangles.Data[y, left, 2] = 255;

                                    imageRectangles.Data[y, left + 1, 0] = 0;
                                    imageRectangles.Data[y, left + 1, 1] = 0;
                                    imageRectangles.Data[y, left + 1, 2] = 255;

                                    imageRectangles.Data[y, right - 1, 0] = 0;
                                    imageRectangles.Data[y, right - 1, 1] = 0;
                                    imageRectangles.Data[y, right - 1, 2] = 255;

                                    imageRectangles.Data[y, right, 0] = 0;
                                    imageRectangles.Data[y, right, 1] = 0;
                                    imageRectangles.Data[y, right, 2] = 255;
                                }*/
                            }

                        }

                        // Regionsnummer erhöhen
                        currentLabel++;
                    }
                }
            }
        }


        // Bild erzeugen
        public void createImage()
        {
            // Pixel durchlaufen
            for (int r = 0; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    imageRoadsigns.Data[r, c, 0] = (byte)pixels[r, c].color.Blue;
                    imageRoadsigns.Data[r, c, 1] = (byte)pixels[r, c].color.Green;
                    imageRoadsigns.Data[r, c, 2] = (byte)pixels[r, c].color.Red;
                }
            }
        }


        // Bild zurückgeben
        public Image<Bgr, Byte> getRoadsignImage()
        {
            return imageRoadsigns;
        }


        // Input Bild zurückgeben
        public Image<Bgr, Byte> getInputImage()
        {
            return imageInput;
        }

        // Bild mit Rechtecken zurückgeben
        public Image<Bgr, Byte> getRectangleImage()
        {
            return imageRectangles;
        }

        // Escalera Bild zurückgeben
        public Image<Bgr, Byte> getEscaleraImage()
        {
            return imageEscalera;
        }

        // Escalera Bild zurückgeben
        public List<Rectangle> getFoundRecs()
        {
            return foundRecs;
        }


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



        // ----- RGB to HSI (wird als HSV Objekt zurückgegeben) -----
        private Hsv BGRtoHSI(Bgr bgr)
        {
            //Console.WriteLine("Bgr = " + bgr);
            Rgb rgb = new Rgb(bgr.Red, bgr.Green, bgr.Blue);
            //Console.WriteLine("Rgb = " + rgb);

            double red = bgr.Red / 255;
            double green = bgr.Green / 255;
            double blue = bgr.Blue / 255;     

            // Intensity
            double i = (red + green + blue) / 3;

            // Hue
            double h = Math.Acos(((2 * red) - green - blue) / (2 * Math.Sqrt(Math.Pow(red - green, 2) + (red - blue) * (green - blue))));
            
            // Saturation
            double s = 1 - (3 / (red + green + blue)) * Math.Min(Math.Min(red, green), blue);

            Hsv hsv = new Hsv(h, s, i);
            //Console.WriteLine("Hsi = " + hsv);

            return hsv;
        }
    }




    class RoadsignPixel
    {

        public int x;
        public int y;
        public int label;
        public bool foreground;
        public Bgr color;

        // Konstruktor
        public RoadsignPixel(int _y, int _x)
        {
            x = _x;
            y = _y;
            label = -1;
            foreground = false;
            color = new Bgr(0, 0, 0);
        }

        public void setRed(){
            color = new Bgr(0,0,255);
            foreground = true;
        }

        public void setBlue(){
            color = new Bgr(255,0,0);
            foreground = true;
        }

        public void setYellow()
        {
            color = new Bgr(0, 255, 255);
            foreground = true;
        }

        public void setWhite()
        {
            color = new Bgr(255, 255, 255);
            foreground = true;
        }

        public void setBlack()
        {
            color = new Bgr(0, 0, 0);
            foreground = false;
        }

        public void setGray()
        {
            color = new Bgr(100, 100, 100);
            foreground = false;
        }

        public void setOriginalColor(){
            //color = new Bgr(imageOriginal.Data[r, c, 0], imageOriginal.Data[r, c, 1], imageOriginal.Data[r, c, 2]);
        }

    }




}
