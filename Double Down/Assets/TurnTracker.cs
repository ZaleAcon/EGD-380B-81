﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnTracker : MonoBehaviour
{
    public Transform t1Storage;
    public Transform t2Storage;
    public List<Image> tracker;
    [HideInInspector] public List<GameObject> t1;
    [HideInInspector] public List<GameObject> t2;
    public GameObject image;

    public float seconds = 3.0f;
    public float timeIncrements = 0.005f;

    public bool bigButton = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (bigButton)
        {
            bigButton = false;
            StartCoroutine(MoveTurnTrackerUI());
        }
    }

    public void SetUpTrackers(List<GameObject> l)
    {
        t1.Add(null);

        for (int i = 0; i < l.Count; ++i)
            CreateTracker(l[i], i);

        StartCoroutine(MoveTurnTrackerUI());
    }

    // Creates a tracker at a specific index
    //  - Used to create the beginning turn order
    //  - Used to slide enemies into the turn order
    public void CreateTracker(GameObject theImage, int i)
    {
        GameObject g = Instantiate(image, t1Storage);
        //g.GetComponent<Image>() = i.GetComponent<CharData>().GetPortrait();
        g.GetComponent<Image>().color = theImage.GetComponent<SpriteRenderer>().color;
        g.transform.position = tracker[i + 1].rectTransform.position;
        //g.transform.localScale = tracker[i + 1].rectTransform.localScale;

        t1.Add(g);
    }

    public void SetTrackers(List<GameObject> l)
    {
        for (int i = 0; i < l.Count; ++i)
        {
            tracker[i].color = Color.blue;
        }
    }

    // Moves the turn order to the LEFT whenever a character performs an action
    IEnumerator MoveTurnTrackerUI()
    {
        bool moving = true;
        yield return new WaitForSeconds(0.05f);

        float mainInc = (240 / seconds) * timeIncrements;
        float incs = (200 / seconds) * timeIncrements;

        while (moving)
        {
            for (int i = 0; i < t1.Count; ++i)
            {
                if (i > 1 && t1[i].transform.position.x > tracker[i - 1].transform.position.x)
                {
                    t1[i].transform.position -= new Vector3(incs, 0);
                }

                else if (i == 1 && t1[1].transform.position.x > tracker[0].transform.position.x)
                {
                    t1[1].transform.position -= new Vector3(mainInc, 0);
                    if (t1[1].transform.localScale.x < 1.5f)
                        t1[1].transform.localScale += new Vector3(0.005f * mainInc, 0.005f * mainInc, 0.005f * mainInc);
                }

                else if (i == 0 && t1[0] != null)
                {
                    t1[0].transform.position -= new Vector3(mainInc, 0);
                    t1[0].GetComponent<Image>().color -= new Color(0, 0, 0, 0.01f * mainInc);
                }
            }

            yield return new WaitForSeconds(timeIncrements);

            if (t1.Count > 1 && t1[1].transform.position.x <= tracker[0].transform.position.x)
            {
                t1[1].transform.position = tracker[0].transform.position;
                moving = false;
            }
        }

        Destroy(t1[0]);
        t1.Remove(t1[0]);

        yield return null;
    }
}