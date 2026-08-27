using UnityEngine;

public class GoalLocationManager : MonoBehaviour
{
    public static GoalLocationManager instance;
    public GoalLocation[] availableLocations;

    private void Awake()
    {
        instance = this;
    }

    public GoalLocation GetLocation(int ID)
    {
        foreach (var item in availableLocations)
        {
            if (ID == item.locationID)
                return item;
        }

        return null;
    }
}
