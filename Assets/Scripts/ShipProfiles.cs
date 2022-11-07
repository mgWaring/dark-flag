using UnityEngine;

//Use ProfileHunter() where relevant to aquire ship stats.
public class ShipProfiles : MonoBehaviour
{   
    //testShip public floats are here for development, delete before final release.
    public float testShipMass = 3.0f;
    public float testShipDrag = 1.5f;
    public float testShipAngularDrag = 90.0f;
    public float testShipThrustSpeed = 6000.0f;
    public float testShipYawSpeed = 100.0f;
    public float testShipHoverForce = 26.8f;
    public float testShipHoverConstant = 2.5f;
    public float testShipHoverHeight = 1.0f;
    public float testShipBotSpeedModifier = 1.0f;
    
    //Private function that stores stat values for different ships.
    private float Profiles(int profileIndex, int statIndex)
    {
        /*
            Ship stats stored by Profiles()
            ---------------------------------------
            0) "mass" (rb).
            1) "drag" (rb).
            2) "angular_drag" (rb).
            3) "thrust_speed" (mc).
            4) "yaw_speed" (mc).        
            5) "hover_force" (agm).
            6) "hover_constant" (agm).
            7) "hover_height" (agm).
            8) "bot_speed_modifier" (bm).
        */

        float requestedValue;
        float[] testShip = new float[9] {testShipMass, testShipDrag, testShipAngularDrag, testShipThrustSpeed, testShipYawSpeed, testShipHoverForce, testShipHoverConstant, testShipHoverHeight, testShipBotSpeedModifier};
        float[] ship1 = new float[9] {0, 1, 2, 3, 4, 5, 6, 7, 8};
        float[] ship2 = new float[9] {0, 1, 2, 3, 4, 5, 6, 7, 8};
        float[] ship3 = new float[9] {0, 1, 2, 3, 4, 5, 6, 7, 8};

        float[][] profileArray = new float[4][] {testShip, ship1, ship2, ship3};

        requestedValue = profileArray[profileIndex][statIndex];
        Debug.Log("From Profiles() in ShipProfileBuilder. Requested Value = " + requestedValue);

        return requestedValue;
    }

    //Public function that searches through Profiles() for a stat value.
    public float ProfileHunter(string shipName, string shipStatName)
    {
        float requestedStat;
        int shipNumber = 0;
        int shipStatIndex = 0;
        string[] shipNameArray = new string[4] {"testShip", "ship1", "ship2", "ship3"};
        string[] statNameArray = new string[9] {"mass", "drag", "angular_drag", "thrust_speed", "yaw_speed", "hover_force", "hover_constant", "hover_height", "bot_speed_modifier"};

        for (int q = 0; q < shipNameArray.Length; q++)
        {
            Debug.Log("PH() Checking shipNameArray... " + shipNameArray[q]);
            if (shipName == shipNameArray[q])
            {
                shipNumber = q;
                Debug.Log("From ProfileHunter in ShipProfileBuilder. shipNumber = " + shipNumber);
                q = shipNameArray.Length;//Closes loop.
            }
        }

        for (int i = 0; i < statNameArray.Length; i++)
        {
            Debug.Log("PH() Checking statNameArray... " + statNameArray[i]);
            if (shipStatName == statNameArray[i])
            {
                shipStatIndex = i;
                Debug.Log("From ProfileHunter in ShipProfileBuilder. shipStatIndex = " + shipStatIndex);
                i = statNameArray.Length;//Closes loop.
            }
        }        
        
        requestedStat =  Profiles(shipNumber, shipStatIndex);

        return requestedStat;
    }
}
