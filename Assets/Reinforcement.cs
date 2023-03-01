using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Reinforcement : MonoBehaviour
{
    public GameObject Enemy;
    public Vector3 StartingPos;

    private void OnDisable() {
        gameObject.SetActive(true);
        gameObject.transform.localPosition = new Vector3(0,5,0) + StartingPos;
        gameObject.transform.DOMove(StartingPos,1);
    }
    private void OnDestroy() {
        GameObject NewEnemy = Instantiate(Enemy);
        NewEnemy.SetActive(true);
        NewEnemy.transform.localPosition = new Vector3(0,5,0) + StartingPos;
        NewEnemy.transform.DOMove(StartingPos,1);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartingPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
