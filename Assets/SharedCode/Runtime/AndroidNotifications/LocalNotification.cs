﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class LocalNotification
{
    #if UNITY_ANDROID && !UNITY_EDITOR
    private static string fullClassName = "net.agasper.unitynotification.UnityNotificationManager";
    #endif

    static List<int> registeredNotificationsId = new List<int>(); 
    static void SaveRegisteredNotificationsId ()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < registeredNotificationsId.Count; i++)
        {
            if(i != 0) sb.Append("_");   
            sb.Append(registeredNotificationsId[i]);   
        }    
        PlayerPrefs.SetString("LNRID", sb.ToString());
    }

    static void LoadRegisteredNotificationsId ()
    {
        registeredNotificationsId.Clear();
        string[] idsString = PlayerPrefs.GetString("LNRID").Split('_');
        for (int i = 0; i < idsString.Length; i++)
        { 
            try
            {
                registeredNotificationsId.Add(int.Parse(idsString[i]));
            }
            catch { }
        }
    }

    static bool initialized;
    static void Initialize()
    {
        if(initialized) return;

        LoadRegisteredNotificationsId();

        initialized = true;
    }

    public static int SendNotification(TimeSpan delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
    {
        int id = new System.Random().Next();
        return SendNotification(id, (int)delay.TotalSeconds * 1000, title, message, bgColor, sound, vibrate, lights, bigIcon);
    }

    public static int SendNotification(int id, TimeSpan delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
    {
        return SendNotification(id, (int)delay.TotalSeconds * 1000, title, message, bgColor, sound, vibrate, lights, bigIcon);
    }

    public static int SendNotification(int id, long delayMs, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
    {
        Initialize();
        registeredNotificationsId.Add(id);
        SaveRegisteredNotificationsId();
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetNotification", id, delayMs, title, message, message, 
                sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_icon_small", 
        bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, Application.identifier);
        }
        return id;
#elif UNITY_IOS && !UNITY_EDITOR
        UnityEngine.iOS.LocalNotification notification = new UnityEngine.iOS.LocalNotification();
        DateTime fireDate = DateTime.Now.AddSeconds(delayMs / 1000);
        notification.fireDate = fireDate;
        notification.alertBody = message;
        notification.alertAction = title;
        notification.hasAction = false;

        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notification);

        return (int)fireDate.Ticks;
#else
        return 0;
        #endif
    }

    public static int SendRepeatingNotification(TimeSpan delay, TimeSpan timeout, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
    {
        int id = new System.Random().Next();
        return SendRepeatingNotification(id, (int)delay.TotalSeconds * 1000, (int)timeout.TotalSeconds, title, message, bgColor, sound, vibrate, lights, bigIcon);
    }

    public static int SendRepeatingNotification(int id, TimeSpan delay, TimeSpan timeout, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
    {
        return SendRepeatingNotification(id, (int)delay.TotalSeconds * 1000, (int)timeout.TotalSeconds, title, message, bgColor, sound, vibrate, lights, bigIcon);
    }

    public static int SendRepeatingNotification(int id, long delayMs, long timeoutMs, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
    {
        Initialize();
        registeredNotificationsId.Add(id);
        SaveRegisteredNotificationsId();
        #if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetRepeatingNotification", id, delayMs, title, message, message, timeoutMs, 
                sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_icon_small", 
                bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, Application.identifier);
        }
        return id;
        #elif UNITY_IOS && !UNITY_EDITOR
        throw new System.NotImplementedException();
        #else
        return 0;
        #endif
    }

    public static void CancelNotification(int id)
    {
        Initialize();
        if (registeredNotificationsId.Contains(id))
        {
            registeredNotificationsId.Remove(id);
            SaveRegisteredNotificationsId();
        }
        #if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null) {
            pluginClass.CallStatic("CancelPendingNotification", id);
        }
        #endif

        #if UNITY_IOS && !UNITY_EDITOR
        foreach (UnityEngine.iOS.LocalNotification notif in UnityEngine.iOS.NotificationServices.scheduledLocalNotifications) 
        { 
            if ((int)notif.fireDate.Ticks == id)
            {
                UnityEngine.iOS.NotificationServices.CancelLocalNotification(notif);
            }
        }
        #endif
    }

    public static void CancelAllNotifications()
    {
        Initialize(); 
        for (int i = 0; i < registeredNotificationsId.Count; i++)
        {
            int id = registeredNotificationsId[i];
            #if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
            if (pluginClass != null) {
            pluginClass.CallStatic("CancelPendingNotification", id);
            }
            #endif

            #if UNITY_IOS && !UNITY_EDITOR
            foreach (UnityEngine.iOS.LocalNotification notif in UnityEngine.iOS.NotificationServices.scheduledLocalNotifications) 
            { 
            if ((int)notif.fireDate.Ticks == id)
            {
            UnityEngine.iOS.NotificationServices.CancelLocalNotification(notif);
            }
            }
            #endif
        }
        registeredNotificationsId.Clear();
        SaveRegisteredNotificationsId(); 
    }

    public static void ClearNotifications()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null) {
            pluginClass.CallStatic("ClearShowingNotifications");
        }
        #endif

        #if UNITY_IOS && !UNITY_EDITOR
        UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
        #endif
    }
}