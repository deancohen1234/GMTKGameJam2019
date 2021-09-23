using System;
using UnityEngine;

[System.Serializable]
public class DashAction : PlayerAction
{
    //TODO make sure this is never longer than the action length
    public int DashDisarmLength = 3; //in frames

    public Action OnDashDisarmEnd;

    public override void CheckActionCompleteness(float xInput, float yInput)
    {
        base.CheckActionCompleteness(xInput, yInput);

        if (IsExecuting)
        {
            //Dash disarm window is over
            if (Time.frameCount - StartingFrame >= DashDisarmLength + StartDelay)
            {
                if (OnActionEnd != null)
                {
                    OnDashDisarmEnd.Invoke();
                }
            }
        }
    }

    public override void ForceStopAction()
    {
        base.ForceStopAction();

        OnDashDisarmEnd.Invoke();
    }
}
