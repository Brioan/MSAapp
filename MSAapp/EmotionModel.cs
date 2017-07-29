using System;
using Newtonsoft.Json;

namespace MSAapp
{
	public class MSAemotiontable
	{
		[JsonProperty(PropertyName = "Id")]
		public string ID { get; set; }

		




		[JsonProperty(PropertyName = "PhotoID")]
        public int PhotoID { get; set; }

		[JsonProperty(PropertyName = "Emotion")]
		public string Emotion { get; set; }

		[JsonProperty(PropertyName = "Score")]
		public float Score { get; set; }

	}
}
