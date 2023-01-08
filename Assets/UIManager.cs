using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField]
    GameObject infoBoardContainer;
    [SerializeField]
    GameObject store;
    [SerializeField]
    GameObject scoreboard;
    [SerializeField]
    GameObject targetPoint;
    [SerializeField]
    float animSpeed = 1000f;

    Vector3 ScoreHome;

    bool repeat = true;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        ScoreHome = infoBoardContainer.transform.position;
        Debug.Log("scorehome: " + ScoreHome);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowScore()
    {
        scoreboard.SetActive(true);
    }
    public void ShowBoard()
    {
        MoveItem(infoBoardContainer.transform, targetPoint.transform.position);
    }
    public void RemoveBoard()
    {
        infoBoardContainer.transform.position = ScoreHome;
        scoreboard.SetActive(false);
        store.SetActive(false);
    }
    public void MoveItem(Transform source, Vector3 target)
    {
        StartCoroutine(MoveScreen(source,target));
    }
    public IEnumerator MoveScreen(Transform source, Vector3 target)
    {

        // Debug.Log(Vector3.Distance(source.position, target));
        while (true)
        {
            while(Vector3.Distance(source.position, target)>1f)
            {
                //Debug.Log(Vector3.Distance(source.position, target));
                source.transform.position = Vector3.MoveTowards(source.position, target,Time.deltaTime* animSpeed);
                yield return new WaitForSeconds(.01f);
            }
            
            yield return new WaitForSeconds(1f);
        }


    }

    public void ResetScoreboard()
    {
        infoBoardContainer.transform.position = ScoreHome;
    }
}
