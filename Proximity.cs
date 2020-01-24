using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proximity : MonoBehaviour
{
  // Start is called before the first frame update
  private BoxCollider2D proximityBox;
  private Player player;
  void Start()
  {
    proximityBox = GetComponent<BoxCollider2D>();
    player = GetComponentInParent<Player>();
  }

  // Update is called once per frame
  void Update()
  {

  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Proximity"))
    {
      player.InRange = true;
    }
  }

  void OnTriggerExit2D(Collider2D other)
  {
    if (other.CompareTag("Proximity"))
    {
      player.InRange = false;
    }
  }
}
