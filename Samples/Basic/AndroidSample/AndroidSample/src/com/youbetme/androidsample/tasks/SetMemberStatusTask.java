package com.youbetme.androidsample.tasks;

import com.youbetme.androidsample.dataobjects.BetStatus;
import com.youbetme.androidsample.proxies.Proxy;

public class SetMemberStatusTask extends SuperTask{

	private String betId;
	private BetStatus status;
	
	public SetMemberStatusTask(String token, String betId, BetStatus status){
		super(token);
		this.betId = betId;
		this.status = status;
	}
	
    protected String[] doInBackground(String...request) {
    	 int count = request.length;
         String[] response = new String[count];
         Proxy proxy = new Proxy();
         for (int i = 0; i < count; i++) {
        	response[i] = proxy.SetMemberStatus(this.token, request[i], this.betId, this.status);
         }
         return response;
    }

    protected void onProgressUpdate(Integer... progress) {
      
    }

    protected void onPostExecute(String result) {
        
    }
}
