﻿#region File Description
//-----------------------------------------------------------------------------
// FlappyMonkey.iOSGame.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;

#endregion
#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Youbetme.Communication;
using Youbetme.DataObjects;
using Youbetme.Validation;
using Youbetme.Proxies;
using Youbetme.Helpers;

#endregion




namespace FlappyMonkey
{


	public enum GameState
	{
		Menu,
		Playing,
		Score
	}

	/// <summary>
	/// Default Project Template
	/// </summary>
	public class Game1 : Game
	{
		#region Fields


		// YouBetMe helper (while we dont have a menu system
		private static readonly Customer testAccount1 = new Customer () {
			EMail = "test7@mm.mm",
			PasswordHash = "mmmm",
			FirstName = "TestOne",
			LastName = "Account"
		};
		private static readonly Customer testAccount2 = new Customer () {
			EMail = "test8@mm.mm",
			PasswordHash = "mmmm",
			FirstName = "TestTwo",
			LastName = "Account"
		};

		// YBM - Test code instructions...
		//basic set up
		//... on device 1) run with testAccount1 below
		//... on devince 2) run with testAccount2 below
		//
		//to set up new account
		//... set new credentials in testAccount1&2 above
		//... enable the forceSignUp flag below
		//... run one time per account (i.e. once with testAccount1, once with testAccount2
		//... displabe the forceSignUp flag below
		private static readonly Customer testAccount = testAccount1;
		//private static readonly Customer testAccount = testAccount2;
		private static readonly Customer adversaryAccount = testAccount == testAccount1 ? testAccount2 : testAccount1;
		// true forces to signing in with the test account (otherwise will use last SessionToken
		private const bool forceSignIn = true;
		// true forces SignUp (use this once to create your test account
		private const bool forceSignUp = false;



		public static GameState State { get; set; }




		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		// Represents the player
		Player player;
		// Keyboard states used to determine key presses
		KeyboardState currentKeyboardState;
		KeyboardState previousKeyboardState;
		// Gamepad states used to determine button presses
		GamePadState currentGamePadState;
		GamePadState previousGamePadState;
		TouchCollection previousTouches;
		TouchCollection currentTouches;
		ParallaxingBackground ground;
		ParallaxingBackground buildings;
		ParallaxingBackground bushes;
		ParallaxingBackground clouds1;
		ParallaxingBackground clouds2;
		Texture2D wallTexture, topWallCapTexture, bottomWallCapTexture, 
			playerTexture, groundBottom, gameOverTexture, scoreBoardTexture, scoreTexture, highScoreTexture;
		List<Wall> walls = new List<Wall> ();
		// The rate at which the walls appear
		double wallSpanTime, previousWallSpawnTime;
		// A random number generator
		Random random = new Random ();
		//Number that holds the player score
		int score;
		// The font used to display UI elements
		SpriteFont font;
		bool accelActive;
		int wallHeight;
		Rectangle bottomGroundRect;
		int scoreBoardPadding = 0;

		#endregion

		#region Initialization

		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this) {
				#if __OUYA__
				SupportedOrientations = DisplayOrientation.LandscapeLeft |  DisplayOrientation.LandscapeRight,
				#else 
				SupportedOrientations = DisplayOrientation.Portrait,
				#endif
				IsFullScreen = true,
			};
			
