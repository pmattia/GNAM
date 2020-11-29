using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SqueezableFood : GrabbableEvents
{
    public Animator foodAnimator;
    public Eatable eatableContainer;
    public Transform squeezableFoodBody;
    
    private Rigidbody bodyRigidBody; 
    private Vector3 bodyStartPosition;
    private Vector3 bodyTargetPosition;
    private float prevTriggerValue = 0;
    private bool isBodyFree = false;

    // Start is called before the first frame update
    public void Awake()
    {
        bodyStartPosition = squeezableFoodBody.localPosition;
        bodyTargetPosition = Vector3.zero;
        bodyRigidBody = squeezableFoodBody.GetComponent<Rigidbody>();

        if (eatableContainer != null)
        {
            eatableContainer.onEated += (mouth) => DetachBody();
        }

        //Solo quando l'utente preme il calippo può mangiare il ghiacciolo
        //SetChunksStatus(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DetachBody()
    {
        squeezableFoodBody.transform.parent = null;
        bodyRigidBody.isKinematic = false;
        squeezableFoodBody.GetComponent<Collider>().enabled = true;
        isBodyFree = true;
    }

    private IEnumerator SqueezeThrow()
    {
        DetachBody();
        yield return new WaitForFixedUpdate();
        bodyRigidBody.AddForce(transform.up * 5, ForceMode.Impulse);
    }

    public override void OnTrigger(float triggerValue)
    {
        Debug.Log(triggerValue);
        if (foodAnimator != null)
        {

            foodAnimator.SetFloat("Intensity", triggerValue);

            if (!isBodyFree)
            {
                float deltaY = ((((triggerValue - 0.5f) / 0.5f) * 0.7f) + 0.15f) / 4;

                if (deltaY < bodyStartPosition.y)
                {
                    //SetChunksStatus(false);
                    bodyTargetPosition = bodyStartPosition;
                }
                else
                {
                    var speedMag = (triggerValue - prevTriggerValue) / Time.deltaTime;
                    prevTriggerValue = triggerValue;

                    if ((triggerValue > 0.9f && speedMag > 5f)
                            || (triggerValue > 0.95f && speedMag > 1f)
                            )
                    {
                        StartCoroutine(SqueezeThrow());
                    }
                    else
                    {
                        //SetChunksStatus(true);
                        Vector3 delta = new Vector3(0, deltaY, 0);
                        bodyTargetPosition = bodyStartPosition + delta;
                        squeezableFoodBody.localPosition = (bodyTargetPosition);
                    }
                }
            }
        }
    }
}
