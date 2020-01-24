using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnimator : MonoBehaviour
{
  // Start is called before the first frame update

  //Raw scriptable Object data for each move goes into animationMoves list
  //then extract into classes for runtime
  public List<AnimationMove> animationMoves = new List<AnimationMove>();
  private List<CharAnimationInfo> Animations = new List<CharAnimationInfo>();
  private CharAnimationInfo CurrentAnimation;

  private int CurrentAnimationFrameNumber;
  private int NextAnimationFrameNumber = 0;

  public float CurrentFrameTime;

  public float NextFrameTime;

  public float comboWindowOpenTime;

  private SpriteRenderer spriteRenderRef;

  public Player player;


  void Start()
  {
    //At start, access player script load idle animation
    //ALSO, INITIALIZE SCRIPTABLEOBJECT DATA INTO MEMORY
    spriteRenderRef = this.gameObject.GetComponent<SpriteRenderer>();
    ExtractFromScriptable();
    // SetAnimation("Ken_LowMediumKick");
    CurrentAnimation = Animations[0];
  }

  // Update is called once per frame
  void FixedUpdate()
  {
    AdvanceFrame();
    // Debug.Log($"CURRENT ANIMATION IS {CurrentAnimation.animName} ");
  }
  void Update()
  {

  }

  private void AdvanceFrame()
  {
    CurrentFrameTime = Time.timeSinceLevelLoad * 1000;
    //ADD ifLastFrame == true logic here. If its true, ADD time.
    //this time will be the window where player is in idle and is allowed to move before auto resuming the attack again.
    if (CurrentFrameTime >= NextFrameTime)
    {
      //nextAnimationFrameNumber will be OUT OF INDEX. this might result in undefined.
      //if (NextAnimationFrameNumber < CurrentAnimation.animationFrames.Count)
      if (CurrentAnimation.hasBoxes)
      {
        //WARNING: the line below evnetually has out of range INDEX error. check to make sure its in range.
        //WARNING: WE MAY NOT NEED THIS SHIT. HAVE BOXPREFAB.CS emit the event to trigger window to combocount++
        if (NextAnimationFrameNumber <= CurrentAnimation.animationFrames.Count - 1 && CurrentAnimation.animationFrames[NextAnimationFrameNumber].activeFrame)
        {
          //for loop to figure out remaining active frames.
          float activeFramesLeft = 0;
          for (int i = NextAnimationFrameNumber; i < CurrentAnimation.animationFrames.Count; i++)
          {
            if (CurrentAnimation.animationFrames[i].activeFrame)
            {
              activeFramesLeft++;
            }
          }
          //now set a GLOBAL variable CANCELWINDOWWAITTIME which keeps updating
          //each activeFrame.
          comboWindowOpenTime = (GetCurrentAnimMillisecondFrameTime() / 1000) * activeFramesLeft;
        }

        if (NextAnimationFrameNumber < CurrentAnimation.animationFrames.Count && CurrentAnimation.animationFrames[NextAnimationFrameNumber].isLastFrame == true)
        {
          player.AllowAttack(GetCurrentAnimMillisecondFrameTime() / 1000);
          // player.AllowAttack(0.111111f);
        }
        //
      }
      NextFrameTime = CurrentFrameTime + GetCurrentAnimMillisecondFrameTime();
      SetSprite(NextAnimationFrameNumber);
      //spawn boxes.
    }
  }

  private void SetSprite(int frame)
  {
    //SpriteRenderer reference sprite will equal currentAnim sprite
    //THE NEXT LINE, what it does is it Clears the hitBoxes from the last sprite .
    //I made a chagne where i Added <= to make instead of < than because it would 
    //make Frame7 hitbox still linger in the game, It would never turn off. 
    if (CurrentAnimation.hasBoxes && CurrentAnimationFrameNumber <= CurrentAnimation.animationFrames.Count - 1)
    {
      //here!! We can do player.isAttacking= true;
      ClearBoxes(CurrentAnimationFrameNumber);
      //THIS CLEARS CurrentAnimationFrameNumber
      //Because right after this we immediately go to the NextAnimationFrameNumber
    }

    //  HOW THIS WORKS. When we get to the last frame of mediumkick, aka index 6 (Frame7)
    // we want to make sure we start at index 0 to replay the animation again. SO ! 
    // we start over at index 0. If we go to index 7 it'll be undefined.
    if (frame > CurrentAnimation.animationFrames.Count - 1)
    {
      //HERE!!  maybe we can do player.isAttacking = false;
      NextAnimationFrameNumber = 0;
    }

    CurrentAnimationFrameNumber = NextAnimationFrameNumber;
    // Debug.Log($"CurrentAnimationFrameNumber IS {CurrentAnimationFrameNumber}");
    spriteRenderRef.sprite = CurrentAnimation.animationFrames[CurrentAnimationFrameNumber].sprite;
    ActivateBoxes(CurrentAnimationFrameNumber);
    NextAnimationFrameNumber = NextAnimationFrameNumber + 1;
    //ACTIVATE HURTBOX AND HITBOX PREFAB ON THIS LINE TOO..METHOD IS REALLY
    //CALLED SETSPRITE AND BOX.
  }

  private void ActivateBoxes(int boxNum)
  {
    if (CurrentAnimation.hasBoxes)
    {
      CurrentAnimation.animationFrames[boxNum].hitBox.SetActive(true);
      CurrentAnimation.animationFrames[boxNum].hurtBox.SetActive(true);
    }
    else
    {
      return;
    }
  }

  private void ClearBoxes(int boxNum)
  {
    if (CurrentAnimation.hasBoxes)
    {
      CurrentAnimation.animationFrames[boxNum].hitBox.SetActive(false);
      CurrentAnimation.animationFrames[boxNum].hurtBox.SetActive(false);
    }
    else
    {
      return;
    }
  }

  public float GetCurrentAnimMillisecondFrameTime()
  {
    return ((1 / (float)CurrentAnimation.framesPerSecond) * 1000);
  }

  private void ExtractFromScriptable()
  {
    //PUT MOVES INTO CharAnimationInfo Animations
    for (int i = 0; i < animationMoves.Count; i++)
    {
      CharAnimationInfo animClip = new CharAnimationInfo();
      animClip.framesPerSecond = animationMoves[i].framesPerSecond;
      animClip.animName = animationMoves[i].animationName;
      animClip.hasBoxes = animationMoves[i].hasBoxes;

      for (int y = 0; y < animationMoves[i].sprites.Count; y++)
      {

        CharAnimationFrames currentSpriteFrame = new CharAnimationFrames();
        currentSpriteFrame.sprite = animationMoves[i].sprites[y];
        if (y == animationMoves[i].sprites.Count - 1)
        {
          //This is the last sprite, tag it with a boolean that says isLastFrame true
          currentSpriteFrame.isLastFrame = true;
        }
        if (animationMoves[i].hasBoxes)
        {
          // Here to tag which frames are ACTIVE
          if (animationMoves[i].activeFrameIndeces.Length > 0)
          {
            for (int a = 0; a < animationMoves[i].activeFrameIndeces.Length; a++)
            {

              if (animationMoves[i].activeFrameIndeces[a] == y)
              {
                currentSpriteFrame.activeFrame = true;
                // currentSpriteFrame.comboWindowOpenTime
              }
            }
          }
          currentSpriteFrame.hitBox = animationMoves[i].hitBoxPreFabs[y];
          currentSpriteFrame.hurtBox = animationMoves[i].hurtBoxPreFabs[y];

          currentSpriteFrame.hitBox = Instantiate(currentSpriteFrame.hitBox, transform.position, Quaternion.identity, transform);
          currentSpriteFrame.hurtBox = Instantiate(currentSpriteFrame.hurtBox, transform.position, Quaternion.identity, transform);
          currentSpriteFrame.hitBox.gameObject.tag = "HitBox";
          currentSpriteFrame.hurtBox.gameObject.tag = "HurtBox";
          currentSpriteFrame.hitBox.SetActive(false);
          currentSpriteFrame.hurtBox.SetActive(false);
        }
        //TRY INSTANTIATING BOXES HERE WHILE WERE ALREADY INSIDE THE SCRIPTABLEOJBECT. THEN SETACTIVE FALSE.
        animClip.animationFrames.Add(currentSpriteFrame);
        //I ADDED THE SPRITE TO ANIMATIONFRAMES FOR THIS ANIMCLIP. BUT IT NEEDS
        //THE HITBOX AND HURTBOX ATTACHED.
        //WHICH WE CAN JUST ACTIVATE BY TAGGING ON AND OFF. SET ACTIVE ON OFF
      }
      Animations.Add(animClip);
    }
    Debug.Log("FINISHED EXTRACTING");
  }
  public void SetAnimation(string animationName, int startFrame = 0)
  {
    Debug.Log($"ANIMATION SET TO {animationName}");
    if (CurrentAnimation.animName == animationName)
    {
      //If the current animation is already SET. 
      //let it run. Don't need to change it. 
      return;
    }
    //he purpose of this if statement was for...
    //wait..what is this doing again??
    //NOTE: INVESTIGATE THESE NEXT 3 LINES.
    if (CurrentFrameTime > NextFrameTime)
    {
      return;
    }
    //THIS WILL SET ANIMATION
    //ITERATE THRU ANIMATIONS ARRAY AND FIND RIGHT CLIP.
    //named "Ken_LowMediumKick"
    foreach (CharAnimationInfo anim in Animations)
    {
      if (animationName == anim.animName)
      {
        //We clear Boxes in advance cause..no matter what
        //we are switching animations. therefore this next line will delete
        //the previous boxes.
        ClearBoxes(CurrentAnimationFrameNumber);
        CurrentAnimation = anim;
        NextAnimationFrameNumber = startFrame;
        return;
      }
    }
  }

  public class CharAnimationInfo
  {
    public string animName;

    public bool hasBoxes;
    public int framesPerSecond;

    public List<CharAnimationFrames> animationFrames = new List<CharAnimationFrames>();

  }

  public class CharAnimationFrames
  {
    public Sprite sprite;

    public bool isLastFrame = false;
    // public List<CharBoxData> boxes = new List<CharBoxData>();
    public GameObject hitBox;
    public GameObject hurtBox;

    public bool activeFrame = false;

    public float comboWindowOpenTime = 0;

  }

  //   public class CharBoxData
  //   {
  //     public float boxX;
  //     public float boxY;
  //     public float boxWidth;

  //     public float boxHeight;

  //     public float boxRotation;

  //     public bool isHurtBox;
  //   }
}
