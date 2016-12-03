using Android.App;
using Android.Widget;
using Android.OS;
using System.Net.Sockets;
using System;
using System.Text;
using System.Threading;
using Android.Content;
using Android.Views;

namespace Nimbuzz_Xamarin_Android_Db
{

	[Activity(Label = "Nimbuzz Xamarin Android By Db~@NC", MainLauncher = true, Icon = "@mipmap/icon")]

	public class MainActivity : Activity
	{
		//	Make Socket Universal throughout App (Make Use socket anywhere in any Activity of this App)
		public static TcpClient client;
		public static string passid;



		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			Button btnSignIn = FindViewById<Button>(Resource.Id.btnSignin);

			btnSignIn.Click += delegate
			{
				login();
			};
		}

		void login()
		{
			//	Get UserName and PassWord From User
			EditText userName = FindViewById<EditText>(Resource.Id.txtUser);
			EditText userPwd = FindViewById<EditText>(Resource.Id.txtPwd);
			var usrid = userName.Text;
			var usrpwd = userPwd.Text;



			//	Initialize New TcpClient Connection
			try
			{
				client = new TcpClient() { NoDelay = true };
				client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

				client.Client.Connect("o.nimbuzz.com", 5222);
				Toast.MakeText(this, "Connecting...", ToastLength.Short).Show();
				NetworkStream stream = client.GetStream();

				if (client.Client.Connected)
				{
					client.Client.Send(Encoding.UTF8.GetBytes("<stream:stream xmlns='jabber:client' to='nimbuzz.com' version='1.0' xmlns:stream='http://etherx.jabber.org/streams' xml:lang='en' >"));
					string str = Convert.ToBase64String(Encoding.UTF8.GetBytes("\0" + usrid + "\0" + usrpwd));
					client.Client.Send(Encoding.UTF8.GetBytes("<auth id='sasl2' xmlns='urn:ietf:params:xml:ns:xmpp-sasl' mechanism='PLAIN'>" + str + "</auth>"));

				}

				//	Start Thread On Successfull Connection
				Thread rec = new Thread(new ThreadStart(dbThread));
				rec.IsBackground = true;
				rec.Start();

			}

			catch (SocketException ex) { 
			}



		}


		void dbThread()
		{
			
				try
				{

					while (client.Connected)
					{

						byte[] buffbyte = new byte[client.ReceiveBufferSize];

						int i = client.Client.Receive(buffbyte);
						if (i != 0)
						{
							if (i < buffbyte.Length)
							{
								byte[] buffbyte2 = new byte[i];
								for (int b = 0; b < i; b++)
								{
									buffbyte2[b] = buffbyte[b];
								}
								buffbyte = buffbyte2;
							}

							//	Handles the incoming data received from Server side
							string xml = Encoding.UTF8.GetString(buffbyte);
							DataArrival(xml);

							//	Ping Server every 60 seconds to avoiding auto-disconnect

							int startin = 60 - DateTime.Now.Second;
							var t = new System.Threading.Timer(o => client.Client.Send(Encoding.UTF8.GetBytes("<iq to='nimbuzz.com' type='get' id='db1_a2'><ping xmlns='urn:xmpp:ping' /></iq>")), null, startin * 1000, 60000);
							

					}

				}

				}
				catch (Exception ex)
				{
					//Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
				}

			
		}

		void DataArrival(string xml)
		{
			xml = xml.Replace("\"", "'");
				
			if (xml.IndexOf("<success xmlns='urn:ietf:params:xml:ns:xmpp-sasl'/>", System.StringComparison.OrdinalIgnoreCase) + 1 != 0)
			{
				//	Register Client
				//	Bind Resource
				//	Open session
				client.Client.Send(Encoding.UTF8.GetBytes("<stream:stream xmlns='jabber:client' to='nimbuzz.com' version='1.0' xmlns:stream='http://etherx.jabber.org/streams' xml:lang='en' >"));
				client.Client.Send(Encoding.UTF8.GetBytes("<iq type='set'><bind xmlns='urn:ietf:params:xml:ns:xmpp-bind'><resource>dbhere</resource></bind></iq>"));
				client.Client.Send(Encoding.UTF8.GetBytes("<iq id='jcl_2' type='set'><session xmlns='urn:ietf:params:xml:ns:xmpp-session'/></iq>"));

			}

			if (xml.IndexOf("<bind xmlns='urn:ietf:params:xml:ns:xmpp-bind'>", System.StringComparison.OrdinalIgnoreCase) + 1 != 0)
			{
				//Toast.MakeText(this, "Logged in Successfully!", ToastLength.Short).Show();

				client.Client.Send(Encoding.UTF8.GetBytes("<presence><status>Online Via Xamarin C# (Android)</status><priority>1</priority><show>Online</show><nick xmlns='http://jabber.org/protocol/nick'>XamarinDb</nick></presence>"));

				EditText userName = FindViewById<EditText>(Resource.Id.txtUser);
				passid = userName.Text;

				var LoggedInt = new Intent(this, typeof(LoggedActivity));
				LoggedInt.PutExtra("usrid", passid);			//pass value to Logged Activity
				StartActivity(LoggedInt);

			}

			if (xml.Contains("<failure xmlns="))
			{
				//Toast.MakeText(this, "Invalid Username & Password", ToastLength.Short).Show();
				client.Client.Close();		// close socket

			}
		}



		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			base.OnCreateOptionsMenu(menu);
			// Inflate the menu; this adds items to the action bar if it is present.
			MenuInflater inflater = this.MenuInflater;
			inflater.Inflate(Resource.Menu.menuoption, menu);

			return true;
		}


		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			base.OnOptionsItemSelected(item);
			switch (item.ItemId) {

				case Resource.Id.about:
					aboutDialog();
					break;
				case Resource.Id.exit:
					Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
					break;
				default:
					break;	
					
			}

			return true;

		}

		void aboutDialog()
		{
			/*
			var myalert = new AlertDialog.Builder(this);

			myalert.SetTitle("About App");
			myalert.SetMessage("This is Xamarin Android App Coded in C# based on Socket Programming");

			myalert.SetNeutralButton("OK", (senderAlert, args) =>
			{
				this.Dispose();

			});

			Dialog dialog = myalert.Create();
			dialog.Show();
			**/

			Toast.MakeText(this, "Xamarin Android Coded By Db~@NC", ToastLength.Long).Show();
		}
	}

}

