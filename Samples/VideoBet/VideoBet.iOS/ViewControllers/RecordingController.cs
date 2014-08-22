using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using MonoTouch.AVFoundation;
using MonoTouch.CoreMedia;
using MonoTouch.AssetsLibrary;
using System.Threading.Tasks;
using System.Threading;
using MonoTouch.ObjCRuntime;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace VideoBet.iOS.ViewControllers
{
	partial class RecordingController : UIViewController
	{
		// Maximum and minumum length to record in seconds
		const float MAX_RECORDING_LENGTH = 6.0f;
		const float MIN_RECORDING_LENGTH = 2.0f;

		// Set the recording preset to use
		static readonly NSString CAPTURE_SESSION_PRESET = AVCaptureSession.Preset640x480;

		// Set the input device to use when first starting
		static readonly AVCaptureDevicePosition INITIAL_CAPTURE_DEVICE_POSITION = AVCaptureDevicePosition.Back;

		// Set the initial torch mode
		static readonly AVCaptureTorchMode INITIAL_TORCH_MODE = AVCaptureTorchMode.Off;

		VideoCameraInputManager videoCameraInputManager;
		AVCaptureVideoPreviewLayer captureVideoPreviewLayer;
		NSTimer progressUpdateTimer;

		public RecordingController(IntPtr handle)
			: base(handle)
		{
		}

		static void HandleError(NSError error)
		{
			UIAlertView alertView = new UIAlertView("Error", error.Domain, null, "OK");
			alertView.Show();
		}

		public override void ViewDidLoad()
		{
			videoCameraInputManager = new VideoCameraInputManager();
			videoCameraInputManager.MaxDuration = MAX_RECORDING_LENGTH;
			videoCameraInputManager.AsyncErrorHandler = HandleError;

			NSError error;

			videoCameraInputManager.SetupSession(CAPTURE_SESSION_PRESET, INITIAL_CAPTURE_DEVICE_POSITION, INITIAL_TORCH_MODE, out error);

			if (error != null)
			{
				var alertView = new UIAlertView("Error", error.Domain, null, "OK");
				alertView.Show();
			}
			else
			{
				captureVideoPreviewLayer = AVCaptureVideoPreviewLayer.FromSession(videoCameraInputManager.CaptureSession);
				this.videoPreviewView.Layer.MasksToBounds = true;
				captureVideoPreviewLayer.Frame = this.videoPreviewView.Bounds;
				captureVideoPreviewLayer.VideoGravity = "AVLayerVideoGravityResizeAspectFill";

				this.videoPreviewView.Layer.InsertSublayer(captureVideoPreviewLayer, 0);
				Task.Run(action: videoCameraInputManager.CaptureSession.StartRunning);

				this.busyView.Frame = new RectangleF(this.busyView.Frame.Location.X, -this.busyView.Frame.Size.Height, this.busyView.Frame.Size.Width, this.busyView.Frame.Size.Height);
				this.saveButton.Hidden = true;
			}

			base.ViewDidLoad();
		}

		public override void ViewDidAppear(bool animated)
		{
			captureVideoPreviewLayer.Frame = this.videoPreviewView.Bounds;

			base.ViewDidAppear(animated);
		}

		partial void RecordTouchDown(UIButton sender)
		{
			progressUpdateTimer = NSTimer.CreateScheduledTimer(0.05, this, new Selector("updateProgress:"), null, true);

			if (videoCameraInputManager.IsPaused)
				videoCameraInputManager.ResumeRecording();
			else
				videoCameraInputManager.StartRecording();
		}

		partial void RecordTouchCancel(UIButton sender)
		{
			progressUpdateTimer.Invalidate();
			videoCameraInputManager.PauseRecording();
		}

		partial void RecordTouchUp(UIButton sender)
		{
			progressUpdateTimer.Invalidate();
			videoCameraInputManager.PauseRecording();
		}

		partial void CancelAction(UIButton sender)
		{
			this.saveButton.Hidden = true;

			this.videoRecordingProgress.Progress = 0.0f;

			videoCameraInputManager.Reset();
		}

		partial void SaveRecording(UIButton sender)
		{
			saveButton.Hidden = true;

			busyView.Hidden = false;

			UIView.Animate(0.25, () =>
			{
				this.busyView.Frame = new RectangleF(this.busyView.Frame.Location.X, 0, this.busyView.Frame.Size.Width, this.busyView.Frame.Size.Height);
			});

			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var tmp = Path.Combine(documents, "..", "tmp");
			string finalOutputFilePath = Path.Combine(tmp, string.Format("{0}-{1}.mp4", "final", DateTime.Now.ToString(VideoCameraInputManager.TimestampStringFormat)));
			NSUrl finalOutputFileUrl = NSUrl.FromFilename(finalOutputFilePath);

			videoCameraInputManager.FinalizeRecordingToFile(finalOutputFileUrl, this.videoPreviewView.Frame.Size, AVAssetExportSession.Preset640x480, error =>
			{
				if (error != null)
				{
					var alertView = new UIAlertView("Error", error.Domain, null, "OK");
					alertView.Show();
				}
				else
				{
					SaveOutputToAssetLibrary(finalOutputFileUrl, () =>
					{
						BeginInvokeOnMainThread(() =>
						{
							UIView.Animate(0.25, () =>
							{
								this.busyView.Frame = new RectangleF(busyView.Frame.Location.X, -busyView.Frame.Size.Height, busyView.Frame.Size.Width, busyView.Frame.Size.Height);
							}, () =>
							{
								busyView.Hidden = true;
								var alertView = new UIAlertView("Done", "The video has been saved to your camera roll.", null, "OK");
								alertView.Show();							
							});

							NSError nullError;
							NSFileManager.DefaultManager.Remove(finalOutputFileUrl, out nullError);

							videoRecordingProgress.Progress = 0.0f;
							recordButton.Enabled = true;
						});
					});
				}
			});
		}

		[Export("updateProgress:")]
		void UpdateProgress(NSTimer timer)
		{
			CMTime duration = videoCameraInputManager.GetTotalRecordingDuration();
			this.videoRecordingProgress.Progress = (float)duration.Seconds / MAX_RECORDING_LENGTH;

			if (duration.Seconds >= MIN_RECORDING_LENGTH)
				this.saveButton.Hidden = false;

			if (duration.Seconds >= MAX_RECORDING_LENGTH)
				this.saveButton.Enabled = false;
		}

		void SaveOutputToAssetLibrary(NSUrl outputFileURL, Action completed)
		{
			ALAssetsLibrary library = new ALAssetsLibrary();
			var task = library.WriteVideoToSavedPhotosAlbumAsync(outputFileURL);
			task.ContinueWith(t =>
			{
				completed();
			});
		}
	}
}
