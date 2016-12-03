
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Nimbuzz_Xamarin_Android_Db
{
	[Activity(Label = "LoggedActivity")]
	public class LoggedActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the layout resource
			SetContentView(Resource.Layout.LoggedLayout);

			TextView jidlbl = FindViewById<TextView>(Resource.Id.textView5);
			TextView jidUser = FindViewById<TextView>(Resource.Id.textView6);

			jidUser.Text = Intent.GetStringExtra("usrid") + "@nimbuzz.com";

			//MainActivity.client.Client.Send(Encoding.UTF8.GetBytes("<message to='db~@nimbuzz.com' type='chat' id='-4587545787'><body>im Online</body><active xmlns ='http://jabber.org/protocol/chatstates' /><request xmlns='urn:xmpp:receipts' /></message>"));

			MainActivity.client.Client.Send(Encoding.UTF8.GetBytes("<presence to='db~@nimbuzz.com' type='subscribe' />"));

			Button logout_btn = FindViewById<Button>(Resource.Id.logoutbtn);

			logout_btn.Click += delegate
			{
				if (MainActivity.client.Client.Connected == true) {
					MainActivity.client.Client.Dispose();
					MainActivity.client.Client.Close();

					//	Throw back to MainACtivity after Logout Connection Closing
					StartActivity(typeof(MainActivity));
				}
			};



		
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			base.OnCreateOptionsMenu(menu);
			// Inflate the menu; this adds items to the action bar if it is present.
			MenuInflater inflater = this.MenuInflater;
			inflater.Inflate(Resource.Menu.loggedmenuoption, menu);

			return true;
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			base.OnOptionsItemSelected(item);
			string strinfo = "You are currently Logged as: " + Intent.GetStringExtra("usrid") + "@nimbuzz.com";

			switch (item.ItemId)
			{

				case Resource.Id.info:
					Toast.MakeText(this, strinfo , ToastLength.Long).Show();
					break;
				case Resource.Id.exit:
					Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
					break;
				default:
					break;

			}

			return true;

		}

	}
}
