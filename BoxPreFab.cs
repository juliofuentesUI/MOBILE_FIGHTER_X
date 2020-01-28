using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPreFab : MonoBehaviour
{
  public bool isHurtBox = false;

  public bool isInUse = false;

  public bool isDebug = true;

  private DummyCPU dummyCpu;

  private Player player;
  // Start is called before the first frame update
  void Awake()
  {
    dummyCpu = FindObjectOfType<DummyCPU>();
    player = FindObjectOfType<Player>();
  }
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (this.isHurtBox)
    {
      return;
    }

    if (other.CompareTag("DummyHurtBox"))
    {
      //start ComboWindowTimerOpen ON PLAYER script.
      // player.OpenComboWindow();
      dummyCpu.animateStun();
    }
  }
}
