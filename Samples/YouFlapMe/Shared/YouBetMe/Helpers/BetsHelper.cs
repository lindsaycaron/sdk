using System;
using System.Threading.Tasks;
using Youbetme.Communication;
using Youbetme.DataObjects;
using Youbetme.Proxies;
using Youbetme.Validation;
using System.Collections.Generic;
using System.Linq;

namespace Youbetme.Helpers
{
	public class BetsHelper
	{
		// we use this string in the terms to know that it is for this app


		//public static void BetAllMyFriendsAsync (int scoreToBeat, string prizeOrForfit){}


		public static void BetThisPerson (string CustomerToBetId, int scoreToBeat, string prizeOrForfit)
		{
			// Note: Create the opponent team
			var opponentTeam = new Team ();
			opponentTeam.Side = TeamSide.Opponent;

			// Add the first opponent
			var opponent1 = new Member ();
			opponent1.CustomerId = CustomerToBetId;
			opponentTeam.Members.Add (opponent1);

			// Opptionally add another opponent
			//var opponent2 = new Member ();
			//opponent2.CustomerId = friends [2].Id;
			//opponentTeam.Members.Add (opponent2);

			sendTheBet (opponentTeam, scoreToBeat, prizeOrForfit);
		}

		private static void sendTheBet (Team opponentTeam, int scoreToBeat, string prizeOrForfit)
		{
			var bet = new Bet ();
			bet.CustomerId = LoginHelper.LocalCustomer.Id;
			bet.BetTitle = "Beat Score of " + scoreToBeat + " In YouFlapMe";
			bet.BetTerms = getTermsString (scoreToBeat, 0);
			bet.BetOutcome = prizeOrForfit;
			//bet.Context ["AppId"] = LoginHelper.AppId;
			//bet.Context ["ScoreToBeat"] = scoreToBeat;
			//bet.Context ["AcceptBetDueDate"] = DateTime.UtcNow.AddDays (1);


			// Note: Create the bet maker team
			var betMakerTeam = new Team ();
			betMakerTeam.Side = TeamSide.BetMaker;

			// Add the bet maker to the team
			var betMaker = new Member ();
			betMaker.CustomerId = LoginHelper.LocalCustomer.Id;
			betMaker.BetStatus = BetStatus.Active;
			betMakerTeam.Members.Add (betMaker);

			// Optionally add a teammate
			//var teammate = new Member ();
			//teammate.CustomerId = friends [0].Id;
			//betMakerTeam.Members.Add (teammate);

			bet.Teams.Add (betMakerTeam);

			// Note: Add the opponent team
			bet.Teams.Add (opponentTeam);

			// Note: Set "uploadInProgress" to true if you're about to submit a photo attached to the bet
			var response = BetProxy.CreateNewBet (LoginHelper.SessionToken, bet, true);
			if (response.Status != Status.Success) {
				// TODO: Display response.Message
				return;
			}
			BetListsAreValid = false;
		}



		private static bool PingInProgress = false;
		private static int pingPageNum;
		private static Action onSuccess;

		public static void PingServerAndHandleBets (Action onSuccess)
		{
			if (PingInProgress)
				return;
			BetsHelper.onSuccess = onSuccess;
			inProgressBetsImWinning = new List<Bet> ();
			inProgressBetsImLoosing = new List<Bet> ();
			pingServerAndHandleBetsForPage (0);
		}

		private static void pingServerAndHandleBetsForPage (int pageNumber)
		{
			BetProxy.GetBetsCompleted += handleGetBetsCompleted;

			var filter = new BetFilter ();
			filter.Statuses = new BetStatus[] { BetStatus.Pending, BetStatus.Active };
			filter.Types = new BetType[] { BetType.Custom, BetType.Sports };
			filter.CustomerId = LoginHelper.LocalCustomer.Id;
			filter.PageNumber = pageNumber;
			filter.PageSize = 50;

			pingPageNum = pageNumber;

			BetProxy.GetBetsAsync (LoginHelper.SessionToken, filter, null);
		}

		public static List<Bet> BetsImWinning = new List<Bet> ();
		public static List<Bet> BetsImLoosing = new List<Bet> ();
		public static bool BetListsAreValid = false;
		static List<Bet> inProgressBetsImWinning = new List<Bet> ();
		static List<Bet> inProgressBetsImLoosing = new List<Bet> ();

