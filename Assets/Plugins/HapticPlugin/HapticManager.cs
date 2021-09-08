using System.Runtime.InteropServices;
using UnityEngine;

namespace ArmNomads.Haptic
{
	public enum NotificationFeedback
	{
		Success,
		Warning,
		Error
	}


	public enum ImpactFeedback
	{
		Light,
		Medium,
		Heavy
	}

	public class HapticManager : MonoBehaviour
	{
		private static HapticManager instance;

		private static AndroidJavaClass hapticFeedbackClass;

		[SerializeField]
		[HideInInspector]
		private int[] hapticConfig = { 50, 100, 200 };

		private void Awake()
		{
			if (instance)
				Destroy(gameObject);
			else
				instance = this;
			DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID && !UNITY_EDITOR
			InitAndroidPlugin();
#endif
		}

		private void InitAndroidPlugin()
		{
			hapticFeedbackClass = new AndroidJavaClass("com.armnomads.hapticfeedback.HapticFeedback");
			using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				AndroidJavaObject activityContext = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
				hapticFeedbackClass.CallStatic("Init", activityContext, hapticConfig);
			}
		}

		public static bool IsTapticSupported()
		{
			string deviceId = SystemInfo.deviceModel;
			if (deviceId == null || !deviceId.Contains("iPhone"))
				return false;
			deviceId = deviceId.Substring(6, deviceId.IndexOf(',') - 6);
			int idNumber = 0;
			if (int.TryParse(deviceId, out idNumber) && idNumber > 8)
				return true;
			return false;
		}

		public static void Notification(NotificationFeedback feedback)
		{
#if UNITY_EDITOR

#elif UNITY_IOS
		_unityTapticNotification((int)feedback);
#endif
		}

		public static void Impact(ImpactFeedback feedback)
		{
#if UNITY_EDITOR

#elif UNITY_IOS
		_unityTapticImpact((int)feedback);
#elif UNITY_ANDROID
		hapticFeedbackClass.CallStatic("HapticImpact", (int)feedback);
#endif
		}

		public static void Selection()
		{
#if UNITY_EDITOR

#elif UNITY_IOS
		_unityTapticSelection();
#endif
		}

#if UNITY_IOS
		[DllImport("__Internal")]
		private static extern void _unityTapticNotification(int feedback);
		[DllImport("__Internal")]
		private static extern void _unityTapticSelection();
		[DllImport("__Internal")]
		private static extern void _unityTapticImpact(int feedback);
#endif
	}

}