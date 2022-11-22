using System.Collections.Generic;
using UnityEngine;

//Add this script to the vehicle object that has a rigidbody and the colliders.
//Forces applied by the AntiGravManager are affected by the rigidbody mass.
namespace Multiplayer {
    public class AntiGravManager : MonoBehaviour
    {
        private ShipsScriptable ss;

        //Unity editor options.//
        public bool aGMRaysOn = true;
        public float debugRayTime = 0.02f;

        private Rigidbody vehicleRB;
        private Vector3 stableOffsetVector;

        //Collider finding stuff.
        private Collider[] colliderList;
        private Vector3[] colliderSizes;
        private Vector3 ultimateVector;
        private Vector3 centerOffset;

        //y axis force.
        private float hoverForce;
        private float hoverConstant;
        private RaycastHit hoverHitInfo;     

        //z axis torque.
        private Ray rollRay1;
        private Ray rollRay2;
        private float rollDiff;
        private float rollRayDistance;
        private float rollForce;
        private RaycastHit rollHit1;
        private RaycastHit rollHit2;
        private float rollHitInfo1;
        private float rollHitInfo2;
        private bool pitchIsStable;

        //x axis torque.
        private Ray pitchRay1;
        private Ray pitchRay2;
        private Ray bowRay;
        private float pitchDiff;
        private float pitchRayDistance;
        private float bowHitDistance = 0.6f;
        private float pitchForce;
        private float pitchRollConstant;
        private RaycastHit pitchHit1;
        private RaycastHit pitchHit2;
        private float pitchHitInfo1;
        private float pitchHitInfo2;

        private void Start()
        {
            ss = GetComponent<Ship>().details;
            vehicleRB = GetComponent<Rigidbody>();        

            //Collider extent finding.
            colliderList = GetComponents<Collider>();
            colliderSizes = ExtentHunter();
            ultimateVector = ExtentFighter();

            //Gets information from relevent ShipsScriptable.
            hoverForce = ss.hoverForce;
            hoverConstant = ss.hoverStiffness;
            rollForce = ss.rollForce;
            rollRayDistance = ss.rollRayDistance;
            pitchForce = ss.pitchForce;
            pitchRayDistance = ss.pitchRayDistance;
            pitchRollConstant = ss.pitchRollStiffness;
            centerOffset = ss.colliderCalcOffset;
            stableOffsetVector = ss.fineTuneOffset;
        }

