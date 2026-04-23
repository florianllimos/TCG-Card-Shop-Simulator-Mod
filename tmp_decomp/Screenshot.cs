using System;
using System.Collections;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
	protected const string MEDIA_STORE_IMAGE_MEDIA = "android.provider.MediaStore$Images$Media";

	protected static AndroidJavaObject m_Activity;

	protected static AndroidJavaObject Activity
	{
		get
		{
			if (m_Activity == null)
			{
				m_Activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
			}
			return m_Activity;
		}
	}

	protected static string SaveImageToGallery(Texture2D a_Texture, string a_Title, string a_Description)
	{
		using AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.provider.MediaStore$Images$Media");
		using AndroidJavaObject androidJavaObject = Activity.Call<AndroidJavaObject>("getContentResolver", Array.Empty<object>());
		AndroidJavaObject androidJavaObject2 = Texture2DToAndroidBitmap(a_Texture);
		return androidJavaClass.CallStatic<string>("insertImage", new object[4] { androidJavaObject, androidJavaObject2, a_Title, a_Description });
	}

	protected static AndroidJavaObject Texture2DToAndroidBitmap(Texture2D a_Texture)
	{
		byte[] array = a_Texture.EncodeToPNG();
		using AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.graphics.BitmapFactory");
		return androidJavaClass.CallStatic<AndroidJavaObject>("decodeByteArray", new object[3] { array, 0, array.Length });
	}

	public void CaptureScreenshot()
	{
	}

	private IEnumerator CaptureScreenshotCoroutine(int width, int height)
	{
		yield return new WaitForEndOfFrame();
		Texture2D tex = new Texture2D(width, height);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		yield return tex;
		string text = SaveImageToGallery(tex, "Name", "Description");
		Debug.Log("Picture has been saved at:\n" + text);
	}
}
