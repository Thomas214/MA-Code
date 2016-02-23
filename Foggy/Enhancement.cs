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
    class Enhancement
    {
        // Bilder
        Image<Bgr, Byte> imageInput;
        Image<Bgr, Byte> imageEnhanced;

        // Konstruktor
        public Enhancement(Image<Bgr, Byte> image)
        {
            imageInput = image.Copy();
            //imageTrafficsigns = imageBgr.Copy();
            imageEnhanced = new Image<Bgr, Byte>(imageInput.Width, imageInput.Height);
        }



        // Verbesserung durchführen
        public void performEnhancement(int method)
        {
            //imageTrafficsigns = imageOriginal;
            switch (method)
            {
                case 0:
                    broggi();
                    break;
                case 1:
                    ruta();
                    break;
                case 2:
                    greyworld();
                    break;
            }
        }



        // ---------- RGB Enhancement - Broggi ----------
        public void broggi()
        {

            // Farbe der Lichtquelle bestimmen
            // Grauwert
            Bgr gray = new Bgr(130, 130, 130);

            // Farbe der Straße bestimmen
            double streetBlue = 0;
            double streetGreen = 0;
            double streetRed = 0;
            int pixelCount = 0;
            for (int r = imageInput.Height - 30; r < imageInput.Height; r++)
            {
                for (int c = imageInput.Width / 2 - 100; c < imageInput.Width / 2 + 100; c++)
                {
                    streetBlue += imageInput.Data[r, c, 0];
                    streetGreen += imageInput.Data[r, c, 1];
                    streetRed += imageInput.Data[r, c, 2]; ;
                    pixelCount++;
                }
            }
            streetBlue /= pixelCount;
            streetGreen /= pixelCount;
            streetRed /= pixelCount;
            Bgr streetColor = new Bgr(streetBlue, streetGreen, streetRed);

            // Cyan Kanal des CMYK Models
            double kStreet = 1 - Math.Max(streetBlue / 255, Math.Max(streetGreen / 255, streetRed / 255));
            double cyanStreet = (1 - streetRed - kStreet) / (1 - kStreet);

            double offsetBlue = gray.Blue - streetBlue;
            double offsetGreen = gray.Green - streetGreen;
            double offsetRed = gray.Red - streetRed;

            // Bild durchlaufen
            for (int r = 0; r < imageInput.Height; r++)
            {
                for (int c = 0; c < imageInput.Width; c++)
                {
                    double blue = imageInput.Data[r, c, 0] + offsetBlue;
                    double green = imageInput.Data[r, c, 1] + offsetGreen;
                    double red = imageInput.Data[r, c, 2] + offsetRed;

                    if (blue < 0) { blue = 0; }
                    if (green < 0) { green = 0; }
                    if (red < 0) { red = 0; }
                    if (blue > 255) { blue = 255; }
                    if (green > 255) { green = 255; }
                    if (red > 255) { red = 255; }

                    imageEnhanced.Data[r, c, 0] = (byte)blue;
                    imageEnhanced.Data[r, c, 1] = (byte)green;
                    imageEnhanced.Data[r, c, 2] = (byte)red;
                }
            }
        }




        // ---------- RGB Enhancement - Ruta ----------
        public void ruta()
        {

            // Bild durchlaufen
            for (int r = 0; r < imageInput.Height; r++)
            {
                for (int c = 0; c < imageInput.Width; c++)
                {
                    double blue = (double)imageInput.Data[r, c, 0] / 255;
                    double green = (double)imageInput.Data[r, c, 1] / 255;
                    double red = (double)imageInput.Data[r, c, 2] / 255;

                    // Rot verbessern
                    double s = blue + green + red;

                    //Console.WriteLine("oldRed = " + red);

                    double enhanceRed = Math.Max(0, Math.Min((red - green), (red - blue)) / s);
                    double enhanceBlue = Math.Max(0, Math.Min((blue - red), (blue - green)) / s);
                    double enhanceYellow = Math.Max(0, Math.Min((red - blue), (green - blue)) / s);


                    // Enhance Red
                    imageEnhanced.Data[r, c, 0] = imageInput.Data[r, c, 0];
                    imageEnhanced.Data[r, c, 1] = imageInput.Data[r, c, 1];
                    imageEnhanced.Data[r, c, 2] = (byte)Math.Min(255, (imageInput.Data[r, c, 2] + (enhanceRed * 255)));

                    //Enhance Blue
                    //imageTrafficsigns.Data[r, c, 0] = (byte)Math.Min(255, (imageOriginal.Data[r, c, 0] + (enhanceBlue * 255)));
                    //imageTrafficsigns.Data[r, c, 1] = imageOriginal.Data[r, c, 1];
                    //imageTrafficsigns.Data[r, c, 2] = imageOriginal.Data[r, c, 2];

                    // Enhance Yellow
                    //imageTrafficsigns.Data[r, c, 0] = imageOriginal.Data[r, c, 0];
                    //imageTrafficsigns.Data[r, c, 1] = (byte)Math.Min(255, (imageOriginal.Data[r, c, 1] + (enhanceYellow * 255)));
                    //imageTrafficsigns.Data[r, c, 2] = (byte)Math.Min(255, (imageOriginal.Data[r, c, 2] + (enhanceYellow * 255)));

                }
            }
        }



        // ---------- RGB Enhancement - Ruta ----------
        public void greyworld()
        {
            double blue = 0, green = 0, red = 0;

            // Bild durchlaufen und Durchschnittfarbswerte bestimmen
            for (int r = 0; r < imageInput.Height; r++)
            {
                for (int c = 0; c < imageInput.Width; c++)
                {
                    blue += (double)imageInput.Data[r, c, 0];
                    green += (double)imageInput.Data[r, c, 1];
                    red += (double)imageInput.Data[r, c, 2];
                }
            }

            blue /= imageInput.Height * imageInput.Width;
            green /= imageInput.Height * imageInput.Width;
            red /= imageInput.Height * imageInput.Width;

            // Koeffizienten
            double alpha = green / red;
            double beta = green / blue;

            // Bild durchlaufen und neue Farbwerte festlegen
            for (int r = 0; r < imageInput.Height; r++)
            {
                for (int c = 0; c < imageInput.Width; c++)
                {
                    imageEnhanced.Data[r, c, 0] = (byte)Math.Min(255, (beta * imageInput.Data[r, c, 0]));
                    imageEnhanced.Data[r, c, 1] = imageInput.Data[r, c, 1];
                    imageEnhanced.Data[r, c, 2] = (byte)Math.Min(255, (alpha * imageInput.Data[r, c, 2]));
                }
            }
        }



        // Bild zurückgeben
        public Image<Bgr, Byte> getImage()
        {
            return imageEnhanced;
        }

    }
}
