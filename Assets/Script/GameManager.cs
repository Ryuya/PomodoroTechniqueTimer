using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
public enum State
{
    DEFAULT = 0,
    WORK,
    KYUKEI,
    PAUSE,
 }

public class GameManager : MonoBehaviour
{
    //順番に残り勤務時間、合計獲得金額、現在勤務時間
    public Text c1, c2, c3, c4;
    public State state;

    public float prevTime, 経過時間, 現在休憩時間;
    public int 合計獲得金額;

    public GameObject defaultPanel,workPanel;
    public Text pauseText;

    private float interval;
    public bool isPomo = true;

    public AudioSource audioSource;
    public AudioClip workClip, pauseClip;

    /* setting */
    public int pomodoroTime = 25 * 60;
    public int breakTime = 5 * 60;
    public Camera cam;
    TimeSpan ts;
    // Use this for initialization
    void Start()
    {
        //pausePanel.SetActive(false);
        workPanel.SetActive(false);
        Application.targetFrameRate = 13;
        //cam = GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
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
            if (interval > 1.0f)
            {   
                ts = new TimeSpan(0, 0, (int)prevTime);
                //c1.text = (((int)(prevTime / 60 % 60)).ToString() + ":" + ((int)(prevTime % 60)).ToString());
                c1.text = ts.ToString(@"mm\:ss");
                interval = 0;
                if (prevTime > 0)
                {
                    prevTime--;
                }
                else //タイマーが０より下だったら
                {
                    switch (state)
                    {
                        case State.WORK:
                            this.setKyukei();
                            break;
                        case State.KYUKEI:
                            this.setWork();
                            break;
                    }
                    prevTime--;
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
        }

    }

    public void SetPomorodo()
    {
        //prevTime = pomodoroTime * 60;

        //state = State.WORK;
        //prevTime = 25 * 60;
        setWork();
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
        }
        //2 pausetyuu
    }

    public void FinishButton()
    {
        state = State.DEFAULT;
        workPanel.SetActive(false);
        defaultPanel.SetActive(true);
        prevTime = pomodoroTime;
        c1.text = ("00:00");
    }

    public void setWork()
    {
        audioSource.clip = workClip;
        audioSource.Play();
        prevTime = pomodoroTime;
        defaultPanel.SetActive(false);
        workPanel.SetActive(true);
        audioSource.Play();
        state = State.WORK;
        cam.backgroundColor = new Color(0, 0.7411765f, 1, 0);
    }
    public void setKyukei()
    {
        audioSource.clip = pauseClip;
        audioSource.Play();
        prevTime = breakTime;
        state = State.KYUKEI;
        cam.backgroundColor = new Color(1, 0.3349057f, 0.4370091f, 0);
    }
    public void setPause()
    {
        state = State.PAUSE;
        defaultPanel.SetActive(true);
        workPanel.SetActive(false);
    }
}
