using System.Numerics;
using System;

public partial class Main
{
    
    
        //mass Ekg(10^18) size km
    //  Sun Macury Venus Earth,moon Mars,phobos,deimos Juptor,Io,europa,ganymede,callisto 
    //Saturn,titan,enceladus,dione,tethys,rhea,mimas Neptune,miranda,ariel,umbriel,titania,oberon
    //Uranus,triton,prometeus Plto,kalon
    //sun mercury venus earth mars jupitor saturn neptune uranus pluto
        
    public static double date2SHT(in string a){ //グレゴリオ暦で2000とのHour差を符号付きで計算します..たぶん３０万年ぐらいでオーバーフローするからちゅうい.
        int.TryParse($"{a[0]}{a[1]}{a[2]}{a[3]}", out int year);
        int.TryParse($"{a[5]}{a[6]}", out int month);
        int.TryParse($"{a[8]}{a[9]}", out int date);
        int.TryParse($"{a[11]}{a[12]}", out int hour);
        int.TryParse($"{a[14]}{a[15]}", out int minute);
        int.TryParse($"{a[17]}{a[18]}", out int second);
        Godot.GD.Print(month);
       
        //year = 2003;month=7;date=1;hour=0;    
        double result=0;
        if(year < 2000){///2000年前後で処理を分ける
            int uruuQty = year/4;
            uruuQty -= year/100;
            uruuQty += year/400;//グレゴリオ暦によるうるう年の定義
            uruuQty = Main.URUUQTY2000 - uruuQty; //うるう年計算終了
            
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
                    result+= Main.mNissu[i-1];//配列が0始まりだkら
                } 
            }
            result += (Main.mNissu[month-1]) - date;
            result = result * 24;
            