			Content.RootDirectory = "Content";

		}

		/// <summary>
		/// Overridden from the base Game.Initialize. Once the GraphicsDevice is setup,
		/// we'll use the viewport to initialize some values.
		/// </summary>
		protected override void Initialize ()
		{
			State = GameState.Menu;
			wallHeight = Math.Min (GraphicsDevice.Viewport.Height, MaxWallheight);
			bottomGroundRect = new Rectangle (0, wallHeight + 1, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height - MaxWallheight);
			player = new Player ();
			ground = new ParallaxingBackground ();
			buildings = new ParallaxingBackground ();
			bushes = new ParallaxingBackground ();
			clouds1 = new ParallaxingBackground ();
			clouds2 = new ParallaxingBackground ();


			toast = new Toast (this);
			overlay = new UIOverlay (this);


			youBetMeInitialize ();

			base.Initialize ();

		}

		UIOverlay overlay;
		Toast toast;

		private void youBetMeInitialize ()
		{
			ybmUpdateOverlayMode ();

			// Initialization 
			Youbetme.DataObjects.Content.Init (Configuration.CdnUri);

			//#error Provide your api token and secret below
			// To obtain a valid apie token and secret sign up at http://dev.youbetme.com/portal
			// Then contact a Youbetme representative or sent email to support@youbetme.com to request access to the apie
			// Once your accoutn has been granted access, use your credentails to init the SuperProxy below
			SuperProxy.Init (Configuration.ServiceUri, "joe@joebooth.com", "YBMSuperSecretPassword");

			if (forceSignIn) {
				youBetMeTempSignin ();
				return;
			}

			// load from saved token if it exists
			toast.String = "Attempting to log user in from token";
			LoginHelper.GetCustomerFromTokenAsync (SavedData.Token,
				() => {
					toast.String = "Signed In User From Save: " + LoginHelper.LocalCustomer.Name;
					ybmPingServerAndHandleBets ();
				},
				(tokenErrorMsg) => {
					toast.String = "GetCustomerFromTokenAsync Failed: " + tokenErrorMsg;
					// No valid token in saved Data (most likley the user has not yet logged in)
					youBetMeTempSignin ();
				}
			);

		}


		private void youBetMeTempSignin ()
		{
			// TODO create UI to handle SignUp and Login
			//
			// For now we'll uses a fake id...
			// if false will login the fake, true will SignUp the fake account
			if (forceSignUp == false) {
				toast.String = "Attemting Login: " + testAccount.EMail;
				LoginHelper.LoginAsync (testAccount.EMail, testAccount.PasswordHash,
					() => {
						toast.String = "Login Success: " + testAccount.EMail;
						SavedData.Token = LoginHelper.SessionToken;
						ybmPingServerAndHandleBets ();
					},
					(errorMsg) => {
						//Console.WriteLine (errorMsg);
						toast.String = "Login Failed: " + errorMsg;
					});
			} else {
				toast.String = "SignUp: " + testAccount.EMail;
				LoginHelper.SignUpAsync (testAccount.FirstName, testAccount.LastName, testAccount.EMail, testAccount.PasswordHash,
					() => {
						toast.String = "SignUp Success: " + testAccount.EMail;
						SavedData.Token = LoginHelper.SessionToken;
						ybmPingServerAndHandleBets ();
					},
					(errorMsg) => {
						//Console.WriteLine (errorMsg);
						toast.String = "SignUp Failed: " + errorMsg;
					});
			}
		}



		int scoreScale = 3;

		/// <summary>
		/// Load your graphics content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be use to draw textures.
			spriteBatch = new SpriteBatch (graphics.GraphicsDevice);
			
			// TODO: use this.Content to load your game content here eg.
			playerTexture = Content.Load<Texture2D> ("player");

			player.Initialize (playerTexture, new Vector2 (graphics.GraphicsDevice.Viewport.Width / 3, graphics.GraphicsDevice.Viewport.Height / 2));
			wallTexture = Content.Load<Texture2D> ("pipe");
			topWallCapTexture = Content.Load<Texture2D> ("pipeTopCap");
			bottomWallCapTexture = Content.Load<Texture2D> ("pipeBottomCap");

			font = Content.Load<SpriteFont> ("gameFont");

			groundBottom = Content.Load<Texture2D> ("bottomGround");
			ground.Initialize (Content, "ground", wallHeight, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -GamePhysics.WallSpeed, false);
			clouds1.Initialize (Content, "clouds1", 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -.25f, true);
			clouds2.Initialize (Content, "clouds2", 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -(GamePhysics.WallSpeed + .5f), true);
			bushes.Initialize (Content, "bushes", wallHeight, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -1, false, true);
			buildings.Initialize (Content, "buildings", wallHeight, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -.5f, false, true);

			Number.Initialize (Content);
			gameOverTexture = Content.Load<Texture2D> ("gameOver");
			scoreBoardTexture = Content.Load<Texture2D> ("scoreBackground");
			highScoreTexture = Content.Load<Texture2D> ("highScore");
			scoreTexture = Content.Load<Texture2D> ("score");
			gameOverPosition.X = (GraphicsDevice.Viewport.Width - (gameOverTexture.Width * 8)) / 2;
			scoreBoardRect.X = (int)gameOverPosition.X;
			scoreBoardRect.Width = gameOverTexture.Width * 8;
			scoreBoardPadding = gameOverTexture.Height;
			scoreBoardRect.Height = (scoreBoardPadding * 6) + highScoreTexture.Height + scoreTexture.Height + (int)(Number.Height * scoreScale * 2);

			overlay.Initialize (font);
			toast.Initialize (font);

			Reset ();
		}

		public void Reset ()
		{
			State = GameState.Menu;
			wallSpanTime = GamePhysics.StartWallSpawnRate;
			player.Active = true;
			player.Health = 1;
			maxGap = GamePhysics.MaximumGapSize;
			score = 0;
			walls.Clear ();

			var playerPosition = new Vector2 (graphics.GraphicsDevice.Viewport.Width / 3, graphics.GraphicsDevice.Viewport.Height / 2);
			player.Position = playerPosition;

		}

		#endregion

		#region YouBetMeLogic

		void ybmPingServerAndHandleBets ()
		{
			//ybmUpdateOverlayMode ();
			if (LoginHelper.UserIsLoggedIn) {
				BetsHelper.PingServerAndHandleBets (
					() => {
						ybmUpdateOverlayMode ();
						if (overlay.Mode == UIOverlayMode.NotYourGo
						    || overlay.Mode == UIOverlayMode.NoBetsInProgress) {
							// TODO: switch to using notifications to recive updates
							// DO NOT RELEASE WITH THIS PATTERN
							// (it will throttle the server and will have to disable your app)
							ybmPingServerAndHandleBets ();
						}
					});
			}

		}

		void ybmUpdateOverlayMode ()
		{
			if (LoginHelper.UserIsLoggedIn == false) {
				overlay.Mode = UIOverlayMode.LoggingInToYBMServer;
				return;
			}
			if (BetsHelper.BetListsAreValid) {
				if (BetsHelper.BetsImLoosing.Count > 0) {
					if (State == GameState.Playing)
						overlay.Mode = UIOverlayMode.None;
					else
						overlay.Mode = UIOverlayMode.YourGo;
				} else if (BetsHelper.BetsImWinning.Count > 0) {
					overlay.Mode = UIOverlayMode.NotYourGo;
				} else {
					overlay.Mode = UIOverlayMode.NoBetsInProgress;
				}
			} else {
				overlay.Mode = UIOverlayMode.WaitingToSyncWithYBMServer;
			}
		}

		void ybmGameOver ()
		{
			if (BetsHelper.BetsImLoosing.Count > 0) {
				// check to see if i've beaten any bets i'm loosing
				BetsHelper.UpdateBetsWithNewScore (score, (successMessage) => {
					if (successMessage != null)
						toast.String = successMessage;
					ybmPingServerAndHandleBets ();
				});
			}

			if (BetsHelper.BetsImLoosing.Count == 0
			    && BetsHelper.BetsImWinning.Count == 0) {
				// create new bet with adversary
				if (LoginHelper.UserIsLoggedIn) {
					FriendsHelper.GetIdFromEmailAsync (adversaryAccount.EMail,
						(friendId) => {
							toast.String = "Found Friend Id " + friendId;
							BetsHelper.BetThisPerson (friendId, score, "Respect");
							//ybmUpdateOverlayMode ();
						},
						(errorMsg) => {
							toast.String = "Error obtaining friend id: " + errorMsg;
						});
				}
			}



		}


		#endregion

		#region Update

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			base.Update (gameTime);

			// Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
			previousGamePadState = currentGamePadState;
			previousKeyboardState = currentKeyboardState;
			previousTouches = currentTouches;

			// Read the current state of the keyboard and gamepad and store it
			currentKeyboardState = Keyboard.GetState ();
			currentGamePadState = GamePad.GetState (PlayerIndex.One);
			currentTouches = TouchPanel.GetState ();


			//Update the player
			var shouldFly = Toggled (); //currentTouches.Any() || currentKeyboardState.IsKeyDown (Keys.Space) || currentGamePadState.IsButtonDown(Buttons.A) ;

			if (overlay.IsScreenAndInputBlocked)
				shouldFly = false;

			if (shouldFly && State == GameState.Menu)
				State = GameState.Playing;
			else if (shouldFly && State == GameState.Score)
				shouldFly = false;
			player.Update (gameTime, shouldFly, wallHeight + 1, State == GameState.Menu);


		
			if (State != GameState.Score) {
				ground.Update ();
				buildings.Update ();
				bushes.Update ();
				clouds1.Update ();
				clouds2.Update ();
			}

			if (State == GameState.Playing) {
				UpdateWalls (gameTime);
				// Update the collision
				UpdateCollision ();
			} else if (State == GameState.Score) {
				UpdateGameOver (gameTime);
				if (gameOverAnimationDuration <= gameOverTimer && Toggled ())
					Reset ();

			}
			toast.Update (gameTime);

			ybmUpdateOverlayMode ();
		}

		bool Toggled (Buttons button)
		{
			return previousGamePadState.IsButtonUp (button) && currentGamePadState.IsButtonDown (button);
		}

		bool ToggledTappped ()
		{
			return !previousTouches.Any () && currentTouches.Any ();
		}

		bool Toggled (Keys key)
		{
			return previousKeyboardState.IsKeyUp (key) && currentKeyboardState.IsKeyDown (key);
		}

		bool Toggled ()
		{
			return ToggledTappped () || Toggled (Buttons.A) || Toggled (Keys.Space);
		}

		int maxGap;
		const int MaxWallheight = 920;

		private void AddWall ()
		{
			maxGap = MathHelper.Clamp (maxGap - 10, GamePhysics.MinimumGapSize, GamePhysics.MaximumGapSize);
			int gapSize = random.Next (GamePhysics.MinimumGapSize, maxGap);
			var gapY = random.Next (100, wallHeight - (gapSize + 100));
			var wall = new Wall ();

			var position = new Vector2 (GraphicsDevice.Viewport.Width + wallTexture.Width / 2, 0);
			// Initialize the animation with the correct animation information
			wall.Initialize (wallTexture, topWallCapTexture, bottomWallCapTexture, new Rectangle ((int)position.X, gapY, wallTexture.Width, gapSize), GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Height);

			walls.Add (wall);
		}

		private void UpdateWalls (GameTime gameTime)
		{
			// Spawn a new enemy enemy every 1.5 seconds
			previousWallSpawnTime += gameTime.ElapsedGameTime.TotalMilliseconds;
			if (previousWallSpawnTime > wallSpanTime) {
				previousWallSpawnTime = 0;
				wallSpanTime -= 200;
				wallSpanTime = Math.Max (wallSpanTime, GamePhysics.MinimumWallSpawnRate);
				// Add an Enemy
				AddWall ();
			}
			var deadWalls = new List<Wall> ();
			foreach (var wall in walls) {
				wall.Update (gameTime);
				if (wall.Position.X < -wall.Width)
					deadWalls.Add (wall);
			}

			foreach (var wall in deadWalls)
				walls.Remove (wall);

		}

		private void UpdateCollision ()
		{
			// Use the Rectangle's built-in intersect function to 
			// determine if two objects are overlapping
			var rectangle1 = player.Rectangle;

			//If it collides with a wall, you die
			foreach (var wall in walls.Where(x=> x.Collides(rectangle1))) {
				gameOver ();
			}

			var points = walls.Sum (x => x.CollectPoints ());
			score += points;

			if (rectangle1.Bottom >= wallHeight) {
				gameOver ();
			}
		}

		double gameOverTimer = 0;
		Vector2 gameOverPosition = Vector2.Zero;
		Rectangle scoreBoardRect = Rectangle.Empty;
		double gameOverAnimationDuration = 500;

		void gameOver ()
		{
			SavedData.HighScore = score;

			ybmGameOver ();

			gameOverTimer = 0;
			player.Health = 0;
			player.Active = false;
			State = GameState.Score;
		}

		void UpdateGameOver (GameTime gameTime)
		{
			gameOverTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
			if (gameOverTimer > gameOverAnimationDuration + 10)
				return;
			var sin = (float)Math.Sin (gameOverTimer * .7 * Math.PI / gameOverAnimationDuration);
			var y = (int)((GraphicsDevice.Viewport.Height / 3) * sin);
			gameOverPosition.Y = y;
			scoreBoardRect.Y = GraphicsDevice.Viewport.Height - y - scoreBoardRect.Height;
		}

		#endregion

		#region Draw

		/// <summary>
		/// This is called when the game should draw itself. 
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>

		protected override void Draw (GameTime gameTime)
		{
			GraphicsDevice.Clear (Color.CornflowerBlue);
			// Start drawing
			spriteBatch.Begin (SpriteSortMode.Deferred, BlendState.AlphaBlend, 
				SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

			buildings.Draw (spriteBatch);
			bushes.Draw (spriteBatch);

			clouds1.Draw (spriteBatch);

			walls.ForEach (x => x.Draw (spriteBatch));


			// Draw the Player
			//uncomment to draw the hitbox.
			//spriteBatch.Draw (groundBottom, player.Rectangle, Color.Red);
			player.Draw (spriteBatch);

			clouds2.Draw (spriteBatch);

			spriteBatch.Draw (groundBottom, bottomGroundRect, Color.White);
			ground.Draw (spriteBatch);


//			// Draw the score
			if (State == GameState.Playing)
				Number.Draw (spriteBatch, score, Number.Alignment.Center, new Rectangle (0, GraphicsDevice.Viewport.TitleSafeArea.Height / 4, GraphicsDevice.Viewport.TitleSafeArea.Width, 0), scoreScale);

			if (State == GameState.Score) {
				spriteBatch.Draw (gameOverTexture, gameOverPosition, null, null, null, 0, new Vector2 (8, 8), Color.White);

				spriteBatch.Draw (scoreBoardTexture, null, scoreBoardRect, null, null, 0, new Vector2 (1, 1), Color.White);

				var y = scoreBoardRect.Top + scoreBoardPadding;
				var x = (GraphicsDevice.Viewport.TitleSafeArea.Width - scoreTexture.Width) / 2;
				spriteBatch.Draw (scoreTexture, new Vector2 (x, y), Color.White);

				y += scoreTexture.Height + scoreBoardPadding;
				Number.Draw (spriteBatch, score, Number.Alignment.Center, new Rectangle (0, y, GraphicsDevice.Viewport.TitleSafeArea.Width, 0), scoreScale);

				y += (Number.Height * scoreScale) + scoreBoardPadding;
				x = (GraphicsDevice.Viewport.TitleSafeArea.Width - highScoreTexture.Width) / 2;
				spriteBatch.Draw (highScoreTexture, new Vector2 (x, y), Color.White);

				y += highScoreTexture.Height + scoreBoardPadding;
				Number.Draw (spriteBatch, SavedData.HighScore, Number.Alignment.Center, new Rectangle (0, y, GraphicsDevice.Viewport.TitleSafeArea.Width, 0), scoreScale);


			}
//			// Draw the player health
//			spriteBatch.DrawString (font, "health: " + player.Health, new Vector2 (GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);


			// Stop drawing
			spriteBatch.End ();


			// TODO: Add your drawing code here
			overlay.Draw (gameTime);
			toast.Draw (gameTime);


			base.Draw (gameTime);
		}

		#endregion
	}
}
