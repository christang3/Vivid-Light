using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TurnBased : MonoBehaviour
{
    public GameObject PlrSkillsMenu;
    public GameObject PlrInvMenu;

    public GameObject Player;
    public GameObject Enemy;

    public GameObject PlayerHpUI;
    public GameObject EnemyHPUI;

    private int Turn = 0;
    public float TimeLimit = 10;
    public int MaxTurn = 30;

    public float GunDamage = 1;
    public float GunCD = 3;
    public float EnemyDamage = 1;
    public float EnemyCD = 5;
    private float CurrentTime;
    private float Score;
    private float EnemyGivePoint = 700;

    private bool Attacking = false;

    public GameObject Camera;
    private Vector3 CamStartingPos;

    public GameObject PlrSKill1SFX;
    public GameObject PlrInv1SFX;
    public GameObject EnemyATKSFX;

    public GameObject TimelimitText;
    public GameObject ScoreText;

    private TextMeshProUGUI Text1;
    private TextMeshProUGUI Text2;

    private Slider HPBar;
    public GameObject HPBarUI;

    public GameObject HealEFX;
    public GameObject HealEFX2;

    public GameObject ScoreUp;

    IEnumerator CameraShake(float Duration,float magnitude)
     {
        Vector3 StartingP = Camera.transform.position;

        float elapsedTime = 0f;
        while (elapsedTime < Duration)
        {
            float RandomX = Random.Range(-.05f,.05f)*magnitude;
            float RandomY = Random.Range(-.05f,.05f)*magnitude;

            Camera.transform.localPosition = new Vector3(RandomX,RandomY,StartingP.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = StartingP;
     }

    IEnumerator EnemyAIStuff()
    {
        while (true)
        {
            yield return new WaitForSeconds(EnemyCD);
            EnemyAttack();
        }
    }
    IEnumerator IdleSprite(Transform Obj,float WanderNum)
     {
        Vector3 StartingP = Obj.position;
        while (!Attacking)
        {
            if (!Attacking)
            {
                float RandomX = Random.Range(-0.5f,0.5f)*WanderNum;
                float RandomY = Random.Range(-.5f,.5f)*WanderNum;
                Obj.DOMove(new Vector3(StartingP.x+RandomX,StartingP.y+RandomY,0),.2f).SetEase(Ease.InQuad);
                yield return new WaitForSeconds(.2f);
                Obj.DOMove(StartingP,.2f).SetEase(Ease.OutQuad);
                yield return new WaitForSeconds(.2f);
            }
        }
     }

     IEnumerator PlaySFX(GameObject SFX)
     {
        GameObject SFXCopy = Instantiate(SFX);
        SFXCopy.SetActive(true);
        yield return new WaitForSeconds(5f);
        Destroy(SFXCopy);
     }

     IEnumerator PlrAttack(GameObject Attacker,bool Side,float Damage,GameObject Target,GameObject SFX)
     {
        if (Target.TryGetComponent<Health>(out Health HComp))
            {
                Attacking = true;
                Vector3 StartingP = Attacker.transform.position;
                StartCoroutine(PlaySFX(SFX));

                float xOffset = 2f;
                if (Side)
                {
                    xOffset = -xOffset;
                }
                Attacker.transform.DOMove(new Vector3(StartingP.x+xOffset,StartingP.y,0),.2f).SetEase(Ease.OutElastic);
                StartCoroutine(CameraShake(.1f,2f));
                yield return new WaitForSeconds(.1f);
                HComp.TakeDMG(Damage);
                if (Target == Player)
                {
                    HPBar.value = HComp.GetHP()/HComp.GetMaxHP();
                }
                yield return new WaitForSeconds(.1f);
                Attacker.transform.position = StartingP;
                //Attacker.DOMove(StartingP,.5f);
                //yield return new WaitForSeconds(.5f);
                yield return new WaitForSeconds(.4f);
                Attacking = false;
            }
     }

     public void ChangeImgAlpha(Image image,float Alp)
     {
        image.color = new Color(image.color.r, image.color.g, image.color.b, Alp);
     }

     IEnumerator PlrHeal(GameObject Attacker,GameObject SFX)
     {
        if (Attacker.TryGetComponent<Health>(out Health HComp))
            {
                Attacking = true;
                Vector3 StartingP = Attacker.transform.position;
                StartCoroutine(PlaySFX(SFX));

                HealEFX.SetActive(true);
                Vector3 OriginalSize = HealEFX.transform.localScale;

                HealEFX.transform.DOScale(OriginalSize*2,.8f);
                Image Image1 = HealEFX.GetComponent<Image>();
                Image1.DOFade(.2f, .8f);

                yield return new WaitForSeconds(.5f);
                StartCoroutine(CameraShake(.1f,.5f));
                yield return new WaitForSeconds(.3f);
                HComp.FullHeal();
                //StartCoroutine(CameraShake(.1f,5f));
                HealEFX2.SetActive(true);
                Image Image2 = HealEFX2.GetComponent<Image>();
                Image2.DOFade(0f, .2f);
                HPBar.value = HComp.GetHP()/HComp.GetMaxHP();
                Attacking = false;
                yield return new WaitForSeconds(.2f);
                HealEFX.SetActive(false);
                HealEFX2.SetActive(false);
                ChangeImgAlpha(Image1,1);
                ChangeImgAlpha(Image2,1);
                HealEFX.transform.localScale = OriginalSize;
            }
     }

     IEnumerator ScoreUP()
     {
        ScoreUp.SetActive(true);
        Vector3 OriginalPos = ScoreUp.transform.position;
        ScoreUp.transform.DOMove(OriginalPos+new Vector3(0,3,0),.5f);
        yield return new WaitForSeconds(.5f);
        ScoreUp.transform.position = OriginalPos;
        ScoreUp.SetActive(false);
     }
     

     public void ReTarget()
     {
        if (!Player.activeSelf)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        if (!Enemy.activeSelf)
        {
            Vector3 StartingP = Enemy.transform.position;
            Score += EnemyGivePoint;
            StartCoroutine(ScoreUP());
            Text2.SetText("Score: "+Score.ToString("00000"));
            CurrentTime = EnemyCD+1;
            if (Enemy.TryGetComponent<Health>(out Health HComp))
            {
                HComp.FullHeal();
            }
            Enemy.SetActive(true);
            Enemy.transform.localPosition = new Vector3(0,5,0) + StartingP;
            Enemy.transform.DOMove(StartingP,1);
            //Enemy = GameObject.Find("Enemy(Clone)");
        }
     }

     public void Skill1()
     {
        Debug.Log("'yeah'");
        CloseAllTabs();
        if (!Attacking)
        {
            StartCoroutine(PlrAttack(Player,false,GunDamage,Enemy,PlrSKill1SFX));
        }
     }

     public void Item1()
     {
        Debug.Log("'yeah'");
        CloseAllTabs();
        if (!Attacking)
        {
            StartCoroutine(PlrHeal(Player,PlrInv1SFX));
        }
     }

     public void EnemyAttack()
     {
        if (!Attacking)
        {
            StartCoroutine(PlrAttack(Enemy,true,EnemyDamage,Player,EnemyATKSFX));
        }
     }

    // Start is called before the first frame update
    void Start()
    {
        HPBar = HPBarUI.GetComponent<Slider>();
        Text1 = TimelimitText.GetComponent<TextMeshProUGUI>();
        Text2 = ScoreText.GetComponent<TextMeshProUGUI>();
        CamStartingPos = Camera.transform.position;
        CurrentTime = EnemyCD;
        StartCoroutine(IdleSprite(Player.transform,.3f));
        StartCoroutine(IdleSprite(Enemy.transform,.3f));
    }

    public void CloseAllTabs()
    {
        PlrSkillsMenu.SetActive(false);
        PlrInvMenu.SetActive(false);
    }

    public void OpenPlrSkillsMenu()
    {
        CloseAllTabs();
        PlrSkillsMenu.SetActive(true);
    }

    public void OpenPlrInventory()
    {
        CloseAllTabs();
        PlrInvMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        ReTarget();
        CurrentTime -= Time.deltaTime;
        Text1.SetText("Time Limit "+CurrentTime.ToString("0.00"));
        if (CurrentTime <= 0)
        {
            if (!Attacking)
            {
                CurrentTime = EnemyCD;
                EnemyAttack();
            }
        }
    }
}