            result += 23 - hour; // 24 - ( Hour +1 )を変形した形
            result += (59 - minute) / 60; //上に同じ原理
            result = -result;
            
        }else{

            year--; //現在年のうるう年はかぞえたらいけない
            int uruuQty = year/4;
            uruuQty -= year/100;
            uruuQty += year/400;
            uruuQty = uruuQty - Main.URUUQTY2000; //うるう年計算終了
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
                    result+=Main.mNissu[i];
                } 
            }
            date--; //１始まりだから
            result += date;
            result = result * 24;　//太陽日からhourへ
            result += hour;
            result += (double)minute/60;
            result += (double)second/3600;
        }
        return result;

    }/*
    public static void fixEpochTime(in int ci , in float SHTtime){//近日点通過時刻を修正する
        float TpeD;
        if(SHTtime < Main.CelE.o[ci].epo){
            TpeD = (Main.CelE.o[ci].epo - SHTtime) / Main.CelE.o[ci].period;
            TpeD = Main.CelE.o[ci].period * (int)TpeD;
            TpeD += Main.CelE.o[ci].period;//１つ前の近日点通過時刻にする必要があるため
            TpeD = -TpeD;
        }else{
            TpeD = (SHTtime - Main.CelE.o[ci].epo) / Main.CelE.o[ci].period;
            TpeD = Main.CelE.o[ci].period * (int)TpeD;
        }
        Main.CelE.Fepo[ci] = Main.CelE.o[ci].epo + TpeD;
        
    }*/
    public static void calOrbiterPosition(in int ci , in double SHTtime, in bool isV){ //newton method
        if( ci == 0 ){

        }else{
            double M, E, tmp, f;
            float a,b,c;

            M = SHTtime / Main.CelE.o[ci].period;
            M = M % 1; //小数部分o
            M = M * 2 * Math.PI + (double)Main.CelE.o[ci].epo; //平均近点角rad
            if( 2*Math.PI <= M){
                M -= 2*Math.PI;
            }
            //                                                                                 Godot.GD.Print(toDeg(M));
            a = (float)(M / (1 - Main.CelE.o[ci].e));
            b = (float)(M + Main.CelE.o[ci].e);
            c = (float)((M + Math.PI * Main.CelE.o[ci].e)/(1 + Main.CelE.o[ci].e));
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
            for(int i=0; i<4; i++){
                tmp = E - Main.CelE.o[ci].e * Math.Sin(E) -M;//分母
                tmp = tmp /( 1 - Main.CelE.o[ci].e * Math.Cos(E));//分子
                if((float)tmp == 0F){
                    E -= tmp;//
                    break;
                }
                E -= tmp;//
            } 
            tmp = Math.Cos(E); 
            f = (tmp - Main.CelE.o[ci].e)/(1 - Main.CelE.o[ci].e * tmp);
            //E = 極座標のr  変数をつりたくなかった
            E = Main.CelE.o[ci].l / (1 - Main.CelE.o[ci].e * f);
            f = Math.Acos(f);
            //cosなので半分越えてたら反転
            if( M > Math.PI){
                f = -f;
            }
            f += CelE.o[ci].s0; //近日点黄経追加
            M   = E * Math.Cos(f);
            tmp = E * Math.Sin(f);
            if(isV){
                Main.vpositions[ci , 0] = (M * (double)Main.CelE.o[ci].c2) - (tmp * (double)Main.CelE.o[ci].c1 * (double)Main.CelE.o[ci].s2);
                Main.vpositions[ci , 1] = (M * (double)Main.CelE.o[ci].s2) + (tmp * (double)Main.CelE.o[ci].c1 * (double)Main.CelE.o[ci].c2);
                Main.vpositions[ci , 2] = tmp * (double)Main.CelE.o[ci].s1;
            }else{
                Main.positions[ci , 0] = (M * (double)Main.CelE.o[ci].c2) - (tmp * (double)Main.CelE.o[ci].c1 * (double)Main.CelE.o[ci].s2);
                Main.positions[ci , 1] = (M * (double)Main.CelE.o[ci].s2) + (tmp * (double)Main.CelE.o[ci].c1 * (double)Main.CelE.o[ci].c2);
                Main.positions[ci , 2] = tmp * (double)Main.CelE.o[ci].s1;
            }
        }

    }

    public static Orbit solveOrbit(Vector3 v, int ci, Vector3 r, double v0, double r0){ //速度, 主星, 主星からの位置
        Orbit o = new Orbit();
        double tmp = (r0 * Math.Pow(v0, 2) * 1000)/ (CelE.grav[ci]); //たくさん出てくるので   単位あわせで1000かけてる
        double sin = Vector3.Dot(v, r) / (v0 * r0); //sinγ
        double cos = Math.Sqrt(1 - Math.Pow(sin, 2)); //なす角はpi未満
        o.e = (float)Math.Sqrt( ((tmp - 2) * tmp * Math.Pow(cos, 2)) + 1);
        // e ok
        

        // 軌道面を決定
        Vector3 n = Vector3.Cross(r, v); //軌道面の法線 
        float n0 = n.Length();
        o.c1 = n.Z/n0; //n0!=0
        o.s1 = MathF.Sqrt(1 - MathF.Pow(o.c1 , 2));
        if(o.s1 != 0){ //ゼロ除算防止 軌道傾斜角0ならそもΩ定まらず
            o.s2 = n.X/(o.s1*n0);
            o.c2 = -n.Y/(o.s1*n0);
        }


        //vis-viva 方程式によるLaxsis
        //準線と焦点を使った楕円の定義から導出
        o.l = (CelE.grav[ci]*r0 / (((CelE.grav[ci]/500) - Math.Pow(v0, 2)*r0)*1000)) * (1+(double)o.e)*(1-(double)o.e);

        //近点引数を計算

            //軌道面上に変換したやつ
        Vector2 vv = new Vector2(v.X * o.c2 + v.Y * o.s2 , o.c1 * ( v.Y * o.c2 - v.X * o.s2 ) + v.Z * o.s1);//  ,  v.Z * o.c1 - o.s1 * ( v.Y * o.c2 - v.X * o.s2 ) );
        Vector2 rr = new Vector2(r.X * o.c2 + r.Y * o.s2 , o.c1 * ( r.Y * o.c2 - r.X * o.s2 ) + r.Z * o.s1);//  ,  r.Z * o.c1 - o.s1 * ( r.Y * o.c2 - r.X * o.s2 ) );
        //double[] vv = new double[2]{(float)(v.X * o.c2 + v.Y * o.s2)  ,  (float)(o.c1 * ( v.Y * o.c2 - v.X * o.s2 ) + v.Z * o.s1)};
        //double[] rr = new double[2]{(float)(r.X * o.c2 + r.Y * o.s2)  ,  (float)(o.c1 * ( r.Y * o.c2 - r.X * o.s2 ) + r.Z * o.s1)};

        float bunbo = (float)((tmp * Math.Pow(cos, 2))-1); //c^2 -1 = -s^2
        float bunshi = (float)(tmp * sin * cos);

        float som = 0;
        if(MathF.Log(MathF.Abs(bunshi)) - MathF.Log(MathF.Abs(bunbo)) < 7){ //許容する桁数 10 だったら 10^10までのtanを許容
        //aをだす

            float tf = bunshi/bunbo;//tan(真近点角)

            //楕円の極方程式より cos(真近点角)
            //float cf = -((float)r0 - o.l)/(o.e*(float)r0);

            Vector2 rrr = new Vector2(rr.X + (tf*rr.Y) , rr.Y - (tf*rr.X));
            som = MathF.Atan2(rrr.Y , rrr.X); // -pi< <pi atan2はすばらしい
            /*
            if(cf < 0){
                som -= MathF.PI;
            }
            */
            if(r0 < o.l){  // 正負がわかれば計算しなくていい
                som -= MathF.PI;
            }
            



        }else{
            ///f=pi/2 , 3pi/2

            som = MathF.Atan2(-rr.X , rr.Y);
            if( Vector2.Dot(rr,vv) < 0 ){ //とおざかっているときは逆がわ
                som += MathF.PI;
            }
        }
            o.s0 = som; //rad
        return o;
    }
    public static String SHT2date(double t){
        int yd = (int)Math.Truncate(t / 24);
        int y = yd / 365;
        int u = 0;
        if( 0 < t ){
            y -= 1;
            u += y / 4;
            u -= y / 100;
            u += y / 400;
            if( 0 <= y ){
                u++; //2000のも含めたいから
            }
            yd -= u;
        }else{
            y += 2001;
            u += y / 4;
            u -= y / 100;
            u += y / 400;
            u = Main.URUUQTY2000 - u;
            yd += u;
        }
        // yが正しくないので注意
        y = yd / 365;
        int d = yd - 365*y + 1; //1 base ydを調整してあるのでつねに365日としてよい
        int m = 0;
        for(m=0; mNissu[m]<d; m++){
            d -= mNissu[m];
            if(((y%4 == 0 && y%100 != 0) || (y%400 == 0))&&(d!=29)&&(m==1)){
                d--;
            }
        }
        m++;
        y += 2000;

        float h = (float)(t % 24);
        float min = (h % 1)*60;
        float s = (min % 1)*60;
        
        h = MathF.Truncate(h);
        min = MathF.Truncate(min);
        s = MathF.Truncate(s);

        return $"{y.ToString().PadLeft(4, '0')}-{m.ToString().PadLeft(2, '0')}-{d.ToString().PadLeft(2, '0')} {h.ToString().PadLeft(2, '0')}:{min.ToString().PadLeft(2, '0')}:{s.ToString().PadLeft(2, '0')}";

    }
    //平行投影
    public static Godot.Vector2 paraPro(float x, float y, float z, in float s0, in float s1, in float c0, in float c1){
        Godot.Vector2 v;
            v.X =  -x * s0;
            v.X +=  y * c0;
            v.Y =   x * s1 * c0;
            v.Y +=  y * s1 * s0;
            v.Y += -z * c1;
            return v;
    }
    //(0,0,|v|)からの回転を取得
    public static Quaternion V2Q(in float x, in float y, in float z){
        if( x == 0 && y == 0 ){
            if( 0 <= z ){
                return new Quaternion(0,0,0,1);
            }else{
                return new Quaternion(1,0,0,0);
            }
        }else{
            float a = (float)(Math.Pow(x, 2) + Math.Pow(y, 2));
            float d = (float)-Math.Sqrt( a + Math.Pow(z, 2) );
            a = (float)Math.Sqrt(a);
            float cos = z / d;
            float sin = a / d;
            cos = (float)Math.Sqrt( (1+cos)/2 );
            sin = sin / ( 2 * cos );
            return new Quaternion( sin * x / a , sin * -y / a , 0, cos );
        }
    }
    
    //黄経0黄緯0からの黄道座標系での回転した単位ベクトル
    public static Vector3 stdRotate(in float sita, in float phi){
        Vector3 v;
        float c1 = MathF.Cos(phi);
        v.X = c1 * MathF.Cos(sita);
        v.Y = c1 * MathF.Sin(sita);
        v.Z = MathF.Sin(phi);
        return v;


    }
    public static Vector3 QVQ(in Quaternion q, Vector3 v){
        Quaternion q2 = System.Numerics.Quaternion.Concatenate( q , new Quaternion(v.X, v.Y, v.Z, 0) );
        q2 = System.Numerics.Quaternion.Concatenate( q2, System.Numerics.Quaternion.Conjugate(q) );
        v.X = q2.X;
        v.Y = q2.Y;
        v.Z = q2.Z;
        return v;
    }
    //class
    /*
    public class Thruster{
        public Vector3 F ;
        public Vector3 FR; 
        public float f;
        //public float fr;
        public float rate = 0;
    }
    */
    public class Thruster{
        public Vector3 F;
        public Vector3 T;  //r*F
        public float rate = 0;
    }
    public class Rotate{
         public Vector3 v;
         public float s;
         public Rotate(float x, float y, float z, float w){
            v = new Vector3(x, y, z);
            s = w;
         }
     }
    public class Orbit{
        public double period = 1000 ;//公転周期
        public float  e      = 0    ;//離心率
        public double l  = 10000;//軌道長半径
        public float  epo    = 0    ;//SHT0でのM

        public float s0 = 0; //近日点黄経rad
        //0文字目がsin,cos 1文字目が，1：軌道傾斜角 2：昇交点赤経
        public float s1 = 0;
        public float s2 = 0;
        public float c1 = 1;
        public float c2 = 1;

        public int ci = 0; //主星
        public Orbit(double p, float e, double l, float epo, float s0, float s1, float s2, float c1, float c2, int ci){
            period = p;
            this.e = e;
            this.l = l;
            this.epo = epo;
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.c1 = c1;
            this.c2 = c2;
            this.ci = ci;
        }
        public Orbit(){
            period = 1000;
            e = 0;
            l = 10000;
            epo = 0;
            s0 = 0;
            s1 = 0;
            s2 = 0;
            c1 = 1;
            c2 = 1;
            ci = 0;
        }
    }
    public static Godot.Vector3 toGV(in Vector3 v){
        return new Godot.Vector3(v.X , v.Y , v.Z);
    }
    public static Vector3 toMV(in Godot.Vector3 v){
        return new Vector3(v.X , v.Y , v.Z);
    }
    public static double toRad(double deg){
        return deg * Math.PI / 180;
    }
    public static double toDeg(double rad){
        return rad * 180 / Math.PI;
    }
    public static float toRadF(float deg){
        return deg * MathF.PI / 180;
    }
    public static float toDegF(float rad){
        return rad * 180 / MathF.PI;
    }
}
