package com.youbetme.androidsample.tasks;

import android.os.AsyncTask;

import com.youbetme.androidsample.proxies.Proxy;

public class GetBetSuggestionsTask extends AsyncTask<Void, Integer, String> {
	
	private String token;
	
	public GetBetSuggestionsTask(String token){
		this.token = token;
	}
	
    protected void onProgressUpdate(Integer... progress) {
      
    }

    protected void onPostExecute(String result) {
        
    }

	@Override
	protected String doInBackground(Void... params) {
        Proxy proxy = new Proxy();
        return proxy.GetBetSuggestions(this.token);
	}
}