using System;
using System.Drawing;
using MonoTouch.AVFoundation;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreMedia;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VideoBet.iOS
{
	public class AVAssetStitcher
	{
		SizeF outputSize;

		AVMutableComposition composition;
		AVMutableCompositionTrack compositionVideoTrack;
		AVMutableCompositionTrack compositionAudioTrack;

		NSMutableArray instructions;

		public AVAssetStitcher(SizeF outSize)
		{
			outputSize = outSize;

			composition = AVMutableComposition.Create();
			compositionVideoTrack = composition.AddMutableTrack(AVMediaType.Video, 0);
			compositionAudioTrack = composition.AddMutableTrack(AVMediaType.Audio, 0);
			instructions = new NSMutableArray();
		}

		public void AddAsset(AVUrlAsset asset, Func<AVAssetTrack, CGAffineTransform> transformToApply, Action<NSError> errorHandler)
		{
			AVAssetTrack videoTrack = asset.TracksWithMediaType(AVMediaType.Video)[0];

			AVMutableVideoCompositionInstruction instruction = new AVMutableVideoCompositionInstruction();
			AVMutableVideoCompositionLayerInstruction layerInstruction = AVMutableVideoCompositionLayerInstruction.FromAssetTrack(compositionVideoTrack);

			// Apply a transformation to the video if one has been given. If a transformation is given it is combined
			// with the preferred transform contained in the incoming video track.
			if (transformToApply != null)
				layerInstruction.SetTransform(CGAffineTransform.Multiply(videoTrack.PreferredTransform, transformToApply(videoTrack)), CMTime.Zero);
			else
				layerInstruction.SetTransform(videoTrack.PreferredTransform, CMTime.Zero);

			instruction.LayerInstructions = new AVVideoCompositionLayerInstruction[]{ layerInstruction };

			CMTime startTime = CMTime.Zero;
			for (int i = 0; i < instructions.Count; i++)
			{
				startTime = CMTime.Add(startTime, instructions.GetItem<AVMutableVideoCompositionInstruction>(i).TimeRange.Duration);
			}

			CMTimeRange timeRange;
			timeRange.Start = startTime;
			timeRange.Duration = asset.Duration;
			instruction.TimeRange = timeRange;

			instructions.Add(instruction);

			NSError error;

			CMTimeRange videoTimeRange;
			videoTimeRange.Start = CMTime.Zero;
			videoTimeRange.Duration = asset.Duration;
			compositionVideoTrack.InsertTimeRange(videoTimeRange, videoTrack, CMTime.Zero, out error);

			if (error != null)
			{
				errorHandler(error);
				return;
			}

			AVAssetTrack audioTrack = asset.TracksWithMediaType(AVMediaType.Audio)[0];
			CMTimeRange audioTimeRange;
			audioTimeRange.Start = CMTime.Zero;
			audioTimeRange.Duration = asset.Duration;
			compositionAudioTrack.InsertTimeRange(audioTimeRange, audioTrack, CMTime.Zero, out error);

			if (error != null)
			{
				errorHandler(error);
				return;
			}
		}

		public void ExportTo(NSUrl outputFile, NSString preset, Action<NSError> completionHandler)
		{
			AVMutableVideoComposition videoComposition = AVMutableVideoComposition.Create();

			AVMutableVideoCompositionInstruction[] stronglyTypedInstructions = new AVMutableVideoCompositionInstruction[instructions.Count];
			for (int i = 0; i < instructions.Count; i++)
				stronglyTypedInstructions[i] = instructions.GetItem<AVMutableVideoCompositionInstruction>(i);
				
			videoComposition.Instructions = stronglyTypedInstructions;
			videoComposition.RenderSize = outputSize;
			videoComposition.FrameDuration = new CMTime(1, 30);

			AVAssetExportSession exporter = AVAssetExportSession.FromAsset(composition, preset);

			if (exporter == null)
				throw new Exception("Exporter is null");

			exporter.OutputFileType = AVFileType.Mpeg4;
			exporter.VideoComposition = videoComposition;
			exporter.OutputUrl = outputFile;

			var task = exporter.ExportTaskAsync();
			task.ContinueWith(t =>
			{
				switch (exporter.Status)
				{
				case AVAssetExportSessionStatus.Failed:
					completionHandler(exporter.Error);
					break;
				case AVAssetExportSessionStatus.Cancelled:
				case AVAssetExportSessionStatus.Completed:
					completionHandler(null);
					break;
				default:
					completionHandler(new NSError(new NSString("Unknown export error"), 100));
					break;
				}
			});
		}
	}
}

