using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project_Auction_House_Client
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Login(object sender, RoutedEventArgs e)
		{
			string ip = IPTextBox.Text;

			Regex regex = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
			MatchCollection matches = regex.Matches(ip);
			if(matches.Count > 0)
			{
				foreach (Match match in matches)
				{
					ip = match.Value;
					Client c = new Client();
					if (c.Connect(ip))
					{
						c.Show();
						c.Start();
						this.Close();
						break;
					}
				}
			}else
			{
				MessageBox.Show("Wrong IP");
			}
		}
	}
}
