using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class TrafficSignalsManager : MonoBehaviour
{
    public Light[] red;
    public Light[] yellow;
    public Light[] green;

    private float redTime = 33f; // roșu stă un pic mai mult ca să nu fie accident în intersecție
    private float yellowTime = 5;
    private float greenTime = 30;

    private int dim;
    private int[] flag, change;
    private bool[] isGreen, isYellow, isRed;


    void Start()
    {

        dim = gameObject.transform.childCount;
        flag = new int[dim]; // random start green/red
        change = new int[dim]; // when the light was changed
        isGreen = new bool[dim]; // the green is on
        isRed = new bool[dim]; // the red is on
        isYellow = new bool[dim]; // the yellow is on

        red = new Light[dim];
        yellow = new Light[dim];
        green = new Light[dim];

        for (int i = 0; i < gameObject.transform.childCount; ++i)
        {
            flag[i] = 0;
            change[i] = 0;
            isGreen[i] = false;
            isYellow[i] = false;
            isRed[i] = false;

            StartTrafficLights(gameObject.transform.GetChild(i), i);
        }

    }

    private void Update()
    {
        for (int i = 0; i < gameObject.transform.childCount; ++i)
        {
            UpdateTrafficLights(gameObject.transform.GetChild(i), i);
        }
    }

    float DegreesBetweenVectors(Vector3 vA, Vector3 vB)
    {
        Vector3 origin = this.gameObject.transform.position;

        var angle = Vector3.Angle(vA - origin, vB - origin);
        return angle;
    }

    private Dictionary<List<int>, float> GetAngles()
    {
        List<int> points = new List<int>();
        Dictionary<List<int>, float> angles = new Dictionary<List<int>, float>();

        for (int i = 0; i < gameObject.transform.childCount; ++i)
        {
            for (int j = i; j < gameObject.transform.childCount; ++j)
            {
                Vector3 pi = gameObject.transform.GetChild(i).transform.position;
                Vector3 pj = gameObject.transform.GetChild(j).transform.position;
                points.Clear();
                //points.Add(new Vector3(pi.x, pi.y, pi.z));
                //points.Add(new Vector3(pj.x, pj.y, pj.z));
                points.Add(i);
                points.Add(j);
                if (pi != pj)
                {
                    if (!angles.ContainsKey(points))
                        angles.Add(new List<int>(points), DegreesBetweenVectors(pi, pj));
                }
            }
        }
        return angles;
    }
    private List<List<int>> SyncTrafficLights()
    {

        Dictionary<List<int>, float> angles = GetAngles();
        List<float> aux = new List<float>(); // Conține toate unghiurile calculate
        foreach (var angle in angles)
        {
            aux.Add(angle.Value);

        }
        
        aux.Sort();// Sortăm descrescător
        aux.Reverse();
        List<List<int>> syncLights = new List<List<int>>();
        // dacă sunt 3 semafoare atunci doar două semafoare vor fi la fel, cele cu unghiul cel mai mare

        if (aux.Count == 3)
        {
            foreach (var angle in angles)
            {
                if (angle.Value == aux[0]) // Luăm unghiul cel mai mare
                {
                    syncLights.Add(angle.Key); // Găsim semafoarele care formează unghiul respectiv
                    break;
                }

            }
        }

        if (aux.Count >= 4)
        {
            int a = 0;
            int b = 0;
            bool flag = true;
            while (flag)
            {
                // Găsesc perechea de semafoare care formează unghiul cel mai mare
                foreach (var angle in angles)
                {
                    if (angle.Value == aux[0])
                    {
                        syncLights.Add(angle.Key);
                        break;
                    }
                }

                // Găsesc perechea de semafoare care formează al doilea unghi cel mai mare
                foreach (var angle in angles)
                {
                    if (angle.Value == aux[1])
                    {
                        syncLights.Add(angle.Key);
                        break;
                    }
                }

                // Verific dacă semafoarele de la prima pereche se găsesc printre semafoarele de la a doua
                if ((syncLights[0])[0] == (syncLights[1])[0] || (syncLights[0])[0] == (syncLights[1])[1] ||
                     (syncLights[0])[1] == (syncLights[1])[0] || (syncLights[0])[1] == (syncLights[1])[1])
                {

                    aux.Remove(aux[0]); // Șterg unghiul cel mai mare
                    syncLights.Clear(); // Curăț lista cu semafoarele sincronizate
                }

                else
                    flag = false;
            }

        }
        return syncLights;
    }

    // Salvez toate culorile semafoarelor
    private void GetLights(Transform trafficLights, int i)
    {
        GameObject rlight;
        GameObject ylight;
        GameObject glight;

        rlight = trafficLights.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
        //Debug.Log(rlight.name);
        ylight = trafficLights.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
        //Debug.Log(ylight.name);
        glight = trafficLights.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject;
        //Debug.Log(glight.name);

        if (rlight.GetComponentInChildren<Light>() != null)
            red[i] = rlight.GetComponentInChildren<Light>();

        if (ylight.GetComponentInChildren<Light>() != null)
            yellow[i] = ylight.GetComponentInChildren<Light>();

        if (glight.GetComponentInChildren<Light>() != null)
            green[i] = glight.GetComponentInChildren<Light>();

        red[i].enabled = false;
        yellow[i].enabled = false;
        green[i].enabled = false;
    }
    private void StartTrafficLights(Transform trafficLights, int i)
    {
        // Preiau componentele Light de la fiecare semafor
        GetLights(trafficLights, i);
        
        red[i].intensity = 0;
        yellow[i].intensity = 0;
        green[i].intensity = 0;

        // Dacă sunt doar două semafoare atunci sunt sincronizate
        if (gameObject.transform.childCount == 2)
            StartCoroutine(GreenLight(i));
        else
        {
            dim = gameObject.transform.childCount;
            // Fiecare pereche are un start diferit
            if (dim >= 2 && dim <= 4)
            {
                if ((trafficLights.name == "TfL1") || (trafficLights.name == "TfL2") )
                    StartCoroutine(GreenLight(i));
                else
                    StartCoroutine(RedLight(i));
            }
        }

    }
    private void UpdateTrafficLights(Transform trafficLights, int i)
    {

        if (change[i] == 1)
        {
            if (isGreen[i] == true)
            {
                isGreen[i] = false;
                change[i] = 0;
                StartCoroutine(GreenLight(i));
            }
            if (isYellow[i] == true)
            {
                isYellow[i] = false;
                change[i] = 0;
                StartCoroutine(YellowLight(i));
            }
            if (isRed[i] == true)
            {
                isRed[i] = false;
                change[i] = 0;
                StartCoroutine(RedLight(i));
            }
        }
    }
    IEnumerator RedLight(int i)
    {
        red[i].enabled = true;
        red[i].intensity = 3;

        yield return new WaitForSeconds(redTime);

        red[i].intensity = 0;
        red[i].enabled = false;
        change[i] = 1;
        isGreen[i] = true;
    }

    IEnumerator YellowLight(int i)
    {
        yellow[i].enabled = true;
        yellow[i].intensity = 4;

        yield return new WaitForSeconds(yellowTime);

        yellow[i].intensity = 0;
        yellow[i].enabled = false;
        change[i] = 1;
        isRed[i] = true;
    }

    IEnumerator GreenLight(int i)
    {
        green[i].enabled = true;
        green[i].intensity = 3;

        yield return new WaitForSeconds(greenTime);

        green[i].intensity = 0;
        green[i].enabled = false;
        change[i] = 1;
        isYellow[i] = true;
    }
}