		static void handleGetBetsCompleted (Response<List<Bet>> response, object asyncState = null)
		{
			BetProxy.GetBetsCompleted -= handleGetBetsCompleted;
			if (response.Status != Status.Success) {
				// TODO: display message
				PingInProgress = false;
				return;
			}

			foreach (var bet in response.Result) {
				if (BetWasFromThisApp (bet)) {
					if (GetTimeLeft (bet) < TimeSpan.Zero) {
						// handle bet time is up!
						closeBet (bet, true);
						break;
					}
						
					//bool iAmBetMaker = bet.ContextCustomerTeamSide == TeamSide.BetMaker;
					bool iAmBetMaker = bet.GetTeam (LoginHelper.LocalCustomer.Id) == bet.GetTeamBySide (TeamSide.BetMaker);
					
					bool setToActive = false;
					if (bet.Status == BetStatus.Pending
					    && iAmBetMaker == false)
						setToActive = true;

					if (setToActive) {
						// convert to active
						BetProxy.SetMemberStatusAsync (LoginHelper.SessionToken, LoginHelper.LocalCustomer.Id, bet.Id, BetStatus.Active, null);
					}

					bool listAsBetImLoosing = false;
					bool listAsBetImWinning = false;
					if (bet.Status == BetStatus.Active
					    && iAmBetMaker == false)
						listAsBetImLoosing = true;
					if (bet.Status == BetStatus.Active
					    && iAmBetMaker == true)
						listAsBetImWinning = true;
					if (bet.Status == BetStatus.Pending
					    && iAmBetMaker == false)
						listAsBetImLoosing = true;
					if (bet.Status == BetStatus.Pending
					    && iAmBetMaker == true)
						listAsBetImWinning = true;
					int streak = GetStreakFromBet (bet);
					int betToBeat = GetScoreFromBet (bet);

					if (listAsBetImLoosing) {
						Console.WriteLine ("BetImLoosing: '" + betPreviewText (bet) + "'. ToBeat=" + betToBeat + ". Streak=" + streak + ". ID:" + bet.Id);
						inProgressBetsImLoosing.Add (bet);
					}
					if (listAsBetImWinning) {
						Console.WriteLine ("BetImWinning: '" + betPreviewText (bet) + "'. ToBeat=" + betToBeat + ". Streak=" + streak + ". ID:" + bet.Id);
						inProgressBetsImWinning.Add (bet);
					}
				}
			}

			// Public bets sucessfully retrieved
			if (response.Result.Count == 50)
				pingServerAndHandleBetsForPage (pingPageNum + 1);
			else {
				// completed pinging server for bets
				PingInProgress = false;
				BetsImLoosing = inProgressBetsImLoosing;
				BetsImWinning = inProgressBetsImWinning;
				BetListsAreValid = true;
				if (onSuccess != null)
					onSuccess ();
			}
		}

		static private string betPreviewText (Bet bet)
		{
			var lightBet = bet.GetLightObject (LoginHelper.LocalCustomer.Id);
			return lightBet.PreviewText;
		}

		public static int GetScoreFromBet (Bet bet)
		{
			string[] tokens = bet.BetTerms.Split (new string[] { termsBegin, termsMiddle, termsEnd }, StringSplitOptions.RemoveEmptyEntries);
			int score = 1;
			try {
				score = (int)Convert.ToInt64 (tokens [0]);
			} catch (Exception ex) {
			}
			return score;
		}

		public static int GetStreakFromBet (Bet bet)
		{
			string[] tokens = bet.BetTerms.Split (new string[] { termsBegin, termsMiddle, termsEnd }, StringSplitOptions.RemoveEmptyEntries);
			int streak = 1;
			try {
				streak = (int)Convert.ToInt64 (tokens [1]);
			} catch (Exception ex) {
			}
			return streak;
		}

		const string termsBegin = "Score more than ";
		const string termsMiddle = " within 24 hours. Your streak is ";
		const string termsEnd = ".";

		private static string getTermsString (int score, int streak)
		{
			var str = termsBegin + score + termsMiddle + streak + termsEnd;
			return str;
		}

		public static bool BetWasFromThisApp (Bet bet)
		{
			//TODO: Implement more robust way of knowing if this app created this bet 
			// (see FormsExample for example of Parse.com)
			string[] tokens = bet.BetTerms.Split (new string[] { termsBegin, termsMiddle, termsEnd }, StringSplitOptions.RemoveEmptyEntries);
			if (tokens [0] == bet.BetTerms)
				return false;
			else
				return true;
		}

