using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace TheHarborWPF
{

    public partial class MainWindow : Window
    {
        private BindingList<Harbor> harbor = new BindingList<Harbor>();
        private BindingList<Harbor> quayside1 = new BindingList<Harbor>();
        private BindingList<Harbor> quayside2 = new BindingList<Harbor>();
        private List<Boat> anchoredBoats = new List<Boat>();
        public int rejectedBoats = 0;
        string[,] savedBoats = new string[64, 9];
        Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();

            //Create berths/mooringlocations at startup 
            BindingList<Harbor> harbor1 = CreateMoorings(harbor);

            //Adds saved boats to harbor at startup
            AddSavedBoats(harbor, savedBoats, anchoredBoats);
            foreach (var h in harbor)
            {
                if (h.MooringNumber < 33)
                {
                    quayside1.Add(h);
                }
                else if (h.MooringNumber >= 33)
                {
                    quayside2.Add(h);
                }
            }
            Quayside2.ItemsSource = quayside2;
            Quayside1.ItemsSource = quayside1;

            //Adds infotext if Saved.txt exists
            if ((File.Exists("Saved.txt")))
            {
                tb.Text = rejectedBoats.ToString();
                freeSpaceCounter.Text = CalcFreeSpace(harbor).ToString();
                maxSpeedText.Text = CalcSpeed(harbor).ToString();
                totalWeight.Text = CalcWeight(harbor).ToString();
                RBCount.Text = CalcNumOfRowboats(harbor).ToString();
                MBCount.Text = CalcNumOfMotorboats(harbor).ToString();
                SBCount.Text = CalcNumOfSailboat(harbor).ToString();
                CSCount.Text = CalcNumOfCargoship(harbor).ToString();
                KCount.Text = CalcNumOfCatamaran(harbor).ToString();
                TotCount.Text = (CalcNumOfRowboats(harbor) + CalcNumOfMotorboats(harbor) + CalcNumOfSailboat(harbor) + CalcNumOfCargoship(harbor) + CalcNumOfCatamaran(harbor)).ToString();
            }
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            quayside1.Clear();
            quayside2.Clear();

            //Updates number of boats added each day
            try
            {
                int numberOfBoatsIncoming = int.Parse(NumberOfNewBoats.Text);

            //Updates harbor
            BindingList<Harbor> harbor1 = UpdateHarbor(rnd, harbor, anchoredBoats, numberOfBoatsIncoming);
                foreach (var h in harbor)
                {
                    if (h.MooringNumber < 33)
                    {
                        quayside1.Add(h);
                    }
                    else if (h.MooringNumber >= 33)
                    {
                        quayside2.Add(h);
                    }
                }
                Quayside2.ItemsSource = quayside2;
                Quayside1.ItemsSource = quayside1;

                //Updates infotext
                tb.Text = rejectedBoats.ToString();
                freeSpaceCounter.Text = CalcFreeSpace(harbor).ToString();
                NumberOfNewBoats.Text = numberOfBoatsIncoming.ToString();
                maxSpeedText.Text = CalcSpeed(harbor).ToString();
                totalWeight.Text = CalcWeight(harbor).ToString();
                RBCount.Text = CalcNumOfRowboats(harbor).ToString();
                MBCount.Text = CalcNumOfMotorboats(harbor).ToString();
                SBCount.Text = CalcNumOfSailboat(harbor).ToString();
                CSCount.Text = CalcNumOfCargoship(harbor).ToString();
                KCount.Text = CalcNumOfCatamaran(harbor).ToString();
                TotCount.Text = (CalcNumOfRowboats(harbor) + CalcNumOfMotorboats(harbor) + CalcNumOfSailboat(harbor) + CalcNumOfCargoship(harbor) + CalcNumOfCatamaran(harbor)).ToString();
                SaveBoats(harbor);
            }
            catch
            {
                MessageBox.Show("You have to write a digit in the textbox under 'Number of new boats'");
            }
        }

        private BindingList<Harbor> UpdateHarbor(Random rnd, BindingList<Harbor> harbor, List<Boat> anchoredBoats, int numberOfBoatsIncoming)
        {
            //Calculates when boats will depart
            foreach (var a in anchoredBoats.ToList())
            {
                a.Stays--;

                if (a.Stays <= 0)
                {
                    foreach (var p in harbor.ToList())
                    {
                        if (a.ID == p.Boat.ID)
                        {
                            p.Boat = new Rowboat("-", "-", 0, 0, 0, "-", 0);
                            p.Space1 = true;
                            anchoredBoats.Remove(a);
                        }
                        if (a.ID == p.Boat2.ID)
                        {
                            p.Boat2 = new Rowboat("-", "-", 0, 0, 0, "-", 0);
                            p.Space2 = true;
                            anchoredBoats.Remove(a);
                        }
                    }
                }
            }

            //Adds new boats to harbor
            List<Boat> incomingBoats = CreateBoats(rnd, numberOfBoatsIncoming);
            var inc = incomingBoats
                .OrderByDescending((p) => p.Size);

            foreach (var i in inc)
            {
                if (i.Type == "Rowboat")
                {
                    bool checkIfRBPlaced = false;
                    foreach (var p in harbor)
                    {
                        if (p.Space1 == true)
                        {
                            p.Space1 = false;
                            p.Boat = i;
                            anchoredBoats.Add(i);
                            checkIfRBPlaced = true;
                            break;
                        }
                        else if (p.Space2 == true)
                        {
                            p.Space2 = false;
                            p.Boat2 = i;
                            anchoredBoats.Add(i);
                            checkIfRBPlaced = true;
                            break;
                        }
                    }
                    if (checkIfRBPlaced == false)
                    {
                        rejectedBoats++;
                    }
                }
                if (i.Type == "Motorboat")
                {
                    bool checkIfMBPlaced = false;
                    foreach (var p in harbor)
                    {
                        if (p.Space1 == true && p.Space2 == true)
                        {
                            p.Space1 = false;
                            p.Space2 = false;
                            p.Boat = i;
                            p.Boat2 = i;
                            anchoredBoats.Add(i);
                            checkIfMBPlaced = true;
                            break;
                        }
                    }
                    if (checkIfMBPlaced == false)
                    {
                        rejectedBoats++;
                    }
                }
                if (i.Type == "Sailboat")
                {
                    bool checkIfSailboatPlaced = false;
                    foreach (var p in harbor)
                    {
                        if (p.Space1 == true && p.Space2 == true)
                        {
                            foreach (var p2 in harbor)
                            {
                                if (p2.Space1 == true && p2.Space2 == true && (p.MooringNumber + 1) == p2.MooringNumber && p2.MooringNumber != 33)
                                {
                                    p.Space1 = false;
                                    p.Space2 = false;
                                    p2.Space1 = false;
                                    p2.Space2 = false;
                                    p.Boat = i;
                                    p.Boat2 = i;
                                    p2.Boat = i;
                                    p2.Boat2 = i;
                                    anchoredBoats.Add(i);
                                    checkIfSailboatPlaced = true;
                                    break;
                                }
                            }
                            if (checkIfSailboatPlaced == true)
                            {
                                break;
                            }
                        }
                    }
                    if (checkIfSailboatPlaced == false)
                    {
                        rejectedBoats++;
                    }
                }
                if (i.Type == "Cargoship")
                {
                    bool checkIfCargoshipPlaced = false;
                    foreach (var p in harbor)
                    {
                        if (p.Space1 == true && p.Space2 == true)
                        {
                            foreach (var p2 in harbor)
                            {
                                if (p2.Space1 == true && p2.Space2 == true && (p.MooringNumber + 1) == p2.MooringNumber && p2.MooringNumber != 33)
                                {
                                    foreach (var p3 in harbor)
                                    {
                                        if (p3.Space1 == true && p3.Space2 == true && (p2.MooringNumber + 1) == p3.MooringNumber && p3.MooringNumber != 33)
                                        {
                                            foreach (var p4 in harbor)
                                            {
                                                if (p4.Space1 == true && p4.Space2 == true && (p3.MooringNumber + 1) == p4.MooringNumber && p4.MooringNumber != 33)
                                                {
                                                    p.Space1 = false;
                                                    p.Space2 = false;
                                                    p2.Space1 = false;
                                                    p2.Space2 = false;
                                                    p3.Space1 = false;
                                                    p3.Space2 = false;
                                                    p4.Space1 = false;
                                                    p4.Space2 = false;
                                                    p.Boat = i;
                                                    p.Boat2 = i;
                                                    p2.Boat = i;
                                                    p2.Boat2 = i;
                                                    p3.Boat = i;
                                                    p3.Boat2 = i;
                                                    p4.Boat = i;
                                                    p4.Boat2 = i;
                                                    anchoredBoats.Add(i);
                                                    checkIfCargoshipPlaced = true;
                                                    break;
                                                }
                                            }
                                            if (checkIfCargoshipPlaced == true)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (checkIfCargoshipPlaced == true)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (checkIfCargoshipPlaced == true)
                            {
                                break;
                            }
                        }
                    }
                    if (checkIfCargoshipPlaced == false)
                    {
                        rejectedBoats++;
                    }
                }
                if (i.Type == "Catamaran")
                {
                    bool checkIfPlaced = false;
                    foreach (var p in harbor)
                    {
                        if (p.Space1 == true && p.Space2 == true)
                        {
                            foreach (var p2 in harbor)
                            {
                                if (p2.Space1 == true && p2.Space2 == true && (p.MooringNumber + 1) == p2.MooringNumber && p2.MooringNumber != 33)
                                {
                                    foreach (var p3 in harbor)
                                    {
                                        if (p3.Space1 == true && p3.Space2 == true && (p2.MooringNumber + 1) == p3.MooringNumber && p3.MooringNumber != 33)
                                        {
                                            p.Space1 = false;
                                            p.Space2 = false;
                                            p2.Space1 = false;
                                            p2.Space2 = false;
                                            p3.Space1 = false;
                                            p3.Space2 = false;
                                            p.Boat = i;
                                            p.Boat2 = i;
                                            p2.Boat = i;
                                            p2.Boat2 = i;
                                            p3.Boat = i;
                                            p3.Boat2 = i;
                                            anchoredBoats.Add(i);
                                            checkIfPlaced = true;
                                            break;
                                        }
                                    }
                                    if (checkIfPlaced == true)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (checkIfPlaced == true)
                            {
                                break;
                            }
                        }
                    }
                    if (checkIfPlaced == false)
                    {
                        rejectedBoats++;
                    }
                }
            }
            return harbor;
        }

        //Create new boats
        private static List<Boat> CreateBoats(Random rnd, int numberOfBoatsIncoming)
        {
            List<Boat> incomingBoats = new List<Boat>();

            for (int i = 0; i < numberOfBoatsIncoming; i++)
            {
                int rnd2 = Random(rnd);
                if (rnd2 == 0)
                {
                    incomingBoats.Add(new Rowboat("Rowboat", ("R-" + RandomString(rnd)), RndBoatProp(rnd, 100, 301), RndBoatProp(rnd, 0, 6), 1, $"Number of passengers allowed: {RndBoatProp(rnd, 1, 5)}", 1));
                }
                if (rnd2 == 1)
                {
                    incomingBoats.Add(new Motorboat("Motorboat", ("M-" + RandomString(rnd)), RndBoatProp(rnd, 200, 3001), RndBoatProp(rnd, 0, 111), 2, $"{RndBoatProp(rnd, 10, 1001)} hp", 3));
                }
                if (rnd2 == 2)
                {
                    incomingBoats.Add(new Sailboat("Sailboat", ("S-" + RandomString(rnd)), RndBoatProp(rnd, 800, 6001), RndBoatProp(rnd, 0, 22), 4, $"Length: {RndBoatProp(rnd, 3, 18)} m", 4));
                }
                if (rnd2 == 3)
                {
                    incomingBoats.Add(new Cargoship("Cargoship", ("C-" + RandomString(rnd)), RndBoatProp(rnd, 3000, 20001), RndBoatProp(rnd, 0, 37), 8, $"Number of containers: {RndBoatProp(rnd, 0, 500)}", 6));
                }
                if (rnd2 == 4)
                {
                    incomingBoats.Add(new Catamaran("Catamaran", ("K-" + RandomString(rnd)), RndBoatProp(rnd, 1200, 8000), RndBoatProp(rnd, 0, 22), 6, $"Number of beds: {RndBoatProp(rnd, 1, 5)}", 3));
                }
            }
            return incomingBoats;
        }

        //Create berths/mooringlocations
        static BindingList<Harbor> CreateMoorings(BindingList<Harbor> harbor)
        {
            int nr = 1;
            List<Harbor> harbor2 = new List<Harbor>();
            for (int i = 0; i < 64; i++)

            {
                harbor2.Add(new Harbor(nr, true, true, new Rowboat("-", "-", 0, 0, 0, "-", 0), new Rowboat("-", "-", 0, 0, 0, "-", 0)));
                nr++;
            }
            foreach (var i in harbor2)
            {
                harbor.Add(i);
            }
            return harbor;
        }

        //Create random boattype
        static int Random(Random rnd)
        {
            int t = rnd.Next(0, 5);
            return t;
        }

        //Random prop for new boats
        static int RndBoatProp(Random rnd, int min, int max)
        {
            int t = rnd.Next(min, max);
            return t;
        }

        //Create IDs for new boats
        static string RandomString(Random rnd)
        {
            int length = 3;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }

        //Calculate free spots in the harbor
        static double CalcFreeSpace(BindingList<Harbor> harbor)
        {
            double freeSpace = 0;
            foreach (var s in harbor)
            {
                if (s.Space1 == true && s.Space2 == true)
                {
                    freeSpace++;
                }
                else if (s.Space1 == true && s.Space2 == false)
                {
                    freeSpace += 0.5;
                }
                else if (s.Space1 == false && s.Space2 == true)
                {
                    freeSpace += 0.5;
                }
            }

            return freeSpace;
        }
        //Calculate total weight of boats in the harbor
        static int CalcWeight(BindingList<Harbor> harbor)
        {
            int totalWeight = 0;
            foreach (var b in harbor)
            {
                if (b.Space1 == false)
                {
                    if (b.Boat.Type == "Rowboat")
                    {
                        totalWeight += b.Boat.Weight;
                    }
                    else if (b.Boat.Type == "Motorboat")
                    {
                        totalWeight += b.Boat.Weight;
                    }
                    else if (b.Boat.Type == "Sailboat")
                    {
                        totalWeight += (b.Boat.Weight / 2);
                    }
                    else if (b.Boat.Type == "Cargoship")
                    {
                        totalWeight += (b.Boat.Weight / 4);
                    }
                    else if (b.Boat.Type == "Catamaran")
                    {
                        totalWeight += (b.Boat.Weight / 3);
                    }
                }
                if (b.Space2 == false)
                {
                    if (b.Boat2.Type == "RowBoat")
                    {
                        totalWeight += b.Boat2.Weight;
                    }
                }
            }
            return totalWeight;
        }

        //Calculate average max speed:
        static int CalcSpeed(BindingList<Harbor> harbor)
        {
            int maxSpeed = 0;
            int ticker = 0;
            foreach (var b in harbor)
            {
                if (b.Space1 == false)
                {
                    maxSpeed += b.Boat.MaxSpeed;
                    ticker++;
                }
                if (b.Space2 == false)
                {
                    if (b.Boat.Type == "Rowboat")
                    {
                        maxSpeed += b.Boat2.MaxSpeed;
                        ticker++;
                    }
                }
            }

            maxSpeed /= ticker;
            int averageMaxSpeed = maxSpeed;
            return averageMaxSpeed;
        }
        //Calculate number of rowboats in the harbor
        static int CalcNumOfRowboats(BindingList<Harbor> harbor)
        {
            int numberOfRB = 0;
            foreach (var h in harbor)
            {
                if (h.Boat.Type == "Rowboat")
                {
                    numberOfRB++;
                }
                if (h.Boat2.Type == "Rowboat")
                {
                    numberOfRB++;
                }
            }
            return numberOfRB;
        }
        //Calculate number of motorboats in the harbor
        static int CalcNumOfMotorboats(BindingList<Harbor> harbor)
        {
            int numberOfMB = 0;
            foreach (var h in harbor)
            {
                if (h.Boat.Type == "Motorboat")
                {
                    numberOfMB++;
                }
            }
            return numberOfMB;
        }
        //Calculate number of sailboats in the harbor
        static int CalcNumOfSailboat(BindingList<Harbor> harbor)
        {
            int numberOfSB = 0;
            foreach (var h in harbor)
            {
                if (h.Boat.Type == "Sailboat")
                {
                    numberOfSB++;
                }
            }
            return (numberOfSB / 2);
        }
        //Calculate number of cargoships in the harbor
        static int CalcNumOfCargoship(BindingList<Harbor> harbor)
        {
            int numberOfCS = 0;
            foreach (var h in harbor)
            {
                if (h.Boat.Type == "Cargoship")
                {
                    numberOfCS++;
                }
            }
            return (numberOfCS / 4);
        }

        //Calculate number of catamarans in the harbor
        static int CalcNumOfCatamaran(BindingList<Harbor> harbor)
        {
            int numberOfK = 0;
            foreach (var h in harbor)
            {
                if (h.Boat.Type == "Catamaran")
                {
                    numberOfK++;
                }
            }
            return (numberOfK / 3);
        }

        //Save boats in txtfile:
        static void SaveBoats(BindingList<Harbor> harbor)
        {
            int savedBoatsCounter2 = 0;
            StreamWriter tw = new StreamWriter("Saved.txt");
            foreach (var b in harbor)
            {
                if (b.MooringNumber == 64)
                {
                    tw.Write($"{b.MooringNumber},{b.Space1},{b.Space2},{b.Boat.Type},{b.Boat.ID},{b.Boat.Weight},{b.Boat.MaxSpeed},{b.Boat.SpecialProp},{b.Boat.Stays}");
                    savedBoatsCounter2++;
                }
                else if (b.MooringNumber < 64)
                {
                    tw.WriteLine($"{b.MooringNumber},{b.Space1},{b.Space2},{b.Boat.Type},{b.Boat.ID},{b.Boat.Weight},{b.Boat.MaxSpeed},{b.Boat.SpecialProp},{b.Boat.Stays}");
                    savedBoatsCounter2++;
                }
            }
            tw.Close();
        }
        // Add saved boats from txtfile:
        static void AddSavedBoats(BindingList<Harbor> harbor, string[,] savedBoats, List<Boat> anchoredboats)
        {
            if ((File.Exists("Saved.txt")))
            {
                int i = 0;
                int j;
                string text2 = File.ReadAllText("Saved.txt");

                foreach (var row in text2.Split('\n'))
                {
                    j = 0;
                    foreach (var col in row.Split(','))
                    {
                        savedBoats[i, j] = col;
                        j++;
                    }
                    i++;
                }

                foreach (var h in harbor.ToList())
                {
                    for (int t = 0; t <= 64 - 1; t++)
                    {
                        if (h.MooringNumber == Convert.ToInt32(savedBoats[t, 0]) && savedBoats[t, 3] == "Motorboat")
                        {
                            h.MooringNumber = int.Parse(savedBoats[t, 0]);
                            h.Space1 = bool.Parse(savedBoats[t, 1]);
                            h.Space2 = bool.Parse(savedBoats[t, 2]);
                            h.Boat = new Motorboat("Motorboat", savedBoats[t, 4], int.Parse(savedBoats[t, 5]), int.Parse(savedBoats[t, 6]), 2, savedBoats[t, 7], int.Parse(savedBoats[t, 8]));
                            h.Boat2 = new Motorboat("Motorboat", savedBoats[t, 4], int.Parse(savedBoats[t, 5]), int.Parse(savedBoats[t, 6]), 2, savedBoats[t, 7], int.Parse(savedBoats[t, 8]));
                            anchoredboats.Add(h.Boat);
                            anchoredboats.Add(h.Boat2);
                        }
                        else if (h.MooringNumber == Convert.ToInt32(savedBoats[t, 0]) && savedBoats[t, 3] == "Sailboat")
                        {
                            h.MooringNumber = int.Parse(savedBoats[t, 0]);
                            h.Space1 = bool.Parse(savedBoats[t, 1]);
                            h.Space2 = bool.Parse(savedBoats[t, 2]);
                            h.Boat = new Sailboat("Sailboat", savedBoats[t, 4], int.Parse(savedBoats[t, 5]), int.Parse(savedBoats[t, 6]), 2, savedBoats[t, 7], int.Parse(savedBoats[t, 8]));
                            h.Boat2 = new Sailboat("Sailboat", savedBoats[t, 4], int.Parse(savedBoats[t, 5]), int.Parse(savedBoats[t, 6]), 2, savedBoats[t, 7], int.Parse(savedBoats[t, 8]));
                            anchoredboats.Add(h.Boat);
                            anchoredboats.Add(h.Boat2);
                        }
                        else if (h.MooringNumber == Convert.ToInt32(savedBoats[t, 0]) && savedBoats[t, 3] == "Cargoship")
                        {
                            h.MooringNumber = int.Parse(savedBoats[t, 0]);
                            h.Space1 = bool.Parse(savedBoats[t, 1]);
                            h.Space2 = bool.Parse(savedBoats[t, 2]);
                            h.Boat = new Cargoship("Cargoship", savedBoats[t, 4], int.Parse(savedBoats[t, 5]), int.Parse(savedBoats[t, 6]), 2, savedBoats[t, 7], int.Parse(savedBoats[t, 8]));
                            h.Boat2 = new Cargoship("Cargoship", savedBoats[t, 4], int.Parse(savedBoats[t, 5]), int.Parse(savedBoats[t, 6]), 2, savedBoats[t, 7], int.Parse(savedBoats[t, 8]));
                            anchoredboats.Add(h.Boat2);
                        }
                        else if (h.MooringNumber == Convert.ToInt32(savedBoats[t, 0]) && savedBoats[t, 3] == "Catamaran")
                        {
                            h.MooringNumber = int.Parse(savedBoats[t, 0]);
                            h.Space1 = bool.Parse(savedBoats[t, 1]);
                            h.Space2 = bool.Parse(savedBoats[t, 2]);
                            h.Boat = new Catamaran("Catamaran", savedBoats[t, 4], int.Parse(savedBoats[t, 5]), int.Parse(savedBoats[t, 6]), 2, savedBoats[t, 7], int.Parse(savedBoats[t, 8]));
                            h.Boat2 = new Catamaran("Catamaran", savedBoats[t, 4], int.Parse(savedBoats[t, 5]), int.Parse(savedBoats[t, 6]), 2, savedBoats[t, 7], int.Parse(savedBoats[t, 8]));
                            anchoredboats.Add(h.Boat);
                            anchoredboats.Add(h.Boat2);
                        }
                    }

                }
            }
        }

        // Create better layout for list *Not working*
        /*private BindingList<Harbor> ChangeLayoutOfListOfHarbor(BindingList<Harbor> harbor)
        {
            BindingList<Harbor> harborChangedLayout = new BindingList<Harbor>();
            foreach (var h in harbor)
            {
                harborChangedLayout.Add(h);
            }

            foreach (var h in harborChangedLayout)
            {
                if (h.Boat2.Type == "Motorboat")
                {
                    h.Boat2 = new Motorboat("Motorboat", "-", 0, 0, 2, "-", h.Boat.Stays);
                }
                else if (h.Boat2.Type == "Sailboat")
                {
                    h.Boat2 = new Sailboat("Sailboat", "-", 0, 0, 4, "-", h.Boat.Stays);
                }
                else if (h.Boat2.Type == "Cargoship")
                {
                    h.Boat2 = new Cargoship("Cargoship", "-", 0, 0, 8, "-", h.Boat.Stays);
                }
                else if (h.Boat2.Type == "Catamaran")
                {
                    h.Boat2 = new Catamaran("Catamaran", "-", 0, 0, 6, "-", h.Boat.Stays);
                }
            }
            return harborChangedLayout;
        }*/
    }
}
