using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    public float speed { get; private set; }
    public float distanceTravelled { get; private set; }

    private List<BackgroundTile> backgroundTiles;
    private PlayerController pc;

    public GameObject backgroundPrefab;

    public static World world { get; private set; }

    void Awake()
    {
        //Make this a singleton
        if (world != null)
        {
            Destroy(gameObject);
        }
        else
        {
            world = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //Setting an initial speed for testing. Should be calculated based on character speed in the future.
        speed = 2f;

        //Initialize our background tiles. We assume some tiles are already placed in the scene
        backgroundTiles = new List<BackgroundTile>();
        foreach (BackgroundTile bt in GetComponentsInChildren<BackgroundTile>())
        {
            backgroundTiles.Add(bt);
        }

        pc = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pc.underWater)
        {
            //PlayerController playerController = FindObjectOfType<PlayerController>();
            speed = 2 * pc.moveSpeed;


            //Check if the oldest tile is off screen yet using AABB
            BackgroundTile tile = backgroundTiles[0];

            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            if (!GeometryUtility.TestPlanesAABB(planes, tile.GetComponent<Collider2D>().bounds))
            {
                //If the check fails, we can remove the tile
                backgroundTiles.RemoveAt(0);
                Destroy(tile.gameObject);

                //Create a new tile at the end since we can assume our newest tile does not completely cover the screen anymore
                BackgroundTile newTile = Instantiate(backgroundPrefab, transform).GetComponent<BackgroundTile>();

                //I cheated and placed the camera so that 0, 0, 0 is at the bottom, since thats much easier than calculating
                //the location of the bottom of the camera. Probably not the best idea to cheat this early, but at the same time,
                //getting this thing up and running asap sounds like a solid idea. Also, the coffee is kicking in. Might need some
                //more tho...hmmm.
                //TODO: test on different screen sizes
                newTile.transform.position = Vector3.zero;
                backgroundTiles.Add(newTile);

            }



            //Move each tile upwards
            foreach (BackgroundTile go in backgroundTiles)
            {
                go.transform.position = go.transform.position + Vector3.left * speed * Time.deltaTime;
            }

            if (pc.underWater)
            {
                //Update the players distance travelled. Might add a modifier here to make the number more appealing to the player.
                distanceTravelled += speed * Time.deltaTime;
            }
        }
    }


    public BackgroundTile GetNewestTile()
    {
        return backgroundTiles[1];
    }
    
    public void ResetRun()
    {
        distanceTravelled = 0;
    }

    public void AddNewFish(Fish fishPrefab, Vector3 location)
    {
        //All fish will spawn slightly to the right of the screen, so we attach it to the newest BackgroundTile to easily have it move with
        //the worlds move speed
        BackgroundTile newestTile = GetNewestTile();
        Fish newFish = Instantiate(fishPrefab, newestTile.transform);

        Vector3 newScale = newestTile.transform.localScale;
        newScale.x = 1f / newScale.x;
        newScale.y = 1f / newScale.y;
        newScale.z = 1f / newScale.z;
        newFish.transform.localScale = newScale;

        //Give it a randomized height
        newFish.transform.position = location;
    }
}
