using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBoss : MonoBehaviour
{
    public GlobalData globalData;

    public bool bossStarted = false;

    public enum BossPhase {
        attackWait = 0,
        chargeAttack = 1,
        hurt = 2
    }



    public BossPhase currentBossPhase = BossPhase.attackWait;


    public float currentAttackWaitTime = 0.0f;
    public float attackWaitCooldown = 1.5f;

    private float chargeAttackValue = 0.0f;
    public float currentChargeAttackTime = 0.0f;
    public float chargeAttackTime = 1.0f;

    public Transform playerTarget;

    public Transform shootFrom;

    public GameObject chargeAttackObject;

    public GameObject spiderProjectile;

    public float projectileSpeed = 10.0f;

    public Player player;


    public int hp = 3;

    public SpiderKiller spiderKiller;

    public GameObject bossAliveForm;
    public GameObject bossDeadForm;

    public Transform playerSpiderGroup;

    public SineMovement sineMovement;

    public ParticleSystem chargeParticles;

    public Cinemachine.CinemachineVirtualCamera spiderBossCamera;

    public Cinemachine.CinemachineTargetGroup spiderGroup;

    public bool hasDefeatedBoss = false;

    public float maxTimeToNextLevel = 10.0f;
    private float levelTimer = 0.0f;

    private void Start() {
        player = globalData.player;
        playerTarget = player.characterController.transform;
        spiderBossCamera = player.spiderBossCamera;
        spiderGroup.m_Targets[0].target = player.characterController.transform;
        //spiderBossCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>().targ = playerSpiderGroup;
        spiderBossCamera.Follow = playerSpiderGroup;
        spiderBossCamera.LookAt = playerSpiderGroup;

    }

    // Update is called once per frame
    void Update()
    {
        if(hasDefeatedBoss == true) {
            levelTimer += Time.deltaTime;

            if(levelTimer >= maxTimeToNextLevel) {
                hasDefeatedBoss = false;
                levelTimer = 0.0f;
                globalData.player.gameManager.levelTransition.GoToNextLevel();
            }
            
        }
        
        if(Input.GetKeyDown(KeyCode.Tab)) {
            //StartBoss();
        }
        

        if(bossStarted) {


            if(currentBossPhase == BossPhase.attackWait) {
                currentAttackWaitTime += Time.deltaTime;
                if (currentAttackWaitTime >= attackWaitCooldown) {
                    currentAttackWaitTime = 0.0f;
                    currentBossPhase = BossPhase.chargeAttack;
                    chargeParticles.Play();
                }
            }

            if(currentBossPhase == BossPhase.chargeAttack) {
                currentChargeAttackTime += Time.deltaTime;
                chargeAttackValue = Mathf.Lerp(0.0f, 1.0f, currentChargeAttackTime / chargeAttackTime);
                Vector3 chargeSize = new Vector3(chargeAttackValue, chargeAttackValue, chargeAttackValue);


                chargeAttackObject.transform.localScale = chargeSize;

                if(currentChargeAttackTime >= chargeAttackTime) {
                    ShootProjectiles();
                    chargeAttackValue = 0.0f;
                    currentChargeAttackTime = 0.0f;
                    chargeAttackObject.transform.localScale = Vector3.zero;
                    currentBossPhase = BossPhase.attackWait;
                    chargeParticles.Stop();
                }

            }
            


        }
    }

    public void ShootProjectiles() {
        GameObject newProjectile = Instantiate(spiderProjectile, shootFrom.position, Quaternion.identity);
        Vector3 shootVector = new Vector3();
        shootVector = (playerTarget.position - shootFrom.position);
        shootVector.y = 0.0f;
        shootVector = shootVector.normalized * projectileSpeed;
        newProjectile.GetComponent<Rigidbody>().AddForce(shootVector);
        newProjectile.GetComponent<SpiderProjectile>().damage = 2;


        GameObject newProjectile1 = Instantiate(spiderProjectile, shootFrom.position, Quaternion.identity);
        Vector3 shootVector1 = new Vector3();
        shootVector1 = (playerTarget.position - shootFrom.position);
        shootVector1.y = 0.0f;
        shootVector1.x += 2.0f;
        shootVector1 = shootVector1.normalized * projectileSpeed;
        newProjectile1.GetComponent<Rigidbody>().AddForce(shootVector1);
        newProjectile1.transform.localScale *= 0.6f;
        newProjectile1.GetComponent<SpiderProjectile>().damage = 1;


        GameObject newProjectile2 = Instantiate(spiderProjectile, shootFrom.position, Quaternion.identity);
        Vector3 shootVector2 = new Vector3();
        shootVector2 = (playerTarget.position - shootFrom.position);
        shootVector2.y = 0.0f;
        shootVector2.x -= 2.0f;
        shootVector2 = shootVector2.normalized * projectileSpeed;
        newProjectile2.GetComponent<Rigidbody>().AddForce(shootVector2);
        newProjectile2.transform.localScale *= 0.6f;
        newProjectile2.GetComponent<SpiderProjectile>().damage = 1;

    }

    public void StartBoss() {


        bossStarted = true;

        // Change camera
        /*
        player.virtualCamera.Follow = playerSpiderGroup;
        var transposer = player.virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
        transposer.m_FollowOffset.x = 0.0f;
        transposer.m_FollowOffset.y = 11.65f;
        transposer.m_FollowOffset.z = -14.66f;
        */
        player.spiderBossCamera.Priority = 11;

    }

    public void HurtBoss() {
        hp--;
        
        if(hp <= 0) {
            spiderKiller.DropSpiderKiller();
        }

    }

    public void BossDefeated() {
        bossAliveForm.SetActive(false);
        bossDeadForm.SetActive(true);
        chargeAttackObject.SetActive(false);

        bossStarted = false;
        player.jumpUnlocked = true;

        player.virtualCamera.Follow = playerTarget;

        var transposer = player.virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
        transposer.m_FollowOffset.x = 0.0f;
        transposer.m_FollowOffset.y = 8.0f;
        transposer.m_FollowOffset.z = -7.0f;

        sineMovement.enabled = false;

        chargeParticles.Stop();

        player.spiderBossCamera.Priority = 1;

        player.inputMessages.ShowJump();

        hasDefeatedBoss = true;
    }

}
