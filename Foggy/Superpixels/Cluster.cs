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

    // ====================================================================================
    // KLASSE: Cluster zur Berechnung von Superpixeln
    // ====================================================================================
    class Cluster
    {

        public Vector5 oldCenter;
        public Vector5 currentCenter;

        public Dictionary<Pixel, int> pixels = new Dictionary<Pixel, int>();

        public Dictionary<Cluster, int> neighbours = new Dictionary<Cluster, int>();

        public Bgr color;

        public bool scanned;

        public int regionNr;

        // Konstruktor
        public Cluster()
        {
            scanned = false;
            regionNr = -1;
        }

        // Pixel zum Cluster hinzufügen
        public void addPixel(Pixel pixel)
        {
            pixels.Add(pixel, 0);
        }

        // Pixel vom Cluster entfernen
        public void removePixel(Pixel pixel)
        {
            pixels.Remove(pixel);
        }

        // Farbe des Clusters aus allen enthaltenen Pixeln bestimmen
        public void calcColor()
        {
            color = new Bgr(0, 0, 0);

            foreach (KeyValuePair<Pixel, int> p in pixels)
            {
                color.Blue += p.Key.bgr.Blue;
                color.Green += p.Key.bgr.Green;
                color.Red += p.Key.bgr.Red;
            }

            color.Blue = color.Blue / pixels.Count;
            color.Green = color.Green / pixels.Count;
            color.Red = color.Red / pixels.Count;
        }

        // Farbdistanz zu anderem Cluster bestimmen
        public double colorDistance(Cluster c)
        {
            double distance = Math.Sqrt(Math.Pow(color.Blue - c.color.Blue, 2) + Math.Pow(color.Green - c.color.Green, 2) + Math.Pow(color.Red - c.color.Red, 2));
            return distance;
        }


        // niedrigste Y-Koordinate zurückgeben
        public int getY()
        {
            int y = 0;
            foreach (KeyValuePair<Pixel, int> p in pixels)
            {
                if (p.Key.vector.y > y)
                {
                    y = p.Key.vector.y;
                }
            }
            return y;
        }

        // Cluster Zentrum als X-Koordinate zurückgeben
        public int getX()
        {
            return currentCenter.x;
        }
    }

}
