using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public IntReference playerHp;
    public int gameOverFramesDelay = 60;

    GameObject showHide;
    int currentFrameDelay = 0;

    // Start is called before the first frame update
    void Start()
    {
        showHide = this.transform.GetChild(0).gameObject;
        showHide.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHp.value < 1)
        {
            if (currentFrameDelay >= gameOverFramesDelay)
            {
                showHide.SetActive(true);
            }

            currentFrameDelay++;
        }
        else
        {
            currentFrameDelay = 0;
        }
    }
}
