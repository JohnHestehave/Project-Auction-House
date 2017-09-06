using System.Threading;

namespace Project_Auction_House_Server {
    public static class AuctionTimer {
        //time in secounds
        public const int Auctionlenght = 10;
        public const int LastChance = 3 + 1;
        public static int Timer = Auctionlenght+1;


        public static void Start() {
            while (true) {
                if (Repo.Clients.Count > 0) {
                    Thread.Sleep(1000);
                    Timer--;
                    if (Timer > 0) {
                        Broadcast();
                    }
                    if (Timer == 0) {
                        Timer = Auctionlenght;
                        Broadcast();
                        if (Repo.AuctionItem < Repo.Items.Count - 1) {

                            Repo.AuctionItem++;
                            Repo.Bid = Repo.Items[Repo.AuctionItem].startPrice;
                            Repo.Clients[0].SendFTInfo();
                            //Start();

                        } else {
                            string message = "MESSAGE";
                            message += "¤" + "All items are sold.";
                            foreach (var client in Repo.Clients) {
                                client.Writer().WriteLine(message);
                            }
                            break;
                        }
                    }
                }
            }
        }

        private static void Broadcast() {
            string message = "TIMER";
            message += "¤" + Timer.ToString();

            foreach (var client in Repo.Clients) {
                client.Writer().WriteLine(message);
            }
        }
    }
}