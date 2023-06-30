using FPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Unity.VisualScripting.Member;

public class EnemyAIRefactor : MonoBehaviour, IDamage
{
    [Header("-----Components-----")]
    [SerializeField] private NavMeshAgent agent; //the enemy's navmesh
    [SerializeField] private Animator anim; //the enemy's animator
    [SerializeField] private Transform headPosition;
    [SerializeField] private Transform shootPosition;

    [Header("----- UIComponents-----")]
    [SerializeField] private GameObject enemyUIParent; //the entirety of enemy UI. Healthbar and detection.
    [SerializeField] public GameObject wholeHealthBar; //the healthbar parent
    [SerializeField] private Transform HPBar; //the healthbar itself
    [SerializeField] private GameObject spottingUI; // question mark UI
    [SerializeField] private GameObject spottedUI; //exclamation point UI
    [SerializeField] private Transform qmarkTransform; //the red question mark that gets scaled with being spotted

    [Header("-----Audio-----")]
    [SerializeField] AudioSource audSource;
    [SerializeField] AudioClip hmSound;
    [SerializeField] float hmSoundVol;
    public AudioClip shootSound;
    [SerializeField] float shootSoundVol;
    public AudioClip deathSound;
    [SerializeField] float deathSoundVol;

    [Header("-----Enemy Stats-----")]
    [SerializeField] string currentState; //This variable is just a visual representation of the current state of the enemy.
    [SerializeField] private int HP; //the health of the enemy
    [SerializeField] private float speed; //speed at which the enemy moves
    [SerializeField] private int playerFaceSpeed; //the speed at which the enemy rotates to the player when stopped
    [SerializeField] private int viewConeAngle; //the angle from which the enemy can see the player (calculated from middle)
    [SerializeField] private bool atStart; //a visual bool just to show if the AI is where it started.
    [SerializeField] private bool spotted = false;
    [SerializeField] private bool highAlert = false;
    [SerializeField] private float spottingDistance;

    [Header("-----Pathfinding-----")]
    [SerializeField] private List<Vector3> patrolSpots = new List<Vector3>(); //the locations the enemy will follow when patroling
    [SerializeField] private List<int> patrolRotations = new List<int>(); //the rotations the enemy will face too at once reaching a patrol spot
    [SerializeField] private float patrolStoppingDistance;
    [SerializeField] float patrolStopTime;
    [SerializeField] float highAlertPatrolStopTime;
    [Header("-----Roaming-----")]
    [SerializeField, Range(1, 10)] private float roamTimer; //how long the enemy waits before roaming
    [SerializeField, Range(10, 100)] private int roamDist; //how far away the enemy will roam
    [Header("-----Enemy Stats-----")]
    [SerializeField] private float shootRate; //how fast the enemy shoots
    [SerializeField] private GameObject bullet; //the bullet the enemy shoots
    [SerializeField] private float burstRate; //how fast of a burst the enemy shoots
    [SerializeField] private bool isBurstShot; //if the enemy shoots regular or in burst
    [SerializeField] private bool isShotgun;
    private int numOfPatrolSpots; //the number of spots the enemy patrols to
    private Vector3 playerDirection; //the direction from the enemy to the player
    private bool playerInRange; //is the player within range of the enemy
    private float angleToPlayer; //the angle to the player
    private bool seesPlayer; //does the enemy see the player
    private bool destinationChosen; //bool used in roam to show if the enemy has found a spot to roam to
    private Vector3 startingPos; //where the enemy was before roam started
    private float stoppingDistanceOriginal; //the original stopping distance of the enemy
    //private bool isPatrolling; //if the enemy patrols or roams
    private int currentPointIndex; //the index of the last patrol spot the enemy was at
    private int enemyHPOriginal; //starting hp of the enemy
    private float percentSpotted; //float repressenting how much the player has been spotted                            
    private Coroutine searchCopy; //a variable to store the last coroutine of "losingPlayer"        
    private float timeCount = 0.0f; //time variable for slerping after a patrol
    private bool isShooting;

    ///newVars
    public bool pullAlarm = false;
    bool patrolTurnAround = false;
    bool searching = false;
    Vector3 playerLastSeenAt;
    // Start is called before the first frame update

