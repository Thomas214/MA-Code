using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foggy
{

    // =========================================================================
    // KLASSE: 5 Dimensionaler Vektor für Berechnung von Superpixeln
    // =========================================================================
    class Vector5
    {

        public double l, a, b;
        public int x, y;

        // Konstruktor
        public Vector5(double _l, double _a, double _b, int _x, int _y)
        {
            l = _l;
            a = _a;
            b = _b;
            x = _x;
            y = _y;
        }

        // L2-Norm des Vektors bestimmen
        public double l2Norm()
        {
            double norm = Math.Sqrt(Math.Pow(Math.Abs(l), 2) + Math.Pow(Math.Abs(a), 2) + Math.Pow(Math.Abs(b), 2));
            return norm;
        }

        // Vektor-Subtraktion
        public Vector5 sub(Vector5 v)
        {
            Vector5 vsub = new Vector5(l - v.l, a - v.a, b - v.b, x - v.x, y - v.y);
            return vsub;
        }

    }
}
