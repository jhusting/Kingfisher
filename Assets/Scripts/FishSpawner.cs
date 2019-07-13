using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Allows for the designer to specify what fish can spawn where in the level
[System.Serializable]
public struct FishSpawnData
{
    public Fish fish;
    public int minimumDistanceToSpawn;
    public int maximumDistanceToSpawn;
}

public class FishSpawner : MonoBehaviour
{
    public List<FishSpawnData> fishSpawnData;

    public float timeBetweenSpawns = 4f;
    public float spawnTimeVariance = 1.5f;

    public float minimumSpawnHeight = -2;
    public float maximumSpawnHeight = 2;

    private World world;

    // Start is called before the first frame update
    void Start()
    {
        world = GetComponent<World>();

        //StartCoroutine(SpawnFish());
    }


    public IEnumerator SpawnFish()
    {
        PlayerController pc = FindObjectOfType<PlayerController>();
        while (true)
        {
            if (pc.underWater)
            {
                //Calculating when the next fish should spawn
                float timeTillNextSpawn = timeBetweenSpawns + Random.Range(-1 * spawnTimeVariance, spawnTimeVariance);
                yield return new WaitForSeconds(timeTillNextSpawn);



                //Get all possible fish
                float distance = world.distanceTravelled;
                List<Fish> possibleFish = new List<Fish>();

                foreach (FishSpawnData fsd in fishSpawnData)
                {
                    if (distance > fsd.minimumDistanceToSpawn && distance < fsd.maximumDistanceToSpawn)
                    {
                        possibleFish.Add(fsd.fish);
                    }
                }

                //Select the fish from the list of valid fish
                int chosenFishIndex = Mathf.FloorToInt(Random.Range(0, possibleFish.Count));


                //Give it a randomized height
                Vector3 fishPosition = Vector3.up * Random.Range(minimumSpawnHeight, maximumSpawnHeight);

                //Tell the world to add the new fish
                world.AddNewFish(possibleFish[chosenFishIndex], fishPosition);
                yield return null;
            }

            yield return null;
        }
    }


    public void RemoveKingFish()
    {
        fishSpawnData.RemoveAt(fishSpawnData.Count - 1);
    }
}
