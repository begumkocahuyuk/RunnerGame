using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidingCylincer : MonoBehaviour
{
    private bool _filled;
    private float _value;

    public void IncrementCylincerVolume(float value)
    {
        _value+=value;
        if(_value>1)
        {
            float leftValue=_value-1;
            int cylinderCount=PlayerController.Current.cylinders.Count;
            transform.localPosition=new Vector3(transform.localPosition.x,-0.5f*(cylinderCount-1)-0.25f,transform.localPosition.z);
            transform.localScale=new Vector3(0.5f,transform.localScale.y,0.5f);
            PlayerController.Current.CreateCyclinder(leftValue);
        }
        else if(_value<0)
        {
            PlayerController.Current.DestroyCyclinder(this);
        }
        else{
            int cylinderCount=PlayerController.Current.cylinders.Count;
            transform.localPosition=new Vector3(transform.localPosition.x,-0.5f*(cylinderCount-1)-0.25f*_value,transform.localPosition.z);
            transform.localScale=new Vector3(0.5f*_value,transform.localScale.y,0.5f*_value);
        }
    }
}
