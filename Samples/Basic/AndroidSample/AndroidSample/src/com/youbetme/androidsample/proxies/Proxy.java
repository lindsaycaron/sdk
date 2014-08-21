package com.youbetme.androidsample.proxies;

import java.util.ArrayList;
import java.util.List;

import org.ksoap2.HeaderProperty;
import org.ksoap2.SoapEnvelope;
//import org.ksoap2.serialization.PropertyInfo;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;
//import org.ksoap2.transport.HttpTransportSE;
import org.ksoap2.transport.HttpsTransportSE;

import com.youbetme.androidsample.dataobjects.*;

import android.util.Base64;

public class Proxy {
	
	// TODO: Provide your API token and secret below
	// Note: To obtain a valid API token and secret sign up at http://dev.youbetme.com/portal
	// Then contact a Youbetme representative or sent email to support@youbetme.com to request access to the API
	// Once your account has been granted access, use your credentails (email and password) to init the SuperProxy below
	public static String TOKEN = "api-token-here";
	public static String SECRET = "api-secret-here";
	
	private static final String SERVICE_NAMESPACE = "http://www.youbetme.com/";
	
	private static final String SERVER = "dev.youbetme.com"; 
	
	private static final String SERVICE_PATH = "/svc/service.asmx"; 
	
	public String SignUp(String customerJson){
		String OPERATION_NAME = "CreateNewCustomer";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("request", customerJson);

		return GetResponse(OPERATION_NAME, request);
	}
	
	public String LogIn(String usernameOrEmail, String password){
		String OPERATION_NAME = "LoginWithPassword";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("usernameOrEmail", usernameOrEmail);
		request.addProperty("password", password);

		return GetResponse(OPERATION_NAME, request);	
	}

	public String LogInWithFacebook(String socialNetwork){
		String OPERATION_NAME = "LoginWithFacebook";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("request", socialNetwork);
		
		return GetResponse(OPERATION_NAME, request);
	}

	public String GetCustomerFromSession(String token){
		String OPERATION_NAME = "GetCustomerFromSession";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("token", token);

		return GetResponse(OPERATION_NAME, request);
	}
	
	public String GetCustomerFromId(String token, String customerId){
		String OPERATION_NAME = "GetCustomerFromId";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("token", token);
		request.addProperty("customerId", customerId);

		return GetResponse(OPERATION_NAME, request);
	}
	
	public String UpdateCustomer(String token, String customer){
		String OPERATION_NAME = "UpdateCustomer";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("token", token);
		request.addProperty("request", customer);

		return GetResponse(OPERATION_NAME, request);
	}
	
	public String GetRecentFriends(String token, String filter){
		String OPERATION_NAME = "GetRecentFriendsLight";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("token", token);
		request.addProperty("request", filter);

		return GetResponse(OPERATION_NAME, request);
	}
	
	public String GetFriends(String token, String filter){
		String OPERATION_NAME = "GetRecentFriendsLight";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("token", token);
		request.addProperty("request", filter);

		return GetResponse(OPERATION_NAME, request);
	}
	
	public String GetFollowers(String token, String filter){
		String OPERATION_NAME = "GetFollowersLight";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("token", token);
		request.addProperty("request", filter);

		return GetResponse(OPERATION_NAME, request);
	}
	
	public String GetFollowees(String token, String filter){
		String OPERATION_NAME = "GetFollowingLight";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("token", token);
		request.addProperty("request", filter);

		return GetResponse(OPERATION_NAME, request);
	}
	
	public String PerformCustomerSearch(String token, String filter){
		String OPERATION_NAME = "PerformCustomerSearch";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("token", token);
		request.addProperty("request", filter);

		return GetResponse(OPERATION_NAME, request);
	}
	
	public String GetBetSuggestions(String token){
		String OPERATION_NAME = "GetBetSuggestions";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("token", token);

		return GetResponse(OPERATION_NAME, request);
	}
	
