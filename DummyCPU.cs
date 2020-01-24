using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCPU : MonoBehaviour
{
  // Start is called before the first frame update
  private CharAnimator charAnimator;
  public enum State { Idle, Running, JumpFall, Attack1, LowMediumKick, WalkForward, WalkBackward, HurtStanding, HurtCrouching, HurtAirbourne }
  public State state = State.Idle;
  void Start()
  {
    charAnimator = GetComponent<CharAnimator>();
  }

  // Update is called once per frame
  void Update()
  {
    switch (state)
    {
      case State.Idle:
        Debug.Log("URIEN IS IDLE");
        charAnimator.SetAnimation("Urien_Idle");
        break;
      case State.HurtStanding:
        Debug.Log("URIEN HURT STANDING");
        charAnimator.SetAnimation("Urien_HurtStanding");
        break;
    }
  }

  public void animateStun()
  {
    //This setsState to either HurtGrounded, HurtStanding
    //or hurtAirbourne.
    state = State.HurtStanding;
    //THEN PHYSICS
    //FUTURE PROOF: BEFORE ANIMATING. CHECK IF CURRENT STATE IS
    //IN CROUCH OR STANDING. THEN ANIMATE HURT BSED ON THAT
  }

  public void launchDummy()
  {

  }
}
