using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
    //bool mTimeSwitch;
    const float MAX_WEIT_TIME = 5;
    float mTime;

    private void Start()
    {
        mTime = 0;
       // mTimeSwitch = false;
    }

    // Update is called once per frame
    void Update()
    {
        mTime += Time.deltaTime;

        if (mTime <= MAX_WEIT_TIME)
        {
            return;
            // mTimeSwitch = true;
        }

        if (Input.anyKeyDown/* && mTimeSwitch*/ || mTime > MAX_WEIT_TIME)
        {
			this.enabled = false;
			GameObject.Find("FadeCanvas").GetComponent<Fade>().FadeIn(MAX_WEIT_TIME, () => {
				Application.Quit();
			});
        }
    }
}
