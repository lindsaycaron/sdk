// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace VideoBet.iOS.ViewControllers
{
	[Register ("RecordingController")]
	partial class RecordingController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView busyView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton cancelButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton recordButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton saveButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView videoPreviewView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIProgressView videoRecordingProgress { get; set; }

		[Action ("cancelAction:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void CancelAction (UIButton sender);

		[Action ("recordTouchCancel:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void RecordTouchCancel (UIButton sender);

		[Action ("recordTouchDown:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void RecordTouchDown (UIButton sender);

		[Action ("recordTouchUp:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void RecordTouchUp (UIButton sender);

		[Action ("saveRecording:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void SaveRecording (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (busyView != null) {
				busyView.Dispose ();
				busyView = null;
			}
			if (cancelButton != null) {
				cancelButton.Dispose ();
				cancelButton = null;
			}
			if (recordButton != null) {
				recordButton.Dispose ();
				recordButton = null;
			}
			if (saveButton != null) {
				saveButton.Dispose ();
				saveButton = null;
			}
			if (videoPreviewView != null) {
				videoPreviewView.Dispose ();
				videoPreviewView = null;
			}
			if (videoRecordingProgress != null) {
				videoRecordingProgress.Dispose ();
				videoRecordingProgress = null;
			}
		}
	}
}
