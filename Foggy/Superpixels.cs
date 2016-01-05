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
    class Superpixels
    {
        private int n = 0;      // Anzahl Pixels
        private int k = 0;      // Anzahl Superpixels
        private double s = 0;      // Superpixel Intervall Int
        private int area = 0;      // Suchgebiet um Superpixel-Zentrum
        private double m = 0;      // je höher, desto kompakter die Cluster [0,20]

        private Image<Lab, Byte> imageLab;

        private Pixel[,] pixels;  // Pixel mit (l, a, b , x, y) Vektor, distance und ClusterNr

        private Cluster[] clusters;

        // =============== Konstruktor ===============
        public Superpixels(Image<Bgr, Byte> imageBgr, int superpixelCount)
        {
            // Werte setzen
            k = superpixelCount;
            n = imageBgr.Width * imageBgr.Height;
            s = Math.Sqrt((double)n / (double)k);
            area = Convert.ToInt32(2 * s * 2 * s);
            m = 10;

            // BGR to LAB Umrechnung und Vektormatrix erstellen
            imageLab = imageBgr.Convert<Lab, Byte>();
            pixels = new Pixel[imageBgr.Width, imageBgr.Height];
            for (int r = 0; r < imageLab.Height; r++)
            {
                for (int c = 0; c < imageLab.Width; c++)
                {
                    double l = (double)imageLab.Data[r, c, 0] * 100 / 255;
                    double a = (double)imageLab.Data[r, c, 1] - 128;
                    double b = (double)imageLab.Data[r, c, 2] - 128;

                    Bgr bgr = new Bgr(imageBgr.Data[r, c, 0], imageBgr.Data[r, c, 1], imageBgr.Data[r, c, 2]);

                    pixels[c, r] = new Pixel(new Vector5(l, a, b, c, r), bgr);

                    //Console.WriteLine("BGR = " + imageBgr.Data[r, c, 0] + " " + imageBgr.Data[r, c, 1] + " " + imageBgr.Data[r, c, 2]);
                    //Console.WriteLine("RGB = " + imageBgr.Data[r, c, 2] + " " + imageBgr.Data[r, c, 1] + " " + imageBgr.Data[r, c, 0]);
                    //Console.WriteLine("LAB = " + labValues[r, c].X + " " + labValues[r, c].Y + " " + labValues[r, c].Z);
                }
            }
        }


        // =============== Superpixels berechnen ===============
        public void computeSuperpixels()
        {
            // Cluster-Center gleichmäßig positionieren
            initializeClusterCenters();

            // Jedes Cluster-Center in n*n Nachbarschaft zu kleinstem Gradient verschieben
            moveCentersToLowestGradient(3);

            // Clustern neue Pixel zuweisen
            int steps = 4; // 4-10 Schritte empfohlen
            int stepCounter = 0;
            do
            {
                // Schritte erhöhen
                stepCounter++;
                //Console.WriteLine("Step " + stepCounter);

                // Pixel zu besten Clustern hinzufügen
                assignPixelsToBestCluster();

                // neue Cluster-Centers berechnen
                computeNewClusterCenters();

                // Fehler berechnen für automatischen Stop ?
                // TO-DO
            }
            while (stepCounter < steps);


            // Regionen ohne Verbindung zum Cluster neu zuordnen
            enforceConnectivity();
            
            // Clusterfarben berechnen
            foreach (Cluster i in clusters)
            {
                i.calcColor();
            }

        }


        // =============== Cluster-Center gleichmäßig positionieren ===============
        private void initializeClusterCenters()
        {
            //Clusters initializieren
            clusters = new Cluster[k];
            for (int i = 0; i < k; i++)
            {
                clusters[i] = new Cluster();
            }

            // Anzahl Clusters in x und y Richtung bestimmen
            int numberYclusters = Convert.ToInt32(imageLab.Height / s);
            int numberXclusters = Convert.ToInt32(imageLab.Width / s);
            while ((numberYclusters * numberXclusters) > k)
            {
                numberXclusters--;
            }

            // Abstand der Cluster-Center zum Rand bestimmen
            int borderY = Convert.ToInt32((imageLab.Height - s * (numberYclusters - 1)) / 2);
            int borderX = Convert.ToInt32((imageLab.Width - s * (numberXclusters - 1)) / 2);

            // Cluster-Centerpositionen festlegen
            int index = 0;
            for (int r = 0; r < numberYclusters; r++)
            {
                for (int c = 0; c < numberXclusters; c++)
                {
                    int x = Convert.ToInt32(borderX + c * s);
                    int y = Convert.ToInt32(borderY + r * s);
                    clusters[index].currentCenter = new Vector5(pixels[x, y].vector.l, pixels[x, y].vector.a, pixels[x, y].vector.b, x, y);
                    index++;
                }
            }
            //Console.WriteLine("Cluster-Centers erstellt");
        }


        // =============== Jedes Cluster-Center in 3x3 Nachbarschaft zu kleinstem Gradient verschieben ===============
        private void moveCentersToLowestGradient(int neighbourhood){

            int n = neighbourhood - 1 / 2;

            for (int i = 0; i < k; i++)
            {
                //Console.WriteLine("Center Nr = " + i);
                double lowestGradient = 9999;
                Pixel bestPixel = new Pixel(new Vector5(0,0,0,0,0), new Bgr(0,0,0));
                for (int r = -n; r <= n; r++)
                {
                    for (int c = -n; c <= n; c++)
                    {
                        int x = clusters[i].currentCenter.x;
                        int y = clusters[i].currentCenter.y;

                        double newGradient = calcGradient(pixels[x + c, y + r].vector);

                        if (newGradient < lowestGradient)
                        {
                            lowestGradient = newGradient;
                            bestPixel.vector = pixels[x + c, y + r].vector;
                            //Console.WriteLine("niedriger Gradient");
                        }
                    }
                }
                clusters[i].currentCenter = bestPixel.vector;
                //Console.WriteLine("fertig verschoben");
            }
        }



        // =============== Pixel zum besten Superpixel in Suchumgebung zuordnen ===============
        private void assignPixelsToBestCluster()
        {
            // Abstand zwischen Cluster-Centern als int-Wert
            int sInt = Convert.ToInt32(s); 

            // für alle Cluster
            for (int i = 0; i < k; i++)
            {
                //Console.WriteLine("------------ Cluster {0} ----------", i);

                // aktuelles Cluster
                Cluster cluster = clusters[i];

                //int betterPixel = 0;

                // Such-Nachbarschaft durchlaufen
                for (int r = -sInt; r <= sInt; r++)
                {
                    for (int c = -sInt; c < sInt; c++)
                    {
                        // Randbehandlung
                        if (cluster.currentCenter.x + r >= 0 && cluster.currentCenter.x + r < imageLab.Width && cluster.currentCenter.y + c >= 0 && cluster.currentCenter.y + c < imageLab.Height)
                        {
                            // aktuelle Koordinaten
                            int x = cluster.currentCenter.x + r;
                            int y = cluster.currentCenter.y + c;

                            // aktueller Pixel
                            //Pixel pixel = pixels[cluster.currentCenter.x + r, cluster.currentCenter.y + c];

                            // Ähnlichkeit des Pixels zum Cluster-Center berechnen
                            double distance = calcDistance(pixels[x,y].vector, cluster.currentCenter);


                            // Wenn Ähnlichkeit zum Cluster-Center besser als vorher
                            if (distance < pixels[x, y].distance)
                            {
                               // betterPixel++;

                                // Pixel aus vorherigem Cluster löschen
                                if (pixels[x, y].clusterNr >= 0)
                                {
                                    clusters[pixels[x, y].clusterNr].removePixel(pixels[x, y]);
                                    
                                    //Console.WriteLine("delete Pixel");
                                }

                                // Pixel im aktuellen Cluster speichern
                                cluster.addPixel(pixels[x, y]);
                                //Console.WriteLine("add Pixel");

                                // neue Distance im Pixel speichern
                                pixels[x, y].distance = distance;
                                // neue Cluster Nr im Pixel speichern
                                pixels[x, y].clusterNr = i;

                                // Pixel mit neuen Werten im Array speichern
                                //pixels[cluster.currentCenter.x + r, cluster.currentCenter.y + c] = pixel;
                            }
                        }
                    }
                }
                //Console.WriteLine("better Pixel = " + betterPixel);
                //Console.WriteLine("Center " + i + "   neue Distanz: " + counter);
            }
        }


        // =============== neue Cluster-Centers berechnen ===============
        private void computeNewClusterCenters()
        {
            foreach (Cluster c in clusters) {

                int xSum = 0;
                int ySum = 0;

                // x und y Werte addieren
                foreach (KeyValuePair<Pixel, int> p in c.pixels)
                {
                    xSum = xSum + p.Key.vector.x;
                    ySum = ySum + p.Key.vector.y;
                }

                // altes Cluster-Center merken
                c.oldCenter = c.currentCenter;

                // Durchschnitt berechnen
                int newX = Convert.ToInt32(Convert.ToDouble(xSum) / Convert.ToDouble(c.pixels.Count));
                int newY = Convert.ToInt32(Convert.ToDouble(ySum) / Convert.ToDouble(c.pixels.Count));

                // neues Cluster-Center festlegen
                c.currentCenter = pixels[newX, newY].vector;
            }
        }

        
        // =============== alle Pixel ohne Verbindung zum Cluster neu labeln ===============
        private void enforceConnectivity()
        {
            // Bild durchlaufen
            for (int r = 0; r < imageLab.Height; r++)
            {
                for (int c = 0; c < imageLab.Width; c++)
                {
                    //Wenn der Pixel noch nicht bearbeitet wurde
                    if (!pixels[c, r].scanned)
                    {
                        // zugehörige Region bearbeiten
                        handlePixelRegion(pixels[c, r]);
                    }
                }
            }
        }


        // =============== Größe der Region prüfen und ggf mit Nachbarcluster verbinden ===============
        private void handlePixelRegion(Pixel pixel)
        {
            // minimale Größe von erlaubten Regionen
            int regionSizeLimit = 200;

            // aktuellen Pixel markieren
            pixel.scanned = true;

            // Liste der aktuell zu prüfenden Pixel
            Dictionary<Pixel, int> checkPixels = new Dictionary<Pixel, int>();
            checkPixels.Add(pixel, 0);

            // Liste der neu zu prüfenden Pixel
            Dictionary<Pixel, int> newPixels = new Dictionary<Pixel, int>();

            // Liste aller Pixel der Region
            Dictionary<Pixel, int> allPixels = new Dictionary<Pixel, int>();
            allPixels.Add(pixel, 0);

            // Liste alle benachbarten Cluster-Nummern
            List<int> neighbourClusters = new List<int>();

            // neue Region-Pixel gefunden?
            bool newPixelsFound;

            // Solange wiederholen, bis alle Pixel der Region gefunden wurden
            do
            {
                newPixelsFound = false;

                // alle aktuell zu prüfenden Pixel durchlaufen
                foreach (KeyValuePair<Pixel, int> p in checkPixels)
                {
                    int x = p.Key.vector.x;
                    int y = p.Key.vector.y;

                    // benachbarte Pixel mit unterschiedlichen Clusternummern finden
                    if (y - 1 >= 0 && pixels[x, y - 1].clusterNr != pixel.clusterNr && pixels[x, y - 1].scanned) {
                        neighbourClusters.Add(pixels[x, y - 1].clusterNr);
                    }
                    if (y + 1 < imageLab.Height && pixels[x, y + 1].clusterNr != pixel.clusterNr && pixels[x, y + 1].scanned)
                    {
                        neighbourClusters.Add(pixels[x, y + 1].clusterNr);
                    }
                    if (x - 1 >= 0 && pixels[x - 1, y].clusterNr != pixel.clusterNr && pixels[x - 1, y].scanned)
                    {
                        neighbourClusters.Add(pixels[x - 1, y].clusterNr);
                    }
                    if (x + 1 < imageLab.Width && pixels[x + 1, y].clusterNr != pixel.clusterNr && pixels[x + 1, y].scanned)
                    {
                        neighbourClusters.Add(pixels[x + 1, y].clusterNr);
                    }

                    // benachbarte Pixel mit gleicher Clusternummer finden und ggf neuer Pixelliste hinzufügen
                    // if (Randbehandlung && ClusterNr && bereits in newPixels && bereits in allPixels)
                    if (y - 1 >= 0 && pixels[x, y - 1].clusterNr == pixel.clusterNr && !pixels[x, y - 1].scanned)
                    {
                        newPixels.Add(pixels[x, y - 1], 0);
                        pixels[x, y - 1].scanned = true;
                        newPixelsFound = true;
                    }
                    if (y + 1 < imageLab.Height && pixels[x, y + 1].clusterNr == pixel.clusterNr && !pixels[x, y + 1].scanned)
                    {
                        newPixels.Add(pixels[x, y + 1], 0);
                        pixels[x, y + 1].scanned = true;
                        newPixelsFound = true;
                    }
                    if (x - 1 >= 0 && pixels[x - 1, y].clusterNr == pixel.clusterNr && !pixels[x - 1, y].scanned)
                    {
                        newPixels.Add(pixels[x - 1, y], 0);
                        pixels[x - 1, y].scanned = true;
                        newPixelsFound = true;
                    }
                    if (x + 1 < imageLab.Width && pixels[x + 1, y].clusterNr == pixel.clusterNr && !pixels[x + 1, y].scanned)
                    {
                        newPixels.Add(pixels[x + 1, y], 0);
                        pixels[x + 1, y].scanned = true;
                        newPixelsFound = true;
                    }
                }

                // neu gefundene Pixel im nächsten Schritt überprüfen
                checkPixels.Clear();
                checkPixels = checkPixels.Concat(newPixels).ToDictionary(x => x.Key, x => x.Value);
                // neu gefundene Pixel zur kompletten Liste hinzufügen
                allPixels = allPixels.Concat(newPixels).ToDictionary(x => x.Key, x => x.Value);
                // Liste der neu gefundenen Pixel leeren
                newPixels.Clear();
            }
            while (newPixelsFound);


            // wenn die Region zu klein ist, mit am häufigst gefundenem Nachbarcluster verbinden
            if (allPixels.Count() < regionSizeLimit)
            {
                // neue Cluster-Nummer der Region
                int newClusterNr;

                // wenn benachbarte Clusternummern gefunden wurden
                if (neighbourClusters.Count() != 0)
                {
                    // am häufigsten gefunde benachbarte Cluster-Nummer bestimmen
                    newClusterNr = neighbourClusters.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();

                    // jeden Pixel in neues Cluster aufnehmen
                    foreach (KeyValuePair<Pixel, int> p in allPixels)
                    {
                        // Pixel aus vorherigem Cluster löschen
                        clusters[p.Key.clusterNr].removePixel(p.Key);

                        // Pixel im neuen Cluster speichern
                        clusters[newClusterNr].addPixel(p.Key);

                        // neue Cluster Nr im Pixel speichern
                        p.Key.clusterNr = newClusterNr;
                    }
                }
            }
        }


        // =============== Gradient berechnen ===============
        private double calcGradient(Vector5 center)
        {
            int x = center.x;
            int y = center.y;

            double g = Math.Pow(pixels[x + 1, y].vector.sub(pixels[x - 1, y].vector).l2Norm(),2) + Math.Pow(pixels[x, y + 1].vector.sub(pixels[x, y - 1].vector).l2Norm(),2);

            return g;
        }



        // =============== Distanz berechnen ===============
        private double calcDistance(Vector5 pixel1, Vector5 pixel2)
        {
            double dLab = Math.Sqrt(Math.Pow((pixel1.l - pixel2.l), 2) + Math.Pow((pixel1.a - pixel2.a), 2) + Math.Pow((pixel1.b - pixel2.b), 2));
            double dXy = Math.Sqrt(Math.Pow((pixel1.x - pixel2.x), 2) + Math.Pow((pixel1.y - pixel2.y), 2));

            double ds = dLab + m / s * dXy;

            return ds;
        }


        // =============== Cluster zum Zeichnen zurückgeben ===============
        public Cluster[] getClusters()
        {
            return clusters;
        }

    }
}




    // =========================================================================
    // KLASSE: 5 Dimensionaler Vektor
    // =========================================================================
    class Vector5 {

        public double l, a, b;
        public int x, y;

        public Vector5(double _l, double _a, double _b, int _x, int _y)
        {
            l = _l;
            a = _a;
            b = _b;
            x = _x;
            y = _y;
        }

        public double l2Norm()
        {
            double norm = Math.Sqrt(Math.Pow(Math.Abs(l), 2) + Math.Pow(Math.Abs(a), 2) + Math.Pow(Math.Abs(b), 2));
            return norm;
        }

        public Vector5 sub(Vector5 v)
        {
            Vector5 vsub = new Vector5(l - v.l, a - v.a, b - v.b, x - v.x, y - v.y);
            return vsub;
        }
    }


    // =========================================================================
    // KLASSE: Pixel mit Vektor, Distanz und Center-Nr
    // =========================================================================
    class Pixel
    {
        public Vector5 vector;
        public double distance;
        public int clusterNr;
        public Bgr bgr;

        public bool scanned;

        public Pixel(Vector5 _vector, Bgr _bgr)
        {
            vector = _vector;
            distance = 9999;
            clusterNr = -1;
            bgr = _bgr;
            scanned = false;
        }
    }

    // =========================================================================
    // KLASSE: Cluster mit aktuellem Center, neuem Center und Liste mit Pixeln
    // =========================================================================
    class Cluster
    {

        public Vector5 oldCenter;
        public Vector5 currentCenter;

        public Dictionary<Pixel, int> pixels = new Dictionary<Pixel, int>();

        public Bgr color;

        public Cluster()
        {
        }

        public void addPixel(Pixel pixel){
            pixels.Add(pixel, 0);
        }

        public void removePixel(Pixel pixel)
        {
            pixels.Remove(pixel);
        }

        public void clearPixelList()
        {
            pixels.Clear();
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

        public Dictionary<Pixel, int> getPixelList()
        {
            return pixels;
        }

        public int getPixelCount()
        {
            return pixels.Count;
        }
    }