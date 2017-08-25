namespace Accountability
{
    class Airman
    {
        private string name;
        private string room;
        private string mtl;
        private bool inHouse;
        private string shift;
        private string barcode;

        public Airman(string name, string room, string mtl, string barcode, string shift, bool inHouse)
        {
            this.name = name;
            this.room = room;
            this.mtl = mtl;
            this.inHouse = inHouse;
            this.shift = shift;
            this.barcode = barcode;
        }
        
        public string Name { get => name; set => name = value; }
        public string Room { get => room; set => room = value; }
        public string Mtl { get => mtl; set => mtl = value; }
        public bool InHouse { get => inHouse; set => inHouse = value; }
        public string Barcode { get => barcode; set => barcode = value; }
        public string Shift { get => shift; set => shift = value; }
    }
}
