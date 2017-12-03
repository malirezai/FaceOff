﻿using System;
using System.IO;
using System.Threading.Tasks;

using Plugin.Media;
using Plugin.Media.Abstractions;

using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;

using Xamarin;
using Xamarin.Forms;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace FaceOff
{
	public class PictureViewModel : BaseViewModel
	{
		#region Constant Fields
		readonly string[] _emotionStrings = { "Anger", "Contempt", "Disgust", "Fear", "Happiness", "Neutral", "Sadness", "Surprise" };
		readonly string[] _emotionStringsForAlertMessage = { "angry", "disrespectful", "disgusted", "scared", "happy", "blank", "sad", "surprised" };

		readonly string[] ErrorMessage = { "No Face Detected", "Error" };
		const string MakeAFaceAlertMessage = "take a selfie looking ";
		const string CalculatingScore = "Analyzing";
		#endregion

		#region Fields
		ImageSource _photo1ImageSource, _photo2ImageSource;
		string _scoreButton1Text, _scoreButton2Text;
		bool _isTakeLeftPhotoButtonEnabled = true;
		bool _isTakeLeftPhotoButtonVisible = true;
		bool _isTakeRightPhotoButtonEnabled = true;
		bool _isTakeRightPhotoButtonVisible = true;
		bool _isResetButtonEnabled;
		string _pageTitle;
		int _emotionNumber;
		bool _isCalculatingPhoto1Score, _isCalculatingPhoto2Score;
		bool _isScore1ButtonEnabled, _isScore2ButtonEnabled, _isScore1ButtonVisable, _isScore2ButtonVisable;
		bool _isPhotoImage1Enabled, _isPhotoImage2Enabled;
		string _photo1Results, _photo2Results;

		public event EventHandler<AlertMessageEventArgs> DisplayEmotionBeforeCameraAlert;
		public event EventHandler<TextEventArgs> DisplayAllEmotionResultsAlert;
		public event EventHandler DisplayMultipleFacesDetectedAlert;
		public event EventHandler DisplayNoCameraAvailableAlert;
		public event EventHandler RevealScoreButton1WithAnimation;
		public event EventHandler RevealScoreButton2WithAnimation;
		public event EventHandler RevealPhotoImage1WithAnimation;
		public event EventHandler RevealPhotoImage2WithAnimation;
		#endregion

		#region Constructors
		public PictureViewModel()
		{
			IsResetButtonEnabled = false;

			SetEmotion();

			TakePhoto1ButtonPressed = new Command(async () =>
			{
				IsTakeRightPhotoButtonEnabled = false;
				IsScore1ButtonEnabled = false;

                Analytics.TrackEvent(MobileCenterConstants.PhotoButton1Tapped);

				if (!(await DisplayPopUpAlertAboutEmotion(1)))
				{
					IsTakeRightPhotoButtonEnabled = true;
					IsScore1ButtonEnabled = true;
					return;
				}

				var imageMediaFile = await GetMediaFileFromCamera("FaceOff", "PhotoImage1");

				if (imageMediaFile == null)
				{
					IsTakeRightPhotoButtonEnabled = true;
					IsScore1ButtonEnabled = true;
					return;
				}
				//Ensure the event is subscribed
				while (RevealPhotoImage1WithAnimation == null)
					await Task.Delay(100);

				OnRevealPhotoImage1WithAnimation();

				Analytics.TrackEvent(MobileCenterConstants.PhotoTaken);

				IsTakeLeftPhotoButtonEnabled = false;
				IsTakeLeftPhotoButtonVisible = false;

				ScoreButton1Text = CalculatingScore;


				Photo1ImageSource = ImageSource.FromStream(() =>
				{
					return GetPhotoStream(imageMediaFile, false);
				});

				IsCalculatingPhoto1Score = true;
				IsResetButtonEnabled = !(IsCalculatingPhoto1Score || IsCalculatingPhoto2Score);

				//Yeild to the UI Thread to ensure the PhotoImageAnimation has completed
				await Task.Delay((int)(AnimationConstants.PhotoImageAninmationTime * 2.5));
				//Ensure the event is subscribed
				while (RevealScoreButton1WithAnimation == null)
					await Task.Delay(100);

				OnRevealScoreButton1WithAnimation();

				var emotionArray = await GetEmotionResultsFromMediaFile(imageMediaFile, false);

				var emotionScore = GetPhotoEmotionScore(emotionArray, 0);

				bool doesEmotionScoreContainErrorMessage = DoesStringContainErrorMessage(emotionScore);

				if (doesEmotionScoreContainErrorMessage)
				{
					if (emotionScore.Contains(ErrorMessage[0]))
						Analytics.TrackEvent(MobileCenterConstants.NoFaceDetected);
					else if (emotionScore.Contains(ErrorMessage[1]))
                        Analytics.TrackEvent(MobileCenterConstants.MultipleFacesDetected);

					ScoreButton1Text = emotionScore;
				}
				else
					ScoreButton1Text = $"Score: {emotionScore}";

				_photo1Results = GetStringOfAllPhotoEmotionScores(emotionArray, 0);

				IsCalculatingPhoto1Score = false;
				IsResetButtonEnabled = !(IsCalculatingPhoto1Score || IsCalculatingPhoto2Score);

				imageMediaFile.Dispose();

				//Yeild to the UI Thread to ensure the ScoreButtonAnimation has completed
				await Task.Delay((int)(AnimationConstants.ScoreButonAninmationTime * 2.5));
			});

			TakePhoto2ButtonPressed = new Command(async () =>
			{
				IsTakeLeftPhotoButtonEnabled = false;
				IsScore2ButtonEnabled = false;

                Analytics.TrackEvent(MobileCenterConstants.PhotoButton2Tapped);

				if (!(await DisplayPopUpAlertAboutEmotion(2)))
				{
					IsTakeLeftPhotoButtonEnabled = true;
					IsScore2ButtonEnabled = true;
					return;
				}

				var imageMediaFile = await GetMediaFileFromCamera("FaceOff", "PhotoImage2");
				if (imageMediaFile == null)
				{
					IsTakeLeftPhotoButtonEnabled = true;
					IsScore2ButtonEnabled = true;
					return;
				}
				//Ensure the event is subscribed
				while (RevealPhotoImage2WithAnimation == null)
					await Task.Delay(100);

				OnRevealPhotoImage2WithAnimation();

				IsTakeRightPhotoButtonEnabled = false;
				IsTakeRightPhotoButtonVisible = false;

				ScoreButton2Text = CalculatingScore;

				Photo2ImageSource = ImageSource.FromStream(() =>
				{
					return GetPhotoStream(imageMediaFile, false);
				});

				IsCalculatingPhoto2Score = true;
				IsResetButtonEnabled = !(IsCalculatingPhoto1Score || IsCalculatingPhoto2Score);

				//Yeild to the UI Thread to ensure the PhotoImageAnimation has completed
				await Task.Delay((int)(AnimationConstants.PhotoImageAninmationTime * 2.5));

				//Ensure the event is subscribed
				while (RevealScoreButton2WithAnimation == null)
					await Task.Delay(100);

				OnRevealScoreButton2WithAnimation();

				var emotionArray = await GetEmotionResultsFromMediaFile(imageMediaFile, false);

				var emotionScore = GetPhotoEmotionScore(emotionArray, 0);

				bool doesEmotionScoreContainErrorMessage = DoesStringContainErrorMessage(emotionScore);

				if (doesEmotionScoreContainErrorMessage)
				{
					if (emotionScore.Contains(ErrorMessage[0]))
                        Analytics.TrackEvent(MobileCenterConstants.NoFaceDetected);
					else if (emotionScore.Contains(ErrorMessage[1]))
                        Analytics.TrackEvent(MobileCenterConstants.MultipleFacesDetected);

					ScoreButton2Text = emotionScore;
				}
				else
				{
					ScoreButton2Text = $"Score: {emotionScore}";
				}

				_photo2Results = GetStringOfAllPhotoEmotionScores(emotionArray, 0);

				IsCalculatingPhoto2Score = false;
				IsResetButtonEnabled = !(IsCalculatingPhoto1Score || IsCalculatingPhoto2Score);

				imageMediaFile.Dispose();

				//Yeild to the UI Thread to ensure the ScoreButtonAnimation has completed
				await Task.Delay((int)(AnimationConstants.ScoreButonAninmationTime * 2.5));
			});

			ResetButtonPressed = new Command(() =>
			{
                Analytics.TrackEvent(MobileCenterConstants.ResetButtonTapped);

				SetEmotion();

				Photo1ImageSource = null;
				Photo2ImageSource = null;

				IsTakeLeftPhotoButtonEnabled = true;
				IsTakeLeftPhotoButtonVisible = true;

				IsTakeRightPhotoButtonEnabled = true;
				IsTakeRightPhotoButtonVisible = true;

				ScoreButton1Text = null;
				ScoreButton2Text = null;

				IsScore1ButtonEnabled = false;
				IsScore2ButtonEnabled = false;

				IsScore1ButtonVisable = false;
				IsScore2ButtonVisable = false;

				_photo1Results = null;
				_photo2Results = null;

				IsPhotoImage1Enabled = false;
				IsPhotoImage2Enabled = false;
			});

			Photo1ScoreButtonPressed = new Command(() =>
			{
                Analytics.TrackEvent(MobileCenterConstants.ResultsButton1Tapped);
				OnDisplayAllEmotionResultsAlert(_photo1Results);
			});

			Photo2ScoreButtonPressed = new Command(() =>
			{
                Analytics.TrackEvent(MobileCenterConstants.ResultsButton2Tapped);
				OnDisplayAllEmotionResultsAlert(_photo2Results);
			});
		}
		#endregion

		#region Properties
		public Command TakePhoto1ButtonPressed { get; protected set; }
		public Command TakePhoto2ButtonPressed { get; protected set; }
		public Command ResetButtonPressed { get; protected set; }
		public Command SubmitButtonPressed { get; protected set; }
		public Command Photo1ScoreButtonPressed { get; protected set; }
		public Command Photo2ScoreButtonPressed { get; protected set; }

		public ImageSource Photo1ImageSource
		{
			get
			{
				return _photo1ImageSource;
			}
			set
			{
				SetProperty<ImageSource>(ref _photo1ImageSource, value);
			}
		}

		public ImageSource Photo2ImageSource
		{
			get
			{
				return _photo2ImageSource;
			}
			set
			{
				SetProperty<ImageSource>(ref _photo2ImageSource, value);
			}
		}

		public bool IsPhotoImage1Enabled
		{
			get
			{
				return _isPhotoImage1Enabled;
			}
			set
			{
				SetProperty<bool>(ref _isPhotoImage1Enabled, value);
			}
		}

		public bool IsPhotoImage2Enabled
		{
			get
			{
				return _isPhotoImage2Enabled;
			}
			set
			{
				SetProperty<bool>(ref _isPhotoImage2Enabled, value);
			}
		}

		public bool IsTakeLeftPhotoButtonEnabled
		{
			get
			{
				return _isTakeLeftPhotoButtonEnabled;
			}
			set
			{
				SetProperty<bool>(ref _isTakeLeftPhotoButtonEnabled, value);
			}
		}

		public bool IsTakeLeftPhotoButtonVisible
		{
			get
			{
				return _isTakeLeftPhotoButtonVisible;
			}
			set
			{
				SetProperty<bool>(ref _isTakeLeftPhotoButtonVisible, value);
			}
		}

		public bool IsTakeRightPhotoButtonEnabled
		{
			get
			{
				return _isTakeRightPhotoButtonEnabled;
			}
			set
			{
				SetProperty<bool>(ref _isTakeRightPhotoButtonEnabled, value);
			}
		}

		public bool IsTakeRightPhotoButtonVisible
		{
			get
			{
				return _isTakeRightPhotoButtonVisible;
			}
			set
			{
				SetProperty<bool>(ref _isTakeRightPhotoButtonVisible, value);
			}
		}

		public string PageTitle
		{
			get
			{
				return _pageTitle;
			}
			set
			{
				SetProperty<string>(ref _pageTitle, value);
			}
		}

		public string ScoreButton1Text
		{
			get
			{
				return _scoreButton1Text;
			}
			set
			{
				SetProperty<string>(ref _scoreButton1Text, value);
			}
		}

		public string ScoreButton2Text
		{
			get
			{
				return _scoreButton2Text;
			}
			set
			{
				SetProperty<string>(ref _scoreButton2Text, value);
			}
		}

		public bool IsCalculatingPhoto1Score
		{
			get
			{
				return _isCalculatingPhoto1Score;
			}
			set
			{
				SetProperty<bool>(ref _isCalculatingPhoto1Score, value);
			}
		}

		public bool IsCalculatingPhoto2Score
		{
			get
			{
				return _isCalculatingPhoto2Score;
			}
			set
			{
				SetProperty<bool>(ref _isCalculatingPhoto2Score, value);
			}
		}

		public bool IsResetButtonEnabled
		{
			get
			{
				return _isResetButtonEnabled;
			}
			set
			{
				SetProperty<bool>(ref _isResetButtonEnabled, value);
			}
		}

		public bool IsScore1ButtonEnabled
		{
			get
			{
				return _isScore1ButtonEnabled;
			}
			set
			{
				SetProperty<bool>(ref _isScore1ButtonEnabled, value);
			}
		}

		public bool IsScore2ButtonEnabled
		{
			get
			{
				return _isScore2ButtonEnabled;
			}
			set
			{
				SetProperty<bool>(ref _isScore2ButtonEnabled, value);
			}
		}

		public bool IsScore1ButtonVisable
		{
			get
			{
				return _isScore1ButtonVisable;
			}
			set
			{
				SetProperty<bool>(ref _isScore1ButtonVisable, value);
			}
		}

		public bool IsScore2ButtonVisable
		{
			get
			{
				return _isScore2ButtonVisable;
			}
			set
			{
				SetProperty<bool>(ref _isScore2ButtonVisable, value);
			}
		}

		public bool UserHasAcknowledgedPopUp { get; set; } = false;
		public bool UserResponseToAlert { get; set; }

		#endregion

		#region Methods
		Stream GetPhotoStream(MediaFile mediaFile, bool disposeMediaFile)
		{
			var stream = mediaFile.GetStream();

			if (disposeMediaFile)
				mediaFile.Dispose();

			return stream;
		}

		async Task<MediaFile> GetMediaFileFromCamera(string directory, string filename)
		{
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				OnDisplayNoCameraAvailableAlert();
				return null;
			}

			var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
			{
				Directory = directory,
				Name = filename,
				DefaultCamera = CameraDevice.Front,
				OverlayViewProvider = DependencyService.Get<ICameraService>()?.GetCameraOverlay()
			});

			return file;
		}

		async Task<Emotion[]> GetEmotionResultsFromMediaFile(MediaFile mediaFile, bool disposeMediaFile)
		{
			if (mediaFile == null)
				return null;

			try
			{
				var emotionClient = new EmotionServiceClient(Keys.EmotionApiKey);

				//using (var handle = Analytics.TrackTime(MobileCenterConstants.AnalyzeEmotion))
				//{
				return await emotionClient.RecognizeAsync(GetPhotoStream(mediaFile, disposeMediaFile));
				//}
			}
			catch (Exception e)
			{
                Analytics.TrackEvent("Exception Occured:"+ e.Message);
				return null;
			}
		}

		int GetRandomNumberForEmotion()
		{
			int randomNumber;

			do
			{
				var rnd = new Random();
				randomNumber = rnd.Next(0, _emotionStrings.Length);
			} while (randomNumber == _emotionNumber);

			return randomNumber;
		}

		void SetPageTitle(int emotionNumber)
		{
			PageTitle = _emotionStrings[emotionNumber];
		}

		void SetEmotion(int? emotionNumber = null)
		{
			if (emotionNumber != null && emotionNumber >= 0 && emotionNumber <= _emotionStrings.Length - 1)
				_emotionNumber = (int)emotionNumber;
			else
				_emotionNumber = GetRandomNumberForEmotion();

			SetPageTitle(_emotionNumber);
		}

		string GetPhotoEmotionScore(Emotion[] emotionResults, int emotionResultNumber)
		{
			float rawEmotionScore;

			if (emotionResults == null || emotionResults.Length < 1)
				return ErrorMessage[0];

			if (emotionResults.Length > 1)
			{
				OnDisplayMultipleFacesError();
				return ErrorMessage[1];
			}

			try
			{
				switch (_emotionNumber)
				{
					case 0:
						rawEmotionScore = emotionResults[emotionResultNumber].Scores.Anger;
						break;
					case 1:
						rawEmotionScore = emotionResults[emotionResultNumber].Scores.Contempt;
						break;
					case 2:
						rawEmotionScore = emotionResults[emotionResultNumber].Scores.Disgust;
						break;
					case 3:
						rawEmotionScore = emotionResults[emotionResultNumber].Scores.Fear;
						break;
					case 4:
						rawEmotionScore = emotionResults[emotionResultNumber].Scores.Happiness;
						break;
					case 5:
						rawEmotionScore = emotionResults[emotionResultNumber].Scores.Neutral;
						break;
					case 6:
						rawEmotionScore = emotionResults[emotionResultNumber].Scores.Sadness;
						break;
					case 7:
						rawEmotionScore = emotionResults[emotionResultNumber].Scores.Surprise;
						break;
					default:
						return ErrorMessage[0];
				}

				var emotionScoreAsPercentage = ConvertFloatToPercentage(rawEmotionScore);

				return emotionScoreAsPercentage;
			}
			catch (Exception e)
			{
                Analytics.TrackEvent("Exception Occured:" + e.Message);
				return ErrorMessage[0];
			}
		}

		string GetStringOfAllPhotoEmotionScores(Emotion[] emotionResults, int emotionResultNumber)
		{
			if (emotionResults == null || emotionResults.Length < 1)
				return ErrorMessage[0];

			string allEmotionsString = "";

			allEmotionsString += $"Anger: {ConvertFloatToPercentage(emotionResults[emotionResultNumber].Scores.Anger)}\n";
			allEmotionsString += $"Contempt: {ConvertFloatToPercentage(emotionResults[emotionResultNumber].Scores.Contempt)}\n";
			allEmotionsString += $"Disgust: {ConvertFloatToPercentage(emotionResults[emotionResultNumber].Scores.Disgust)}\n";
			allEmotionsString += $"Fear: {ConvertFloatToPercentage(emotionResults[emotionResultNumber].Scores.Fear)}\n";
			allEmotionsString += $"Happiness: {ConvertFloatToPercentage(emotionResults[emotionResultNumber].Scores.Happiness)}\n";
			allEmotionsString += $"Neutral: {ConvertFloatToPercentage(emotionResults[emotionResultNumber].Scores.Neutral)}\n";
			allEmotionsString += $"Sadness: {ConvertFloatToPercentage(emotionResults[emotionResultNumber].Scores.Sadness)}\n";
			allEmotionsString += $"Surprise: {ConvertFloatToPercentage(emotionResults[emotionResultNumber].Scores.Surprise)}";

			return allEmotionsString;
		}

		string ConvertFloatToPercentage(float floatToConvert)
		{
			return floatToConvert.ToString("#0.##%");

		}

		async Task<bool> DisplayPopUpAlertAboutEmotion(int playerNumber)
		{
			var alertMessage = new AlertMessageModel
			{
				Title = _emotionStrings[_emotionNumber],
				Message = "Player " + playerNumber + ", " + MakeAFaceAlertMessage + _emotionStringsForAlertMessage[_emotionNumber]
			};
			OnDisplayEmotionBeforeCameraAlert(alertMessage);

			while (!UserHasAcknowledgedPopUp)
			{
				await Task.Delay(5);
			}
			UserHasAcknowledgedPopUp = false;

			return UserResponseToAlert;
		}

		public void SetPhotoImage1(string photo1ImageString)
		{
			Photo1ImageSource = photo1ImageString;

			var allEmotionsString = "";
			allEmotionsString += $"Anger: 0%\n";
			allEmotionsString += $"Contempt: 0%\n";
			allEmotionsString += $"Disgust: 0%\n";
			allEmotionsString += $"Fear: 0%\n";
			allEmotionsString += $"Happiness: 100%\n";
			allEmotionsString += $"Neutral: 0%\n";
			allEmotionsString += $"Sadness: 0%\n";
			allEmotionsString += $"Surprise: 0%";

			_photo1Results = allEmotionsString;
			ScoreButton1Text = "Score: 100%";

			SetEmotion(4);

			IsTakeLeftPhotoButtonEnabled = false;
			IsTakeLeftPhotoButtonVisible = false;

			OnRevealPhotoImage1WithAnimation();
			OnRevealScoreButton1WithAnimation();
		}

		public void SetPhotoImage2(string photo2ImageString)
		{
			Photo2ImageSource = photo2ImageString;

			var allEmotionsString = "";
			allEmotionsString += $"Anger: 0%\n";
			allEmotionsString += $"Contempt: 0%\n";
			allEmotionsString += $"Disgust: 0%\n";
			allEmotionsString += $"Fear: 0%\n";
			allEmotionsString += $"Happiness: 100%\n";
			allEmotionsString += $"Neutral: 0%\n";
			allEmotionsString += $"Sadness: 0%\n";
			allEmotionsString += $"Surprise: 0%";

			_photo2Results = allEmotionsString;
			ScoreButton2Text = "Score: 100%";

			SetEmotion(4);

			IsTakeRightPhotoButtonEnabled = false;
			IsTakeRightPhotoButtonVisible = false;

			OnRevealPhotoImage2WithAnimation();
			OnRevealScoreButton2WithAnimation();
		}

		bool DoesStringContainErrorMessage(string stringToCheck)
		{
			foreach (string errorMessage in ErrorMessage)
			{
				if (stringToCheck.Contains(errorMessage))
					return true;
			}

			return false;
		}

		void OnDisplayAllEmotionResultsAlert(string emotionResults)
		{
			var handle = DisplayAllEmotionResultsAlert;
			handle?.Invoke(null, new TextEventArgs(emotionResults));
		}

		void OnDisplayNoCameraAvailableAlert()
		{
			var handle = DisplayNoCameraAvailableAlert;
			handle?.Invoke(null, EventArgs.Empty);
		}

		void OnDisplayEmotionBeforeCameraAlert(AlertMessageModel alertMessage)
		{
			var handle = DisplayEmotionBeforeCameraAlert;
			handle?.Invoke(null, new AlertMessageEventArgs(alertMessage));
		}

		void OnRevealPhotoImage1WithAnimation()
		{
			var handle = RevealPhotoImage1WithAnimation;
			handle?.Invoke(null, EventArgs.Empty);
		}

		void OnRevealScoreButton1WithAnimation()
		{
			var handle = RevealScoreButton1WithAnimation;
			handle?.Invoke(null, EventArgs.Empty); ;
		}

		void OnRevealPhotoImage2WithAnimation()
		{
			var handle = RevealPhotoImage2WithAnimation;
			handle?.Invoke(null, EventArgs.Empty);
		}

		void OnRevealScoreButton2WithAnimation()
		{
			var handle = RevealScoreButton2WithAnimation;
			handle?.Invoke(null, EventArgs.Empty);
		}

		void OnDisplayMultipleFacesError()
		{
			var handle = DisplayMultipleFacesDetectedAlert;
			handle?.Invoke(null, EventArgs.Empty);
		}
		#endregion
	}
}

