using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace TheHarborWPF
{

    class Harbor : INotifyPropertyChanged
    {
        public int mooringNumber;
        public int MooringNumber
        {
            get
            {
                return mooringNumber;
            }
            set
            {
                if (mooringNumber != value)
                {
                    mooringNumber = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool space1;
        public bool Space1
        {
            get
            {
                return space1;
            }
            set
            {
                if (space1 != value)
                {
                    space1 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool space2;
        public bool Space2
        {
            get
            {
                return space2;
            }
            set
            {
                if (space2 != value)
                {
                    space2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Boat boat;
        public Boat Boat
        {
            get
            {
                return boat;
            }
            set
            {
                if (boat != value)
                {
                    boat = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public Boat boat2;
        public Boat Boat2
        {
            get
            {
                return boat2;
            }
            set
            {
                if (boat2 != value)
                {
                    boat2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Harbor(int mooringNumber, bool space1, bool space2, Boat boat, Boat boat2)
        {
            MooringNumber = mooringNumber;
            Space1 = space1;
            Space2 = space2;
            Boat = boat;
            Boat2 = boat2;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }
    }
    class Boat
    {
        public string Type { get; set; }
        public string ID { get; set; }
        public int Weight { get; set; }
        public int MaxSpeed { get; set; }
        public double Size { get; set; }
        public string SpecialProp { get; set; }
        public int Stays { get; set; }
    }

    class Rowboat : Boat
    {
        public Rowboat(string type, string id, int weight, int maxSpeed, int size, string specialProp, int stays)
        {
            Type = type;
            ID = id;
            Weight = weight;
            MaxSpeed = maxSpeed;
            Size = size;
            SpecialProp = specialProp;
            Stays = stays;
        }

    }

    class Motorboat : Boat
    {
        public Motorboat(string type, string id, int weight, int maxSpeed, int size, string specialProp, int stays)
        {
            Type = type;
            ID = id;
            Weight = weight;
            MaxSpeed = maxSpeed;
            Size = size;
            SpecialProp = specialProp;
            Stays = stays;
        }
    }

    class Sailboat : Boat
    {
        public Sailboat(string type, string id, int weight, int maxSpeed, int size, string specialProp, int stays)
        {
            Type = type;
            ID = id;
            Weight = weight;
            MaxSpeed = maxSpeed;
            Size = size;
            SpecialProp = specialProp;
            Stays = stays;
        }
    }

    class Cargoship : Boat
    {
        public Cargoship(string type, string id, int weight, int maxSpeed, int size, string specialProp, int stays)
        {
            Type = type;
            ID = id;
            Weight = weight;
            MaxSpeed = maxSpeed;
            Size = size;
            SpecialProp = specialProp;
            Stays = stays;
        }
    }
    class Catamaran : Boat
    {
        public Catamaran(string type, string id, int weight, int maxSpeed, int size, string specialProp, int stays)
        {
            Type = type;
            ID = id;
            Weight = weight;
            MaxSpeed = maxSpeed;
            Size = size;
            SpecialProp = specialProp;
            Stays = stays;
        }
    }
}
