using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Project_Auction_House_Server {
    public class ClientHandler {
        TcpClient client;
        StreamReader read;
        StreamWriter write;
        IPEndPoint IPEP;
        

        public ClientHandler(TcpClient client) {
            this.client = client;
            read = new StreamReader(client.GetStream());
            write = new StreamWriter(client.GetStream());
            write.AutoFlush = true;

            IPEP = (IPEndPoint)client.Client.RemoteEndPoint;
        }

        internal IPEndPoint IP() {
            return IPEP;
        }
        internal StreamWriter Writer() {
            return write;
        }

        public void SendFTInfo() {
            Console.WriteLine("Sending Current Auction to New Client");
            if (Repo.WinningClient != null) {
                write.WriteLine("ITEM¤"
                + Repo.Bid.ToString() + "¤"
                + Repo.Items[Repo.AuctionItem].name.ToString() + "¤"
                + Repo.Items[Repo.AuctionItem].description.ToString() + "¤"
                + Repo.WinningClient.IP());
            } else {
                write.WriteLine("ITEM¤"
                + Repo.Bid.ToString() + "¤"
                + Repo.Items[Repo.AuctionItem].name.ToString() + "¤"
                + Repo.Items[Repo.AuctionItem].description.ToString() + "¤"
                + "No one is currently Winning");
            }
        }

        private void ShowBidders() {
            string message = "IP¤";
            foreach (var client in Repo.Clients) {
                message += client.IP().ToString() + "¤";
            }

            foreach (var client in Repo.Clients) {
                client.Writer().WriteLine(message);
            }
        }
        internal void Handle() {
            ShowBidders();
            SendFTInfo();

            while (true) {
                try {
                    string message = read.ReadLine();
                    bool conversion;
                    int BidAttempt = 0;

                    if (message != "" && message != "EXIT") {
                        Repo.BidLog.Add(message);
                        conversion = int.TryParse(message, out BidAttempt);
                    }
                    Repo.Lock.WaitOne();
                    if (BidAttempt < 0) {
                        write.WriteLine("ERROR¤" + "Can't Bid under Zero!");
                    } else if (BidAttempt <= Repo.Bid) {
                        int minimumBid = Repo.Bid + 1;
                        write.WriteLine("ERROR¤" + "Too low of a Bid, Needs to be atleast " + minimumBid);
                    } else if (BidAttempt > Repo.Bid) {
                        Repo.WinningClient = this;
                        Repo.Bid = BidAttempt;
                        if (AuctionTimer.Timer < AuctionTimer.LastChance)
                        {
                            AuctionTimer.Timer = AuctionTimer.LastChance;
                        }
                        write.WriteLine("MESSAGE¤" + "You Are the Highest Bidder");
                    }
                    Repo.Lock.ReleaseMutex();
                    if (!client.Connected || message == "EXIT") {
                        Repo.Clients.Remove(this);
                        Console.WriteLine(IPEP + " - Disconnected");
                        Console.WriteLine(Repo.Clients.Count + " Client(s) Connected");
                        break;
                    }
                } catch (Exception e) {
                    if (e.GetType() == typeof(IOException)) {
                        client.Close();
                        Repo.Clients.Remove(this);
                        Console.WriteLine(IPEP + " - Terminated");
                        Console.WriteLine(Repo.Clients.Count + " Client(s) Connected");
                        break;
                    }
                }
            }
        }
    }
}
