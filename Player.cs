using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  // Start is called before the first frame update
  public FixedJoystick joystick;

  public Animation anim;
  public Movement movement;
  private bool inRange;
  private bool canMove = true;


  AnimatorClipInfo[] m_CurrentClipInfo;

  private Animator animator;

  private BoxCollider2D proximityBox;

  private CharAnimator charAnimator;
  [SerializeField] [Range(0, 10)] float forwardSpeed;
  [SerializeField] [Range(0, 10)] float backSpeed;
  [SerializeField] [Range(0, 100)] int health;
  [SerializeField] [Range(0, 10)] int meterGain;

  public float ForwardSpeed { get { return forwardSpeed; } }
  public float BackSpeed { get { return backSpeed; } }
  public bool InRange { set { inRange = value; } get { return inRange; } }

  public enum State
  {
    Idle,
    Running,
    JumpFall,
    Attack1,
    LowMediumKick,
    StandMediumKick,
    WalkForward,
    WalkBackward
  }
  public State state = State.Idle;

  private State[] comboSequence = { State.LowMediumKick, State.StandMediumKick };

  private bool comboWindowActive = false;

  private float timePressedDown;

  private Coroutine CR_HANDLE_COMBOWINDOW;
  private int currentComboIndex = 0;
  private DummyCPU dummyCpu;

  void Start()
  {
    movement = GetComponent<Movement>();
    proximityBox = GetComponentInChildren<BoxCollider2D>();
    animator = GetComponent<Animator>();
    charAnimator = GetComponent<CharAnimator>();
    dummyCpu = FindObjectOfType<DummyCPU>();
    //Don't need clipLength anymore..were working with frames.
    // var animController = GetComponent<Animator>().runtimeAnimatorController;
    // var clip = animController.animationClips;
    // for (int i = 0; i < clip.Length; i++)
    // {
    //   if (clip[i].name == "Ken_Standing_MP")
    //   {
    //     Debug.Log(clip[i].length);
    //     clipLength = clip[i].length;
    //   }
    // }
  }

  // Update is called once per frame
  void Update()
  {
    //PUT A GUARD CLAUSE WRAPPING .move() and attemptAttack();
    // if (this.canMove)
    // {
    //   //IF they are NOT ATTACKING. Attempt to move() or attemptAttack();
    //   movement.Move();
    // }
    // attemptAttack();
    AttackOrMove();

    switch (state)
    {
      case State.Idle:
        Debug.Log("WE IDLE");
        charAnimator.SetAnimation("Ken_Idle");
        break;
      case State.LowMediumKick:
        Debug.Log("LOW MEDIUM KICK");
        charAnimator.SetAnimation("Ken_LowMediumKick");
        break;
      case State.WalkForward:
        Debug.Log("WALKING FORWARD");
        charAnimator.SetAnimation("Ken_Walk_Forward");
        break;
      case State.WalkBackward:
        Debug.Log("WALKING BACKWARDS");
        charAnimator.SetAnimation("Ken_Walk_Backward");
        break;
      case State.StandMediumKick:
        Debug.Log("KEN STAND MEDIUM KICK");
        charAnimator.SetAnimation("Ken_StandMediumKick");
        break;
    }
  }

  //
  void AttackOrMove()
  {
    if (this.canMove)
    {
      //IF they are NOT ATTACKING. Attempt to move() or attemptAttack();
      movement.Move();
    }

    if (joystick.isPressed)
    {
      timePressedDown += Time.deltaTime;
      //guard clause that were already counting.
    }

    if (joystick.isPressed == false)
    {
      if (timePressedDown > 0.05f && timePressedDown < 0.1f)
      {
        Debug.Log("ATTACK!!");
        this.canMove = false;
        state = comboSequence[0];
        timePressedDown = 0;
      }
      else
      {
        timePressedDown = 0;
      }
    }

    //whatever i do here, make sure movement.Move() comes after.

    // DIFFERENTIATE INPUT BETWEEN TAPPED RELEASE AND TAP AND HELD.
    // A MOVE THAT IS TAP HELD IS AN ATTACK.
    //THE END GOAL OF THIS IS THAT ken won't be over-riding attack2 back to attack1
    //automatically. we want more manualness.
    //the problem was when i was in attack 2, the code would want ken to attack1
  }

  // void attemptAttack()
  // {
  //   if (inRange == true && joystick.isPressed == false && !comboWindowActive)
  //   {
  //     state = comboSequence[currentComboIndex];
  //     // state = State.LowMediumKick;
  //     this.canMove = false;
  //   }

  //   if (inRange == true && joystick.isPressed && comboWindowActive)
  //   {
  //     state = comboSequence[currentComboIndex];
  //   }
  //   //if comboWindowIsOpen and joystick.isPressed. 
  // }



  public void ChangeState(string newState)
  {
    switch (newState)
    {
      case "WalkForward":
        state = State.WalkForward;
        break;
      case "WalkBackward":
        state = State.WalkBackward;
        break;
      case "Idle":
        state = State.Idle;
        break;
    }
  }

  public void AllowAttack(float waitTime)
  {
    //Remember, this functions purpose
    //is so that after the last frame of an attack
    //fully plays. we have a small window to move again before
    //auto repeating next attack.
    float time = 0f;

    while (time < waitTime)
    {
      time += Time.fixedDeltaTime;
      Debug.Log($"TIME NOW IS {time}");
    }
    this.canMove = true;
    Debug.Log($"canMove is set to {this.canMove}");
    // StartCoroutine(AllowAttackCR(waitTime));
  }

  //Combo Window Open should get called by BoxPrefab script

  //oPENCOMBOWINDOW will be called by BoxPreFab.CS when hit is detected
  public void OpenComboWindow()
  {
    //uses charAnimator.comboWindowOpenTime to set timer.
    //set comboIndex to increment by 1. 
    //start while loop to start counting down time
    //once while loop ends. reset comboindex back to 0.
    CR_HANDLE_COMBOWINDOW = StartCoroutine(CR_OpenComboWindow(charAnimator.comboWindowOpenTime));
  }

  private IEnumerator CR_OpenComboWindow(float comboTime)
  {
    float time = 0f;
    currentComboIndex++;
    comboWindowActive = true;
    // State previousState = state;
    //cache current INDEX. index will represent current move.
    int previousIndex = currentComboIndex;

    while (time < comboTime)
    {
      time += Time.fixedDeltaTime;
      if (previousIndex != currentComboIndex)
      {
        comboWindowActive = false;
        StopCoroutine(CR_HANDLE_COMBOWINDOW);
      }
      //if create a CR running bool
      //compare LAST STATE lowmk with current state. 
      //if state has changed. WE MUST EXIT THIS TIMER! cause a new one will start
      yield return null;
    }
    currentComboIndex = 0;
    comboWindowActive = false;
  }

}
