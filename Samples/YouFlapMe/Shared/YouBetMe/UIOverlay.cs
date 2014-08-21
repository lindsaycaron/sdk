using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Youbetme.Helpers;
using Youbetme.DataObjects;

namespace FlappyMonkey
{

	public enum  UIOverlayMode
	{
		None,
		WaitingToSyncWithYBMServer,
		LoggingInToYBMServer,
		NotYourGo,
		YourGo,
		NoBetsInProgress,
		Error,
	};

	public class UIOverlay : DrawableGameComponent
	{
		SpriteBatch spriteBatch;
		Texture2D dummyTexture;
		Rectangle dummyRectangle;
		Color Colori;
		SpriteFont font;


		private static readonly Dictionary<UIOverlayMode, string> statusString = new Dictionary<UIOverlayMode, string> {
			{ UIOverlayMode.None, "" },
			{ UIOverlayMode.LoggingInToYBMServer, "WAIT:\n Logging into to YBM Server..." },
			{ UIOverlayMode.WaitingToSyncWithYBMServer, "WAIT:\n ...syncing with the YBM Server..." },
			{ UIOverlayMode.NotYourGo, "Game is on!, \nwaiting for the other player..." },
			{ UIOverlayMode.YourGo, "Game is valid,\n it's your turn..." },
			{ UIOverlayMode.NoBetsInProgress, "No Bets \nIn Progress" },
			{ UIOverlayMode.Error, "Something's gone wrong, \nlook at debug info..." },
		};

		private static readonly Dictionary<UIOverlayMode, bool> isScreenAndInputBlocked = new Dictionary<UIOverlayMode, bool> {
			{ UIOverlayMode.None, false },
			{ UIOverlayMode.LoggingInToYBMServer, true },
			{ UIOverlayMode.WaitingToSyncWithYBMServer, true },
			{ UIOverlayMode.NotYourGo, true },
			{ UIOverlayMode.YourGo, false },
			{ UIOverlayMode.NoBetsInProgress, false },
			{ UIOverlayMode.Error, true },
		};

		public UIOverlayMode Mode = UIOverlayMode.None;

		public bool IsScreenAndInputBlocked {
			get {
				bool res = isScreenAndInputBlocked [Mode];
				return res;
			}
		}

		public UIOverlay (Game game)
			: base (game)
		{
			dummyRectangle = new Rectangle (0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height);
			Colori = new Color (0.0f, 0.0f, 0.0f, 0.50f);
			DrawOrder = 01000;
		}

		public UIOverlay (Rectangle rect, Color colori, int drawOrder, Game game)
			: base (game)
		{
			DrawOrder = drawOrder;
			dummyRectangle = rect;
			Colori = colori;
		}

		public void Initialize (SpriteFont font)
		{
			this.font = font;
			base.Initialize ();
		}

		protected override void LoadContent ()
		{
			spriteBatch = new SpriteBatch (GraphicsDevice);
			dummyTexture = new Texture2D (GraphicsDevice, 1, 1);
			dummyTexture.SetData (new Color[] { Color.White });
		}

		public override void Draw (GameTime gameTime)
		{
			//if (IsScreenAndInputBlocked == false)
			//	return;


			var drawStr = statusString [Mode];
			if (Mode == UIOverlayMode.NotYourGo) {
				drawStr = drawStr + "\n--- Bets Your Winning ---\n";
				drawStr = drawStr + getBetDetailString (BetsHelper.BetsImWinning);
			}
			if (Mode == UIOverlayMode.YourGo) {
				drawStr = drawStr + "\n--- Bets Your Loosing ---\n";
				drawStr = drawStr + getBetDetailString (BetsHelper.BetsImLoosing);
			}

			spriteBatch.Begin ();
			if (IsScreenAndInputBlocked)
				spriteBatch.Draw (dummyTexture, dummyRectangle, Colori);
			spriteBatch.DrawString (
				font, 
				drawStr, 
				new Vector2 (GraphicsDevice.Viewport.TitleSafeArea.X + 10, GraphicsDevice.Viewport.Height / 2), 
				Color.White);
			spriteBatch.End ();
		}

		static string getBetDetailString (List<Bet>bets)
		{
			var output = "";
			foreach (var bet in bets) {
				var betLight = bet.GetLightObject (LoginHelper.LocalCustomer.Id);
				output = output + betLight.PreviewText + "\n";
				output = output + "...remaining Time: "
				+ BetsHelper.GetTimeLeft (bet).Hours + ":"
				+ BetsHelper.GetTimeLeft (bet).Minutes + ":"
				+ BetsHelper.GetTimeLeft (bet).Seconds
				+ ".\n";
				output = output + "...score to beat: " + BetsHelper.GetScoreFromBet (bet) + ".\n";
				output = output + "...streak: " + BetsHelper.GetStreakFromBet (bet) + ".\n";
				var streakStr = "";
				switch (BetsHelper.GetStreakFromBet (bet)) {
				case 0: 
					break;
				case 1: 
					streakStr = "game on";
					break;
				case 2: 
					streakStr = "keep it up";
					break;
				case 3: 
					streakStr = "groovie";
					break;
				case 4: 
					streakStr = "oh my";
					break;
				case 5: 
					streakStr = "my hero!";
					break;
				case 6: 
					streakStr = "how high can you go?";
					break;
				case 7: 
					streakStr = "sunshine";
					break;
				case 8: 
					streakStr = "not so shabby";
					break;
				case 9: 
					streakStr = "go Hawks!";
					break;
				case 10: 
					streakStr = "super groovie";
					break;
				case 11: 
					streakStr = "out of this world";
					break;
				case 12: 
					streakStr = "who's in the house";
					break;

				default:
					streakStr = "yikes, out of things to say - extend the app!";
					break;
				}
				output = output + "   ---" + streakStr + "---\n";
				output = output + "...playing for: '" + bet.BetOutcome + "'.\n";
			}
			return output;
		}

	}
}