        private void FixedUpdate()
        {
            rollRay1 = new Ray(transform.localPosition + (transform.right * (ultimateVector.x - centerOffset.x - stableOffsetVector.x)) + (transform.up * (ultimateVector.y + centerOffset.y)), transform.up * -1);//starboard
            rollRay2 = new Ray(transform.localPosition - (transform.right * (ultimateVector.x + centerOffset.x - stableOffsetVector.x)) + (transform.up * (ultimateVector.y + centerOffset.y)), transform.up * -1);//port
            pitchRay1 = new Ray(transform.localPosition + (transform.forward * (ultimateVector.z - centerOffset.z)) + (transform.up * (ultimateVector.y + centerOffset.y)), transform.up * -1);//bow
            pitchRay2 = new Ray(transform.localPosition - (transform.forward * (ultimateVector.z + centerOffset.z)) + (transform.up * (ultimateVector.y + centerOffset.y)), transform.up * -1);//stern
            bowRay = new Ray(pitchRay1.origin - (transform.forward * stableOffsetVector.z), transform.forward);

            ;       //Draws all rays for dev purposes. Disable in editor.
            if (aGMRaysOn)
            {
                Debug.DrawRay(rollRay1.origin, rollRay1.direction * rollRayDistance, Color.blue, debugRayTime, true);
                Debug.DrawRay(rollRay2.origin, rollRay2.direction * rollRayDistance, Color.blue, debugRayTime, true);
                Debug.DrawRay(pitchRay1.origin, pitchRay1.direction * pitchRayDistance, Color.red, debugRayTime, true);
                Debug.DrawRay(pitchRay2.origin, pitchRay2.direction * pitchRayDistance, Color.red, debugRayTime, true);
                Debug.DrawRay(bowRay.origin, bowRay.direction * bowHitDistance, Color.red, debugRayTime, true);
            }

            //Applies vertical force and pitch torque.
            if (Physics.Raycast(pitchRay1, out pitchHit1, pitchRayDistance) && Physics.Raycast(pitchRay2, out pitchHit2, pitchRayDistance))
            {
                pitchIsStable = true;
                //y force.
                vehicleRB.AddRelativeForce(Vector3.up * (HoverSmoother(new Ray[] { pitchRay1, pitchRay2}) * Time.fixedDeltaTime), ForceMode.Impulse);
                //x torque.
                //This should turn off pitch stabalisation if really close to a wall.
                if (Physics.Raycast(bowRay, bowHitDistance) == false)
                {
                    pitchHitInfo1 = pitchHit1.distance;
                    pitchHitInfo2 = pitchHit2.distance;
                    //If ship is pitching the wrong way, swap pitchHitInfo 2 and pitchHitInfo 1 below.
                    pitchDiff = pitchHitInfo1 - pitchHitInfo2;
                    vehicleRB.AddRelativeTorque(Vector3.right * RollPitchSmoother(pitchDiff) * pitchForce * Time.fixedDeltaTime, ForceMode.Impulse);
                }
            }
            else
            {
                pitchIsStable = false;
            }
        
            //Applies roll torque.
            if (Physics.Raycast(rollRay1, out rollHit1, rollRayDistance) && Physics.Raycast(rollRay2, out rollHit2, rollRayDistance) && pitchIsStable)
            {
                //z torque.
                //This should turn off roll stabalisation if really close to a wall.
                rollHitInfo1 = rollHit1.distance;
                rollHitInfo2 = rollHit2.distance;
                //If ship is rolling the wrong way, swap rollHitInfo 2 and rollHitInfo 1 below.
                rollDiff = rollHitInfo2 - rollHitInfo1;
                vehicleRB.AddRelativeTorque(Vector3.forward * RollPitchSmoother(rollDiff) * rollForce * Time.fixedDeltaTime, ForceMode.Impulse);
            }      
        }

        private float HoverSmoother(Ray[] inputRays)
        {
            float distanceToFloor = 0.0f;

            //Combines distance to floor of all rays passed in, then calculates the mean.
            for (int i = 0; i < inputRays.Length; i++)
            {
                Physics.Raycast(inputRays[i], out hoverHitInfo, pitchRayDistance);
                distanceToFloor = distanceToFloor + hoverHitInfo.distance;
            }
            float averageDistance = distanceToFloor / inputRays.Length;

            //regulator increases as distance to the floor lowers. Can be adjusted by changing hoverConstant.
            float regulator = hoverConstant / (averageDistance + 0.1f);
            float newForce = hoverForce * regulator;
            return newForce;
        }

        private float RollPitchSmoother(float distanceDifference)
        {
            float newForce = 0.0f;

            if (distanceDifference != newForce)
            {
                //regulator increases with the difference between pitchRay1 & pitchRay2 distance to hit.
                float regulator = (distanceDifference / pitchRollConstant);
                newForce = hoverForce * regulator;
            }
            return newForce;
        }

        //Finds the extents of all colliders on parent object.
        private Vector3[] ExtentHunter()
        {
            List<Vector3> colliderSizesList = new List<Vector3>();

            for (int i = 0; i < colliderList.Length; i++)
            {
                colliderSizesList.Add(colliderList[i].bounds.extents);
            }

            Vector3[] hunterArray = colliderSizesList.ToArray();
            return hunterArray;
        }

        //Compares extents and keeps the highest for x, y, z.
        public Vector3 ExtentFighter()
        {
            float championZ = 0;
            float championX = 0;
            float championY = 0;

            for (int q = 0; q < colliderSizes.Length; q++)
            {
                float contenderZ = colliderSizes[q].z;
                if (contenderZ > championZ)
                {
                    championZ = contenderZ;
                }

                float contenderX = colliderSizes[q].x;
                if (contenderX > championX)
                {
                    championX = contenderX;
                }

                float contenderY = colliderSizes[q].y;
                if (contenderY > championY)
                {
                    championY = contenderY;
                }
            }

            Vector3 championFusion = new Vector3(championX, championY, championZ);
            return championFusion;
        }
    }
}
