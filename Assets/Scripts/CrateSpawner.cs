using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject cratePrefab;
    [SerializeField]
    SO_Item SO_zuch;
    [SerializeField]
    SO_Item SO_corn;
    [SerializeField]
    SO_Item SO_tomato;
    [SerializeField]
    SO_Item SO_grapes;

    // Create a crate
    // initialize the settings
    // Set the amount needed and timer
    // Instantiate it 
    // Add it to the list of tasks


    public void SpawnCrate(SO_Item product,int amountNeeded,float timer)
    {
        // Eventually update this to pass in a prefab, determined by the difficulty controller
        Crate crate = Instantiate(cratePrefab, transform.position, Quaternion.identity).GetComponent<Crate>();
        crate.setupCrate(amountNeeded, timer, product);
    }

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(Spawn());
    }
    IEnumerator Spawn()
    {
        SpawnCrate(SO_zuch,2,1);
        yield return new WaitForSeconds(1f);
        SpawnCrate(SO_grapes, 10, 1f);
        yield return new WaitForSeconds(1f);
        SpawnCrate(SO_tomato, 10, 1f);
        yield return new WaitForSeconds(5f);
        SpawnCrate(SO_corn, 10, 5f);
        yield return new WaitForSeconds(5f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
