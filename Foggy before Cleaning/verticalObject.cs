using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foggy
{

    class verticalObject
    {
        public List<ClusterRegion> regions;
        public double depthValue;

        // Konstruktor
        public verticalObject()
        {
            regions = new List<ClusterRegion>();
        }

        // Durchschnitts X-Wert zurückgeben
        public int getX()
        {
            int x = 0;

            foreach (ClusterRegion r in regions)
            {
               x += r.getX();
            }
            x = x / regions.Count();
            return x;
        }

        // Niedrigsten Y-Wert zurückgeben
        public int getY()
        {
            int y = 0;
            foreach (ClusterRegion r in regions)
            {
                if (r.getY() > y)
                {
                    y = r.getY();
                }
            }
            return y;
        }
    }
}
