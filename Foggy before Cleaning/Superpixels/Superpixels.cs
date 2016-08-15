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
    // KLASSE: Berechnung von Superpixeln
    // =========================================================================
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
        private List<Cluster> clusterList;

        List<ClusterRegion> regionList = new List<ClusterRegion>();

        int numberYclusters;
        int numberXclusters;

        // =============== Konstruktor ===============
        public Superpixels(Image<Bgr, Byte> imageBgr, int superpixelCount)
        {
            // Werte setzen
            k = superpixelCount;
            n = imageBgr.Width * imageBgr.Height;
            s = Math.Sqrt((double)n / (double)k);
            area = Convert.ToInt32(2 * s * 2 * s);
            m = 12;

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
            Console.WriteLine("Compute Superpixels");
            do
            {
                // Schritte erhöhen
                stepCounter++;

                Console.WriteLine("Step " + stepCounter);

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


            //enforceConnectivity();

            // Clusterfarben berechnen
            foreach (Cluster i in clusters)
            {
                i.calcColor();
            }

            clusterList = clusters.ToList<Cluster>();

            // Ähnliche benachbarte Cluster zusammenfassen
            createRegions();


        }


        // =============== Cluster-Center gleichmäßig positionieren ===============
        private void initializeClusterCenters()
        {
            // Clusters initializieren
            clusters = new Cluster[k];
            for (int i = 0; i < k; i++)
            {
                clusters[i] = new Cluster();
            }

            // Anzahl Clusters in x und y Richtung bestimmen
            numberYclusters = Convert.ToInt32(imageLab.Height / s);
            numberXclusters = Convert.ToInt32(imageLab.Width / s);
            while ((numberYclusters * numberXclusters) > k)
            {
                numberXclusters--;
            }

            // Nachbarcluster hinzufügen
            for (int i = 0; i < k; i++)
            {
                if (i >= numberXclusters)
                {
                    Cluster clusterAbove = clusters[i - numberXclusters];
                    clusters[i].neighbours.Add(clusterAbove, 0);
                }
                if (i < numberXclusters * (numberYclusters - 1))
                {
                    Cluster clusterBelow = clusters[i + numberXclusters];
                    clusters[i].neighbours.Add(clusterBelow, 0);
                }
                if ((i + 1) % numberXclusters != 0)
                {
                    Cluster clusterRight = clusters[i + 1];
                    clusters[i].neighbours.Add(clusterRight, 0);
                }
                if (i % numberXclusters != 0)
                {
                    Cluster clusterLeft = clusters[i - 1];
                    clusters[i].neighbours.Add(clusterLeft, 0);
                }
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
        }


        // =============== Jedes Cluster-Center in 3x3 Nachbarschaft zu kleinstem Gradient verschieben ===============
        private void moveCentersToLowestGradient(int neighbourhood)
        {

            int n = neighbourhood - 1 / 2;

            for (int i = 0; i < k; i++)
            {
                //Console.WriteLine("Center Nr = " + i);
                double lowestGradient = 9999;
                Pixel bestPixel = new Pixel(new Vector5(0, 0, 0, 0, 0), new Bgr(0, 0, 0));
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
                            double distance = calcDistance(pixels[x, y].vector, cluster.currentCenter);


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
            }
        }


        // =============== neue Cluster-Centers berechnen ===============
        private void computeNewClusterCenters()
        {
            foreach (Cluster c in clusters)
            {

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
            Console.WriteLine("Enforce Connectivity");
            // Bild durchlaufen
            for (int r = 0; r < imageLab.Height; r++)
            {
                for (int c = 0; c < imageLab.Width; c++)
                {
                    // Wenn der Pixel noch nicht bearbeitet wurde
                    if (!pixels[c, r].scanned)
                    {
                        // zugehörige Region bearbeiten
                        handleRegionConnectivity(pixels[c, r]);
                    }
                }
            }
        }


        // =============== Größe der Region prüfen und ggf mit Nachbarcluster verbinden ===============
        private void handleRegionConnectivity(Pixel pixel)
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

            // Liste aller Pixel der aktuellen Region
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

                    // benachbarte Pixel mit unterschiedlicher Clusternummer finden
                    if (y - 1 >= 0 && pixels[x, y - 1].clusterNr != pixel.clusterNr && pixels[x, y - 1].scanned)
                    {
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

                    // neue benachbarte Pixel mit gleicher Clusternummer finden und Pixelliste hinzufügen
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


            // wenn die Region zu klein ist, mit dem am häufigsten gefundenem Nachbar-Cluster verbinden
            if (allPixels.Count() < regionSizeLimit)
            {
                // wenn benachbarte Clusternummern gefunden wurden
                if (neighbourClusters.Count() != 0)
                {
                    // am häufigsten gefundene benachbarte Cluster-Nummer bestimmen
                    int newClusterNr = neighbourClusters.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();

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


        // =============== Ähnliche benachbarte Cluster zusammenfassen ===============
        private void createRegions()
        {
            Console.WriteLine("Create Regions");

            int colorDistanceLimit = 1;

            bool clustersAddedToRegion;

            // aktuell zu prüfender Cluster
            Cluster currentCluster;

            // Liste mit aktuell zu prüfenden Nachbarn
            Dictionary<Cluster, int> currentNeighbours = new Dictionary<Cluster, int>();

            // Liste mit neu zu prüfenden Nachbarn
            Dictionary<Cluster, int> newNeighbours = new Dictionary<Cluster, int>();

            int regionNr = -1;

            int i = 0;
            // alle Cluster durchlaufen
            do
            {
                // aktuellen Cluster setzen
                currentCluster = clusters[i];
                //Console.WriteLine("=========================");
                //Console.WriteLine("aktueller Cluster Nr: " + i);



                // wenn aktueller Cluster noch nicht bearbeitet wurde
                if (!currentCluster.scanned)
                {
                    //Console.WriteLine("- neue Cluster-Region für Cluster Nr: " + i);


                    // Cluster bearbeiten
                    currentCluster.scanned = true;

                    // neue Cluster-Region erstellen
                    regionNr++;
                    ClusterRegion currentRegion = new ClusterRegion(regionNr);

                    //aktuellen Cluster zur neu angelegten Region hinzufügen
                    currentRegion.clusters.Add(currentCluster, 0);
                    currentCluster.regionNr = regionNr;


                    // Anfangsnachbarn festlegen
                    currentNeighbours = currentCluster.neighbours;

                    // Cluster-Region weiter scannen, bis keine ähnlichen Nachbarn mehr gefunden werden
                    do
                    {
                        //Console.WriteLine("-- " + currentNeighbours.Count + " neue Cluster untersuchen");

                        // noch keine Cluster zur Cluster-Region hinzugefügt
                        clustersAddedToRegion = false;

                        // aktuell zu prüfende Nachbarcluster auf Ähnlichkeit überprüfen
                        foreach (KeyValuePair<Cluster, int> cluster in currentNeighbours)
                        {
                            //Console.WriteLine("--- check Cluster");

                            // wenn Nachbarcluster noch keiner ClusterRegion zugeordnet wurde und ähnlich zur aktuellen Cluster-Region ist
                            if (!cluster.Key.scanned && currentCluster.colorDistance(cluster.Key) < colorDistanceLimit)
                            {
                                // Console.WriteLine("---- Ähnlichkeit festgestellt");

                                // Cluster markieren
                                cluster.Key.scanned = true;

                                // Cluster zur Cluster-Region hinzufügen
                                currentRegion.clusters.Add(cluster.Key, 0);
                                cluster.Key.regionNr = regionNr;

                                //Console.WriteLine("regionNr = " + regionNr);

                                // erster Cluster übernimmt Pixel des zweiten
                                //currentCluster.pixels = currentCluster.pixels.Concat(neighbour.Key.pixels).ToDictionary(x => x.Key, x => x.Value);

                                // noch nicht markierte Nachbarn des Clusters in Liste merken
                                foreach (KeyValuePair<Cluster, int> n in cluster.Key.neighbours)
                                {
                                    //Console.WriteLine("----- Nachbar von gefundenem Nachbar angucken");
                                    if (!newNeighbours.Contains(n) && !n.Key.scanned)
                                    {
                                        //Console.WriteLine("------ Zu neuer Nachbar-Liste hinzufügen");
                                        newNeighbours.Add(n.Key, 0);
                                    }
                                }
                                // aktuellen Cluster aus der Nachbarliste löschen
                                //newNeighbours.Remove(currentCluster);

                                // zweiten Cluster löschen
                                //clusterList.Remove(neighbour.Key);

                                // Farbe neu berechnen
                                //currentCluster.calcColor();

                                // Farbe der Region neu berechnen
                                /*
                                Bgr newColor = currentRegion.calcColor();
                                // neue Farben allen enthaltenen Clustern zuweisen
                                foreach (KeyValuePair<Cluster, int> c in currentRegion.clusters)
                                {
                                    c.Key.color = newColor;
                                }
                                */

                                // neue Cluster wurden verbunden
                                clustersAddedToRegion = true;

                            }
                        }

                        // wenn Cluster zur aktuellen Region hinzugefügt wurden
                        if (clustersAddedToRegion)
                        {
                            // neue Nachbarn für nächsten Durchlauf
                            currentNeighbours.Clear();
                            currentNeighbours = currentNeighbours.Concat(newNeighbours).ToDictionary(x => x.Key, x => x.Value);

                            // zu bearbeitende Nachbarn löschen
                            newNeighbours.Clear();

                            // neue Nachbarn speichern
                            currentCluster.neighbours = newNeighbours;

                            // Liste mit neuen Nachbarn löschen
                            newNeighbours.Clear();

                            //Console.WriteLine("--- Ähnliche Nachbarn wurden gefunden");
                        }


                    }
                    //bis keine ähnlichen Nachbar-Cluster mehr gefunden werden
                    while (clustersAddedToRegion);

                    // Farbe der Region neu berechnen
                    currentRegion.calcColor();

                    // Region der Liste hinzufügen
                    regionList.Add(currentRegion);

                    //Console.WriteLine("- Größe Cluster-Region: " + currentRegion.clusters.Count());
                    i++;
                }
                else
                {
                    //Console.WriteLine("- überspringen");
                    // wenn Cluster schon markiert, weiter mit nächstem Cluster
                    i++;
                }


            }
            // bis letzter Cluster erreicht wurde
            while (i < clusterList.Count());

        }


        /*
        private void collapseClustersTest2()
        {

            Cluster[,] clusterArray = new Cluster[numberXclusters,numberYclusters];

            int x1 = 0;
            for (int y = 0; y < numberYclusters; )
            {
                clusterArray[x1, y] = clusters[y * numberXclusters + x1];
                x1++;
                if (x1 == numberXclusters)
                {
                    x1 = 0;
                    y++;
                }
            }


            int colorLimit = 50;
            for (int y = 0; y < numberYclusters; y++)
            {
                for (int x = 0; x < numberXclusters; x++)
                {
                    Cluster currentCluster = clusterArray[x, y];

                    if (y - 1 >= 0)
                    {
                        Cluster clusterAbove = clusterArray[x, y - 1];
                        if (currentCluster.colorDistance(clusterAbove) < colorLimit)
                        {

                            double b = (currentCluster.color.Blue + clusterAbove.color.Blue) / 2;
                            double g = (currentCluster.color.Green + clusterAbove.color.Green) / 2;
                            double r = (currentCluster.color.Red + clusterAbove.color.Red) / 2;

                            Bgr newColor = new Bgr(b, g, r);

                            currentCluster.color = newColor;
                            clusterAbove.color = newColor;
                        }
                    }

                    if (y + 1 < numberYclusters)
                    {
                        Cluster clusterBelow = clusterArray[x, y + 1];
                        if (currentCluster.colorDistance(clusterBelow) < colorLimit)
                        {

                            double b = (currentCluster.color.Blue + clusterBelow.color.Blue) / 2;
                            double g = (currentCluster.color.Green + clusterBelow.color.Green) / 2;
                            double r = (currentCluster.color.Red + clusterBelow.color.Red) / 2;

                            Bgr newColor = new Bgr(b, g, r);

                            currentCluster.color = newColor;
                            clusterBelow.color = newColor;
                        }
                    }

                    if (x - 1 >= 0)
                    {
                        Cluster clusterLeft = clusterArray[x - 1, y];
                        if (currentCluster.colorDistance(clusterLeft) < colorLimit)
                        {

                            double b = (currentCluster.color.Blue + clusterLeft.color.Blue) / 2;
                            double g = (currentCluster.color.Green + clusterLeft.color.Green) / 2;
                            double r = (currentCluster.color.Red + clusterLeft.color.Red) / 2;

                            Bgr newColor = new Bgr(b, g, r);

                            currentCluster.color = newColor;
                            clusterLeft.color = newColor;
                        }
                    }

                    if (x + 1 < numberXclusters)
                    {
                        Cluster clusterRight = clusterArray[x + 1, y];
                        if (currentCluster.colorDistance(clusterRight) < colorLimit)
                        {

                            double b = (currentCluster.color.Blue + clusterRight.color.Blue) / 2;
                            double g = (currentCluster.color.Green + clusterRight.color.Green) / 2;
                            double r = (currentCluster.color.Red + clusterRight.color.Red) / 2;

                            Bgr newColor = new Bgr(b, g, r);

                            currentCluster.color = newColor;
                            clusterRight.color = newColor;
                        }
                    }
                   
                }
            }

        }

        */

        // =============== Ähnliche benachbarte Cluster zusammenfassen ===============
        private void collapseClustersTest()
        {

            clusterList = clusters.ToList<Cluster>();

            int i = 0;
            do
            {
                Cluster currentCluster = clusterList.ElementAt(i);
                Cluster nextCluster = clusterList.ElementAt(i + 1);

                if (currentCluster.colorDistance(nextCluster) < 80)
                {
                    //Console.WriteLine("Cluster gleich: " + i + " und " + (i + 1) );

                    //Console.WriteLine("currentClusterSize = " + currentCluster.pixels.Count());
                    //Console.WriteLine("nextClusterSize = " + nextCluster.pixels.Count());

                    // Cluster verbinden
                    currentCluster.pixels = currentCluster.pixels.Concat(nextCluster.pixels).ToDictionary(x => x.Key, x => x.Value);
                    //Console.WriteLine("concat Clusters");


                    //Console.WriteLine("newClusterSize = " + currentCluster.pixels.Count());

                    //Console.WriteLine("clusterListSize = " + clusterList.Count());

                    // zweiten Cluster löschen
                    clusterList.Remove(nextCluster);
                    //Console.WriteLine("nextCluster löschen");

                    //Console.WriteLine("clusterListSize = " + clusterList.Count());


                    // Farbe neu berechnen
                    currentCluster.calcColor();

                    //i++;
                }
                else
                {
                    i++;
                    //Console.WriteLine("neues i = " + i);
                }
            }
            while (i < clusterList.Count() - 1);


        }


        // =============== Gradient berechnen ===============
        private double calcGradient(Vector5 pixel)
        {
            int x = pixel.x;
            int y = pixel.y;

            double g = Math.Pow(pixels[x + 1, y].vector.sub(pixels[x - 1, y].vector).l2Norm(), 2) + Math.Pow(pixels[x, y + 1].vector.sub(pixels[x, y - 1].vector).l2Norm(), 2);

            return g;
        }



        // =============== Distanz zweier Pixel berechnen ===============
        private double calcDistance(Vector5 pixel1, Vector5 pixel2)
        {
            double dLab = Math.Sqrt(Math.Pow((pixel1.l - pixel2.l), 2) + Math.Pow((pixel1.a - pixel2.a), 2) + Math.Pow((pixel1.b - pixel2.b), 2));
            double dXy = Math.Sqrt(Math.Pow((pixel1.x - pixel2.x), 2) + Math.Pow((pixel1.y - pixel2.y), 2));

            double ds = dLab + m / s * dXy;

            return ds;
        }


        // =============== Cluster zum Zeichnen zurückgeben ===============
        public List<Cluster> getClusterList()
        {
            return clusterList;
        }

        // =============== Regionen zum Zeichnen zurückgeben ===============
        public List<ClusterRegion> getRegionList()
        {
            return regionList;
        }

        // =============== Pixel zum Zeichnen zurückgeben ===============
        public Pixel[,] getPixelArray()
        {
            return pixels;
        }

    }

}