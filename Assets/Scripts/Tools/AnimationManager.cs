using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    [SerializeField] private List<AnimationCatagory> _behaviorAnimation;
    public List<AnimationCatagory> AnimationLibraries { get { return _behaviorAnimation; } set { _behaviorAnimation = value; } }

    private void Awake()
    {
        instance = this;
    }

    //public float GetAnimationClipTime(string libraryName)
    //{
    //    foreach (var item in _behaviorAnimation)
    //    {
    //        if (libraryName == item.libraryName)
    //        {
    //            for (int i = 0; i < item.clips.Length; i++)
    //            {
    //                return item.clips[i].length;
    //            }
    //        }
    //    }

    //    return 0;
    //}

    public AnimationClip GetRandomAnimation(AnimationType animationType)
    {
        // getting the library information and clips
        AnimationCatagory selectAnimations = _behaviorAnimation.FirstOrDefault(x =>  x.animationType == animationType);

        if (selectAnimations.clips == null || selectAnimations.clips.Length == 0)
            return null;

        return selectAnimations.clips[Random.Range(0, selectAnimations.clips.Length)];
    }

}

[System.Serializable]
public class AnimationCatagory
{
    public AnimationType animationType;
    public AnimationClip[] clips;
}

public enum AnimationType
{
ACTION_ANIME_REFERENCE,
ACTION_BAD_SMELL,
ACTION_CALL_TAXI,
ACTION_CLAP_CHEER,
ACTION_DANCE,
ACTION_EXERCISE,
ACTION_FAIL_LIFT,
ACTION_GREET,
ACTION_INSTRUMENT,
ACTION_INTERACTION_DRINK,
ACTION_INTERACTION_FLOOR,
ACTION_INTERACTION_HIP,
ACTION_ITCHY,
ACTION_LAUGH,
ACTION_LEAN_RAIL,
ACTION_LEAN_WALL,
ACTION_LOOKING, 
ACTION_PHONE,
ACTION_PRAY,
ACTION_SING,
ACTION_SMOKING,
ACTION_SNEEZE,
ACTION_STRETCH,
ACTION_TAKE_A_PICTURE_PHONE,
ACTION_TALKING,
ACTION_TALKING_PHONE,
ACTION_THINKING,
ACTION_WATCHING,
GESTURE_CONFUSION,
GESTURE_DIRECT,
GESTURE_NEGATIVE,
GESTURE_NERVOUS,
GESTURE_NOD,
GESTURE_POSITIVE,
GESTURE_SCARED,
GESTURE_SHOCKED_SURPRISE,
GESTURE_SUSPIOUS,
LOCOMOTION_IDLE,
LOCOMOTION_JOG,
LOCOMOTION_RUN,
LOCOMOTION_RUN_OBJECT,
LOCOMOTION_WALK,
SEATED_IDLE,
WORK_EQUIPMENT,
WORK_HOUSE,
WORK_SMITH,
WORK_STORE,
WORK_YARD,
}
