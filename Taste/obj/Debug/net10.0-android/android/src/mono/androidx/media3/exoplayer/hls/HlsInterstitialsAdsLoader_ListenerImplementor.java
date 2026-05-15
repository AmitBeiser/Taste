package mono.androidx.media3.exoplayer.hls;


public class HlsInterstitialsAdsLoader_ListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		androidx.media3.exoplayer.hls.HlsInterstitialsAdsLoader.Listener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onAdCompleted:(Landroidx/media3/common/MediaItem;Ljava/lang/Object;II)V:GetOnAdCompleted_Landroidx_media3_common_MediaItem_Ljava_lang_Object_IIHandler:AndroidX.Media3.ExoPlayer.Hls.HlsInterstitialsAdsLoader+IListener, Xamarin.AndroidX.Media3.ExoPlayer.Hls\n" +
			"n_onAssetListLoadCompleted:(Landroidx/media3/common/MediaItem;Ljava/lang/Object;IILandroidx/media3/exoplayer/hls/HlsInterstitialsAdsLoader$AssetList;)V:GetOnAssetListLoadCompleted_Landroidx_media3_common_MediaItem_Ljava_lang_Object_IILandroidx_media3_exoplayer_hls_HlsInterstitialsAdsLoader_AssetList_Handler:AndroidX.Media3.ExoPlayer.Hls.HlsInterstitialsAdsLoader+IListener, Xamarin.AndroidX.Media3.ExoPlayer.Hls\n" +
			"n_onAssetListLoadFailed:(Landroidx/media3/common/MediaItem;Ljava/lang/Object;IILjava/io/IOException;Z)V:GetOnAssetListLoadFailed_Landroidx_media3_common_MediaItem_Ljava_lang_Object_IILjava_io_IOException_ZHandler:AndroidX.Media3.ExoPlayer.Hls.HlsInterstitialsAdsLoader+IListener, Xamarin.AndroidX.Media3.ExoPlayer.Hls\n" +
			"n_onAssetListLoadStarted:(Landroidx/media3/common/MediaItem;Ljava/lang/Object;II)V:GetOnAssetListLoadStarted_Landroidx_media3_common_MediaItem_Ljava_lang_Object_IIHandler:AndroidX.Media3.ExoPlayer.Hls.HlsInterstitialsAdsLoader+IListener, Xamarin.AndroidX.Media3.ExoPlayer.Hls\n" +
			"n_onContentTimelineChanged:(Landroidx/media3/common/MediaItem;Ljava/lang/Object;Landroidx/media3/common/Timeline;)V:GetOnContentTimelineChanged_Landroidx_media3_common_MediaItem_Ljava_lang_Object_Landroidx_media3_common_Timeline_Handler:AndroidX.Media3.ExoPlayer.Hls.HlsInterstitialsAdsLoader+IListener, Xamarin.AndroidX.Media3.ExoPlayer.Hls\n" +
			"n_onMetadata:(Landroidx/media3/common/MediaItem;Ljava/lang/Object;IILandroidx/media3/common/Metadata;)V:GetOnMetadata_Landroidx_media3_common_MediaItem_Ljava_lang_Object_IILandroidx_media3_common_Metadata_Handler:AndroidX.Media3.ExoPlayer.Hls.HlsInterstitialsAdsLoader+IListener, Xamarin.AndroidX.Media3.ExoPlayer.Hls\n" +
			"n_onPrepareCompleted:(Landroidx/media3/common/MediaItem;Ljava/lang/Object;II)V:GetOnPrepareCompleted_Landroidx_media3_common_MediaItem_Ljava_lang_Object_IIHandler:AndroidX.Media3.ExoPlayer.Hls.HlsInterstitialsAdsLoader+IListener, Xamarin.AndroidX.Media3.ExoPlayer.Hls\n" +
			"n_onPrepareError:(Landroidx/media3/common/MediaItem;Ljava/lang/Object;IILjava/io/IOException;)V:GetOnPrepareError_Landroidx_media3_common_MediaItem_Ljava_lang_Object_IILjava_io_IOException_Handler:AndroidX.Media3.ExoPlayer.Hls.HlsInterstitialsAdsLoader+IListener, Xamarin.AndroidX.Media3.ExoPlayer.Hls\n" +
			"n_onStart:(Landroidx/media3/common/MediaItem;Ljava/lang/Object;Landroidx/media3/common/AdViewProvider;)V:GetOnStart_Landroidx_media3_common_MediaItem_Ljava_lang_Object_Landroidx_media3_common_AdViewProvider_Handler:AndroidX.Media3.ExoPlayer.Hls.HlsInterstitialsAdsLoader+IListener, Xamarin.AndroidX.Media3.ExoPlayer.Hls\n" +
			"n_onStop:(Landroidx/media3/common/MediaItem;Ljava/lang/Object;Landroidx/media3/common/AdPlaybackState;)V:GetOnStop_Landroidx_media3_common_MediaItem_Ljava_lang_Object_Landroidx_media3_common_AdPlaybackState_Handler:AndroidX.Media3.ExoPlayer.Hls.HlsInterstitialsAdsLoader+IListener, Xamarin.AndroidX.Media3.ExoPlayer.Hls\n" +
			"";
		mono.android.Runtime.register ("AndroidX.Media3.ExoPlayer.Hls.HlsInterstitialsAdsLoader+IListenerImplementor, Xamarin.AndroidX.Media3.ExoPlayer.Hls", HlsInterstitialsAdsLoader_ListenerImplementor.class, __md_methods);
	}

	public HlsInterstitialsAdsLoader_ListenerImplementor ()
	{
		super ();
		if (getClass () == HlsInterstitialsAdsLoader_ListenerImplementor.class) {
			mono.android.TypeManager.Activate ("AndroidX.Media3.ExoPlayer.Hls.HlsInterstitialsAdsLoader+IListenerImplementor, Xamarin.AndroidX.Media3.ExoPlayer.Hls", "", this, new java.lang.Object[] {  });
		}
	}

	public void onAdCompleted (androidx.media3.common.MediaItem p0, java.lang.Object p1, int p2, int p3)
	{
		n_onAdCompleted (p0, p1, p2, p3);
	}

	private native void n_onAdCompleted (androidx.media3.common.MediaItem p0, java.lang.Object p1, int p2, int p3);

	public void onAssetListLoadCompleted (androidx.media3.common.MediaItem p0, java.lang.Object p1, int p2, int p3, androidx.media3.exoplayer.hls.HlsInterstitialsAdsLoader.AssetList p4)
	{
		n_onAssetListLoadCompleted (p0, p1, p2, p3, p4);
	}

	private native void n_onAssetListLoadCompleted (androidx.media3.common.MediaItem p0, java.lang.Object p1, int p2, int p3, androidx.media3.exoplayer.hls.HlsInterstitialsAdsLoader.AssetList p4);

	public void onAssetListLoadFailed (androidx.media3.common.MediaItem p0, java.lang.Object p1, int p2, int p3, java.io.IOException p4, boolean p5)
	{
		n_onAssetListLoadFailed (p0, p1, p2, p3, p4, p5);
	}

	private native void n_onAssetListLoadFailed (androidx.media3.common.MediaItem p0, java.lang.Object p1, int p2, int p3, java.io.IOException p4, boolean p5);

	public void onAssetListLoadStarted (androidx.media3.common.MediaItem p0, java.lang.Object p1, int p2, int p3)
	{
		n_onAssetListLoadStarted (p0, p1, p2, p3);
	}

	private native void n_onAssetListLoadStarted (androidx.media3.common.MediaItem p0, java.lang.Object p1, int p2, int p3);

	public void onContentTimelineChanged (androidx.media3.common.MediaItem p0, java.lang.Object p1, androidx.media3.common.Timeline p2)
	{
		n_onContentTimelineChanged (p0, p1, p2);
	}

	private native void n_onContentTimelineChanged (androidx.media3.common.MediaItem p0, java.lang.Object p1, androidx.media3.common.Timeline p2);

	public void onMetadata (androidx.media3.common.MediaItem p0, java.lang.Object p1, int p2, int p3, androidx.media3.common.Metadata p4)
	{
		n_onMetadata (p0, p1, p2, p3, p4);
	}

	private native void n_onMetadata (androidx.media3.common.MediaItem p0, java.lang.Object p1, int p2, int p3, androidx.media3.common.Metadata p4);

	public void onPrepareCompleted (androidx.media3.common.MediaItem p0, java.lang.Object p1, int p2, int p3)
	{
		n_onPrepareCompleted (p0, p1, p2, p3);
	}

	private native void n_onPrepareCompleted (androidx.media3.common.MediaItem p0, java.lang.Object p1, int p2, int p3);

	public void onPrepareError (androidx.media3.common.MediaItem p0, java.lang.Object p1, int p2, int p3, java.io.IOException p4)
	{
		n_onPrepareError (p0, p1, p2, p3, p4);
	}

	private native void n_onPrepareError (androidx.media3.common.MediaItem p0, java.lang.Object p1, int p2, int p3, java.io.IOException p4);

	public void onStart (androidx.media3.common.MediaItem p0, java.lang.Object p1, androidx.media3.common.AdViewProvider p2)
	{
		n_onStart (p0, p1, p2);
	}

	private native void n_onStart (androidx.media3.common.MediaItem p0, java.lang.Object p1, androidx.media3.common.AdViewProvider p2);

	public void onStop (androidx.media3.common.MediaItem p0, java.lang.Object p1, androidx.media3.common.AdPlaybackState p2)
	{
		n_onStop (p0, p1, p2);
	}

	private native void n_onStop (androidx.media3.common.MediaItem p0, java.lang.Object p1, androidx.media3.common.AdPlaybackState p2);

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
