﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{

	public GameObject player;
	public GameObject enemy;

	public Transform playerBattleStation;
	public Transform enemyBattleStation;

	Unit playerUnit;
	Unit enemyUnit;

	public Text dialogueText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;

    private EnemyAgent agent;

    // Start is called before the first frame update
    void Start()
    {
		state = BattleState.START;
        agent = GetComponent<EnemyAgent>();
		StartCoroutine(SetupBattle());
    }

	IEnumerator SetupBattle()
	{
		//GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = player.GetComponent<Unit>();

		//GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		enemyUnit = enemy.GetComponent<Unit>();

		dialogueText.text = "A wild " + enemyUnit.unitName + " approaches...";

		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}

	IEnumerator PlayerAttack()
	{
		bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogueText.text = "The attack is successful!";

		yield return new WaitForSeconds(2f);

		if(isDead)
		{
			state = BattleState.WON;
			EndBattle();
		}
        else
		{

            state = BattleState.ENEMYTURN;
            agent.AddReward(-0.2f);
            //StartCoroutine(EnemyTurn());
            //EnemyTurn();
        }
	}

	public void EnemyTurn(float[] vectorAction)
	{
        dialogueText.text = enemyUnit.unitName + " attacks!";
        
        if(vectorAction[0] == 1)
        {
            bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
            playerHUD.SetHP(playerUnit.currentHP);
            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                agent.AddReward(0.5f);
                PlayerTurn();
            }
        }
        else if (vectorAction[1] == 1)
        {
            enemyUnit.Heal(5);
            agent.AddReward(0.5f);
            enemyHUD.SetHP(enemyUnit.currentHP);
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }

        //yield return new waitforseconds(1f);

        //bool isdead = playerunit.takedamage(enemyunit.damage);

        //playerhud.sethp(playerunit.currenthp);

        //yield return new waitforseconds(1f);

        //if (isdead)
        //{
        //    state = battlestate.lost;
        //    endbattle();
        //}
        //else
        //{
        //    state = battlestate.playerturn;
        //    playerturn();
        //}

    }

    void EndBattle()
	{
		if(state == BattleState.WON)
		{
			dialogueText.text = "You won the battle!";
		} else if (state == BattleState.LOST)
		{
			dialogueText.text = "You were defeated.";
		}
	}

	void PlayerTurn()
	{
		dialogueText.text = "Choose an action:";
	}

	IEnumerator PlayerHeal()
	{
		playerUnit.Heal(5);

		playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = "You feel renewed strength!";

		yield return new WaitForSeconds(2f);

		state = BattleState.ENEMYTURN;
	}

	public void OnAttackButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;
        StartCoroutine(PlayerAttack());
		
	}

	public void OnHealButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerHeal());
	}

}
