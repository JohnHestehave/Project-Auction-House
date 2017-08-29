using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class Client:INotifyPropertyChanged {

        TcpClient client;
        NetworkStream stream;
        StreamReader sr;
        StreamWriter sw;

        private bool running = true;

        private string _members;
        public string Members {
            get { return _members; }
            set {
                if (_members != value) {
                    _members = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _itemName;
        public string ItemName {
            get { return _itemName; }
            set {
                if (_itemName != value) {
                    _itemName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _highBid;
        public string HighBid {
            get { return _highBid; }
            set {
                if (_highBid != value) {
                    _highBid = value;
                    OnPropertyChanged();
                }
            }
        }

        public Client() {
            DataContext = this;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Connect(string ip) {
            try {
                client = new TcpClient(ip, 12345);
                stream = client.GetStream();
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
            string[] messages = message.Split('¤');

            // IF IP'S
            Regex regex = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
            MatchCollection matches = regex.Matches(messages[0]);
            if (matches.Count > 0) {
                foreach (Match match in matches) {
                    messages[0] = match.Value;

                    Members = "";
                    foreach (var _message in messages) {
                        Members += _message + "\n";
                    }
                }
            } else {
                HighBid = messages[0];
                try {
                    ItemName = messages[1];
                } catch (Exception) {
                }
            }
        }
        public void Loop() {
            while (running) {
                string message = sr.ReadLine();
                if (message != "") {
                    ServerText.Dispatcher.Invoke(new UpdateText(UpdateText2), message);
                }
            }
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e) {
            sw.Write("EXIT");
            running = false;
            Environment.Exit(1);
        }

        private void BidButton_Click(object sender, RoutedEventArgs e) {
            string bid = BidTextBox.Text;
            sw.Write(bid);
        }
    }
}
