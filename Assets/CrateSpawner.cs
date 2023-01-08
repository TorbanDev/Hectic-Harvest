using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject cratePrefab;

    // Create a crate
    // initialize the settings
    // Set the amount needed and timer
    // Instantiate it 
    // Add it to the list of tasks


    public void SpawnCrate()
    {
        // Eventually update this to pass in a prefab, determined by the difficulty controller
        Crate crate = Instantiate(cratePrefab, transform.position, Quaternion.identity).GetComponent<Crate>();
        crate.setupCrate(5, 100f);
    }

    // Start is called before the first frame update
    void Awake()
    {
        InvokeRepeating("SpawnCrate", 3f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
