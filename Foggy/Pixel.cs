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
    // =========================================================================
    // KLASSE: Pixel zur Berechnung von Superpixeln
    // =========================================================================
    class Pixel
    {
        public Vector5 vector;
        public double distance;
        public int clusterNr;
        public Bgr bgr;

        public bool scanned;

        // Konstruktor
        public Pixel(Vector5 _vector, Bgr _bgr)
        {
            vector = _vector;
            distance = 9999;
            clusterNr = -1;
            bgr = _bgr;
            scanned = false;
        }
    }
}
