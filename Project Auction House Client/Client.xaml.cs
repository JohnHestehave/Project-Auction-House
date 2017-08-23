using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Project_Auction_House_Client {
    /// <summary>
    /// Interaction logic for Client.xaml
    /// </summary>
    public partial class Client:Window {

        TcpClient server;
        NetworkStream stream;
        StreamReader sr;
        StreamWriter sw;
        public Client() {
            InitializeComponent();
        }

        public bool Connect(string ip) {
            try {
                server = new TcpClient(ip, 12345);
                stream = server.GetStream();
                sr = new StreamReader(stream);
                sw = new StreamWriter(stream);
                sw.AutoFlush = true;
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public void Start() {
            Thread thread = new Thread(Loop);
            thread.Start();
        }

        public delegate void UpdateText(string message);
        public void UpdateText2(string message) {
            ServerText.Text = "\n" + message;
        }
        public void Loop() {
            while (true) {
                string message = sr.ReadLine();

                if (message != "")
                {
                    ServerText.Dispatcher.Invoke(new UpdateText(UpdateText2), message);
                }
                continue;
                switch (message) {
                    case "test":
                        ServerText.Text += "\n" + "Test received." + message;
                        break;
                    default:
                        ServerText.Text += "\n" + message;
                        break;
                }
                if (!server.Connected) {
                    ServerText.Text += "\n" + "Disconnected.";
                    break;
                }
            }
        }
    }
}
