package com.youbetme.androidsample;

import java.util.ArrayList;
import java.util.Date;

import org.json.*;

import com.youbetme.androidsample.dataobjects.*;
import com.youbetme.androidsample.proxies.Proxy;
import com.youbetme.androidsample.tasks.*;
import com.youbetme.androidsamples.R;

import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.util.Base64;
import android.view.Menu;
import android.view.MenuItem;

public class MainActivity extends ActionBarActivity {

	private static String Token;
	private static JSONObject ContextCustomer;
	
	 @Override
	    protected void onCreate(Bundle savedInstanceState) {
	        super.onCreate(savedInstanceState);
	        setContentView(R.layout.activity_main);
	        
	    	// TODO: Provide your API token and secret below
	    	// Note: To obtain a valid API token and secret sign up at http://dev.youbetme.com/portal
	    	// Then contact a Youbetme representative or sent email to support@youbetme.com to request access to the API
	    	// Once your account has been granted access, use your credentails (email and password) to init the SuperProxy below
	        Proxy.TOKEN = "api-token-here";
	        Proxy.SECRET = "api-secret-here";
	    }

	    @Override
	    public boolean onCreateOptionsMenu(Menu menu) {
	        // Inflate the menu; this adds items to the action bar if it is present.
	        getMenuInflater().inflate(R.menu.main, menu);
	        return true;
	    }

	    @Override
	    public boolean onOptionsItemSelected(MenuItem item) {
	        // Handle action bar item clicks here. The action bar will
	        // automatically handle clicks on the Home/Up button, so long
	        // as you specify a parent activity in AndroidManifest.xml.
	        int id = item.getItemId();
	        if (id == R.id.action_settings) {
	            return true;
	        }
	        return super.onOptionsItemSelected(item);
	    }
	    
