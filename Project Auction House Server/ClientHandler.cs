using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Project_Auction_House_Server
{
    public class ClientHandler
    {
        TcpClient client;
        StreamReader read;
        StreamWriter write;
        IPEndPoint IPEP;

        public ClientHandler(TcpClient client)
        {
            this.client = client;
            read = new StreamReader(client.GetStream());
            write = new StreamWriter(client.GetStream());
            write.AutoFlush = true;

            IPEP = (IPEndPoint)client.Client.RemoteEndPoint;
        }

        internal IPEndPoint IP()
        {
            return IPEP;
        }
        internal StreamWriter Writer()
        {
            return write;
        }

        private void SendFTInfo()
        {
            Console.WriteLine("Sending Current Auction to New Client");
            if (Repo.WinningClient != null)
            {
                write.WriteLine(Repo.Bid.ToString() + "¤"
                + Repo.Items[Repo.AuctionItem].name.ToString() + "¤"
                + Repo.Items[Repo.AuctionItem].description.ToString() + "¤"
                + Repo.WinningClient.IP());
            }
            else
            {
                write.WriteLine(Repo.Bid.ToString() + "¤"
                + Repo.Items[Repo.AuctionItem].name.ToString() + "¤"
                + Repo.Items[Repo.AuctionItem].description.ToString() + "¤"
                + "No one is currently Winning");
            }
        }

        private void ShowBidders()
        {
            string message = "";
            foreach (var client in Repo.Clients)
            {
                message += client.IP().ToString() + "¤";
            }

            foreach (var client in Repo.Clients)
            {
                client.Writer().WriteLine(message);
            }
        }
        internal void Handle()
        {
            ShowBidders();
            SendFTInfo();

            while (true)
            {
                try
                {
                    string message = read.ReadLine();
                    bool conversion;
                    int BidAttempt = 0;

                    if (message != "" && message != "EXIT")
                    {
                        Repo.BidLog.Add(message);
                        conversion = int.TryParse(message, out BidAttempt);
                    }
                    if (BidAttempt < 0)
                    {
                        write.WriteLine("Can't Bid under Zero!");
                    }
                    else if (BidAttempt <= Repo.Bid)
                    {
                        int minimumBid = Repo.Bid + 1;
                        write.WriteLine("Too low of a Bid, Needs to be atleast " + minimumBid);
                    }
                    else if (BidAttempt > Repo.Bid)
                    {
                        Repo.Bid = BidAttempt;
                        write.WriteLine("You Are the Highest Bidder");
                    }
                    if (!client.Connected || message == "EXIT")
                    {
                        Repo.Clients.Remove(this);
                        Console.WriteLine(IPEP + " - Disconnected");
                        Console.WriteLine(Repo.Clients.Count + " Client(s) Connected");
                        break;
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(IOException))
                    {
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
