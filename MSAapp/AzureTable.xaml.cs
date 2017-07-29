using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using System.Threading.Tasks;
    

namespace MSAapp
{
    public partial class AzureTable : ContentPage
    {

		public AzureTable()
        {
            InitializeComponent();

        }

		async void Handle_ClickedAsync(object sender, System.EventArgs e)
		{
            List<MSAemotiontable> emotionInformation = await AzureManager.AzureManagerInstance.GetEmotionInfomation();

            EmotionList.ItemsSource = emotionInformation;
		}

    }
}
