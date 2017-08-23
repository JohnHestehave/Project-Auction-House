namespace Project_Auction_House_Server {
    public class Item {
        public string name;
        public int startPrice;
        public string description;

        public Item(string v1, int v2, string v3) {
            name = v1;
            startPrice = v2;
            description = v3;
        }
    }
}