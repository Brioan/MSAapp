using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;


namespace MSAapp
{
	public class AzureManager
	{

		private static AzureManager instance;
		private MobileServiceClient client;
        private IMobileServiceTable<MSAemotiontable> MSAemotiontableinfo;


		private AzureManager()
		{
			this.client = new MobileServiceClient("http://brianlinmobileapp.azurewebsites.net");
            this.MSAemotiontableinfo = this.client.GetTable<MSAemotiontable>();

		}

		public MobileServiceClient AzureClient
		{
			get { return client; }
		}

		public static AzureManager AzureManagerInstance
		{
			get
			{
				if (instance == null)
				{
					instance = new AzureManager();
				}

				return instance;
			}
		}

        public async Task<List<MSAemotiontable>> GetEmotionInfomation()
		{
            return await this.MSAemotiontableinfo.ToListAsync();
		}

		public async Task PostEmotionInformation(MSAemotiontable MSAemotionmodel)
		{
			await this.MSAemotiontableinfo.InsertAsync(MSAemotionmodel);
		}
	}
}
