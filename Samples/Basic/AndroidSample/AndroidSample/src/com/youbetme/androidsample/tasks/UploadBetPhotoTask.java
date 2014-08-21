package com.youbetme.androidsample.tasks;

import android.os.AsyncTask;

import com.youbetme.androidsample.proxies.Proxy;

public class UploadBetPhotoTask extends AsyncTask<byte[], Integer, String[]> {
	private String token;
	private String betId;
	private boolean postCreateUpload;
	
	public UploadBetPhotoTask(String token, String betId, boolean postCreateUpload){
		this.token = token; 
		this.betId = betId;
		this.postCreateUpload = postCreateUpload;
	}
	
    protected String[] doInBackground(byte[]...request) {
    	 int count = request.length;
         String[] response = new String[count];
         Proxy proxy = new Proxy();
         for (int i = 0; i < count; i++) {
        	response[i] = proxy.UploadBetPhoto(this.token, this.betId, request[i], this.postCreateUpload);
         }
         return response;
    }

    protected void onProgressUpdate(Integer... progress) {
      
    }

    protected void onPostExecute(String result) {
        
    }
}


