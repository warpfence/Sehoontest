﻿using UnityEngine;
using System.Collections;

public class car : MonoBehaviour {

	public Transform centerOfMass;
	public Transform steerWheel;
	public float steerAngle;
	public Camera[] cam;

	public GameObject[] backLights;
	public GameObject hands;
	public Color color;
	
	public float Cdrag=0.2f;
	public float Fdrag;
	
	public float maxTorque=1000;
	public float maxBrakeTorque = 1000;
	public float MaxWheelRotateAngle= 14;
	public float lowestSpeedAtSteer = 45;
	public float lowSpeedSteerAngle = 32;
	public float highSpeedSteerAngle = 1;
	public float torque;
	public float steerWheelAngle;
	public float steerWheelRotateFactor=1;
	public bool braked;

	public WheelCollider WheelFrontRight;
	public WheelCollider WheelFrontLeft;
	public WheelCollider WheelRearRight;
	public WheelCollider WheelRearLeft;
	

	public GameObject wheelFR;
	public GameObject wheelFL;
	public GameObject wheelRR;
	public GameObject wheelRL;

	float speed;
	float oldAngle=0;
	float newAngle=0;
	float timer=0;
	float angleChangeTimer=0;
	float steerwheelOffset=0;


	int i;
	Quaternion rot;

	void Start () {
		GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
		color=backLights[0].GetComponent<Renderer>().material.GetColor("_TintColor");
		rot=steerWheel.transform.rotation;
		hands.SetActive(false);
	}

	void Update () {

		CameraSwitch();
		control();
		//steerWheel.transform.localRotation=rot * Quaternion.Euler(new Vector3(0,steerAngle,0));
	} 

	void FixedUpdate () {
		speed=GetComponent<Rigidbody>().velocity.magnitude*3.6f;

		HandBrake();
		Fdrag=Cdrag*Mathf.Pow(speed,2);

		WheelOffset(wheelFL, WheelFrontLeft);
		WheelOffset(wheelFR, WheelFrontRight);
		WheelOffset(wheelRL, WheelRearLeft);
		WheelOffset(wheelRR, WheelRearRight);

		for(int i=0;i<backLights.Length;i++)
			backLights[i].GetComponent<Renderer>().material.SetColor("_TintColor",color);
	}
	
	void control (){
		torque=maxTorque*(Input.GetAxis("Vertical"));

		if(Input.GetAxis("Vertical")<0 && speed>20)
			torque=0;
		torque-=Fdrag;

		WheelRearRight.motorTorque = torque;
		WheelRearLeft.motorTorque = torque;
	
		SteerWheelControl();
	}

	void SteerWheelControl(){

		if(angleChangeTimer<0){
			steerwheelOffset=Random.Range(-0.4f,0.4f)*speed;
			angleChangeTimer+=Random.Range(0.2f,1);
		}

		float speedFactor = (speed/3.6f)/lowestSpeedAtSteer;
		float currentSteerAngle = Mathf.Lerp(lowSpeedSteerAngle,highSpeedSteerAngle,speedFactor);
		currentSteerAngle *=Input.GetAxis("Horizontal");
		WheelFrontRight.steerAngle = currentSteerAngle;
		WheelFrontLeft.steerAngle = currentSteerAngle;

		timer+=Time.deltaTime*steerWheelRotateFactor;
		newAngle=Mathf.Lerp(oldAngle,(currentSteerAngle*10)+steerwheelOffset, timer);

		//steerWheelAngle=currentSteerAngle*10;
		steerWheelAngle=newAngle;
		steerWheel.transform.localRotation=rot * Quaternion.Euler(new Vector3(0,steerWheelAngle,0));

		oldAngle=steerWheelAngle;
		if(oldAngle==newAngle)
			timer=0;

		angleChangeTimer-=Time.deltaTime;
	}

	void WheelOffset (GameObject model, WheelCollider collider)
	{
		Quaternion quart;
		Vector3 pos;
		collider.GetWorldPose(out pos,out quart);
		model.transform.position=pos;
		model.transform.rotation=quart;
	}

	void HandBrake(){
		if(Input.GetButton ("Jump") || (Input.GetAxis("Vertical")==0 && (int)speed==0)){
			braked=true;
		}
		else{
			braked=false;
		}
		if(braked){
			WheelRearLeft.brakeTorque = maxBrakeTorque;
			WheelRearRight.brakeTorque = maxBrakeTorque;
			WheelRearRight.motorTorque = 0;
			WheelRearLeft.motorTorque = 0;
			color.a=1f;
		}
		else{
			WheelRearLeft.brakeTorque = 0;
			WheelRearRight.brakeTorque = 0;
			color.a=0.5f;
		}
	}

	void CameraSwitch(){
		if(Input.GetKeyDown(KeyCode.C)){
			cam[i].enabled=false;
			cam[i].GetComponent<AudioListener>().enabled=false;
			i++;
			if(i>=cam.Length)
				i=0;
			cam[i].enabled=true;
			cam[i].GetComponent<AudioListener>().enabled=true;
			hands.SetActive(i==1);
		}

	}
}
