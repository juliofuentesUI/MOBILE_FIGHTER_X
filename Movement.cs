using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
  // Start is called before the first frame update
  private Rigidbody2D rb;
  public FixedJoystick joystick;

  private Player player;
  private Animator animator;
  void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    // joystick = FindObjectOfType<FixedJoystick>();
    player = GetComponent<Player>();
    animator = GetComponent<Animator>();
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void Move()
  {
    //GUARD CLAUSE HERE.. IF player.isAttacking is true. DO NOT MOVE
    //PROBLEM IS..WE NEED TO MAKE SURE SYSTEM IS UPDATED WHEN PLAYER IS DONE ATTACKING.
    //IF STICK IS FORWARD
    if (joystick.Horizontal > 0.8f)
    {
      // player.StopOurCoroutine();
      player.ChangeState("WalkForward");
      //   animator.SetBool("isWalkBackward", false);
      //   animator.SetBool("isWalkForward", true);
      Debug.Log("moving to the right");
      rb.MovePosition(new Vector2(transform.position.x + (player.ForwardSpeed * Time.deltaTime), transform.position.y));
    }

    //IF STICK IS NEUTRAL
    if (joystick.Horizontal > -0.8f && joystick.Horizontal < 0.8f)
    {
      //   animator.SetBool("isWalkBackward", false);
      //   animator.SetBool("isWalkForward", false);
      Debug.Log("STICK IS NEUTRAL, CHANGING STATE TO IDLE");
      player.ChangeState("Idle");
    }

    //IF STICK IS BACK
    if (joystick.Horizontal < -0.8f)
    {
      // player.StopOurCoroutine();
      player.ChangeState("WalkBackward");
      //   animator.SetBool("isWalkForward", false);
      //   animator.SetBool("isWalkBackward", true);
      Debug.Log("moving to the left");
      rb.MovePosition(new Vector2(transform.position.x + (-1f * (player.BackSpeed * Time.deltaTime)), transform.position.y));
    }
  }
}
