﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    public Image healthBar = null;
    public List<Color> colors = null;
    public Canvas parentCanvas = null;
    public TextMeshProUGUI name = null;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.color = colors[0];
    }

    public void ChangeHealth(float newHealthPercent)
    {
        StartCoroutine(AlterHealthBar(newHealthPercent));
    }

    public void CreateUI(string name, Transform objTransform)
    {
        this.name.SetText(name);

        // Get the position on the canvas
        Vector2 uiOffset = new Vector2(parentCanvas.GetComponent<RectTransform>().sizeDelta.x / 2f, parentCanvas.GetComponent<RectTransform>().sizeDelta.y / 2f);

        Vector2 sprite_size = objTransform.gameObject.GetComponent<SpriteRenderer>().sprite.rect.size;

        //convert to screen space size
        Vector3 size = new Vector3(0, (sprite_size.y / 2.0f), 0);

        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(objTransform.position - size);
        Vector2 proportionalPosition = new Vector2(ViewportPosition.x * parentCanvas.GetComponent<RectTransform>().sizeDelta.x, ViewportPosition.y * parentCanvas.GetComponent<RectTransform>().sizeDelta.y);
        Vector2 actualPosition = proportionalPosition - uiOffset;
        actualPosition.y = (actualPosition.y / objTransform.localScale.y) - ((0.5f + (objTransform.localScale.y / 10)) * objTransform.localScale.y) - (2.0f - objTransform.position.z);

        // Set the position and remove the screen offset
        gameObject.transform.localPosition = actualPosition;
    }

    IEnumerator AlterHealthBar(float newHealthPercent)
    {
        bool decrease = true;
        float multiplier = 1;
        if (healthBar.fillAmount < newHealthPercent)
        {
            decrease = false;
            multiplier = -1;
        }

        while ((decrease && healthBar.fillAmount > newHealthPercent)
            || (!decrease && healthBar.fillAmount < newHealthPercent))
        {
            healthBar.fillAmount -= 0.01f * multiplier;

            if (healthBar.fillAmount > 0.5f)
                healthBar.color = colors[0];
            else if (healthBar.fillAmount <= 0.5f && healthBar.fillAmount > 0.25f)
                healthBar.color = colors[1];
            else if (healthBar.fillAmount <= 0.25f)
                healthBar.color = colors[2];

            yield return new WaitForSeconds(Managers.TurnManager.Instance.tracker.timeIncrements);
        }

        healthBar.fillAmount = newHealthPercent;

        yield return null;
    }
}
