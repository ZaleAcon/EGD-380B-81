﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    public bool flying = false;
    private bool canFly = true;
    public GameObject shadow = null;
    public float defaultY = 0.75f;
    public float shadowDistance = -0.8f;
    private CharData data = null;

    public ScreenFlashEffect sf;
    public Color flashColor;
    public float flashIntensity;
    public float flashLength;

    // Start is called before the first frame update
    void Start()
    {
        defaultY = transform.localPosition.y;
        data = GetComponent<CharData>();
    }

    // Update is called once per frame
    void Update()
    {
        if (flying && canFly)
        {
            canFly = false;
            StartCoroutine(FlyAnim());
        }
    }

    public void UpdateAnimations()
    {
        if (flying)
        {
            defaultY = transform.localPosition.y;
            transform.position += new Vector3(0, Random.Range(-0.05f, 0.05f), 0);
        }
        if (!GetComponent<CharData>().dead)
        {
            shadow.SetActive(true);
        }
    }

    public void HideShadow()
    {
        shadow.SetActive(false);
    }

    IEnumerator FlyAnim()
    {
        Transform t = transform;
        Transform st = shadow.transform;

        while (t.position.y > defaultY - 0.1f)
        {
            t.position -= new Vector3(0, 0.005f, 0);
            st.localPosition += new Vector3(0, 0.005f, 0);
            st.localScale += new Vector3(0.005f, 0.005f, 0);
            yield return new WaitForSeconds(0.03f);
        }

        t.position = new Vector3(t.position.x, defaultY - 0.1f, t.position.z);
        st.localPosition = new Vector3(st.localPosition.x, shadowDistance + 0.1f, st.localPosition.z);
        st.localScale = new Vector3(1.1f, 0.35f, 1);

        yield return new WaitForSeconds(0.02f);

        while (t.position.y < defaultY + 0.1f)
        {
            t.position += new Vector3(0, 0.005f, 0);
            st.position -= new Vector3(0, 0.005f, 0);
            st.localScale -= new Vector3(0.005f, 0.005f, 0);
            yield return new WaitForSeconds(0.03f);
        }

        t.position = new Vector3(t.position.x, defaultY + 0.1f, t.position.z);
        st.localPosition = new Vector3(st.localPosition.x, shadowDistance - 0.1f, st.localPosition.z);
        st.localScale = new Vector3(0.9f, 0.15f, 1);

        yield return new WaitForSeconds(0.02f);
        canFly = true;
        yield return null;
    }

    public void BossDeathAnim()
    {
        GetComponent<Animation>().Play();
        StartCoroutine(BossDeathCoroutine());
    }

    IEnumerator BossDeathCoroutine()
    {
        yield return new WaitForSeconds(2.5f);

        Vector3 pos = transform.localPosition;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        int i = 5;
        while (sr.color.a > 0)
        {
            if (i == 5)
            {
                Managers.SoundEffectManager.Instance.PlaySoundClip(SFX.BossFade, 0.35f);
                i = 0;
            }

            transform.localPosition = pos + new Vector3(-0.1f, 0, 0);
            yield return new WaitForSeconds(0.02f);

            transform.localPosition = pos + new Vector3(0.1f, 0, 0);
            yield return new WaitForSeconds(0.02f);

            i++;
        }

        yield return null;
    }

    public void FlashScreen()
    {
        Managers.SoundEffectManager.Instance.PlaySoundClip(SFX.BossFlash, 0.35f);
        sf.Flash(flashColor, flashIntensity, flashLength);
    }
}
