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

        // Konstruktor
        public ColorBasedDetection(Image<Bgr, Byte> imageBgr)
        {
            imageOriginal = imageBgr.Copy();
            //imageTrafficsigns = imageBgr.Copy();
            imageTrafficsigns = new Image<Bgr, Byte>(imageBgr.Width, imageBgr.Height);
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
            }
        }


        // ---------- RGB Thresholding - Benallal and Maunier ----------
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
                        colorRed(r, c);
                    }
                    else if (blue > green && ((blue - green) >= thresholdGB) && ((blue - red) >= thresholdRB))
                    {
                        // Pixel blau
                        colorBlue(r, c);
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
                        // Pixel pink färben
                        colorRed(r, c);
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
                        colorRed(r, c);
                    }
                }
            }
        }




        // ---------- HSI Thresholding - Varun ----------
        public void kuo()
        {
            // HSI Bild erstellen
            Image<Hsv, Byte> imageHSI = imageOriginal.Convert<Hsv, Byte>();

            // Bild durchlaufen
            for (int r = 0; r < imageOriginal.Height; r++)
            {
                for (int c = 0; c < imageOriginal.Width; c++)
                {

                    Bgr bgr = new Bgr(imageOriginal.Data[r, c, 0], imageOriginal.Data[r, c, 1], imageOriginal.Data[r, c, 2]);
                    Hsv hsv = BGRtoHSV(bgr);

                    double hue = hsv.Hue;
                    double sat = hsv.Satuation;
                    double inten = hsv.Value;

                    hue = imageHSI.Data[r, c, 0];
                    sat = imageHSI.Data[r, c, 1];
                    inten = imageHSI.Data[r, c, 2];


                    // Wenn Pixel rot
                    if ((hue >= 0 && hue < 0.111 * Math.PI) || (hue >= 1.8 * Math.PI && hue < 2 * Math.PI))
                    {
                        //Console.WriteLine("hue richtig");
                        if (sat > 0.1 && sat <= 1)
                        {
                            //Console.WriteLine("sat richtig");
                            if (inten > 0.12 && inten < 0.8)
                            {
                                //Console.WriteLine("inten richtig");

                                // Pixel rot färben
                                colorRed(r, c);
                            }
                        }
                    }


                    // Wenn Pixel blau
                    if (hue >= 1.066 * Math.PI && hue <= 1.555 * Math.PI)
                    {
                        //Console.WriteLine("hue richtig");
                        if (sat > 0.28 && sat <= 1)
                        {
                            //Console.WriteLine("sat richtig");
                            if (inten > 0.22 && inten < 0.5)
                            {
                                //Console.WriteLine("inten richtig");

                                // Pixel rot färben
                                colorBlue(r, c);
                            }
                        }
                    }



                }
            }
        }









        // Pixel pink färben
        private void colorPink(int r, int c){
            imageTrafficsigns.Data[r, c, 0] = 147;
            imageTrafficsigns.Data[r, c, 1] = 20;
            imageTrafficsigns.Data[r, c, 2] = 255;
        }

        // Pixel weiß färben
        private void colorWhite(int r, int c)
        {
            imageTrafficsigns.Data[r, c, 0] = 255;
            imageTrafficsigns.Data[r, c, 1] = 255;
            imageTrafficsigns.Data[r, c, 2] = 255;
        }

        // Pixel rot färben
        private void colorRed(int r, int c)
        {
            imageTrafficsigns.Data[r, c, 0] = 0;
            imageTrafficsigns.Data[r, c, 1] = 0;
            imageTrafficsigns.Data[r, c, 2] = 255;
        }

        // Pixel blau färben
        private void colorBlue(int r, int c)
        {
            imageTrafficsigns.Data[r, c, 0] = 255;
            imageTrafficsigns.Data[r, c, 1] = 0;
            imageTrafficsigns.Data[r, c, 2] = 0;
        }

        // Pixel in ursprünglicher Farbe färben
        private void colorOriginal(int r, int c)
        {
            imageTrafficsigns.Data[r, c, 0] = imageOriginal.Data[r, c, 0];
            imageTrafficsigns.Data[r, c, 1] = imageOriginal.Data[r, c, 1];
            imageTrafficsigns.Data[r, c, 2] = imageOriginal.Data[r, c, 2];
        }


        // Bild zurückgeben
        public Image<Bgr, Byte> getImage()
        {
            return imageTrafficsigns;
        }











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
    }
}
