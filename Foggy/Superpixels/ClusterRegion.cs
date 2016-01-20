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
    // KLASSE: Cluster-Region aus mehreren Clustern zur Berechnung von Superpixeln
    // ====================================================================================
    class ClusterRegion
    {

        public Dictionary<Cluster, int> clusters = new Dictionary<Cluster, int>();
        public Bgr color;

        public bool selected = false;

        public int number;

        // Konstruktor
        public ClusterRegion(int nr)
        {
            number = nr;
        }

        // Farbe bestimmen
        public Bgr calcColor()
        {
            color = new Bgr(0, 0, 0);

            foreach (KeyValuePair<Cluster, int> c in clusters)
            {

                color.Blue += c.Key.color.Blue;
                color.Green += c.Key.color.Green;
                color.Red += c.Key.color.Red;
            }

            color.Blue = color.Blue / clusters.Count;
            color.Green = color.Green / clusters.Count;
            color.Red = color.Red / clusters.Count;

            return color;
        }


        // niedrigste Y-Koordinate zurückgeben
        public int getY()
        {
            int y = 0;
            foreach (KeyValuePair<Cluster, int> c in clusters)
            {
                if (c.Key.getY() > y)
                {
                    y = c.Key.getY();
                }
            }
            return y;
        }

        // Durschnitts X-Koordinate zurückgeben
        public int getX()
        {
            int x = 0;
            foreach (KeyValuePair<Cluster, int> c in clusters)
            {
                x += c.Key.getX();
            }
            x = x / clusters.Count();
            return x;
        }

    }
}
