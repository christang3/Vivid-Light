using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Start is called before the first frame update
    public float MaxHP = 1f;
    float HP;
    public bool EpicDead=false;

    bool IFrame = false;

    public Material FlashMat;
    public Color FlashCol = new Color (1,1,1,1);

    SpriteRenderer SP;
    Material OldMat;
    Color OldCol;

    private void Start() {
        HP = MaxHP;
        SP = transform.gameObject.GetComponent<SpriteRenderer>();
        OldMat = SP.material;
        OldCol = SP.color;
        FlashCol = new Color (1,1,1,1);
    }

    IEnumerator Flash()
    {
        IFrame = true;
        SP.material = FlashMat;
        SP.color = new Color (0,0,0,1);
        yield return new WaitForSeconds(.1f);
        SP.color = new Color (1,1,1,1);
        yield return new WaitForSeconds(.1f);
        if(HP <= 0)
        {
            SP.color = new Color (0,0,0,1);
            yield return new WaitForSeconds(.1f);
            SP.color = new Color (1,1,1,1);
            yield return new WaitForSeconds(.1f);
            //gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        IFrame = false;
        SP.material = OldMat;
        SP.color = OldCol;
        if (HP <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void FullHeal()
    {
        HP = MaxHP;
    }

    public float GetHP()
    {
        return HP;
    }

    public float GetMaxHP()
    {
        return MaxHP;
    }

    public void TakeDMG(float DMG) 
    {
        if (IFrame == false)
        {
            HP -= DMG;
            StartCoroutine(Flash());
            /*
            if (gameObject.TryGetComponent<HPBAR>(out HPBAR PComp))
                {
                    PComp.TakeDMG(HP,MaxHP);
                }
            /*
            if(HP <= 0)
            {
                Destroy(transform.parent.gameObject);
            }
            */
        }
    }
}
