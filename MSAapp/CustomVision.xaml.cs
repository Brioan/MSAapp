using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System.Collections.Generic;


namespace MSAapp
{
	public partial class CustomVision : ContentPage{
		public CustomVision(){
			InitializeComponent();
		}

		private async void loadCamera(object sender, EventArgs e){
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported){
				await DisplayAlert("No Camera", ":( No camera available.", "OK");
				return;
			}

			MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions{
				PhotoSize = PhotoSize.Medium,
				Directory = "Sample",
				Name = $"{DateTime.UtcNow}.jpg"
			});

			if (file == null)
				return;

			image.Source = ImageSource.FromStream(() =>{
				return file.GetStream();
			});

			await MakePredictionRequest(file);
            await PostEmotion();
		}

		async Task PostEmotion()
		{
			MSAemotiontable model = new MSAemotiontable()
			{
				PhotoID = 1,
                Emotion = Expression.Text,
                Score = System.Convert.ToSingle(Score.Text)
			};
			await AzureManager.AzureManagerInstance.PostEmotionInformation(model);

		}

		static byte[] GetImageAsByteArray(MediaFile file){
			var stream = file.GetStream();
			BinaryReader binaryReader = new BinaryReader(stream);
			return binaryReader.ReadBytes((int)stream.Length);
		}






		async Task MakePredictionRequest(MediaFile file){
            TagLabel.Text = "Analysing . . .";
			var client = new HttpClient();
			EmotionServiceClient emotionServiceClient = new EmotionServiceClient("5276279029f34e1da89500eb46fe0ac9"); // ***
			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "5276279029f34e1da89500eb46fe0ac9");
			string url = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize";
			HttpResponseMessage response;
			byte[] byteData = GetImageAsByteArray(file);

			using (var content = new ByteArrayContent(byteData)){
				content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				response = await client.PostAsync(url, content);

				/*
                if (response.IsSuccessStatusCode)
                {
                var responseString = await response.Content.ReadAsStringAsync();
                MSAapp.Model.EvaluationModel responseModel = JsonConvert.DeserializeObject<MSAapp.Model.EvaluationModel>(responseString);
                double max = responseModel.Predictions.Max(m => m.Probability);
                TagLabel.Text = (max >= 0.5) ? "Drink" : "Not drink";
                }
                */

				/*
                string emotionString = await response.Content.ReadAsStringAsync();
                emotionResult = await emotionServiceClient.RecognizeAsync(System.Convert.ToString(emotionString));
                TagLabel.Text = System.Convert.ToString(emotionResult.Max()); ;
                */


				if (response.IsSuccessStatusCode)
				{
					var responseString = await response.Content.ReadAsStringAsync();
                    List<EvaluationModel> faceObjs = JsonConvert.DeserializeObject<List<EvaluationModel>>(responseString);
                    EvaluationModel emotion = faceObjs[0];

					double counter = 0.0;
					string maxi = "null";
                    if (System.Convert.ToDouble(emotion.scores.anger) > counter)
					{
						maxi = "Anger";
                        counter = System.Convert.ToDouble(emotion.scores.anger);
					}
                    if (System.Convert.ToDouble(emotion.scores.contempt) > counter)
					{
						maxi = "Contempt";
                        counter = System.Convert.ToDouble(emotion.scores.contempt);
					}
                    if (System.Convert.ToDouble(emotion.scores.disgust) > counter)
					{
						maxi = "Disgust";
                        counter = System.Convert.ToDouble(emotion.scores.disgust);
					}
                    if (System.Convert.ToDouble(emotion.scores.fear) > counter)
					{
						maxi = "Fear";
                        counter = System.Convert.ToDouble(emotion.scores.fear);
					}
                    if (System.Convert.ToDouble(emotion.scores.happiness) > counter)
					{
						maxi = "Happiness";
                        counter = System.Convert.ToDouble(emotion.scores.happiness);
					}
                    if (System.Convert.ToDouble(emotion.scores.neutral) > counter)
					{
						maxi = "Neutral";
                        counter = System.Convert.ToDouble(emotion.scores.neutral);
					}
                    if (System.Convert.ToDouble(emotion.scores.sadness) > counter)
					{
						maxi = "Sadness";
                        counter = System.Convert.ToDouble(emotion.scores.sadness);
					}
                    if (System.Convert.ToDouble(emotion.scores.surprise) > counter)
					{
						maxi = "Surprise";
						counter = System.Convert.ToDouble(emotion.scores.surprise);
					}

                    /*
                    string[] expressions = { "anger", "contempt", "disgust", "fear", "happiness", "neutral", "sadness", "surprise" };
                    foreach(string i in expressions){
                        
                        if (emotion.scores. > counter){
                            
                        }
                    }
                    */

                    TagLabel.Text = "Facial expression: ";
                    Expression.Text = maxi;
                    ScoreLabel.Text = "Score: ";
                    Score.Text = System.Convert.ToString(counter);
                }
            file.Dispose();
			}
		}

		public class EvaluationModel{
			public FaceRectangle faceRectangles { get; set; }
			public Scores scores { get; set; }
		}

		public class FaceRectangle{
			public int left { get; set; }
			public int top { get; set; }
			public int width { get; set; }
			public int height { get; set; }
		}

		public class Scores{
			public double anger { get; set; }
			public double contempt { get; set; }
			public double disgust { get; set; }
			public double fear { get; set; }
			public double happiness { get; set; }
			public double neutral { get; set; }
			public double sadness { get; set; }
			public double surprise { get; set; }
		}

	}

}