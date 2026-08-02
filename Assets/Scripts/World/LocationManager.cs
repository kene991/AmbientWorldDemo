using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public static LocationManager instance;
    public Location[] availableLocations;

    private void Awake()
    {
        instance = this;
    }

    public Location GetLocation(int ID)
    {
        foreach (var item in availableLocations)
        {
            if (ID == item.locationID)
                return item;
        }

        return null;
    }
}
