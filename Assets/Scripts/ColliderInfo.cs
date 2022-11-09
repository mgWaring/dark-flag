using System.Collections.Generic;
using UnityEngine;

//Attach ColliderInfo to wherever the colliders are kept.
public class ColliderInfo : MonoBehaviour
{
    Collider[] colliderList;    
    Vector3[] colliderSizes;

    private void Start()
    {       
        colliderList = GetComponents<Collider>();
        colliderSizes = ExtentHunter();
        ExtentFighter();
    }

    //Creates an array of all collider extents.
    private Vector3[] ExtentHunter()
    {
        List<Vector3> colliderSizesList = new List<Vector3>();

        for (int i = 0; i < colliderList.Length; i++)
        {
            colliderSizesList.Add(colliderList[i].bounds.extents);
            Debug.Log("From SizeHunter in ColliderInfo.cs. " + colliderSizesList[i] + " added to colliserSizesList.");
        }

        Vector3[]  hunterArray = colliderSizesList.ToArray();
        return hunterArray;
    }

    //Compares all x, y and z values of all colliders and outputs the highest values as 1 Vector3.
    private Vector3 ExtentFighter()//This may need to be public.
    {
        float championZ = 0;
        float championX = 0;
        float championY = 0;
        
        for (int q = 0; q < colliderSizes.Length; q++)
        {
            float contenderZ = colliderSizes[q].z;
            Debug.Log("ContenderZ attacks " + championZ + "with" + contenderZ);            
            if (contenderZ > championZ)
            {                
                championZ = contenderZ;
                Debug.Log("New championZ is " + championZ);
            }
            else
            {
                Debug.Log("ChampionZ holds their title at value " + championZ);
                //Delete else clause when done debugging.
            }

            float contenderX = colliderSizes[q].x;
            Debug.Log("ContenderX attacks " + championX + "with" + contenderX);
            if (contenderX > championX)
            {
                championX = contenderX;
                Debug.Log("New championX is " + championX);
            }
            else
            {
                Debug.Log("ChampionX holds their title at value " + championX);
                //Delete else clause when done debugging.
            }

            float contenderY = colliderSizes[q].y;
            Debug.Log("ContenderY attacks " + championY + "with" + contenderY);
            if (contenderY > championY)
            {
                championY = contenderY;
                Debug.Log("New championY is " + championY);
            }
            else
            {
                Debug.Log("ChampionY holds their title at value " + championY);
                //Delete else clause when done debugging.
            }
        }

        Vector3 championFusion = new Vector3(championX, championY, championZ);
        Debug.Log("Your new ultimate Vector is " + championFusion);
        return championFusion;
    }

}
