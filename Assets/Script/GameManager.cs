using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Unity.Notifications.iOS;

public enum State
{
    DEFAULT = 0,
    WORK,
    KYUKEI,
    PAUSE,
 }

public class GameManager : MonoBehaviour
{
    iOSNotification notification;
    //順番に残り勤務時間、合計獲得金額、現在勤務時間
    public Text prevewTimeText, pomoText, c3, c4,pText,fText,diffText;
    public int pomoCount = 0;
    public State state;

    public float diffTime, 経過時間, 現在休憩時間;
    public int 合計獲得金額;

    public GameObject defaultPanel,workPanel;
    public Text pauseText;

    private float interval;
    private float keikajikan;
    public bool isPomo = true;

    public AudioSource audioSource;
    public AudioClip workClip, pauseClip;

    /* setting */
    //public int setPomodoroTime = 25 * 60;
    //public int pomodoroTime = 25 * 60;
    //public int breakTime = 5 * 60;
    public int setPomodoroTime = 25;
    public int pomodoroTime = 25;
    public int breakTime = 5;
    public Camera cam;
    TimeSpan ts;
    //int pauseDateMiliseconds;
    DateTime startDate,pauseDate, focusDate;
    // Use this for initialization
    void Start()
    {

        if (SaveData.Instance.SampleDict.ContainsKey(DateTime.Today.ToString()))
        {
            pomoCount = SaveData.Instance.SampleDict[DateTime.Today.ToString()];
        }
        else
        {
            SaveData.Instance.SampleDict.Add(DateTime.Today.ToString(), pomoCount);
        }
        Debug.Log(DateTime.Today.ToString());
        //pausePanel.SetActive(false);
        Application.runInBackground = true;
        workPanel.SetActive(false);
        Application.targetFrameRate = 13;
        //cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        //残り勤務時間 = ;
        if (State.WORK == state)
        {
            //prevTime = (経過時間);
        }

        interval += Time.deltaTime;
        //intervalはあくまで画面更新頻度のための時間

        if (state != State.PAUSE && state != State.DEFAULT)
        {
            keikajikan += Time.deltaTime;
            switch (state)
            {
                case State.WORK:
                    diffTime = Mathf.FloorToInt(pomodoroTime - keikajikan);
                    break;
                case State.KYUKEI:
                    diffTime = Mathf.FloorToInt(breakTime - keikajikan);
                    break;
            }

            if (interval > (1.0f))
            {
                interval = 0;
                if (diffTime >= 0)
                {
                    //prevTime--;
                    switch (state)
                    {
                        case State.WORK:
                            ts = TimeSpan.FromSeconds(Mathf.FloorToInt(pomodoroTime - (keikajikan)));
                            break;
                        case State.KYUKEI:
                            ts = TimeSpan.FromSeconds(Mathf.FloorToInt(breakTime - Mathf.FloorToInt(keikajikan)));
                            break;
                    }

                }
                else //タイマーが０より下だったら
                {
                    switch (state)
                    {
                        case State.WORK:
                            pomoCount++;
                            pomoText.text = pomoCount.ToString();
                            this.setKyukei();
                            break;
                        case State.KYUKEI:
                            this.setWork();
                            break;
                    }
                    //prevTime--;
                }

            }
            switch (state)
            {
                case State.DEFAULT:
                    break;
                case State.WORK:
                    経過時間 += Time.deltaTime;
                    break;

                case State.KYUKEI:
                    現在休憩時間 += Time.deltaTime;
                    break;
                case State.PAUSE:
                    現在休憩時間 += Time.deltaTime;
                    break;
            }
            prevewTimeText.text = ts.ToString(@"mm\:ss");
            
        }
    }

    public void SetPomorodo()
    {
        //prevTime = pomodoroTime * 60;
        //prevTime = 25 * 60;
        setWork();
        startDate = DateTime.Now;
    }

    public void PauseButton()
    {
        //1 sigototyuu
        if(State.PAUSE == state)
        {
            state = State.WORK;
            pauseText.text = "Pause";
            cam.backgroundColor = new Color(0, 0.7411765f, 1, 0);
        }
        else
        { 
            state = State.PAUSE;
            pauseText.text = "Restart";
            startDate = DateTime.Now;
        }
        //2 pausetyuu
    }

