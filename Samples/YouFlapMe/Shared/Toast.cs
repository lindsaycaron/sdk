using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlappyMonkey
{
	public class Toast: DrawableGameComponent
	{
		SpriteBatch spriteBatch;
		SpriteFont font;

		private string _string = "";

		public string String {
			get {
				if (toastTimer < toastTimerThreashold)
					return _string;
				return "";
			}
			set {
				_string = value;
				Console.WriteLine ("Toast: " + value);
				toastTimer = 0;
			}
		}

		Color ToastStringColor {
			get {
				double alpha = (toastTimerThreashold - toastTimer);
				if (alpha < 0) {
					alpha = 0;
				} else
					alpha = alpha < 500 ? (alpha / 500) : 1.0;
				return new Color (new Vector4 ((float)alpha));
			}
		}

		double toastTimer = 0;
		const double toastTimerThreashold = 4000;


		public Toast (Game game)
			: base (game)
		{
		}

		public override void Update (GameTime gameTime)
		{
			toastTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
			base.Update (gameTime);
		}

		public void Initialize (SpriteFont font)
		{
			this.font = font;
			base.Initialize ();
		}

		protected override void LoadContent ()
		{
			spriteBatch = new SpriteBatch (GraphicsDevice);
		}

		public override void Draw (GameTime gameTime)
		{
			spriteBatch.Begin ();
			spriteBatch.DrawString (font, String, new Vector2 (GraphicsDevice.Viewport.TitleSafeArea.X + 10, GraphicsDevice.Viewport.TitleSafeArea.Y + 40), ToastStringColor);
			spriteBatch.End ();
		}


	}
}

