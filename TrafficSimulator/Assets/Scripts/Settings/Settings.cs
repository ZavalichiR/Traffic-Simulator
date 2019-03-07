using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour {

    [Header("SkyBOX")]
    Material night;
    Material evening;
    Material darkday;
    Material day;

    [Range(1,10)]
    public float TimeScale = 1;
    public Text TimeScaleText;
    public Scrollbar ScrBar;
    private bool _pause = false;
    private float _oldValue;

    void Start () {
        ScrBar.value = 0.1f;
        QualitySettings.lodBias = QualitySettings.lodBias * 3f;
    }

    private void Update()
    {
        // Pause button
        if (Input.GetKeyDown("p"))
        {
            if (_pause == false)
                _pause = true;
                
            else
                _pause = false;

        }

        if (!_pause)
        {
            TimeScale = Mathf.Round(ScrBar.value * 10);
            TimeScaleText.text = "Time Scale: " + TimeScale + "x";
            Time.timeScale = TimeScale;
        }
        else
        {
            TimeScale = 0f;
            TimeScaleText.text = " Pause " ;
            Time.timeScale = TimeScale;
        }

    }

    private void changeSkyBox()
    {
    }
}
