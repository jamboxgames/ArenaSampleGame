namespace UnityDebug
{

    using UnityEngine;
    using System.Collections;
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using UnityEngineInternal;

    /// 
    /// It overrides UnityEngine.Debug to mute debug messages completely on a platform-specific basis.
    /// 
    /// Putting this inside of 'Plugins' foloder is ok.
    /// 
    /// Important:
    ///     Other preprocessor directives than 'UNITY_EDITOR' does not correctly work.
    /// 
    /// Note:
    ///     [Conditional] attribute indicates to compilers that a method call or attribute should be 
    ///     ignored unless a specified conditional compilation symbol is defined.
    /// 
    /// See Also: 
    ///     http://msdn.microsoft.com/en-us/library/system.diagnostics.conditionalattribute.aspx
    /// 
    /// 2012.11. @kimsama
    /// 
    public static class Debug
    {
        public static LogLevel LogLevel = LogLevel.None;
        
        //LogLevel.Debug
        public static void Log(object message)
        {
            if (LogLevel == LogLevel.Debug || LogLevel == LogLevel.All)
                UnityEngine.Debug.Log(message);
        }

        public static void Log(object message, UnityEngine.Object context)
        {
            if (LogLevel == LogLevel.Debug || LogLevel == LogLevel.All)
                UnityEngine.Debug.Log(message, context);
        }

        //LogLevel.Info
        public static void LogInfo(object message)
        {
            if (LogLevel == LogLevel.Info || LogLevel == LogLevel.All)
                UnityEngine.Debug.Log(message);
        }

        //LogLevel.Error
        public static void LogWarning(object message)
        {
            if (LogLevel == LogLevel.Warning || LogLevel == LogLevel.All)
                UnityEngine.Debug.LogWarning(message);
        }

        public static void LogWarning(object message, UnityEngine.Object context)
        {
            if (LogLevel == LogLevel.Warning || LogLevel == LogLevel.All)
                UnityEngine.Debug.Log(message, context);
        }

        //LogLevel.Error
        public static void LogError(object message)
        {
            if (LogLevel == LogLevel.Error || LogLevel == LogLevel.All)
                UnityEngine.Debug.LogError(message);
        }

        public static void LogMessage(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public static void LogFormat(String message, string _msg)
        {
            UnityEngine.Debug.LogFormat(message, _msg);
        }

        /*
        //[System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        //[System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.Log(message, context);
        }

        //[System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        //[System.Diagnostics.Conditional("UNITY_EDITOR")]	
        public static void LogError(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogError(message, context);
        }

        //[System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message.ToString());
        }

        //[System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogException(Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
        }
        //[System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogException(Exception ex, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogException(ex, context);
        }

        //[System.Diagnostics.Conditional("UNITY_EDITOR")] 
        public static void LogWarning(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogWarning(message.ToString(), context);
        }

        //[System.Diagnostics.Conditional("UNITY_EDITOR")] 
        public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
        {
            UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
        }

        //[System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
        {
            UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
        }

        //[System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition)
        {
            if (!condition) throw new Exception();
        }*/
        
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        All,
        None
    }
}
//#endif

