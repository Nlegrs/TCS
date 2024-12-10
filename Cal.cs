using Godot;
using System;

public partial class Cal : Node3D
{
	private double M , E , tmp;
	private double f , r , COSE;
	private float a,b,c;
	
	private int[] mNissu = {1,0,1,0,1,0,1,1,0,1,0,1};//それぞれの月の日の数−３０
	private int URUUQTY2000 = 484; //2000年時てんのうるう年の数
		//mass Ekg(10^18) size km
	//  Sun Macury Venus Earth,moon Mars,phobos,deimos Juptor,Io,europa,ganymede,callisto 
	//Saturn,titan,enceladus,dione,tethys,rhea,mimas Neptune,miranda,ariel,umbriel,titania,oberon
	//Uranus,triton,prometeus Plto,kalon
	//sun mercury venus earth mars jupitor saturn neptune uranus pluto
		private double[] Cgravity  =   {1132712.440,0.2203208,3.248587,3.986004,0.4282829,1267.126,379.3952,57.80159,68.71308,0.01020865};//10^5 km^3 / s^2重力定数
		private double[] Cperiod  =   {0,2111.256,5392.824,8765.812536,16487.52,103976.123686,258873.378343,738546.091254,1444518.24781,2171648.20414};//公転周期
		private float[] Ceccentricity  =   {0f,0.20563f,0.00678f,0.01672f,0.09338f,0.04829f,0.05604f,0.04612f,0.01011f,0.24847f};//離心率
		//private readonly double[] CLauaxis  =   {0.387099,0.723332,1,1.523691,5.202833,9.538762,19.191391,30.061069,39.5294602};//軌道長半径
		private double[] CLaxis = {0,57909185,108208926,149597870,227940928,7783327345,1426978478,2870991216,4497071892,5913523048};//軌道長半径
		private float[]  CepoGHT =   {0F,1098.26F,4649.52F,73.61F,15613F,98210F,30855.4F,449115F,416411F,2085251F};//GHT0での元期時刻
		private float[]  CslopeX =   {0f,0f,0f};//軌道傾斜角x
		private float[]  CslopeY =   {0f,0f,0f};//軌道傾斜角y
		private float[]  CFepoGHT =   {0f,0f,0f,0f,0f,0f,0f,0f,0f,0f};//nowGHTでの元期時刻
		
	public float getNowGHT(){ //グレゴリオ暦で2000とのHour差を符号付きで計算します..たぶん３０万年ぐらいでオーバーフローするからちゅうい.
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
	public void fixEpochTime(int celIndex , float GHTtime){//近日点通過時刻を修正する。もし処理に余裕がなければ時刻が予測不能なズレをするときにのみ使うようにする
		double TpeD;
		if(GHTtime < CepoGHT[celIndex]){
			float peD = CepoGHT[celIndex] - GHTtime;
			TpeD = peD / Cperiod[celIndex];
			TpeD = Cperiod[celIndex] * (int)TpeD;
			TpeD += Cperiod[celIndex];//１つ前の近日点通過時刻にする必要があるため
			TpeD = -TpeD;
		}else{
			float peD = GHTtime - CepoGHT[celIndex];
			TpeD = peD / Cperiod[celIndex];
			TpeD = Cperiod[celIndex] * (int)TpeD;
		}
		
		CFepoGHT[celIndex] = CepoGHT[celIndex] + (float)TpeD;
		
	}
	public void calOrbiterPosition(int celIndex , float GHTtime){
		M = ( GHTtime - CFepoGHT[celIndex] ) / Cperiod[celIndex];//平均近点角rad
		M = M * 2 * Mathf.Pi;
		a = (float)(M / (1 - Ceccentricity[celIndex]));
		b = (float)(M + Ceccentricity[celIndex]);
		c = (float)((M + Mathf.Pi * Ceccentricity[celIndex])/(1 + Ceccentricity[celIndex]));
		//どれが一番小さいか（安定して収束させるためのE0を求める
			if(a < b){
				if(a < c){
					E = a;
				}else{
					E = c;
				}
			}else{ // b < a
				if(b < c){
					E = b;
				}else{
					E = c;
				}
			}
		for(int i=0; i<2; i++){
			tmp = E - Ceccentricity[celIndex] * Mathf.Sin(E) -M;//分母
			tmp = tmp /( 1 - Ceccentricity[celIndex] * Mathf.Cos(E));//分子
			E = E - tmp;//
		}
		COSE = Mathf.Cos(E); 
		//tmp = Mathf.Sqrt(1 - Ceccentricity * Ceccentricity);
		//tmp = tmp * Mathf.Sin(E);
		//f = tmp/(1 - Ceccentricity * COSE);
		//f = Mathf.Asin(f);
		
		f = (COSE - Ceccentricity[celIndex])/(1 - Ceccentricity[celIndex] * COSE);
		f = Mathf.Acos(f);
		if(( GHTtime - CFepoGHT[celIndex] ) > (Cperiod[celIndex] / 2)){
			f = -f;
		}
		
		r = CLaxis[celIndex] * (1 - Ceccentricity[celIndex] * COSE);
		
		Main.positions[celIndex , 0] = r * Mathf.Cos(f);
		Main.positions[celIndex , 2] = r * Mathf.Sin(f);
		
		GD.Print($"{f} , {r}");
	}
}
