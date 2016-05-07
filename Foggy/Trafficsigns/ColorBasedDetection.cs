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

        // gefundene Pixelregionen
        List<Component> components;

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
                    zaklouta();
                    break;
                case 3:
                    varun();
                    break;
                case 4:
                    kuo();
                    break;
                case 5:
                    piccioli();
                    break;
                case 6:
                    paclik();
                    break;
                case 7:
                    escalera();
                    break;
                case 8:
                    qingsong();
                    break;
            }


            //createPixelRegions();
            //filterComponents();


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

                    double thresholdRG = 80;
                    double thresholdRB = 80;

                    // Wenn Pixel rot
                    if (red > green && ((red - green) >= thresholdRG) && ((red - blue) >= thresholdRB))
                    {
                        // Pixel rot färben
                        //pixels[r, c].setRed();
                        pixels[r, c].setWhite();
                    }
                    // Wenn Pixel blau
                    /*
                    else if (blue > green && ((blue - green) >= thresholdGB) && ((blue - red) >= thresholdRB))
                    {
                        // Pixel blau färben
                        pixels[r, c].setBlue();
                    }*/
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
                        //pixels[r, c].setRed();
                        pixels[r, c].setWhite();
                    }
                }
            }
        }


        // ---------- RGB Thresholding - Zaklouta ----------
        public void zaklouta()
        {
            double[,] newReds = new double[imageHeight, imageWidth];

            // Bild durchlaufen und rote Werte rausfiltern
            for (int r = 0; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    double blue = imageInput.Data[r, c, 0];
                    double green = imageInput.Data[r, c, 1];
                    double red = imageInput.Data[r, c, 2];

                    int s = (int)(blue + green + red);

                    double newRed = Math.Max(0, Math.Min(red - green, red - blue) / s);

                    newReds[r, c] = newRed;

                    //Console.WriteLine(newRed);
                }
            }

            // Mittelwert berechnen
            double sum = 0;
            for (int r = 0; r < newReds.GetLength(0); r++)
            {
                for (int c = 0; c < newReds.GetLength(1); c++)
                {
                    sum += newReds[r, c];
                }
            }
            double mue = sum / newReds.Length;


            // Standardabweichung berechnen
            double[,] deviations = new double[newReds.GetLength(0), newReds.GetLength(1)];

            for (int r = 0; r < newReds.GetLength(0); r++)
            {
                for (int c = 0; c < newReds.GetLength(1); c++)
                {
                    deviations[r, c] = newReds[r, c] - mue;
                }
            }
            double deviationSum = 0;
            for (int r = 0; r < newReds.GetLength(0); r++)
            {
                for (int c = 0; c < newReds.GetLength(1); c++)
                {
                    deviationSum += (deviations[r, c] * deviations[r, c]);
                }
            }
            double sigma = Math.Sqrt(deviationSum / newReds.Length);

            // Threshold berechnen
            double threshold = mue + 4 * sigma;

            Console.WriteLine("mue = " + mue);
            Console.WriteLine("sigma = " + sigma);
            Console.WriteLine("threshold = " + threshold);

            // Threshold anwenden
            for (int r = 0; r < newReds.GetLength(0); r++)
            {
                for (int c = 0; c < newReds.GetLength(1); c++)
                {
                    if (newReds[r, c] >= threshold)
                    {
                        pixels[r, c].setWhite();
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
                        //pixels[r, c].setRed();
                        pixels[r, c].setWhite();
                    }
                }
            }
        }



        // ---------- HSI Thresholding - Kuo & Lin ----------               // BENÖTIGT ZUVIEL SPEICHER
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
                    /*
                    if (imageInput.Data[r, c, 0] > imageInput.Data[r, c, 1])
                    {
                        // Hue in Grad umrechnen, von 360 abziehen und zurückrechnen
                        hue = hue * 57.2958;
                        hue = 360 - hue;
                        hue = hue / 57.2958;
                    }*/

                    // Wenn Pixel rot
                    if ((hue >= 0 && hue < 0.111 * Math.PI) || (hue >= 1.8 * Math.PI && hue < 2 * Math.PI))
                    {
                        if (sat > 0.1 && sat <= 1)
                        {
                            if (inten > 0.12 && inten < 0.8)
                            {
                                // Pixel rot färben
                                //pixels[r, c].setRed();
                                pixels[r, c].setWhite();
                            }
                        }
                    }

                    // Wenn Pixel blau
                    /*
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
                    }*/
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
                    //hue = hue * 57.2958;

                    // Wenn Pixel rot
                    if (hue > -30 && hue <= 30 && sat >= 0.2)
                    {
                       // Console.WriteLine("hue = " + hue);

                        // Pixel rot färben
                        //pixels[r, c].setRed();
                        pixels[r, c].setWhite();
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
                    //Hsv hsv = BGRtoHSV(bgr);
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
                            //pixels[r, c].setRed();
                            pixels[r, c].setWhite();
                        }

                        // Wenn Pixel blau
                        /*
                        if (hue <= 240 + range && hue >= 240 - range)
                        {
                            // Pixel blau färben
                            pixels[r, c].setBlue();
                        }
                        */ 

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
        
        
        // ---------- HSI Thresholding - Qingsong ----------
        public void qingsong()
        {
             // Bild durchlaufen
            for (int r = 4; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    // HSI Wert berechnen
                    Bgr bgr = new Bgr(imageInput.Data[r, c, 0], imageInput.Data[r, c, 1], imageInput.Data[r, c, 2]);
                    //Hsv hsi = BGRtoHSI(bgr);
                    Hsv hsi = BGRtoHSI(bgr);

                    double hue = hsi.Hue;
                    double sat = hsi.Satuation;
                    double inten = hsi.Value;

                    // Hue normalisieren
                    hue = hue / 360;

                    // Wenn Pixel rot
                    if (hue > 0.94 || hue < 0.05)
                    {
                        if (sat > 0.18 && sat < 0.71)
                        {
                            //pixels[r, c].setRed();
                            pixels[r, c].setWhite();
                        }
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
                    //Hsv hsi = BGRtoHSI(bgr);
                    Hsv hsi = BGRtoHSI(bgr);

                    //Console.WriteLine("rgb = " + bgr.Red + " " + bgr.Green + " " + bgr.Blue);
                    //Console.WriteLine("hsi = " + hsi.Hue + " " + hsi.Satuation + " " + hsi.Value);
                    //Console.WriteLine("hue1 = " + hsi.Hue);
                    //Console.WriteLine("hue2 = " + hsi2.Hue);
                    //Console.WriteLine("");

                    double hue = hsi.Hue;
                    double sat = hsi.Satuation;
                    double inten = hsi.Value;

                    // hue in Grad umrechnen umrechnen
                    //hue = hue * 57.2958;

                    // hue in 0-255 umrechnen
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
                        //pixels[r, c].setRed();
                        pixels[r, c].setWhite();
                    }    
                }
            }
        }


        // Pixel zu Regionen zusammenfassen
        public void createPixelRegions()
        {

            //Console.WriteLine("new component algo");

            // Anzahl von Komponenten
            int componentCount = 0;
            bool firstPixel = true;

            // Pixel durchlaufen
            for (int r = 0; r < imageHeight; r++)
            {
                for (int c = 0; c < imageWidth; c++)
                {
                    // Wenn der Pixel markiert wurde
                    if (pixels[r, c].foreground)
                    {
                        // Wenn es sich um den ersten Pixel handelt
                        if (firstPixel)
                        {
                            componentCount++;
                            pixels[r, c].label = componentCount;
                            firstPixel = false;
                        }

                        bool neighbourTop = false;
                        bool neighbourLeft = false;

                        //oberen Nachbarn prüfen
                        if (r > 0 && pixels[r - 1, c].foreground)
                        {
                            neighbourTop = true;
                        }
                        //linken Nachbarn prüfen
                        if (c > 0 && pixels[r, c - 1].foreground)
                        {
                            neighbourLeft = true;
                        }

                        // Nachbar oben und links
                        if (neighbourTop && neighbourLeft)
                        {
                            // Label von oben
                            int label = pixels[r - 1, c].label;
                            pixels[r, c].label = label;

                            // Rückwärts nach links neu labeln
                            for (int i = c - 1; i >= 0; i--)
                            {
                                if (pixels[r, i].foreground)
                                {
                                    if (!pixels[r - 1, i].foreground)
                                    {
                                        // wenn oberer Nachbar nicht gelabelt
                                        pixels[r, i].label = label;
                                    }
                                    else
                                    {
                                        // wenn oberer Nachbar schon gelabelt
                                        break;
                                    }
                                }
                                else
                                {
                                    // wenn links kein gelabelter Pixel mehr übrig
                                    break;
                                }
                            }
                        }
                        // Nachbar oben
                        else if (neighbourTop && !neighbourLeft)
                        {
                            // Label von oben
                            pixels[r, c].label = pixels[r - 1, c].label;
                        }
                        // Nachbar links
                        else if (!neighbourTop && neighbourLeft)
                        {
                            // Labeln von links
                            pixels[r, c].label = pixels[r, c - 1].label;
                        }
                        // kein Nachbar
                        else if (!neighbourTop && !neighbourLeft)
                        {
                            //neue Region
                            componentCount++;
                            pixels[r, c].label = componentCount;
                        }
                    }
                }
            }

            // neue Komponenten Liste erstellen
            components = new List<Component>();
            List<Component> tempComponents = new List<Component>();
            for (int i = 0; i < componentCount; i++)
            {
                tempComponents.Add(new Component());
            }


            //Console.WriteLine("components size = " + components.Count);

            // Pixel in temporäre Komponentenliste einfügen
            foreach (RoadsignPixel p in pixels)
            {
                if (p.foreground)
                {
                    tempComponents[p.label - 1].pixels.Add(p);
                }
            }

            //Console.WriteLine("componentCount = " + componentCount);

            // Komponenten updaten
            foreach (Component c in tempComponents)
            {
                if (c.pixels.Count != 0)
                {
                    c.calcValues();
                    //Console.WriteLine("pixel count = " + c.pixels.Count);
                    components.Add(c);
                }

            }
        }


        // Regionen filtern
        public void filterComponents()
        {
            List<Component> tempComponent = new List<Component>(components);
            components.Clear();

            foreach (Component c in tempComponent)
            {
                //Console.WriteLine("size = " + c.size + "   ratio = " + c.ratio);
                if (c.size > 200 && c.ratio > 0.7)
                {
                    components.Add(c);
                }
            }

            // Rechteck für jede Komponente erstellen und in Liste einfügen
            foreach (Component c in components)
            {
                c.calcRectangle();
                foundRecs.Add(c.rec);
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
                                //pixels[p.y, p.x].setGray();
                                //pixels[p.y, p.x].setBlack();
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
                                    //pixels[p.y, p.x].setWhite();
                                    //pixels[p.y, p.x].setBlack();
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


        // ----- BGR to HSV (grad prozent prozent)-----
        private Hsv BGRtoHSV(Bgr bgr)
        {
            //Console.WriteLine("Bgr = " + bgr);
            //Rgb rgb = new Rgb(bgr.Red, bgr.Green, bgr.Blue);
            //Console.WriteLine("Rgb = " + rgb);

            //System.Drawing.Color intermediate = System.Drawing.Color.FromArgb((int)bgr.Red, (int)bgr.Green, (int)bgr.Blue);
            //Hsv hsvPixel = new Hsv(intermediate.GetHue(), intermediate.GetSaturation(), intermediate.GetBrightness());

            double red = bgr.Red / 255;
            double green = bgr.Green / 255;
            double blue = bgr.Blue / 255;

            double max = Math.Max(red, Math.Max(green, blue));
            double min = Math.Min(red, Math.Min(green, blue));

            //double delta = cmax - cmin;

            // Value
            double value = max;

            // Saturation
            double satuation;
            if (max == 0)
            {
                satuation = 0;
            }
            else
            {
                satuation = (max - min) / max;
            }

            // Hue
            double hue = 0;

            double r = (max - red) / (max - min);
            double g = (max - green) / (max - min);
            double b = (max - blue) / (max - min);

            if (red == max && green == min)
            {
                hue = 5 + b;
            }
            else if (red == max && green != min)
            {
                hue = 1 - g;
            }
            else if (green == max && blue == min)
            {
                hue = r + 1;
            }
            else if (green == max && blue != min)
            {
                hue = 3 - b;
            }
            else if (red == min)
            {
                hue = 3 + g;
            }
            else
            {
                hue = 5 - r;
            }

            hue = hue * 60;

            if (hue == 360)
            {
                hue = 0;
            }
            Hsv hsv = new Hsv(hue, satuation, value);

            //Console.WriteLine("Hsv2 = " + hsv);
            //Console.WriteLine("");

            return hsv;
        }


        // ----- BGR to HSV -----
        /*
        private Hsv BGRtoHSVold(Bgr bgr)
        {
            //Console.WriteLine("Bgr = " + bgr);
            //Rgb rgb = new Rgb(bgr.Red, bgr.Green, bgr.Blue);
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

           // Console.WriteLine("Hsv = " + hsv);
            //Console.WriteLine("");

            return hsv;
        }
        */





        // ----- BGR to HSI (wird als HSV Objekt zurückgegeben, grad prozent prozent) -----
        private Hsv BGRtoHSI(Bgr bgr)
        {
            //Console.WriteLine("Bgr = " + bgr);
            //Rgb rgb = new Rgb(bgr.Red, bgr.Green, bgr.Blue);
            //Console.WriteLine("Rgb = " + rgb);

            double red = bgr.Red / 255;
            double green = bgr.Green / 255;
            double blue = bgr.Blue / 255;

            double min = Math.Min(red, Math.Min(green, blue));

            // Intensity
            double i = (red + green + blue) / 3;

            // Saturation
            double s = 1 - (3 / (red + green + blue)) * min;

            // Hue
            double h = Math.Acos((0.5 * ((red - green) + (red - blue))) / (Math.Sqrt(Math.Pow(red - green, 2) + (red - blue) * (green - blue))));

            // H in Grad umrechnen
            h = (360 / (2 * Math.PI)) * h;

            if (blue >= green)
            {
                h = 360 - h;
            }

            Hsv hsv = new Hsv(h, s, i);
            //Console.WriteLine("Hsi = " + hsv);

            return hsv;
        }


        // ----- BGR to HSI (wird als HSV Objekt zurückgegeben) -----
        /*
        private Hsv BGRtoHSIold(Bgr bgr)
        {
            //Console.WriteLine("Bgr = " + bgr);
            //Rgb rgb = new Rgb(bgr.Red, bgr.Green, bgr.Blue);
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
        }*/

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



    class Component
    {
        public List<RoadsignPixel> pixels;
        public int label;
        public int size;
        public double ratio;
        public Rectangle rec;
        
        private int left = 99999, right = 0, top = 99999, bottom = 0;

        // Konstruktor
        public Component()
        {
            pixels = new List<RoadsignPixel>();
        }

        //Werte berechnen
        public void calcValues(){
            label = pixels.First().label;
            size = pixels.Count;

            // Seitenverhältnis bestimmen

            foreach (RoadsignPixel p in pixels)
            {
                if (p.x < left) { left = p.x; }
                if (p.x > right) { right = p.x; }
                if (p.y < top) { top = p.y; }
                if (p.y > bottom) { bottom = p.y; }
            }
            double width = right - left;
            double height = bottom - top;
            if (width > height)
            {
                ratio = height / width;
            }
            else
            {
                ratio = width / height;
            }
        }

        //Rechteck berechnen
        public void calcRectangle() {
            rec = new Rectangle(new Point(left, top), new Size(right - left, bottom - top));
        }
    }


}
