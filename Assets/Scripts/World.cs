using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    public float backgroundSpeed { get; private set; }
    public float middlegroundSpeed { get; private set; }
    public float foregroundSpeed { get; private set; }

    public float distanceTravelled { get; private set; }

    private List<BackgroundTile> backgroundTiles;
    private List<MiddlegroundTile> middlegroundTiles;
    private List<ForegroundTile> foregroundTiles;

    private PlayerController pc;

    public GameObject backgroundPrefab;
    public GameObject middlegroundPrefab;
    public GameObject foregroundPrefab;

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
        backgroundSpeed = 1.5f;
        middlegroundSpeed = 2f;
        foregroundSpeed = 2.5f;

        //Initialize our background tiles. We assume some tiles are already placed in the scene
        backgroundTiles = new List<BackgroundTile>();
        middlegroundTiles = new List<MiddlegroundTile>();
        foregroundTiles = new List<ForegroundTile>();

        foreach (BackgroundTile bt in GetComponentsInChildren<BackgroundTile>())
        {
            backgroundTiles.Add(bt);
        }

        foreach (MiddlegroundTile mt in GetComponentsInChildren<MiddlegroundTile>())
        {
            middlegroundTiles.Add(mt);
        }

        foreach (ForegroundTile ft in GetComponentsInChildren<ForegroundTile>())
        {
            foregroundTiles.Add(ft);
        }

        pc = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pc.underWater)
        {


            //Check if the oldest tile is off screen yet using AABB
            BackgroundTile tile = backgroundTiles[0];

            Vector3 point = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));
            point.z = 0;

            Debug.Log("Point " + point.ToString());
            if (!tile.GetComponent<Collider2D>().OverlapPoint(point))
            {
                Debug.Log("SHOULD REMOVE TILE");
            }


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


            //Check if oldest middlegroundTile is off screen

            //Check if the oldest tile is off screen yet using AABB
            MiddlegroundTile mtile = middlegroundTiles[0];

            planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            if (!GeometryUtility.TestPlanesAABB(planes, tile.GetComponent<Collider2D>().bounds))
            {
                //If the check fails, we can remove the tile
                middlegroundTiles.RemoveAt(0);
                Destroy(mtile.gameObject);

                //Create a new tile at the end since we can assume our newest tile does not completely cover the screen anymore
                MiddlegroundTile newTile = Instantiate(middlegroundPrefab, transform).GetComponent<MiddlegroundTile>();

                //I cheated and placed the camera so that 0, 0, 0 is at the bottom, since thats much easier than calculating
                //the location of the bottom of the camera. Probably not the best idea to cheat this early, but at the same time,
                //getting this thing up and running asap sounds like a solid idea. Also, the coffee is kicking in. Might need some
                //more tho...hmmm.
                //TODO: test on different screen sizes
                newTile.transform.position = Vector3.zero;
                middlegroundTiles.Add(newTile);

            }

            //Check if the oldest tile is off screen yet using AABB
            ForegroundTile ftile = foregroundTiles[0];

            planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            if (!GeometryUtility.TestPlanesAABB(planes, tile.GetComponent<Collider2D>().bounds))
            {
                //If the check fails, we can remove the tile
                foregroundTiles.RemoveAt(0);
                Destroy(ftile.gameObject);

                //Create a new tile at the end since we can assume our newest tile does not completely cover the screen anymore
                ForegroundTile newTile = Instantiate(foregroundPrefab, transform).GetComponent<ForegroundTile>();

                //I cheated and placed the camera so that 0, 0, 0 is at the bottom, since thats much easier than calculating
                //the location of the bottom of the camera. Probably not the best idea to cheat this early, but at the same time,
                //getting this thing up and running asap sounds like a solid idea. Also, the coffee is kicking in. Might need some
                //more tho...hmmm.
                //TODO: test on different screen sizes
                newTile.transform.position = Vector3.zero;
                foregroundTiles.Add(newTile);

            }






            //Move each tile upwards
            foreach (BackgroundTile go in backgroundTiles)
            {
                go.transform.position = go.transform.position + Vector3.left * pc.moveSpeed * backgroundSpeed * Time.deltaTime;
            }

            //Move each tile upwards
            foreach (MiddlegroundTile go in middlegroundTiles)
            {
                go.transform.position = go.transform.position + Vector3.left * pc.moveSpeed * middlegroundSpeed * Time.deltaTime;
            }
            
            //Move each tile upwards
            foreach (ForegroundTile go in foregroundTiles)
            {
                go.transform.position = go.transform.position + Vector3.left * pc.moveSpeed * foregroundSpeed * Time.deltaTime;
            }

            if (pc.underWater)
            {
                //Update the players distance travelled. Might add a modifier here to make the number more appealing to the player.
                distanceTravelled += backgroundSpeed * pc.moveSpeed * Time.deltaTime;
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