	    private void SignUp(){
	    	try {
	    		JSONObject customerJSON = new JSONObject();
		            
	    		// TODO: Set first, last names, email and password
				customerJSON.put("FirstName", "first-name-here");
				customerJSON.put("LastName", "last-name-here");	
				customerJSON.put("Email", "email-here");
				customerJSON.put("Password", "plain-text-password-here");
					
				SignUpTask task = new SignUpTask(); 
					
				String[] response = task.execute(customerJSON.toString()).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
					
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				customerJSON = responseJSON.getJSONObject("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}	
	    }

	    private void LogIn(){
	    	try {	
				LogInTask task = new LogInTask(); 
				
				Credentials credentials = new Credentials();
				// TODO: Set username or email and password below
				credentials.setUsernameOrEmail("username-or-email-here");
				credentials.setPassword("password-here");
				
				String[] response = task.execute(credentials).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONObject loginResult = responseJSON.getJSONObject("Result");
				
				Token = loginResult.getString("Token");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}	
	    }
	    
	    private void LogInWithFacebook(){
	    	try {	
	    		LogInWithFacebookTask task = new LogInWithFacebookTask(); 
				
	    		JSONObject socialNetworkJSON = new JSONObject();
	            
	    		socialNetworkJSON.put("Type", SocialNetworkType.Facebook);
	    		socialNetworkJSON.put("Permission", SocialNetworkPermission.Read);	
	    		// TODO: Set below Facebook access token
	    		socialNetworkJSON.put("Token", "Facebook-access-token-here");
	    		// TODO: Set below token expiration date
	    		socialNetworkJSON.put("ExpirationDate", new Date());
				
				String[] response = task.execute(socialNetworkJSON.toString()).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				Token = responseJSON.getString("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}	
	    }

	    private void GetCustomerFromSession(){
	    	try {	
	    		GetCustomerFromSessionTask task = new GetCustomerFromSessionTask(Token); 
				
	    		String response = task.execute().get();
					
				JSONObject responseJSON = new JSONObject(response);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				ContextCustomer = responseJSON.getJSONObject("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}	
	    }
	    
	    private void UpdateCustomer() {
	    	
	    	try {
	    		
	    		GetCustomerFromSessionTask getTask = new GetCustomerFromSessionTask(Token); 
				
	    		String getResponse = getTask.execute().get();
					
				JSONObject getResponseJSON = new JSONObject(getResponse);
					
				Status status = Status.values()[(Integer)getResponseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONObject customerJSON = getResponseJSON.getJSONObject("Result");
				
				// TODO: Set below first name
				customerJSON.put("FirstName", "first-name-here");
				
				UpdateCustomerTask updateTask = new UpdateCustomerTask(Token);
				
				String[] updateResponse = updateTask.execute(customerJSON.toString()).get();
				
				JSONObject updateResponseJSON = new JSONObject(updateResponse[0]);
				
				status = Status.values()[(Integer)updateResponseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
				
				customerJSON = updateResponseJSON.getJSONObject("Result");
				
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }
	    
	    private void GetCustomerFromId(){
	    	try {	
	    		GetCustomerFromIdTask task = new GetCustomerFromIdTask(Token); 
				
	    		// TODO: Set below customer Id
	    		String[] response = task.execute("customer-id-here").get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONObject customerJSON = responseJSON.getJSONObject("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}	
	    }

	    private void GetRecentFriends()
	    {
	    	try {	
	    		GetRecentFriendsTask task = new GetRecentFriendsTask(Token); 
				
	    		JSONObject filterJSON = new JSONObject();
	    		filterJSON.put("CustomerId", ContextCustomer.get("Id"));
	    		// Note: Specify page number and size
	    		filterJSON.put("PageNumber", 0);
	    		filterJSON.put("PageSize", 50);
	    		// Note: The parameter below is optional and is needed in case you want to search by first, last name or email
	       		// filterJSON.put("Criteria", "Sarah Johnson");
	    		
	    		String[] response = task.execute(filterJSON.toString()).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONArray friendsJSON = responseJSON.getJSONArray("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }
	    
	    private void GetFriends()
	    {
	    	try {	
	    		GetFriendsTask task = new GetFriendsTask(Token); 
				
	    		JSONObject filterJSON = new JSONObject();
	    		filterJSON.put("CustomerId", ContextCustomer.get("Id"));
	    		// Note: Specify page number and size
	    		filterJSON.put("PageNumber", 0);
	    		filterJSON.put("PageSize", 50);
	    		// Note: The parameter below is optional and is needed in case you want to search by first, last name or email
	       		// filterJSON.put("Criteria", "Sarah Johnson");
	    		
	    		String[] response = task.execute(filterJSON.toString()).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONArray friendsJSON = responseJSON.getJSONArray("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }
	    
	    private void GetFollowers()
	    {
	    	try {	
	    		GetFollowersTask task = new GetFollowersTask(Token); 
				
	    		JSONObject filterJSON = new JSONObject();
	    		filterJSON.put("CustomerId", ContextCustomer.get("Id"));
	    		// Note: Specify page number and size
	    		filterJSON.put("PageNumber", 0);
	    		filterJSON.put("PageSize", 50);
	    		// Note: The parameter below is optional and is needed in case you want to search by first, last name or email
	       		// filterJSON.put("Criteria", "Sarah Johnson");
	    		
	    		String[] response = task.execute(filterJSON.toString()).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONArray followersJSON = responseJSON.getJSONArray("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }
	    
	    private void GetFollowees()
	    {
	    	try {	
	    		GetFolloweesTask task = new GetFolloweesTask(Token); 
				
	    		JSONObject filterJSON = new JSONObject();
	    		filterJSON.put("CustomerId", ContextCustomer.get("Id"));
	    		// Note: Specify page number and size
	    		filterJSON.put("PageNumber", 0);
	    		filterJSON.put("PageSize", 50);
	    		// Note: The parameter below is optional and is needed in case you want to search by first, last name or email
	       		// filterJSON.put("Criteria", "Sarah Johnson");
	    		
	    		String[] response = task.execute(filterJSON.toString()).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONArray followeesJSON = responseJSON.getJSONArray("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }
	    
	    private void PerformCustomerSearch()
	    {
	    	try {	
	    		PerformCustomerSearchTask task = new PerformCustomerSearchTask(Token); 
				
	    		JSONObject filterJSON = new JSONObject();
	    		filterJSON.put("CustomerId", ContextCustomer.get("Id"));
	    		// Note: Specify page number and size
	    		filterJSON.put("PageNumber", 0);
	    		filterJSON.put("PageSize", 50);
	    		// Note: The parameter below is required
	    		filterJSON.put("Criteria", "Sarah Johnson");
	    		
	    		String[] response = task.execute(filterJSON.toString()).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONArray customersJSON = responseJSON.getJSONArray("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }
	    
	    private void GetBetSuggestions()
	    {
	    	try {	
	    		GetBetSuggestionsTask task = new GetBetSuggestionsTask(Token); 
				
	    		String response = task.execute().get();
					
				JSONObject responseJSON = new JSONObject(response);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONArray suggestionsJSON = responseJSON.getJSONArray("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }
	    
	    private void CreateBet()
	    {
	    	try {	
	    		JSONObject betJSON = new JSONObject();
	    		
	    		// Specify the bet maker Id
	    		betJSON.put("CustomerId", ContextCustomer.get("Id"));
	    		
	    		// Specify bet type 
	    		betJSON.put("Type", BetType.Custom);
	    		
	    		betJSON.put("BetTitle", "My First Bet");
	    		betJSON.put("BetTerms", "Gets created");
	    		betJSON.put("BetOutcome", "High Five");
	    		
	    		JSONArray teams = new JSONArray();
	    		
	    		// Create the bet maker team
	    		JSONObject betMakerTeamJSON = new JSONObject();
	    		betMakerTeamJSON.put("Side", TeamSide.BetMaker);
	    		JSONArray members = new JSONArray();
	    		
	    		// Add the bet maker to the team
	    		JSONObject betMakerJSON = new JSONObject();
	    		betMakerJSON.put("CustomerId", ContextCustomer.get("Id"));
	    		betMakerJSON.put("BetStatus", BetStatus.Active);
	    		members.put(betMakerJSON);
	    		
	    		// Optionally add a teammate
	    		JSONObject teammateJSON = new JSONObject();
	    		teammateJSON.put("CustomerId", "teammate-customer-id-here");
	    		members.put(teammateJSON);
	    		
	    		betMakerTeamJSON.put("Members", members);
	    		teams.put(betMakerTeamJSON);
	    		
	    		// Create the opponent team
	    		JSONObject opponentTeamJSON = new JSONObject();
	    		opponentTeamJSON.put("Side", TeamSide.Opponent);
	    		members = new JSONArray();
	    		
	    		// Add the first opponent
	    		JSONObject opponent1JSON = new JSONObject();
	    		opponent1JSON.put("CustomerId", "opponent1-id-here");
	    		members.put(opponent1JSON);
	    		
	    		// Optionally add another opponent
	    		JSONObject opponent2JSON = new JSONObject();
	    		opponent2JSON.put("CustomerId", "opponent2-id-here");
	    		members.put(opponent2JSON);
	    		
	    		opponentTeamJSON.put("Members", members);
	    		teams.put(opponentTeamJSON);
	    		
	    		// Optionally add a team of spectators
	    		JSONObject audienceJSON = new JSONObject();
	    		audienceJSON.put("Side", TeamSide.Audience);
	    		members = new JSONArray();
	    		
	    		// Add as many spectators as needed
	    		JSONObject spectatorJSON = new JSONObject();
	    		spectatorJSON.put("CustomerId", "spectator-id-here");
	    		members.put(spectatorJSON);
	    		
	    		audienceJSON.put("Members", members);
	    		teams.put(audienceJSON);
	    		
	    		betJSON.put("Teams", teams);
	    		
	    		// Optionally add bet location
	    		JSONObject locationJSON = new JSONObject();
	    		//locationJSON.put("Latitude", 0);
	    		//locationJSON.put("Longitude", 0);
	    		//locationJSON.put("Altitude", 0);
	    		betJSON.put("Location", locationJSON);
	    		
	    		CreateBetTask task = new CreateBetTask(Token); 
	    		
	    		String[] response = task.execute(betJSON.toString()).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
				
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				betJSON = responseJSON.getJSONObject("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }

	    private void GetPendingBets()
	    {
	    	try {	
	    		GetBetsTask task = new GetBetsTask(Token); 
				
	    		JSONObject filterJSON = new JSONObject();
	    		filterJSON.put("CustomerId", ContextCustomer.get("Id"));
	    		JSONArray statuses = new JSONArray();
	    		statuses.put(BetStatus.Pending);
	    		filterJSON.put("Statuses", statuses);
	    		filterJSON.put("PageNumber", 0);
	    		filterJSON.put("PageSize", 50);
	    		
	    		String[] response = task.execute(filterJSON.toString()).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONArray betsJSON = responseJSON.getJSONArray("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }
	    
	    private void GetActiveBets()
	    {
	    	try {	
	    		GetBetsTask task = new GetBetsTask(Token); 
				
	    		JSONObject filterJSON = new JSONObject();
	    		filterJSON.put("CustomerId", ContextCustomer.get("Id"));
	    		JSONArray statuses = new JSONArray();
	    		statuses.put(BetStatus.Active);
	    		filterJSON.put("Statuses", statuses);
	    		filterJSON.put("PageNumber", 0);
	    		filterJSON.put("PageSize", 50);
	    		
	    		String[] response = task.execute(filterJSON.toString()).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONArray betsJSON = responseJSON.getJSONArray("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }
	    
	    private void GetPayUpBets()
	    {
	    	try {	
	    		GetBetsTask task = new GetBetsTask(Token); 
				
	    		JSONObject filterJSON = new JSONObject();
	    		filterJSON.put("CustomerId", ContextCustomer.get("Id"));
	    		JSONArray statuses = new JSONArray();
	    		statuses.put(BetStatus.Payup);
	    		filterJSON.put("Statuses", statuses);
	    		filterJSON.put("PageNumber", 0);
	    		filterJSON.put("PageSize", 50);
	    		
	    		String[] response = task.execute(filterJSON.toString()).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONArray betsJSON = responseJSON.getJSONArray("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }
	    
	    private void GetClosedBets()
	    {
	    	try {	
	    		GetBetsTask task = new GetBetsTask(Token); 
				
	    		JSONObject filterJSON = new JSONObject();
	    		filterJSON.put("CustomerId", ContextCustomer.get("Id"));
	    		JSONArray statuses = new JSONArray();
	    		statuses.put(BetStatus.Paid);
	    		
	    		statuses.put(BetStatus.Welched);
	    		filterJSON.put("Statuses", statuses);
	    		filterJSON.put("PageNumber", 0);
	    		filterJSON.put("PageSize", 50);
	    		
	    		String[] response = task.execute(filterJSON.toString()).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONArray betsJSON = responseJSON.getJSONArray("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }

	    private void UploadBetPhoto()
	    {
	    	try {	
	    		// TODO: Provide bet Id below
	    		UploadBetPhotoTask task = new UploadBetPhotoTask(Token, "bet-id-here", false);
				
	    		// TODO: Set data to PNG image byte array
				byte[] data = new byte[10];
	    		
	    		String[] response = task.execute(data).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONObject fileName = responseJSON.getJSONObject("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }

	    private void AcceptBet()
	    {
	    	try {	
	    		// TODO: Provide bet Id below
	    		SetMemberStatusTask task = new SetMemberStatusTask(Token, "bet-id-here", BetStatus.Active);
	    		
	    		String[] response = task.execute(ContextCustomer.getString("Id")).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONObject bet = responseJSON.getJSONObject("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }
	    
	    private void DenyBet()
	    {
	    	try {	
	    		// TODO: Provide bet Id below
	    		SetMemberStatusTask task = new SetMemberStatusTask(Token, "bet-id-here", BetStatus.Rejected);
	    	
	    		String[] response = task.execute(ContextCustomer.getString("Id")).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONObject bet = responseJSON.getJSONObject("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }
	    
	    private void CancelBet()
	    {
	    	try {	
	    		// TODO: Provide bet Id below
	    		SetMemberStatusTask task = new SetMemberStatusTask(Token, "bet-id-here", BetStatus.Rejected);
			
	    		String[] response = task.execute(ContextCustomer.getString("Id")).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONObject bet = responseJSON.getJSONObject("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }
	    
	    private void CloseBet()
	    {
	    	try {	
	    		GetBetTask getTask = new GetBetTask(Token);
	    		
	    		// TODO: Provide bet Id below
	    		String[] getResponse = getTask.execute("bet-id-here").get();
	    		
	    		JSONObject getResponseJSON = new JSONObject(getResponse[0]);
				
	    		Status status = Status.values()[(Integer)getResponseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
	    		
				JSONObject betJSON = getResponseJSON.getJSONObject("Result");
				
				JSONArray teams = betJSON.getJSONArray("Teams");
				
				JSONObject betMakerTeam = null;
				JSONObject opponentTeam = null;
				for(int index = 0; index < teams.length(); index++)
				{
					JSONObject team = teams.getJSONObject(index);
					
					if(team.getInt("Side") == TeamSide.BetMaker) betMakerTeam = team;
					
					if(team.getInt("Side") == TeamSide.Opponent) opponentTeam = team;
				}
				
				if(betMakerTeam == null || opponentTeam == null)
				{
					// TODO: Report error
					return;
				}
				
				// TODO: Set winner and loser
				betMakerTeam.put("Status", TeamStatus.Lost);
				opponentTeam.put("Status", TeamStatus.Won);
				
				// Note: Set whether the bet has been paid, welched or is to be paid by changing its status
				betJSON.put("Status", BetStatus.Payup);
				
	    		CloseBetTask task = new CloseBetTask(Token);
			
	    		String[] response = task.execute(betJSON.toString()).get();
					
				JSONObject closeResponseJSON = new JSONObject(response[0]);
		
				status = Status.values()[(Integer)closeResponseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
				
				JSONObject bet = closeResponseJSON.getJSONObject("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }
	    
	    private void GetPublicBets()
	    {
	    	try {	
	    		GetBetsTask task = new GetBetsTask(Token); 
				
	    		JSONObject filterJSON = new JSONObject();
	    		// Note: Optionally use the line below to limit bets by status
	    		ArrayList<BetStatus> statuses = new ArrayList<BetStatus>();
	    		statuses.add(BetStatus.Active);
	    		filterJSON.put("Statuses", statuses);
	     		// Note: Optionally use the line below to limit bets by types
	    		ArrayList<BetType> types = new ArrayList<BetType>();
	    		types.add(BetType.Custom);
	    		types.add(BetType.Sports);
	    		filterJSON.put("Types", types);
	    		// Note: Specify page number and size
	    		filterJSON.put("PageNumber", 0);
	    		filterJSON.put("PageSize", 50);
	    		
	    		String[] response = task.execute(filterJSON.toString()).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONArray betsJSON = responseJSON.getJSONArray("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }
	
	    private void PostText(){
	    	try {
	    		JSONObject postJSON = new JSONObject();
		            
	    		// TODO: Provide bet Id below
		        postJSON.put("BetId", "bet-id-here");
		        postJSON.put("CustomerId", ContextCustomer.getString("Id"));	
				postJSON.put("Text", "Hey! That's a really cool bet you created");
					
				CreatePostTask task = new CreatePostTask(Token); 
					
				String[] response = task.execute(postJSON.toString()).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				postJSON = responseJSON.getJSONObject("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}	
	    }
	    
	    private void PostPhoto(){
	    	try {
	    		JSONObject postJSON = new JSONObject();
		            
	    		// TODO: Provide bet Id below
		        postJSON.put("BetId", "bet-id-here");
		        postJSON.put("CustomerId", ContextCustomer.getString("Id"));
		    	// TODO: Set PNG image data below 
		        byte[] data = new byte[10];
				postJSON.put("ImageData", Base64.encodeToString(data, 0));
					
				CreatePostTask task = new CreatePostTask(Token); 
					
				String[] response = task.execute(postJSON.toString()).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				postJSON = responseJSON.getJSONObject("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}	
	    }
	    
	    private void GetNewsFeed()
	    {
	    	try {	
	    		GetNewsFeedTask task = new GetNewsFeedTask(Token); 
				
	    		JSONObject filterJSON = new JSONObject();
	    		filterJSON.put("CustomerId", ContextCustomer.get("Id"));
	    		filterJSON.put("PageNumber", 0);
	    		filterJSON.put("PageSize", 50);
	    		
	    		String[] response = task.execute(filterJSON.toString()).get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}
		
				JSONArray newsStoriesJSON = responseJSON.getJSONArray("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}
	    }
	    
	    private void SetDevice(){
	    	try {	
	    		// TODO: Set below the device token or registration id
				SetDeviceTask task = new SetDeviceTask("registration-id-here", ContextCustomer.getString("Id")); 
					
				String[] response = task.execute().get();
					
				JSONObject responseJSON = new JSONObject(response[0]);
					
				Status status = Status.values()[(Integer)responseJSON.opt("Status")];
				
				if(status != Status.Success){
					// TODO: Display responseJSON.getString("Message")
					return;
				}

				JSONObject deviceJSON = responseJSON.getJSONObject("Result");
			} 
	    	catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
			}	
	    }
}
