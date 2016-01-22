using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Image<Bgr, Byte> imageOriginal;
        Image<Bgr, Byte> imageTrafficsigns;

        // Arrays
        RoadsignPixel[,] pixels;

        // Konstruktor
        public ColorBasedDetection(Image<Bgr, Byte> imageBgr)
        {
            imageOriginal = imageBgr.Copy();
            //imageTrafficsigns = imageBgr.Copy();
            imageTrafficsigns = new Image<Bgr, Byte>(imageBgr.Width, imageBgr.Height);

            // Pixelarray anlegen
            pixels = new RoadsignPixel[imageBgr.Height, imageBgr.Width];
            for (int r = 0; r < imageBgr.Height; r++)
            {
                for (int c = 0; c < imageBgr.Width; c++)
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
            }

            // Regionen finden und zu kleine löschen
            findRegions();

            // Bild erzeugen
            createImage();
        }


        // ---------- RGB Thresholding - Benallal and Meunier ----------
        public void benallal()
        {
            // Bild durchlaufen
            for (int r = 0; r < imageOriginal.Height; r++)
            {
                for (int c = 0; c < imageOriginal.Width; c++)
                {
                    double blue = imageOriginal.Data[r, c, 0];
                    double green = imageOriginal.Data[r, c, 1];
                    double red = imageOriginal.Data[r, c, 2];

                    double thresholdRG = 100;
                    double thresholdRB = 100;
                    double thresholdGB = 100;

                    if (red > green && ((red - green) >= thresholdRG) && ((red - blue) >= thresholdRB))
                    {
                        // Pixel rot
                        pixels[r, c].setRed();
                    }
                    else if (blue > green && ((blue - green) >= thresholdGB) && ((blue - red) >= thresholdRB))
                    {
                        // Pixel blau
                        pixels[r, c].setBlue();
                    }
                }
            }
        }

        // ---------- RGB Thresholding - Estevez and Mehtarnavaz ----------
        public void estevez()
        {
            // Bild durchlaufen
            for (int r = 0; r < imageOriginal.Height; r++)
            {
                for (int c = 0; c < imageOriginal.Width; c++)
                {
                    double blue = imageOriginal.Data[r, c, 0];
                    double green = imageOriginal.Data[r, c, 1];
                    double red = imageOriginal.Data[r, c, 2];

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
            for (int r = 0; r < imageOriginal.Height; r++)
            {
                for (int c = 0; c < imageOriginal.Width; c++)
                {
                    double blue = imageOriginal.Data[r, c, 0];
                    double green = imageOriginal.Data[r, c, 1];
                    double red = imageOriginal.Data[r, c, 2];

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
            for (int r = 0; r < imageOriginal.Height; r++)
            {
                for (int c = 0; c < imageOriginal.Width; c++)
                {
                    // HSI Wert berechnen
                    Bgr bgr = new Bgr(imageOriginal.Data[r, c, 0], imageOriginal.Data[r, c, 1], imageOriginal.Data[r, c, 2]);
                    Hsv hsi = BGRtoHSI(bgr);

                    double hue = hsi.Hue;
                    double sat = hsi.Satuation;
                    double inten = hsi.Value;

                    // Falls Blauwert größer als Grünwert
                    if (imageOriginal.Data[r, c, 0] > imageOriginal.Data[r, c, 1])
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
            for (int r = 0; r < imageOriginal.Height; r++)
            {
                for (int c = 0; c < imageOriginal.Width; c++)
                {
                    // HSI Wert berechnen
                    Bgr bgr = new Bgr(imageOriginal.Data[r, c, 0], imageOriginal.Data[r, c, 1], imageOriginal.Data[r, c, 2]);
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
            for (int r = 0; r < imageOriginal.Height; r++)
            {
                for (int c = 0; c < imageOriginal.Width; c++)
                {
                    // HSV Wert berechnen
                    Bgr bgr = new Bgr(imageOriginal.Data[r, c, 0], imageOriginal.Data[r, c, 1], imageOriginal.Data[r, c, 2]);
                    Hsv hsv = BGRtoHSI(bgr);

                    double hue = hsv.Hue;
                    double sat = hsv.Satuation;
                    double val = hsv.Value;



                }
            }
        }



        // Regionen zusammenfassen und zu kleine löschen
        public void findRegions()
        {
            int minRegionSize = 800;
            
            int currentLabel = 0;

            // Pixel durchlaufen
            for (int r = 0; r < imageOriginal.Height; r++)
            {
                for (int c = 0; c < imageOriginal.Width; c++)
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
                            if (y + 1 < imageOriginal.Height && pixels[y + 1, x].foreground && pixels[y + 1, x].label == -1)
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
                            if (x + 1 < imageOriginal.Width && pixels[y, x + 1].foreground && pixels[1, x + 1].label == -1)
                            {
                                //Console.WriteLine("rechten Nachbarn hinzufügen");
                                pixels[y, x + 1].label = currentLabel;
                                currentPixels.Add(pixels[y, x + 1]);
                                allPixels.Add(pixels[y, x + 1]);
                            }

                        // solange noch Pixel in der Liste sind
                        } while (currentPixels.Count() > 0);

                        //Console.WriteLine("allPixelsCount = " + allPixels.Count());

                        // Regionsgröße checken und gegebenenfalls als Hintergrund definieren
                        if (allPixels.Count() < minRegionSize)
                        {
                            foreach (RoadsignPixel p in allPixels)
                            {
                                pixels[p.y, p.x].setBlack();
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
            for (int r = 0; r < imageOriginal.Height; r++)
            {
                for (int c = 0; c < imageOriginal.Width; c++)
                {
                    imageTrafficsigns.Data[r, c, 0] = (byte)pixels[r, c].color.Blue;
                    imageTrafficsigns.Data[r, c, 1] = (byte)pixels[r, c].color.Green;
                    imageTrafficsigns.Data[r, c, 2] = (byte)pixels[r, c].color.Red;
                }
            }
        }


        // Bild zurückgeben
        public Image<Bgr, Byte> getImage()
        {
            return imageTrafficsigns;
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

        public void setGreen()
        {
            color = new Bgr(0, 255, 0);
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

        public void setOriginalColor(){
            //color = new Bgr(imageOriginal.Data[r, c, 0], imageOriginal.Data[r, c, 1], imageOriginal.Data[r, c, 2]);
        }

    }




}
