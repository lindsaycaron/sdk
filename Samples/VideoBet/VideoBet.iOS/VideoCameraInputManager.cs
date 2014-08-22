using System;
using MonoTouch.AVFoundation;
using MonoTouch.Foundation;
using MonoTouch.CoreMedia;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace VideoBet.iOS
{
	public class VideoCameraInputManager: AVCaptureFileOutputRecordingDelegate
	{
		public static readonly string TimestampStringFormat = "yyyyMMddhhmmss";

		public AVCaptureSession CaptureSession { get; private set; }

		public bool IsPaused { get; private set; }

		public float MaxDuration { get; set; }

		public Action<NSError> AsyncErrorHandler { get; set; }

		bool setupComplete;

		AVCaptureDeviceInput videoInput;
		AVCaptureDeviceInput audioInput;

		AVCaptureMovieFileOutput movieFileOutput;

		AVCaptureVideoOrientation orientation;

		NSObject deviceConnectedObserver;
		NSObject deviceDisconnectedObserver;
		NSObject deviceOrientationDidChangeObserver;

		List<NSUrl> temporaryFileURLs = new List<NSUrl>();

		string uniqueTimestamp;
		int currentRecordingSegment;

		CMTime currentFinalDuration;
		int inFlightWrites;

		public VideoCameraInputManager()
		{
			movieFileOutput = new AVCaptureMovieFileOutput();
			StartNotificationObservers();
		}

		protected override void Dispose(bool disposing)
		{
			CaptureSession.RemoveOutput(movieFileOutput);
			EndNotificationObservers();

			base.Dispose(disposing);
		}

		public void SetupSession(NSString preset, AVCaptureDevicePosition cd, AVCaptureTorchMode tm, out NSError error)
		{
			NSError nullErr = null;

			if (setupComplete)
			{
				error = new NSError(new NSString("Setup session already complete."), 102);
				return;
			}

			setupComplete = true;

			AVCaptureDevice captureDevice = CameraWithPosition(cd);

			if (captureDevice.HasTorch)
			{
				if (captureDevice.LockForConfiguration(out nullErr))
				{
					if (captureDevice.IsTorchModeSupported(tm))
					{
						captureDevice.TorchMode = AVCaptureTorchMode.Off;
					}
					captureDevice.UnlockForConfiguration();
				}
			}

			CaptureSession = new AVCaptureSession();
			CaptureSession.SessionPreset = preset;

			videoInput = new AVCaptureDeviceInput(captureDevice, out nullErr);
			if (CaptureSession.CanAddInput(videoInput))
			{
				CaptureSession.AddInput(videoInput);
			}
			else
			{
				error = new NSError(new NSString("Error setting video input."), 101);
				return;
			}

			audioInput = new AVCaptureDeviceInput(GetAudioDevice(), out nullErr);
			if (CaptureSession.CanAddInput(audioInput))
			{
				CaptureSession.AddInput(audioInput);
			}
			else
			{
				error = new NSError(new NSString("Error setting audio input."), 101);
				return;
			}
				
			if (CaptureSession.CanAddOutput(movieFileOutput))
			{
				CaptureSession.AddOutput(movieFileOutput);
			}
			else
			{
				error = new NSError(new NSString("Error setting file output."), 101);
				return;
			}

			error = null;
		}

		public void StartRecording()
		{
			temporaryFileURLs.Clear();
		
			uniqueTimestamp = DateTime.Now.ToString(TimestampStringFormat);
			currentRecordingSegment = 0;
			IsPaused = false;
			currentFinalDuration = CMTime.Zero;
		
			AVCaptureConnection videoConnection = ConnectionWithMediaType(AVMediaType.Video, NSArray.FromNSObjects(movieFileOutput.Connections));
			if (videoConnection.SupportsVideoOrientation)
			{
				videoConnection.VideoOrientation = orientation;
			}

			var outputFileURL = NSUrl.FromFilename(ConstructCurrentTemporaryFilename());
			temporaryFileURLs.Add(outputFileURL);
			movieFileOutput.MaxRecordedDuration = (MaxDuration > 0) ? CMTime.FromSeconds(MaxDuration, 600) : CMTime.Invalid;

			movieFileOutput.StartRecordingToOutputFile(outputFileURL, this);
		}

		public void PauseRecording()
		{
			IsPaused = true;
			movieFileOutput.StopRecording();		
			currentFinalDuration = CMTime.Add(currentFinalDuration, movieFileOutput.RecordedDuration);
		}

		public void ResumeRecording()
		{
			currentRecordingSegment++;
			IsPaused = false;
		
			NSUrl outputFileURL = NSUrl.FromFilename(ConstructCurrentTemporaryFilename());
		
			temporaryFileURLs.Add(outputFileURL);
		
			if (MaxDuration > 0)
			{
				movieFileOutput.MaxRecordedDuration = CMTime.Subtract(CMTime.FromSeconds(MaxDuration, 600), currentFinalDuration);
			}
			else
			{
				movieFileOutput.MaxRecordedDuration = CMTime.Invalid;
			}
		
			movieFileOutput.StartRecordingToOutputFile(outputFileURL, this);
		}

		public void Reset()
		{
			if (movieFileOutput.Recording)
			{
				PauseRecording();
			}
		
			IsPaused = false;
		}

		public void FinalizeRecordingToFile(NSUrl finalVideoLocationURL, SizeF videoSize, NSString preset, Action<NSError> completionHandler)
		{
			Reset();
		
			if (File.Exists(finalVideoLocationURL.AbsoluteString))
			{
				completionHandler(new NSError(new NSString("Output file already exists."), 104));
				return;
			}			

			if (inFlightWrites != 0)
			{
				completionHandler(new NSError(new NSString("Can't finalize recording unless all sub-recorings are finished."), 106));
				return;
			}

			AVAssetStitcher stitcher = new AVAssetStitcher(videoSize);
					
			NSError stitcherError = null;
			for (int i = temporaryFileURLs.Count - 1; i >= 0; i--)
			{
				NSUrl outputFileUrl = temporaryFileURLs[i];			

				stitcher.AddAsset(new AVUrlAsset(outputFileUrl, new AVUrlAssetOptions()), videoTrack =>
				{
					float ratioW = videoSize.Width / videoTrack.NaturalSize.Width;
					float ratioH = videoSize.Height / videoTrack.NaturalSize.Height;

					if (ratioW < ratioH)
					{
						// When the ratios are larger than one, we must flip the translation.
						float neg = (ratioH > 1.0f) ? 1.0f : -1.0f;
						float diffH = videoTrack.NaturalSize.Height - (videoTrack.NaturalSize.Height * ratioH);
						return CGAffineTransform.Multiply(CGAffineTransform.MakeTranslation(0f, neg * diffH / 2.0f), CGAffineTransform.MakeScale(ratioH, ratioH));
					}
					else
					{
						// When the ratios are larger than one, we must flip the translation.
						float neg = (ratioW > 1.0f) ? 1.0f : -1.0f;
						float diffW = videoTrack.NaturalSize.Width - (videoTrack.NaturalSize.Width * ratioW);
						return CGAffineTransform.Multiply(CGAffineTransform.MakeTranslation(neg * diffW / 2.0f, 0f), CGAffineTransform.MakeScale(ratioW, ratioW));
					}
				}, error =>
				{
					stitcherError = error;
				});

			}
			;
		
			if (stitcherError != null)
			{
				completionHandler(stitcherError);
				return;
			}
		
			stitcher.ExportTo(finalVideoLocationURL, preset, error =>
			{		
				if (error != null)
				{
					completionHandler(error);
				}
				else
				{
					CleanTemporaryFiles();
					temporaryFileURLs.Clear();		
					completionHandler(null);
				}		
			});
		}

		public CMTime GetTotalRecordingDuration()
		{
			if (CMTime.Compare(CMTime.Zero, currentFinalDuration) == 0)
			{
				return movieFileOutput.RecordedDuration;
			}
			else
			{
				CMTime returnTime = CMTime.Add(currentFinalDuration, movieFileOutput.RecordedDuration);
				return returnTime.IsInvalid ? currentFinalDuration : returnTime;
			}
		}

		#region AVCaptureFileOutputRecordingDelegate

		public override void DidStartRecording(AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections)
		{
			inFlightWrites++;
		}

		public override void FinishedRecording(AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections, NSError error)
		{
			if (error != null)
			{
				if (AsyncErrorHandler != null)
				{
					AsyncErrorHandler(error);
				}
				else
				{
					System.Diagnostics.Debug.WriteLine(string.Format("Error capturing output: {0}", error));
				}
			}

			inFlightWrites--;
		}

		#endregion

		#region Observer start and stop

		void StartNotificationObservers()
		{
			NSNotificationCenter notificationCenter = NSNotificationCenter.DefaultCenter;

			// Reconnect to a device that was previously being used
			deviceConnectedObserver = notificationCenter.AddObserver(AVCaptureDevice.WasConnectedNotification, notification =>
			{
				NSString deviceMediaType = null;

				AVCaptureDevice device = (AVCaptureDevice)notification.Object;
				if (device.HasMediaType(AVMediaType.Audio))
					deviceMediaType = AVMediaType.Audio;
				else if (device.HasMediaType(AVMediaType.Video))
					deviceMediaType = AVMediaType.Video;

				if (deviceMediaType != null)
				{
					foreach (AVCaptureDeviceInput input in  CaptureSession.Inputs)
					{
						if (input.Device.HasMediaType(deviceMediaType))
						{
							NSError error;
							AVCaptureDeviceInput deviceInput = AVCaptureDeviceInput.FromDevice(device, out error);
							if (CaptureSession.CanAddInput(deviceInput))
							{
								CaptureSession.AddInput(deviceInput);
							}

							if (error != null)
							{
								if (AsyncErrorHandler != null)
								{
									AsyncErrorHandler(error);
								}
								else
								{
									System.Diagnostics.Debug.WriteLine(string.Format("Error reconnecting device input: {0}", error));
								}
							}

							break;
						}
					}
				}
			});

			// Disable inputs from removed devices that are being used
			deviceDisconnectedObserver = notificationCenter.AddObserver(AVCaptureDevice.WasDisconnectedNotification, notification =>
			{
				AVCaptureDevice device = (AVCaptureDevice)notification.Object;
				if (device.HasMediaType(AVMediaType.Audio))
				{
					CaptureSession.RemoveInput(audioInput);
					audioInput = null;
				}
				else if (device.HasMediaType(AVMediaType.Video))
				{
					CaptureSession.RemoveInput(videoInput);
					videoInput = null;
				}
			});

			// Track orientation changes. Note: This are pushed into the Quicktime video data and needs
			// to be used at decoding time to transform the video into the correct orientation.
			orientation = AVCaptureVideoOrientation.Portrait;
			deviceOrientationDidChangeObserver = notificationCenter.AddObserver(UIDevice.OrientationDidChangeNotification, notification =>
			{
				switch (UIDevice.CurrentDevice.Orientation)
				{
				case UIDeviceOrientation.Portrait:
					orientation = AVCaptureVideoOrientation.Portrait;
					break;
				case UIDeviceOrientation.PortraitUpsideDown:
					orientation = AVCaptureVideoOrientation.PortraitUpsideDown;
					break;
				case UIDeviceOrientation.LandscapeLeft:
					orientation = AVCaptureVideoOrientation.LandscapeRight;
					break;
				case UIDeviceOrientation.LandscapeRight:
					orientation = AVCaptureVideoOrientation.LandscapeLeft;
					break;
				default:
					orientation = AVCaptureVideoOrientation.Portrait;
					break;
				}
			});
			UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
		}

		void EndNotificationObservers()
		{
			UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications();
			NSNotificationCenter.DefaultCenter.RemoveObserver(deviceConnectedObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver(deviceDisconnectedObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver(deviceOrientationDidChangeObserver);
		}

		#endregion

		#region Device finding methods

		AVCaptureDevice CameraWithPosition(AVCaptureDevicePosition position)
		{
			AVCaptureDevice foundDevice = null;
			foreach (var device in AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video))
			{
				if (device.Position == position)
				{
					foundDevice = device;
					break;
				}
			}
			return foundDevice;
		}

		AVCaptureDevice GetAudioDevice()
		{
			var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Audio);
			if (devices.Length > 0)
			{
				return devices[0];
			}
			return null;
		}

		#endregion

		#region Connection finding method

		AVCaptureConnection ConnectionWithMediaType(string mediaType, NSArray connections)
		{
			AVCaptureConnection foundConnection = null;

			for (int i = 0; i < connections.Count; i++)
			{
				var connection = connections.GetItem<AVCaptureConnection>(i);
				foreach (var port in connection.InputPorts)
				{
					if (port.MediaType == mediaType)
					{
						foundConnection = connection;
						break;
					}

					if (foundConnection != null)
						break;
				}
			}
		
			return foundConnection;
		}

		#endregion

		#region Temporary file handling functions

		string ConstructCurrentTemporaryFilename()
		{
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var tmp = Path.Combine(documents, "..", "tmp");
			string filePath = Path.Combine(tmp, string.Format("{0}-{1}-{2}.mov", "recordingsegment", uniqueTimestamp, currentRecordingSegment));
			return filePath;
		}

		void CleanTemporaryFiles()
		{
			NSError nullError = null;
		
			foreach (var foo in temporaryFileURLs)
				NSFileManager.DefaultManager.Remove(foo, out nullError);
		}

		#endregion
	}
}

