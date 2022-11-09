using UnityEngine;

public class ColliderInfo : MonoBehaviour
{
    Collider[] mColliderList;
    Collider meshCollider;

    private void Start()
    {
        mColliderList = GetComponents<Collider>();
        meshCollider = GetComponent<MeshCollider>();
        Debug.Log("+++++++++++++++++++++colliderList"+mColliderList);
        Debug.Log("+++++++++++++++++++++++++meshCollider Info: "+meshCollider.bounds.extents);

        for (int i = 0; i < mColliderList.Length; i++)
        {
            Debug.Log("++++++++++++++++++++++AAAAAAAAAAAAAAH" + mColliderList[i]);


        }
    }


}
