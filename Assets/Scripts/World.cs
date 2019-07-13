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


    //By default all fish are speed up based on the world speed. Lowering this will lower the effect world speed has on fish.
    public float fishSpeedModifier = 1f;

    public GameObject fishHolder;

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
        //if (pc.underWater)
        {



            //Check if the oldest tile is off screen yet using AABB
            BackgroundTile btile = backgroundTiles[0];

            //planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            //if (!GeometryUtility.TestPlanesAABB(planes, tile.GetComponent<Collider2D>().bounds))
            float backgroundTileSize = btile.GetComponent<BoxCollider2D>().size.x;

            //At -size distance we are entirely on screen, at -2 we are entirely off screen. giving -2.5 for a little bit of leeway
            //so players dont see popping in and out
            float despawnX = backgroundTileSize * -3f;
            if (btile.transform.position.x < despawnX)
            {
                //If the check fails, we can remove the tile
                backgroundTiles.RemoveAt(0);
                Destroy(btile.gameObject);

                //Create a new tile at the end since we can assume our newest tile does not completely cover the screen anymore
                BackgroundTile newTile = Instantiate(backgroundPrefab, transform).GetComponent<BackgroundTile>();

                //I cheated and placed the camera so that 0, 0, 0 is at the bottom, since thats much easier than calculating
                //the location of the bottom of the camera. Probably not the best idea to cheat this early, but at the same time,
                //getting this thing up and running asap sounds like a solid idea. Also, the coffee is kicking in. Might need some
                //more tho...hmmm.
                //TODO: test on different screen sizes
                float pos = backgroundTiles[backgroundTiles.Count - 1].transform.position.x + backgroundTileSize;

                newTile.transform.position = Vector3.right * pos;
                backgroundTiles.Add(newTile);

            }




            //Check if the oldest tile is off screen yet using AABB
            ForegroundTile ftile = foregroundTiles[0];

            //planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            //if (!GeometryUtility.TestPlanesAABB(planes, tile.GetComponent<Collider2D>().bounds))
            float foregroundTileSize = ftile.GetComponent<BoxCollider2D>().size.x;

            //At -size distance we are entirely on screen, at -2 we are entirely off screen. giving -2.5 for a little bit of leeway
            //so players dont see popping in and out
            despawnX = foregroundTileSize * -3f;
            if (ftile.transform.position.x < despawnX)
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
                float pos = foregroundTiles[foregroundTiles.Count - 1].transform.position.x + foregroundTileSize;

                newTile.transform.position = Vector3.right * pos;
                foregroundTiles.Add(newTile);

            }



            //Check if the oldest tile is off screen yet using AABB
            MiddlegroundTile mtile = middlegroundTiles[0];

            //planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            //if (!GeometryUtility.TestPlanesAABB(planes, tile.GetComponent<Collider2D>().bounds))
            float middleTileSize = mtile.GetComponent<BoxCollider2D>().size.x;

            //At -size distance we are entirely on screen, at -2 we are entirely off screen. giving -2.5 for a little bit of leeway
            //so players dont see popping in and out
            despawnX = middleTileSize * -3f;
            if (mtile.transform.position.x < despawnX)
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
                float pos = middlegroundTiles[middlegroundTiles.Count - 1].transform.position.x + middleTileSize;

                newTile.transform.position = Vector3.right * pos;
                middlegroundTiles.Add(newTile);

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




            //Move the fish!
            fishHolder.transform.position = fishHolder.transform.position + Vector3.right * -1 * middlegroundSpeed * fishSpeedModifier * Time.deltaTime;
        }
    }


    public void AttachObjectToForeground(GameObject obj)
    {
        GameObject lastTile = foregroundTiles[foregroundTiles.Count - 1].gameObject;
        obj.transform.SetParent(lastTile.transform, true);
    }

    public void AttachObjectToBackground(GameObject obj)
    {
        GameObject lastTile = backgroundTiles[backgroundTiles.Count - 1].gameObject;
        obj.transform.SetParent(lastTile.transform, true);
    }

    public void AttachObjectToMiddleground(GameObject obj)
    { 
        GameObject lastTile = middlegroundTiles[middlegroundTiles.Count - 1].gameObject;
        obj.transform.SetParent(lastTile.transform, true);
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
        Fish newFish = Instantiate(fishPrefab, fishHolder.transform, true);
        AttachObjectToMiddleground(newFish.gameObject);

        Vector3 newScale = fishHolder.transform.localScale;
        newScale.x = newFish.transform.localScale.x / newScale.x;
        newScale.y = newFish.transform.localScale.y / newScale.y;
        newScale.z = newFish.transform.localScale.y / newScale.z;
        newFish.transform.localScale = newScale;
        

        newFish.transform.position = location;
    }
}
