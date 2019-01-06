
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 200f;
    [SerializeField] float mainThrust = 50f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip explode;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    [SerializeField] float reloadTime = 2f;
  

    Rigidbody rigidbody;
    AudioSource audioSource;
    bool collideOff = false;
    bool isTransitioning = false;
    //enum State { Alive, Dying, Transending };
    //State state = State.Alive;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTransitioning)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();//TODO REMOVE FOR PRODUCTION
        }
    }

    private void RespondToDebugKeys()//TODO REMOVE FOR PRODUCTION
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        if (Input.GetKeyDown(KeyCode.C))//TODO REMOVE FOR PRODUCTION
        {
            collideOff = !collideOff;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning || collideOff == true) { return; }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //Do Nothing
                break;
            case "Finish":
                Succeed();
                break;
            default:
                Die();
                break;
        }
    }

    private void Succeed()
    {
        isTransitioning = true;   
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextLevel", reloadTime);//parameterize this time
    }

    private void Die()
    {
        isTransitioning = true;
        audioSource.Stop();
        mainEngineParticles.Stop();
        deathParticles.Play();
        audioSource.PlayOneShot(explode);
        Invoke("LoadFirstLevel", reloadTime);//parameterize this time
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex =  SceneManager.GetActiveScene().buildIndex;
        //int totalNumberScenes = SceneManager.sceneCountInBuildSettings;
        //if (currentSceneIndex < totalNumberScenes - 1)
        //{
        //    SceneManager.LoadScene(currentSceneIndex + 1);
        //}
        //else
        //{
        //    SceneManager.LoadScene(0);
        //}
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            StopAppylingThrust();
        }
    }

    private void ApplyThrust()
    {
        rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying && !isTransitioning)//Playaudio source once
        {
            audioSource.PlayOneShot(mainEngine);
            mainEngineParticles.Play();
        }
    }

    private void StopAppylingThrust()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void RespondToRotateInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            RotateManually(rcsThrust * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateManually(-rcsThrust * Time.deltaTime);
        }        
    }
    private void RotateManually(float rotationThisFrame)
    {
        rigidbody.freezeRotation = true; //take manual control of rotation
        transform.Rotate(Vector3.forward * rotationThisFrame);
        rigidbody.freezeRotation = false; //resume physics control of rotation
    }
}