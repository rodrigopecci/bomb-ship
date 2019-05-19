using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    [Header("Player informations")]
    [SerializeField] private float runSpeed = 30f;
    private float horizontalMovement = 0f;
    private bool isJumping = false;
    private bool isFalling = false;

    private int health = 3;
    private bool immunity = false;
    private bool charging = false;

    [Header("Player objects")]
    public GameObject chargingBar;
    public GameObject runParticles;

    [Header("Player signals")]
    public SignalInteger UIHealthSignalInteger;

    [Header("Player game objects")]
    private PhotonView pv;
    private CharacterController2D characterController2D;
    private Rigidbody2D theRigidbody2D;
    private Animator theAnimator;
    //public Joystick joystick;

    // Start is called before the first frame update
    void Start() {
        pv = GetComponent<PhotonView>();

        theRigidbody2D = GetComponent<Rigidbody2D>();
        theAnimator = GetComponent<Animator>();
        characterController2D = GetComponent<CharacterController2D>();

        if (pv.IsMine) {
            theAnimator.SetInteger("health", health);
        }

        IgnoreCollision();
    }

    // Update is called once per frame
    void Update() {
        if (pv.IsMine) {
            if (health > 0) {
                horizontalMovement = Input.GetAxisRaw("Horizontal") * runSpeed;
                //horizontalMovement = joystick.Horizontal * runSpeed;

                /*if (joystick.Horizontal >= .2f) {
                    horizontalMovement = runSpeed;
                } else if (joystick.Horizontal <= -.2f) {
                    horizontalMovement = -runSpeed;
                } else {
                    horizontalMovement = 0f;
                }*/

                theAnimator.SetFloat("horizontalMovement", Mathf.Abs(horizontalMovement));

                if (characterController2D.cc_IsGrounded) {
                    if (Mathf.Abs(horizontalMovement) > .01f) {
                        runParticles.SetActive(true);
                    } else {
                        runParticles.SetActive(false);
                    }
                }

                if (Input.GetButtonDown("Jump")) {
                    Jump();
                }

                if (Input.GetKeyDown(KeyCode.F)) {
                    Bomb();
                }
            } else {
                theAnimator.SetFloat("horizontalMovement", 0);
                runParticles.SetActive(false);
            }
        }
    }

    private void FixedUpdate() {
        if (pv.IsMine) {
            characterController2D.Move(horizontalMovement, isJumping);

            if (isJumping) {
                if (theRigidbody2D.velocity.y < .1f) {
                    isJumping = false;
                    isFalling = true;

                    theAnimator.SetBool("isJumping", isJumping);
                    theAnimator.SetBool("isFalling", isFalling);
                }
            }
        }
    }

    public void Jump() {
        if (pv.IsMine) {
            if (characterController2D.cc_IsGrounded) {
                isJumping = true;
                theAnimator.SetBool("isJumping", isJumping);

                runParticles.SetActive(false);

                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "JumpParticles"), new Vector3(transform.position.x, transform.position.y - .2f, transform.position.z), Quaternion.identity);
            }
        }
    }

    public void Bomb() {
        if (pv.IsMine) {
            if (!charging) {
                if (characterController2D.cc_IsFacingRight) {
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Bomb"), new Vector3(transform.position.x + .5f, transform.position.y + .2f, transform.position.z), Quaternion.identity, 0);
                } else {
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Bomb"), new Vector3(transform.position.x - .5f, transform.position.y + .2f, transform.position.z), Quaternion.identity, 0);
                }

                StartCoroutine(ChargingCoroutine());
            }
        }
    }

    public void OnLanding() {
        if (pv.IsMine) {
            isJumping = false;
            isFalling = false;

            theAnimator.SetBool("isJumping", isJumping);
            theAnimator.SetBool("isFalling", isFalling);

            StartCoroutine(GroundingCoroutine());

            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FallParticles"), new Vector3(transform.position.x, transform.position.y - .4f, transform.position.z), Quaternion.identity);
        }
    }

    public void OnDamage(int damage) {
        if (pv.IsMine) {
            if (!immunity) {
                health -= damage;

                UIHealthSignalInteger.Raise(health);
                theAnimator.SetInteger("health", health);
                StartCoroutine(DamageCoroutine());

                if (health <= 0) {
                    StartCoroutine(DisconnectPlayerCoroutine());
                }
            }
        }
    }

    private void IgnoreCollision() {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Object");

        foreach (GameObject obj in objects) {
            if (obj != null) {
                Physics2D.IgnoreCollision(obj.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            }
        }
    }

    private IEnumerator GroundingCoroutine() {
        theAnimator.SetBool("isGrounding", true);
        yield return new WaitForSeconds(.5f);
        theAnimator.SetBool("isGrounding", false);
    }

    private IEnumerator DamageCoroutine() {
        if (characterController2D.cc_IsFacingRight) {
            theRigidbody2D.AddForceAtPosition(new Vector2(-30f, 0f), new Vector2(transform.position.x, transform.position.y), ForceMode2D.Impulse);
        } else {
            theRigidbody2D.AddForceAtPosition(new Vector2(30f, 0f), new Vector2(transform.position.x, transform.position.y), ForceMode2D.Impulse);
        }

        StartCoroutine(ImmunityCoroutine());

        theAnimator.SetBool("isHit", true);
        yield return new WaitForSeconds(.25f);
        theAnimator.SetBool("isHit", false);
    }

    private IEnumerator ImmunityCoroutine() {
        immunity = true;
        yield return new WaitForSeconds(2f);
        immunity = false;
    }

    private IEnumerator ChargingCoroutine() {
        charging = true;
        chargingBar.SetActive(true);

        yield return new WaitForSeconds(.9f);

        charging = false;
        chargingBar.SetActive(false);
    }

    private IEnumerator DisconnectPlayerCoroutine() {
        yield return new WaitForSeconds(2f);
        //SceneManager.LoadScene("GameOverLose");
        RoomController.room.DisconnectPlayer();
    }
}