		public static TimeSpan GetTimeLeft (Bet bet)
		{
			DateTime timeLimit = bet.BetCreated.ToUniversalTime ().AddDays (1);
			TimeSpan timeLeft = timeLimit - DateTime.UtcNow;
			return timeLeft;
		}




		static Action<string> UpdateBetsWithNewScoreOnSuccess = null;

		public static void UpdateBetsWithNewScore (int newScore, Action<string> onSuccess)
		{
			UpdateBetsWithNewScoreOnSuccess = onSuccess;

			var betList = inProgressBetsImLoosing;
			foreach (var bet in betList) {

				// for debugging
				//newScore = GetScoreFromBet (bet) + 1;

				if (newScore > GetScoreFromBet (bet)) {
					var newBet = new Bet ();
					newBet.BetOutcome = bet.BetOutcome;
					newBet.BetTerms = getTermsString (newScore, GetStreakFromBet (bet) + 1);
					newBet.BetTitle = "Beat Score of " + newScore + " In YouFlapMe";
					newBet.CustomerId = LoginHelper.LocalCustomer.Id;
					var betMakerTeam = bet.GetTeamBySide (TeamSide.BetMaker);
					var betOpponentTeam = bet.GetTeamBySide (TeamSide.Opponent);
					var betSpectatorTeam = bet.GetTeamBySide (TeamSide.Audience);
					var newBetMakerTeam = new Team ();
					var newBetOpponentTeam = new Team ();
					newBetMakerTeam.Side = TeamSide.BetMaker;
					newBetOpponentTeam.Side = TeamSide.Opponent;
					foreach (Member member in betMakerTeam.Members) {
						var newMember = new Member ();
						newMember.CustomerId = member.CustomerId;
						newMember.BetStatus = BetStatus.Active;
						newBetOpponentTeam.Members.Add (newMember);
					}
					foreach (Member member in betOpponentTeam.Members) {
						var newMember = new Member ();
						newMember.CustomerId = member.CustomerId;
						newMember.BetStatus = BetStatus.Active;
						if (member.CustomerId == LoginHelper.LocalCustomer.Id) {
							newBetMakerTeam.Members.Add (newMember);
						} else {
							newBetOpponentTeam.Members.Add (newMember);
						}
					}
					newBet.Teams.Add (newBetMakerTeam);
					newBet.Teams.Add (newBetOpponentTeam);
					if (betSpectatorTeam != null)
						newBet.Teams.Add (betSpectatorTeam);


					closeAndRejectBet (bet);

					BetProxy.CreateBetCompleted += handleCreateBetDone;
					BetProxy.CreateNewBetAsync (LoginHelper.SessionToken, newBet, false, null);

					BetListsAreValid = false;
				}
			}
		}

		private static void handleCreateBetDone (Response<Bet> item, object asyncState = null)
		{
			BetProxy.CreateBetCompleted -= handleCreateBetDone;

			if (UpdateBetsWithNewScoreOnSuccess != null)
				UpdateBetsWithNewScoreOnSuccess ("NewBet successfully sent!");
		}

		private static void closeAndRejectBet (Bet bet)
		{
			// TODO: Provide bet Id below
			var betMakerTeam = bet.GetTeamBySide (TeamSide.BetMaker);
			var opponentTeam = bet.GetTeamBySide (TeamSide.Opponent);

			// Note: Det winner and loser
			betMakerTeam.Status = TeamStatus.Won;
			opponentTeam.Status = TeamStatus.Lost;

			// Note: Set whether the bet has been paid or welched by changing its status
			bet.Status = BetStatus.Rejected;

			BetProxy.CloseBetAsync (LoginHelper.SessionToken, bet, null);
		}

		private static void closeBet (Bet bet, bool localUserWon)
		{
			// TODO: Provide bet Id below
			var betMakerTeam = bet.GetTeamBySide (TeamSide.BetMaker);
			var opponentTeam = bet.GetTeamBySide (TeamSide.Opponent);

			// Note: Det winner and loser
			betMakerTeam.Status = TeamStatus.Won;
			opponentTeam.Status = TeamStatus.Lost;

			// Note: Set whether the bet has been paid or welched by changing its status
			bet.Status = BetStatus.Paid;

			BetProxy.CloseBetAsync (LoginHelper.SessionToken, bet, null);
		}

	}

}