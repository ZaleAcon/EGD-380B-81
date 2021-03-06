﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HubEvents
{
    Pass,
    Battle,
    Boss
}

public class EventObj : MonoBehaviour
{
    public HubEvents type;
    public string text;
    public EventTextBox box = null;
    public List<GameObject> enemies = null;
    public GameObject player;
    private bool combatActive = false;
    private bool contactActive = true;
    private bool init = false;
    public int eventNum = 0;

    private void Start()
    {
        Transform enemyCharsContainer = GameObject.Find("NonCombatEnemies").transform;
        List<GameObject> enemyChars = new List<GameObject>();
        for (int i = 0; i < enemyCharsContainer.childCount; ++i)
        {
            if (enemyCharsContainer.GetChild(i).GetComponent<CharData>().attachedEventNum == eventNum)
                enemyChars.Add(enemyCharsContainer.GetChild(i).gameObject);
        }

        enemies.Clear();
        for (int i = 0; i < enemyChars.Count; ++i)
            enemies.Add(enemyChars[i]);

        bool check = false;

        for (int i = 0; i < enemies.Count; ++i)
        {
            if ((type == HubEvents.Battle || type == HubEvents.Boss) && !enemies[i].GetComponent<CharData>().dead)
                check = true;
        }

        if (type == HubEvents.Pass)
        {
            check = true;
        }

        if (!check)
        {
            for (int i = 0; i < enemies.Count; ++i)
                enemies[0].SetActive(false);

            box.DisableBox();
            gameObject.SetActive(false);
        }
        else if (check && enemies.Count > 0)
        {
            for (int i = 0; i < enemies.Count; ++i)
                if (enemies[i].GetComponent<CharData>().isInCombat)
                    combatActive = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        // Combat init
        if (contactActive && other.gameObject == Managers.TurnManager.Instance.t1[0] && !other.gameObject.GetComponent<CharData>().hasActed 
            && !other.gameObject.GetComponent<CharData>().isInCombat && other.gameObject.tag == "Player" && (type == HubEvents.Battle || type == HubEvents.Boss))
        {
            player = other.gameObject;
            Managers.TurnManager.Instance.t1[0].GetComponent<CharAnimator>().PlayAnimations(AnimationClips.CombatIdle);
            box.PassEventIn(text, gameObject);
        }

        // Pass
        if (contactActive && other.gameObject == Managers.TurnManager.Instance.t1[0] && !other.gameObject.GetComponent<CharData>().hasActed
            && !other.gameObject.GetComponent<CharData>().isInCombat && other.gameObject.tag == "Player" && type == HubEvents.Pass && Managers.MovementManager.Instance.movedVar)
        {
            player = other.gameObject;
            Managers.TurnManager.Instance.t1[0].GetComponent<CharAnimator>().PlayAnimations(AnimationClips.Idle);
            box.PassEventIn(text, gameObject);
        }

        // Combat re-enter
        else if (other.gameObject == Managers.TurnManager.Instance.t1[0] && !other.gameObject.GetComponent<CharData>().hasActed 
            && other.gameObject.GetComponent<CharData>().isInCombat && other.gameObject.tag == "Player" && (type == HubEvents.Battle || type == HubEvents.Boss))
        {
            player = other.gameObject;
            Managers.TurnManager.Instance.t1[0].GetComponent<CharAnimator>().PlayAnimations(AnimationClips.CombatIdle);
            PassExistingCombat(type);
        }
    }

    public void PassResponse(bool @bool)
    {
        if (type == HubEvents.Pass && @bool)
            Managers.TurnManager.Instance.EndRound();
        else if ((type == HubEvents.Battle || type == HubEvents.Boss) && @bool && !combatActive)
            Managers.CombatTransitionManager.Instance.CreateNewCombatInstance(type, eventNum, player, enemies);
        else if ((type == HubEvents.Battle || type == HubEvents.Boss) && @bool && combatActive)
            PassExistingCombat(type);
        else if (!@bool)
            Managers.MovementManager.Instance.canMoveChars = true;
    }

    public void PassExistingCombat(HubEvents type)
    {
        //Managers.TurnManager.Instance.t1[0].GetComponent<CharacterController>().enabled = false;
        Managers.CombatTransitionManager.Instance.EnterExistingCombatInstance(type, eventNum, player, enemies);
    }
}
