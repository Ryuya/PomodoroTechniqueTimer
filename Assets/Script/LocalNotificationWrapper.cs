#if UNITY_ANDROID
using Unity.Notifications.Android;

#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif

/// <summary>
/// ローカルプッシュ通知
/// </summary>
public static class LocalNotificationWrapper
{
    private static bool _isInitialized;
#if UNITY_ANDROID
    // 通知チャンネルID
    // 本サンプルはシングルチャンネルです
    private static string ChannelId = "channelId";
#endif
    public static void InitializeIfNeed()
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;
#if UNITY_ANDROID
        // 通知チャンネルの登録
        AndroidNotificationCenter.RegisterNotificationChannel(
            new AndroidNotificationChannel
            {
                Id = ChannelId,
                Name = "Default ChannelName",
                Importance = Importance.High,
                Description = "Channel Description",
                // 1を指定してもバッジがつかない...
                Numbrer = 1,
            });
#endif
    }

    /// <summary>
    /// ローカル通知の予約
    /// </summary>
    public static void ReserveNotification(
        string title,
        string body,
        int afterSec
    )
    {
        InitializeIfNeed();
#if UNITY_ANDROID
        // 通知を送信する
        AndroidNotificationCenter.SendNotification(new AndroidNotification
        {
            Title = title,
            Text = body,
            // アイコンをそれぞれセット
            SmallIcon = "icon_0",
            LargeIcon = "icon_1",
            // 今から何秒後に通知をするか？
            FireTime = System.DateTime.Now.AddSeconds(afterSec)
        }, ChannelId);
#endif

#if UNITY_IOS
        iOSNotificationCenter.ScheduleNotification(new iOSNotification()
        {
            Title = title,
            Body = body,
            ShowInForeground = true,
            Badge = 1,
            // 時間をトリガーにする
            Trigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = new System.TimeSpan(0, 0, afterSec),
                Repeats = false
            }
        });
#endif
    }
}