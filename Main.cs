using Godot;
using System;

public partial class Main : Node3D
{
	Cal cal;
	CelCon celCon;
	
	class ProbeSpec{
		public float mass;
		public float thrustPower;
	}
	public static double[,] positions = new double[10,3];
	public static double[] thrust = {0 , 0 , 0}; //(km/ss)
	public static double[] orbiterSpeed = {0 , 0 , 0}; //km/s
	public static float speedScale;
	public static bool isThrust = false;
	public static float nowSHT;//standard hour time
	
	
	public override void _Ready()
	{
		cal = new Cal();
		celCon = new CelCon();
		ProbeSpec Fukashi = new ProbeSpec();
		Fukashi.mass = 250;//kg
		Fukashi.thrustPower = 200;//N
		
		positions[0,1] = 43270;//静止軌道 高度+地球の半径
		speedScale = 1;
		float NGHT = cal.getNowGHT();
		GD.Print($"nowGHT:{NGHT}");
		for(int i=0;i<10;i++){
			cal.fixEpochTime(i , NGHT);
		}
		cal.calOrbiterPosition(1 , NGHT);
		GD.Print($"a:{positions[1,0]},{positions[1,1]},{positions[1,2]}");
		//double AU = 149597870;
		/*
		for(int i=1;i<10;i++){
			CepoGHT[i] = (CepoGHT[i] /360) * (float)Cperiod[i];
			GD.Print(CepoGHT[i]);
			CepoGHT[i] = 30648 - CepoGHT[i];
			while(CepoGHT[i] > 0){
				CepoGHT[i] -= (float)Cperiod[i];
			}
			while(CepoGHT[i] < 0){
				CepoGHT[i] += (float)Cperiod[i];
			}
		}
		
		GD.Print(" ");
		for(int i=1;i<10;i++){
			GD.Print(CFepoGHT[i]);
		}*/
	}
	
	public override void _Process(double deltaT)
	{
		
	}

}
