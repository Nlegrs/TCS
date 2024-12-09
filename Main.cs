using Godot;
using System;

public partial class Main : Node3D
{//aejo
	Transform3D t = Transform3D.Identity;
	private int[] mNissu = {1,0,1,0,1,0,1,1,0,1,0,1};//それぞれの月の日の数−３０
	private int URUUQTY2000 = 484; //2000年時てんのうるう年の数
		//mass Ekg(10^18) size km
	//  Sun Macury Venus Earth,moon Mars,phobos,deimos Juptor,Io,europa,ganymede,callisto 
	//Saturn,titan,enceladus,dione,tethys,rhea,mimas Neptune,miranda,ariel,umbriel,titania,oberon
	//Uranus,triton,prometeus Plto,kalon
	//sun mercury venus earth mars jupitor saturn neptune uranus pluto
		public double[] Cgravity  =   {1132712.440,0.2203208,3.248587,3.986004,0.4282829,1267.126,379.3952,57.80159,68.71308,0.01020865};//10^5 km^3 / s^2重力定数
		public double[] Cperiod  =   {0,2111.256,5392.824,8765.812536,16487.52,103976.123686,258873.378343,738546.091254,1444518.24781,2171648.20414};//公転周期
		public float[] Ceccentricity  =   {0f,0.20563f,0.00678f,0.01672f,0.09338f,0.04829f,0.05604f,0.04612f,0.01011f,0.24847f};//離心率
		//public readonly double[] CLauaxis  =   {0.387099,0.723332,1,1.523691,5.202833,9.538762,19.191391,30.061069,39.5294602};//軌道長半径
		public double[] CLaxis = {0,57909185,108208926,149597870,227940928,7783327345,1426978478,2870991216,4497071892,5913523048};//軌道長半径
		public float[]  CepoGHT =   {1098.26F,4649.52F,73.61F,15613F,98210F,30855.4F,449115F,416411F,2085251F};//GHT0での元期時刻
		public float[]  CslopeX =   {0f,0f,0f};//軌道傾斜角x
		public float[]  CslopeY =   {0f,0f,0f};//軌道傾斜角y
		public float[]  CFepoGHT =   {0f,0f,0f,0f,0f,0f,0f,0f,0f,0f};//nowGHTでの元期時刻
		
		
	class ProbeSpec{
		public float mass;
		public float thrustPower;
	}
	public double[,] positions = {
		
		{0, 0, 0},     //probe
		{0, 0, 0},     //earth
		{0, 0, 0} //moon
 	};
	public double[] thrust = {0 , 0 , 0}; //(km/ss)
	public double[] orbiterSpeed = {0 , 0 , 0}; //km/s
	public float speedScale;
	public bool isThrust = false;
	public float nowSHT;//standard hour time
	private double[] PROBESPEC = {0,0,0}; //mass kg   thrust power N 
	
	Node3D sunN;
	public override void _Ready()
	{
		ProbeSpec Fukashi = new ProbeSpec();
		Fukashi.mass = 250;//kg
		Fukashi.thrustPower = 200;//N
		
		positions[0,1] = 43270;//静止軌道 高度+地球の半径
		speedScale = 1;
		float NGHT = getNowGHT();
		GD.Print(NGHT);
		for(int i=0;i<10;i++){
			fixEpochTime(i , NGHT);
		}
		//calOrbiterPosition(1 , NGHT);
		/*
		sunN = (Node3D)GetNode("Sun");
		t.Origin = new Vector3(0,0,0);
		sunN.Transform = t;*/
		double AU = 149597870;
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
		}*/
		
