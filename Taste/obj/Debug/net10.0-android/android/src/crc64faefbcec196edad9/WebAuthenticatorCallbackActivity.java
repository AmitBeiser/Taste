package crc64faefbcec196edad9;


public class WebAuthenticatorCallbackActivity
	extends crc6468b6408a11370c2f.WebAuthenticatorCallbackActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Taste.WebAuthenticatorCallbackActivity, Taste", WebAuthenticatorCallbackActivity.class, __md_methods);
	}

	public WebAuthenticatorCallbackActivity ()
	{
		super ();
		if (getClass () == WebAuthenticatorCallbackActivity.class) {
			mono.android.TypeManager.Activate ("Taste.WebAuthenticatorCallbackActivity, Taste", "", this, new java.lang.Object[] {  });
		}
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
