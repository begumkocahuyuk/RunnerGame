using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Current;

    public float limitX;

    public float runningSpeed;
    private float _currentRunningSpeed;
    public float xSpeed;

    public GameObject ridingCyclinderPrefab;
    public List<RidingCylincer> cylinders;

    private bool _spawningBridge;
    public GameObject bridgePiecePrefab;
    private BridgeSpawner _bridgeSpawner;
    private float _creatingBrdigeTimer;

    private bool _finished;


    public Animator animator;
    private float _scoreTimer=0;
    private bool _finishLine;

    private float _lastTouchedX;

    public AudioSource cylinderAudioSource,triggerAudioSource,itemAudioSource;
    public AudioClip gatherAudioClip,dropAudioClip,coinAudioClip,buyAudioClip,equippedItemAudioClip,UnEquipItemAudioClip;
    private float _dropSoundTimer;

    public List<GameObject> wearSpots;

    // Update is called once per frame
    void Update()
    {
        if(LevelController.Current==null || !LevelController.Current.gameActive)
        {
            return;
        }
        float newX=0;
        float touchXDelta=0;
        if(Input.touchCount>0)
        {
            if(Input.GetTouch(0).phase==TouchPhase.Began){
            _lastTouchedX=Input.GetTouch(0).position.x;
            }
            else if(Input.GetTouch(0).phase==TouchPhase.Moved)
            {
                touchXDelta=5*(Input.GetTouch(0).position.x-_lastTouchedX) /Screen.width;
                _lastTouchedX=Input.GetTouch(0).position.x;

            }
        }
   
        else if(Input.GetMouseButton(0))
        {
            touchXDelta=Input.GetAxis("Mouse X");
        }

        newX=transform.position.x+xSpeed*touchXDelta*Time.deltaTime;
        newX=Mathf.Clamp(newX,-limitX,limitX);

        Vector3 newPosition=new Vector3(newX,transform.position.y,transform.position.z +_currentRunningSpeed*Time.deltaTime);
        transform.position=newPosition;

        if(_spawningBridge)
        {
            PlayDropSound();
            _creatingBrdigeTimer -=Time.deltaTime;
            if(_creatingBrdigeTimer<0)
            {
                _creatingBrdigeTimer=0.01f;
                IncrementCylincerVolume(-0.01f);
                GameObject createdBridgePiece=Instantiate(bridgePiecePrefab,this.transform);
                createdBridgePiece.transform.SetParent(null);
                Vector3 direction=_bridgeSpawner.endReference.transform.position-_bridgeSpawner.startReference.transform.position;
                float distance=direction.magnitude;
                direction=direction.normalized;
                createdBridgePiece.transform.forward=direction;
                float characterDistance=transform.position.z-_bridgeSpawner.startReference.transform.position.z;
                characterDistance=Mathf.Clamp(characterDistance,0,distance);
                Vector3 newPiecePosition=_bridgeSpawner.startReference.transform.position+direction*characterDistance;
                newPiecePosition.x=transform.position.x;
                createdBridgePiece.transform.position=newPiecePosition;

                if(_finished)
                {
                    _scoreTimer -=Time.deltaTime;
                    if(_scoreTimer<0)
                    {
                        _scoreTimer=0.3f;
                        LevelController.Current.ChangeScore(1);
                    }
                }
            }
        }
    }
    public void PlayDropSound()
    {
        _dropSoundTimer -=Time.deltaTime;
        if(_dropSoundTimer<0)
        {
            _dropSoundTimer=0.15f;
            cylinderAudioSource.PlayOneShot(dropAudioClip,0.1f);
        }
    }
    public void ChangeSpeed(float value)
    {
        _currentRunningSpeed=value;
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag=="AddCylinder")
        {
            cylinderAudioSource.PlayOneShot(gatherAudioClip,0.1f);
            IncrementCylincerVolume(0.1f);
            Destroy(other.gameObject);
        }
        else if(other.tag=="SpawnBridge")
        {
            StartSpawingBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if(other.tag=="StopSpawnBridge")
        {
           StopSpawingBridge();
           if(_finished)
            {
                LevelController.Current.FinishGame();
            }
        }
        else if(other.tag=="Finish")
        {
           _finished=true;
           StartSpawingBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if(other.tag=="Coin")
        {
            triggerAudioSource.PlayOneShot(coinAudioClip,0.1f);
           other.tag="Untagged"; 
           LevelController.Current.ChangeScore(10);
           Destroy(other.gameObject);
        }
    }
 
    private void OnTriggerStay(Collider other)
    {
        if(LevelController.Current.gameActive)
        {
            if(other.tag=="Traps")
            {
                PlayDropSound();
                IncrementCylincerVolume(-Time.fixedDeltaTime);
            }
        }

    }
    public void IncrementCylincerVolume(float value)
    {
        if(cylinders.Count==0)
        {
            if(value>0)
            {
                CreateCyclinder(value);
            }
            else
            {
                if(_finished)
                {
                    LevelController.Current.FinishGame();
                }
                else{
                    Die();
                }
            }
        }
        else{
            cylinders[cylinders.Count-1].IncrementCylincerVolume(value);
        }
    }
    public void Die()
    {
        animator.SetBool("dead",true);
        gameObject.layer=8;
        Camera.main.transform.SetParent(null);
        LevelController.Current.GameOver();
    }
    public void CreateCyclinder(float value)
    {
        RidingCylincer createCylinder=Instantiate(ridingCyclinderPrefab,transform).GetComponent<RidingCylincer>();
        cylinders.Add(createCylinder);
        createCylinder.IncrementCylincerVolume(value);
    }

    public void DestroyCyclinder(RidingCylincer cylinder)
    {
        cylinders.Remove(cylinder);
        Destroy(cylinder.gameObject);
    }

    public void StartSpawingBridge(BridgeSpawner spawner)
    {
        _bridgeSpawner=spawner;
        _spawningBridge=true;
    }
    public void StopSpawingBridge()
    {
        _spawningBridge=false;
    }
}