    /// <summary>
    /// SCROLL DOWN TO END OF SCRIPT FOR CODE SUMMARY
    /// </summary>
    void Start()
    {
       
        anim.SetBool("Aiming", true);
        audSource = GetComponent<AudioSource>();
        startingPos = transform.position;
        agent.speed = speed;
        enemyHPOriginal = HP;
        numOfPatrolSpots = patrolSpots.Count;
        stoppingDistanceOriginal = agent.stoppingDistance;

        if (patrolSpots.Count > 0) //if there are patrol spots set, ensures there are enough rotations to go w it. If there are no spots, add a spot and rotation
        {
            if (patrolSpots.Count - patrolRotations.Count != 0)
            {
                while (patrolRotations.Count < patrolSpots.Count)
                {
                    patrolRotations.Add(0);
                }
            }
        }
        else
        {
            patrolSpots.Add(transform.position);
            patrolRotations.Add(0);
        }
        agent.destination = patrolSpots[0];
        GameManager.instance.UpdateObjective(1);
    }

    // Update is called once per frame
    void Update()
    {      
        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Enemy Speed", agent.velocity.normalized.magnitude);
            seesPlayer = canSeePlayer();
            rotateUI();
            if (!spotted)
            {
                Spotting();
            }
            StateMachine();
        }
    }
    void StateMachine()
    {
        if (pullAlarm)
        {
            currentState = "Pulling Alarm";
            PullAlarm();
        }
        else if (seesPlayer && spotted) //will fight the player if spotted and currently seen
        {
            currentState = "Engaging in Combat";
            Combat();
        }
        else if (searching) //only called while searching
        {
            currentState = "Search/Roaming";
            StartCoroutine(Roam());
        }
        else if (!seesPlayer && Vector3.Distance(transform.position, playerLastSeenAt) <= 1.5) //starts searching if the enemy cant see the player and the enemy is at the players last known location
        {
            currentState = "Starting Search";
            agent.stoppingDistance = 0;
            
            if (searchCopy == null) { searchCopy = StartCoroutine(Search()); }
        }
        else if (spotted && !seesPlayer) //goes the players last known location if the player has been spotted but isnt seen
        {
            currentState = "Going to last seen position";
            agent.stoppingDistance = 0;
        }
        else if (percentSpotted > 0.5) //stops to look at the player if spotting is over 50%
        {
            currentState = "SpottingPlayer";
            agent.isStopped = true;
        }
        else if (false) //something to make enemy sus from player
        {
            currentState = "Checking out Sus";
            //Path to a game object: Noise, broken alarm
        }
        else //if nothing else, do the patrol route
        {
            currentState = "Patrolling";
            Patrol();
        }
    }
    private void Spotting() //starts detecting the player over time
    {

        if (seesPlayer && percentSpotted < 1) //spots the player at a rate dependent on whether the player is crouching and how far away the player is.
        {
            float spottingCrouchScale;
            if (GameManager.instance.playerScript.isCrouching) { spottingCrouchScale = 0.5f; }
            else { spottingCrouchScale = 1f; }
            float playerDistanceScale = (spottingDistance - Vector3.Distance(transform.position, GameManager.instance.player.transform.position)) / spottingDistance;
            percentSpotted += spottingCrouchScale * playerDistanceScale * Time.deltaTime;
            qmarkTransform.localScale = new Vector3((4 * percentSpotted), qmarkTransform.localScale.y, qmarkTransform.localScale.z);
        }
        else if (!seesPlayer && percentSpotted > 0)
        {
            percentSpotted -= 0.25f * Time.deltaTime;
            qmarkTransform.localScale = new Vector3((4 * percentSpotted), qmarkTransform.localScale.y, qmarkTransform.localScale.z);
        }
        if (percentSpotted >= 1)
        {
            agent.isStopped = false;
            spotted = true;
            spottingUI.SetActive(false);
            StartCoroutine(spottedUIon());
        }
        if (percentSpotted <= 0)
        {
            spottingUI.SetActive(false);
        }
    }
    void Patrol() //follows a set patrol route, turning the set amount after reaching the spot
    {
        agent.stoppingDistance = 0;
        agent.SetDestination(patrolSpots[currentPointIndex]);
        if (Vector3.Distance(transform.position, patrolSpots[currentPointIndex]) < 0.1)
        {
            if (!patrolTurnAround)
            {
                StartCoroutine(PatrolTurnAround());
            }
            Quaternion rotationAmount = Quaternion.Euler(0, patrolRotations[currentPointIndex], 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotationAmount, timeCount);
            timeCount += Time.deltaTime / 10;
        }
    }
    
    void PullAlarm() //finds the closest alarm, paths to it, and pulls the alarm to summon reinforcements
    {
        StopCoroutine(PatrolTurnAround());
        agent.stoppingDistance = 0;
        GameObject[] alarms = GameObject.FindGameObjectsWithTag("Alarm");
        GameObject closestAlarm = alarms[0];
        for (int i = 0; i < alarms.Length; i++)
        {
            if (Vector3.Distance(alarms[i].transform.position, transform.position) < Vector3.Distance(closestAlarm.transform.position, transform.position))
            {
                closestAlarm = alarms[i];
            }
        }
        agent.SetDestination(closestAlarm.transform.position);
        
    

        if (Vector3.Distance(transform.position, closestAlarm.transform.position) < 1)
        {
            Vector3 alarmDirection = closestAlarm.transform.position - headPosition.position;
            Quaternion rot = Quaternion.LookRotation(new Vector3(alarmDirection.x, 0, alarmDirection.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
            StartCoroutine(PullingAlarm());
        }
    }

    bool canSeePlayer()
    {
        if (playerInRange)
        {
            playerDirection = new Vector3(0, 1, 0) + GameManager.instance.player.transform.position - headPosition.position;
            angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, 0, playerDirection.z), transform.forward);
            Debug.DrawRay(headPosition.position, playerDirection);
            RaycastHit hit;
            if (Physics.Raycast(headPosition.position, playerDirection, out hit))
            {
                if (hit.collider.CompareTag("Player") && angleToPlayer <= viewConeAngle)
                {
                    if (!spotted)
                    {
                        spottingUI.SetActive(true);
                    }
                    return true;
                }
            }
        }
        return false;
    }
    private void OnTriggerEnter(Collider other)
    {
        playerInRange = true;
    }
    private void OnTriggerExit(Collider other)
    {
        playerInRange = false;
    }
    void rotateUI() //rotates the enemy's UI towards the player
    {
        if (wholeHealthBar.activeInHierarchy || spottedUI.activeInHierarchy || spottingUI.activeInHierarchy) //if ui is active, set its x rotation to that of the camera and set the y rotation to that of the player
        {
            Vector3 eulerAngleRots = new Vector3(Camera.main.transform.rotation.eulerAngles.x, GameManager.instance.player.transform.rotation.eulerAngles.y, 0.0f);
            Quaternion rotation = Quaternion.Euler(eulerAngleRots);
            enemyUIParent.transform.rotation = rotation;
        }
    }

    void Combat()
    {
        if (searchCopy != null) //if the enemy has started losing the player, stop losing the player.
        {
            StopCoroutine(searchCopy);
            searchCopy = null;
            spottingUI.SetActive(false);
            StartCoroutine(spottedUIon());
            searching = false;
        }
        agent.stoppingDistance = stoppingDistanceOriginal;
        agent.SetDestination(GameManager.instance.player.transform.position);
        playerLastSeenAt = GameManager.instance.player.transform.position;
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, 0, playerDirection.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
        }
        if(!isShooting)
        {
            StartCoroutine(shoot());
        }
        //equivalent of shoot
    }

    public void TakeDamage(int dmg) //logic for taking damage. 
    {
        HP -= dmg;
        audSource.PlayOneShot(hmSound, hmSoundVol);
        StartCoroutine(HitMarker());
        updateEnemyUI();
        if (HP <= 0) //if the enemy is dead, turns of lasersight, stops all active coroutines, stops animations, and turns off collision.
        {
            GameManager.instance.hitMarkKill.gameObject.SetActive(false);
            StopAllCoroutines();
            spottedUI.SetActive(false);
            spottingUI.SetActive(false);
            wholeHealthBar.SetActive(false);

            anim.SetBool("Aiming", false);
            anim.SetBool("Death", true);
            audSource.PlayOneShot(deathSound, deathSoundVol);
            GameManager.instance.UpdateObjective(-1);

            StartCoroutine(HitMarker());

            agent.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;

        }
        else //starts attacking the player and instantly spots them
        {
            agent.SetDestination(GameManager.instance.player.transform.position);
            if (!spotted)
            {
                StartCoroutine(spottedUIon());
            }
            spotted = true;
        }
    }
    private void updateEnemyUI() // Updates enemyHP bar
    {
        HPBar.transform.localScale = new Vector3((float)HP / enemyHPOriginal, HPBar.localScale.y, HPBar.localScale.y);
    }
    public void createBullet() //spawns the bullet at a position. needs work
    {
        playerDirection = new Vector3(0, 1, 0) + GameManager.instance.player.transform.position - shootPosition.position;
        Instantiate(bullet, shootPosition.position, Quaternion.LookRotation(playerDirection));
        audSource.PlayOneShot(shootSound, shootSoundVol);
        Debug.DrawRay(shootPosition.position, playerDirection);
    }
    void ShotgunBlast()
    {
        playerDirection = new Vector3(0, 1, 0) + GameManager.instance.player.transform.position - shootPosition.position;
        Quaternion lookrotation = Quaternion.LookRotation(playerDirection);
        //lookrotation.eulery
        Vector3 randAngles = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
        Instantiate(bullet, shootPosition.position, lookrotation * Quaternion.Euler(randAngles));
        randAngles = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
        Instantiate(bullet, shootPosition.position, lookrotation * Quaternion.Euler(randAngles));
        randAngles = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
        Instantiate(bullet, shootPosition.position, lookrotation * Quaternion.Euler(randAngles));
        randAngles = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
        Instantiate(bullet, shootPosition.position, lookrotation * Quaternion.Euler(randAngles));
        randAngles = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
        Instantiate(bullet, shootPosition.position, lookrotation * Quaternion.Euler(randAngles));



        audSource.PlayOneShot(shootSound, shootSoundVol);
        Debug.DrawRay(shootPosition.position, playerDirection);
        
    }
    ////////////////////
    ///COROUTINES
    ////////////////////
    IEnumerator Roam() //roams fast
    {
        agent.stoppingDistance = 0;

        if (!destinationChosen && agent.remainingDistance < 1.5f)
        {
            destinationChosen = true;
            yield return new WaitForSeconds(roamTimer);
            destinationChosen = false;
            Vector3 randomPos = Random.insideUnitSphere * roamDist;
            randomPos += startingPos;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);
        }
        else if (!destinationChosen && agent.velocity.magnitude < 1 && Vector3.Distance(transform.position, agent.destination) <= patrolStoppingDistance + 2)
        {
            Debug.Log("Got Stuck, Finding new roam");
            Vector3 randomPos = Random.insideUnitSphere * roamDist;
            randomPos += startingPos;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);
        }
    }
    IEnumerator Search()
    {
        searching = true;
        spottingUI.SetActive(true);
        yield return new WaitForSeconds(15);
        spottingUI.SetActive(false);
        searching = false;
        spotted = false;
        percentSpotted = 0;
        StopCoroutine(Roam());
        Debug.Log("Search Complete");
    }
    IEnumerator PatrolTurnAround()
    {
        patrolTurnAround = true;
        agent.isStopped = true;
        if (highAlert)
        {
            yield return new WaitForSeconds(highAlertPatrolStopTime);
        }
        else
        {
            yield return new WaitForSeconds(patrolStopTime);
        }
        agent.isStopped = false;
        patrolTurnAround = false;
        timeCount = 0;
        currentPointIndex++;
        if (currentPointIndex > numOfPatrolSpots - 1)
        {
            currentPointIndex = 0;
        }
    }

    IEnumerator PullingAlarm()
    {
        anim.SetBool("Aiming", false);
        anim.SetBool("Use", true);
        yield return new WaitForSeconds(4);
        pullAlarm = false;
        anim.SetBool("Use", false);
        anim.SetBool("Aiming", true);
        playerLastSeenAt = transform.position;
        highAlert = true;
    }
    IEnumerator spottedUIon() //turns the UI on if the player gets spotted. turns it off after 3seconds
    {
        if (spottingUI.activeInHierarchy)
        {
            spottingUI.SetActive(false);
        }
        spottedUI.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        spottedUI.SetActive(false);
    }
    IEnumerator HitMarker()
    {
        if (HP > 1)
        {
            GameManager.instance.hitMark.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            GameManager.instance.hitMark.gameObject.SetActive(false);
        }
        else if (HP <= 0)
        {
            GameManager.instance.hitMarkKill.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            GameManager.instance.hitMarkKill.gameObject.SetActive(false);
        }

    }
    private IEnumerator shootBurst() //burst that shoots 3 bullets 
    {
        createBullet();
        yield return new WaitForSeconds(burstRate);
        createBullet();
        yield return new WaitForSeconds(burstRate);
        createBullet();
    }
    private IEnumerator shoot() //coroutine to control shooting logic. Shoots a bullet, shoots a burst if needed
    {
        isShooting = true;

        anim.SetTrigger("Shoot");
        if (isBurstShot)
        {
            StartCoroutine(shootBurst());

        }
        if(isShotgun)
        {
            ShotgunBlast();
        }
        else
        {
            createBullet();
        }

        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }
}
