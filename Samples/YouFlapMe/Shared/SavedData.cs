using System;
using System.IO.IsolatedStorage;
using System.IO;

namespace FlappyMonkey
{
	public static class SavedData
	{
		static SavedData ()
		{
			_score = savedData.Score;
		}

		static int _score;

		public static int HighScore {
			get {
				return _score;
			}
			set {
				if (value <= _score)
					return;
				_score = value;
				savedData.Score = _score;
				savedData.Save ();
			}
		}

		public static string Token {
			get {
				return savedData.Token;
			}
			set {
				if (value != savedData.Token) {
					savedData.Token = value;
					savedData.Save ();
				}
			}
		}

		static saveData savedData = new saveData ("savedData");

		private class saveData
		{
			public int Score = 0;
			public string Token = "";
			private IsolatedStorageFile isoStore;
			private string fileName;

			public void Save ()
			{
				using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream (fileName, FileMode.Create, isoStore)) {
					using (StreamWriter writer = new StreamWriter (isoStream)) {
						writer.WriteLine (Token);
						writer.WriteLine ((Int64)Score);
					}
				}
			}

			public saveData (string saveDataFileName)
			{
				isoStore = IsolatedStorageFile.GetStore (IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
				fileName = saveDataFileName;
				try {
					if (isoStore.FileExists (fileName)) {
						using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream (fileName, FileMode.Open, isoStore)) {
							using (StreamReader reader = new StreamReader (isoStream)) {
								string str;
								str = reader.ReadLine ();
								if (str != null)
									Token = str;
								str = reader.ReadLine ();
								if (str != null)
									Score = (int)Convert.ToInt64 (str);
								//int.TryParse (reader.ReadToEnd (), out this.Score);
								return;
							}
						}
					}
				} catch (Exception ex) {
					Console.WriteLine (ex);
				}
				return;
			}
		}

	}
}