    public void FinishButton()
    {
        state = State.DEFAULT;
        workPanel.SetActive(false);
        defaultPanel.SetActive(true);
        diffTime = pomodoroTime;
        prevewTimeText.text = ("00:00");
    }

    public void setWork()
    {
        audioSource.clip = workClip;
        audioSource.Play();
        keikajikan = 0;
        diffTime = pomodoroTime;
        //LocalNotificationWrapper.ReserveNotification("TEST","ポモドーロが完了しました！", pomodoroTime - (int)diffTime);
        this.NotificationExample("TEST", "ポモドーロが完了しました！", Mathf.FloorToInt(pomodoroTime - keikajikan));
        defaultPanel.SetActive(false);
        workPanel.SetActive(true);
        audioSource.Play();
        state = State.WORK;
        if (SaveData.Instance.SampleDict.ContainsKey(DateTime.Today.ToString()))
        {
            SaveData.Instance.SampleDict[DateTime.Today.ToString()] = pomoCount;
            SaveData.Instance.Save();
            Debug.Log(SaveData.Instance.SampleDict[DateTime.Today.ToString()]);
        }
        cam.backgroundColor = new Color(0, 0.7411765f, 1, 0);
        startDate = DateTime.Now;
    }
    public void setKyukei()
    {
        audioSource.clip = pauseClip;
        audioSource.Play();
        keikajikan = 0;
        //LocalNotificationWrapper.ReserveNotification("TEST", "休憩が完了しました！", breakTime - (int)diffTime);
        this.NotificationExample("TEST", "休憩が完了しました！", Mathf.FloorToInt(breakTime - keikajikan));
        diffTime = breakTime;
        state = State.KYUKEI;
        cam.backgroundColor = new Color(1, 0.3349057f, 0.4370091f, 0);
        startDate = DateTime.Now;
    }
    public void setPause()
    {
        state = State.PAUSE;
        defaultPanel.SetActive(true);
        workPanel.SetActive(false);

    }
    //フォーカスがあたったとき
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            SaveData.Instance.Reload();
           pomoCount = SaveData.Instance.SampleDict[DateTime.Today.ToString()];
                pomoText.text = pomoCount.ToString();
            if(notification != null)
            {
                iOSNotificationCenter.RemoveAllScheduledNotifications();
            }

            focusDate = DateTime.Now;
            //pText.text = startDate.ToString();
            //fText.text = focusDate.ToString();
            if (pauseDate != null)
            {
                if(state == State.WORK)
                {
                    keikajikan += ((int)((focusDate - pauseDate).TotalMilliseconds) / 1000);
                    Debug.Log(keikajikan.ToString());
                } else if (state == State.KYUKEI)

                {
                    keikajikan += ((int)((focusDate - pauseDate).TotalMilliseconds) / 1000);
                }

            }

            Debug.Log("OnApplicationFocus:" + hasFocus);
        }
        else
        {
            if (state == State.WORK)
            {
                NotificationExample("TEST2", "ポモドーロが終了しました！", ((startDate.Millisecond / 1000) - (int)diffTime));
            }
            else if (state == State.KYUKEI)

            {
                NotificationExample("TEST2", "休憩が終了しました！", ((startDate.Millisecond / 1000) - (int)diffTime));
            }
            //pauseDate = DateTime.;
            pauseDate = DateTime.Now;
            Debug.Log("OnApplicationPause:" + hasFocus);
        }
    }

    public void NotificationExample(string title,string body,int afterTime)
    {
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new TimeSpan(0, 0, afterTime),
            Repeats = false
        };
        notification = new iOSNotification()
        {
            // You can optionally specify a custom Identifier which can later be 
            // used to cancel the notification, if you don't set one, an unique 
            // string will be generated automatically.
            Identifier = "_notification_01",
            Title = title,
            Body = body,
            Subtitle = "This is a subtitle, something, something important...",
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };
        iOSNotificationCenter.ScheduleNotification(notification);
    }
}
