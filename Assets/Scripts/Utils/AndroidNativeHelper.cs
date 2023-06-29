using System;
using UnityEngine;

namespace BFrame
{
    public class AndroidNativeHelper
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaClass _helper;
        private const string JavaClassName = "com.asta.mergegame.NativeHelper";
        private static AndroidJavaClass AndroidHelper
        {
            get
            {
                if (_helper != null) return _helper;

                _helper = new AndroidJavaClass(JavaClassName);

                if (_helper == null)
                    ErrorNotSupport();

                return _helper;
            }
        }
#endif
        
        private static void ErrorNotSupport()
        {
            throw new Exception("[AndroidNativeHelper.cs]Error on Android Plugin");
        }
        
        public static bool IsAssetExists(string path)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return AndroidHelper.CallStatic<bool>("isAssetExists", path);
#else
            ErrorNotSupport();
            return false;
#endif
        }
        
        public static string GetAssetString(string path)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
#if UNITY_2017_1_OR_NEWER
            var clsPtr = AndroidJNI.FindClass(JavaClassName);
            var methondPtr_getAssetBytes = AndroidJNIHelper.GetMethodID(clsPtr, "getAssetBytes", "(Ljava/lang/String;)[B", true);
            var arg1 = new object[] { path };
            var args = AndroidJNIHelper.CreateJNIArgArray(arg1);
            var byteArray = AndroidJNI.CallStaticObjectMethod(clsPtr, methondPtr_getAssetBytes, args);
            var bytes = AndroidJNI.FromSByteArray(byteArray);
            AndroidJNIHelper.DeleteJNIArgArray(arg1, args);
            AndroidJNI.DeleteLocalRef(byteArray);
            AndroidJNI.DeleteLocalRef(clsPtr);
            return bytes.ToString();
#else
            return AndroidHelper.CallStatic<string>("getAssetString", path);
#endif
#else
            ErrorNotSupport();
            return null;
#endif
        }
        
        public static byte[] GetAssetBytes(string path)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return AndroidHelper.CallStatic<byte[]>("getAssetBytes", path);
#else
            ErrorNotSupport();
            return null;
#endif
        }

        public static bool IsOtherAudioPlaying()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            bool result =  AndroidHelper.CallStatic<bool>("IsOtherAudioPlaying");
            Debug.Log("lyh++++++++IsOtherAudioPlaying"+ (result?"true":"false"));
            return result;
#else
            ErrorNotSupport();
            return false;
#endif
        }
    }
}