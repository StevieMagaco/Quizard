package md5ac9d47ad05b768306a766bf79290cac2;


public class DeckActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("Quizard.Resources.layout.DeckActivity, Quizard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DeckActivity.class, __md_methods);
	}


	public DeckActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DeckActivity.class)
			mono.android.TypeManager.Activate ("Quizard.Resources.layout.DeckActivity, Quizard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