		GD.Print(" ");
		for(int i=1;i<10;i++){
			GD.Print(CFepoGHT[i]);
		}
	}
	
	public override void _Process(double deltaT)
	{
		
	}
	
	float getNowGHT(){ //グレゴリオ暦で2000とのHour差を符号付きで計算します..たぶん３０万年ぐらいでオーバーフローするからちゅうい.
		string a = Time.GetDatetimeStringFromSystem(true,false);
		int.TryParse($"{a[0]}{a[1]}{a[2]}{a[3]}", out int year);
		int.TryParse($"{a[5]}{a[6]}", out int month);
		int.TryParse($"{a[8]}{a[9]}", out int date);
		int.TryParse($"{a[11]}{a[12]}", out int hour);
		int.TryParse($"{a[14]}{a[15]}", out int minute);
	   
		//year = 2003;month=7;date=1;hour=0;    
		float result=0;
		if(year < 2000){///2000年前後で処理を分ける
			int uruuQty = year/4;
			uruuQty -= year/100;
			uruuQty += year/400;//グレゴリオ暦によるうるう年の定義
			uruuQty = URUUQTY2000 - uruuQty; //うるう年計算終了
			
			year++; // 現在年をかぞえたらいけないから
			year = 2000 - year; //yearは０はじまりだから普通に引いてok
			result += uruuQty * 366;
			result += (year - uruuQty) * 365; //yearしょり終了
			year--;//もどした  
			for(int i=12;month<i;i--){ //mのしょり
				if(i==2){ //iは2~12の値を取る
					if(year%4 == 0 && year%100 != 0 && year%200 != 0 && year%300 != 0 ){//うるう年かどうか
						result+=29;
					}else{
						result+=28;
					}
				}else{
					result+= 30 + mNissu[i-1];//配列が0始まりだkら
				} 
			}
			result += (30 + mNissu[month-1]) - date;
			result = result * 24;
			
			result += 23 - hour; // 24 - ( Hour +1 )を変形した形
			result += (59 - minute) / 60; //上に同じ原理
			result = -result;
			
		}else{

			year--; //現在年のうるう年はかぞえたらいけない
			int uruuQty = year/4;
			uruuQty -= year/100;
			uruuQty += year/400;
			uruuQty = uruuQty - URUUQTY2000; //うるう年計算終了
			year++; //もどした

			year = year - 2000;
			result += uruuQty * 366;
			result += (year - uruuQty) * 365; //yearしょり終了
			
			month--; //１始まりだから経験した月の数はマイナス１になる
			for(int i=0;i<month;i++){ //mのしょり 現在月はかぞえないので<
				if(i==1){ //0はじまりだからi=1で２月
					if(year%4 == 0 && year%100 != 0 && year%200 != 0 && year%300 != 0 ){//うるう年かどうか
						result+=29;
					}else{
						result+=28;
					}
				}else{
					result+= 30 + mNissu[i];
				} 
			}
			date--; //１始まりだから
			result += date;
			result = result * 24;　//太陽日からhourへ
			result += hour;
			result += minute/60;
		}
		return result;

	}
	void fixEpochTime(int celIdex , float GHTtime){//近日点通過時刻を修正する。もし処理に余裕がなければ時刻が予測不能なズレをするときにのみ使うようにする
		double TpeD;
		if(GHTtime < CepoGHT[celIdex]){
			float peD = CepoGHT[celIdex] - GHTtime;
			TpeD = peD / Cperiod[celIdex];
			TpeD = Cperiod[celIdex] * (int)TpeD;
			TpeD += Cperiod[celIdex];//１つ前の近日点通過時刻にする必要があるため
			TpeD = -TpeD;
		}else{
			float peD = GHTtime - CepoGHT[celIdex];
			TpeD = peD / Cperiod[celIdex];
			TpeD = Cperiod[celIdex] * (int)TpeD;
		}
		
		CFepoGHT[celIdex] = CepoGHT[celIdex] + (float)TpeD;
		
	}
	private void calOrbiterPosition(int celIdex , float GHTtime){
		double l = Cmove[celIdex] * ( GHTtime - CFepoGHT[celIdex] );//平均近点角rad
		GD.Print(l);
		if(l > 360){
			l = 0;
		}
		double u =0;//離心近点角°
		if(Ceccentricity[celIdex] < 0.4 ){
			u = l;//近似しなくておｋ
		}else{
			//uをlから近似しておく    
		}
		double deltaU;//補正値
		
		deltaU = l - u + ( Ceccentricity[celIdex] * Mathf.Sin(Mathf.DegToRad(u)) );
		deltaU = deltaU / ( 1 - ( Ceccentricity[celIdex] * Mathf.Cos(Mathf.DegToRad(u)) ) );
		u = u + deltaU;
		while(deltaU > 0.01){//しきい値は結果を見て決めよう
			deltaU = l - u + ( Ceccentricity[celIdex] * Mathf.Sin(Mathf.DegToRad(u)) );
			deltaU = deltaU / ( 1 - ( Ceccentricity[celIdex] * Mathf.Cos(Mathf.DegToRad(u)) ) );
			u = u + deltaU;
		}
		
		GD.Print($"u:{u}");
		double OMECU = (1 - (Ceccentricity[celIdex] * Mathf.Cos(u)));//1-ecosuが何度も登場するのでメモリにいれておく
		
		/// arcsinでやる場合
		//double f = Mathf.Sqrt(1 - (Ceccentricity[celIdex] ** 2 ));
		//f = f * Mathf.Sin(u);
		//f = f/OMECU;
		//f = Mathf.Asin(f);
		
		
		double f = Mathf.Cos(u) - Ceccentricity[celIdex];　//真近点角f
		f = f/OMECU;
		f = Mathf.Acos(f);  
		
		double r = CLaxis[celIdex] * OMECU;   //極座標系r
		GD.Print($"f:{f} , r:{r}");
	}

}