	public String CreateBet(String token, String bet){
		String OPERATION_NAME = "CreateNewBet";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("token", token);
		request.addProperty("request", bet);

		return GetResponse(OPERATION_NAME, request);
	}
	
	public String GetBets(String token, String filter){
		String OPERATION_NAME = "GetBetsLight";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("token", token);
		request.addProperty("request", filter);
		
		return GetResponse(OPERATION_NAME, request);
	}

	public String GetBet(String token, String betId){
		String OPERATION_NAME = "GetBet";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("token", token);
		request.addProperty("betId", betId);
		
		return GetResponse(OPERATION_NAME, request);
	}
	
	public String UploadBetPhoto(String token, String betId, byte[] data, boolean postCreateUpload){
		String OPERATION_NAME = "UploadBetImage";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("token", token);
		request.addProperty("betId", betId);
		request.addProperty("bytes", Base64.encodeToString(data, 0));
		request.addProperty("postCreateUpload", postCreateUpload);

		return GetResponse(OPERATION_NAME, request);
	}

	public String SetMemberStatus(String token, String customerId, String betId, BetStatus status){
		String OPERATION_NAME = "SetMemberStatus";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("token", token);
		request.addProperty("customerId", customerId);
		request.addProperty("betId", betId);
		request.addProperty("status", status.ordinal());
		
		return GetResponse(OPERATION_NAME, request);
	}
	
	public String CloseBet(String token, String bet){
		String OPERATION_NAME = "CloseBet";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("token", token);
		request.addProperty("request", bet);

		return GetResponse(OPERATION_NAME, request);
	}

	public String CreatePost(String token, String post){
		String OPERATION_NAME = "CreateNewPost";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("token", token);
		request.addProperty("request", post);

		return GetResponse(OPERATION_NAME, request);
	}
	
	public String SetDevice(String registrationId, String customerId){
		String OPERATION_NAME = "SetDevice";
		
		SoapObject request = new SoapObject(SERVICE_NAMESPACE, OPERATION_NAME);
		request.addProperty("deviceToken", registrationId);
		request.addProperty("system", "Android");
		request.addProperty("customerId", customerId);

		return GetResponse(OPERATION_NAME, request);
	}

	private String GetResponse(String operationName, SoapObject request)
	{
		SoapSerializationEnvelope envelope = GetSerializationEnvelope(request); 

		HttpsTransportSE transport = GetSecureTransport(); 
		transport.debug = true;
		
		Object response = null;
		try
		{	
			transport.call(GetSoapActionName(operationName), envelope, GetHeaders());
			response = envelope.getResponse();
		}
		catch (Exception exception)
		{
			response = exception.toString();
		}
		
		return response.toString();	
	}
	
	private HttpsTransportSE GetSecureTransport()
	{
		HttpsTransportSE transport = new HttpsTransportSE(SERVER, 443, SERVICE_PATH, 36000);
		//HttpTransportSE httpTransport = new HttpTransportSE(SERVICE_ADDRESS);
		transport.setXmlVersionTag("<!--?xml version=\"1.0\" encoding= \"UTF-8\" ?-->");
		
		return transport;	
	}
	
	private HttpTransportSE GetTransport()
	{
		return new HttpTransportSE("http://" + SERVER + "/" + SERVICE_PATH);	
	}
	
	private SoapSerializationEnvelope GetSerializationEnvelope(SoapObject request)
	{
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);
		
		envelope.dotNet = true;
		envelope.implicitTypes = true;
		envelope.setAddAdornments(true);
		envelope.setOutputSoapObject(request);
		
		return envelope;
	}
	
	private String GetSoapActionName(String operationName)
	{
		return SERVICE_NAMESPACE + operationName;
	}
	
	private static List<HeaderProperty> GetHeaders()
	{
		List<HeaderProperty> headers = new ArrayList<HeaderProperty>();
		headers.add(new HeaderProperty("Authorization", "Basic " + Base64.encodeToString((TOKEN + ":" + SECRET).getBytes(), 0)));
		return headers;
	}

}
