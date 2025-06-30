//using Godot;
using System;
using System.Numerics;

public partial class Canvas : Godot.ColorRect
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
    //位置x1,y1の(x2,y2)を画面端まで伸ばしたときに何倍になるかを計算する
    private float calMax(float x1, float y1, float x2, float y2){
        if(x2==-0)
            x2 = 0; //x2 == -0 のとき負の無限大に発散するのを防ぐため
        if(y2==-0)
            y2 = 0; //x2 == -0 のとき負の無限大に発散するのを防ぐため
        float k1,k2;
        if(0<=x2 && 0<=y2){
            return MathF.Min((Main.canvas.Size.Y-y1)/y2,(Main.canvas.Size.X-x1)/x2);
        }else if(x2<0 && 0<=y2){
            return MathF.Min((Main.canvas.Size.Y-y1)/y2,-x1/x2);
        }else if(x2<0 && y2<0){
            return MathF.Min(-y1/y2,-x1/x2);
        }else{
            return MathF.Min(-y1/y2,(Main.canvas.Size.X-x1)/x2);
        }

    }

    public override void _Draw()
    {
        /*
        for(int i=0; i<Main.orbits.Count; i++){
            //GD.Print(Main.orbits[i].e);
            if(Main.orbits[i].e == 0){
                //円
                float sita=0;
                Godot.Vector2 pos = new Godot.Vector2(
                        Main.orbits[i].l + Main.orbits[i].x, 
                        Main.orbits[i].l + Main.orbits[i].y);
                for(int j=0; j<Main.drawPoly; j++){
                    sita = 2 * MathF.PI * (j/Main.drawPoly);
                    DrawLine(new Godot.Vector2(Main.orbits[i].l * MathF.Cos(sita) + Main.orbits[i].x, Main.orbits[i].l * MathF.Cos(sita) + Main.orbits[i].y) , pos , new Godot.Color(1,1,1));
                    pos.X = Main.orbits[i].l * MathF.Cos(sita) + Main.orbits[i].x;
                    pos.Y = Main.orbits[i].l * MathF.Sin(sita) + Main.orbits[i].y;
                }
            }else if(Main.orbits[i].e == 1){
                //放物線
            }else{
                //楕円
                float r1;
                Godot.Vector2 pos1;        
                float r2 = Main.orbits[i].l / 
                        (1+ (Main.orbits[i].e));
                float sita=0;
                Godot.Vector2 pos2 = new Godot.Vector2(
                                r2 * MathF.Cos(Main.orbits[i].angl) + Main.orbits[i].x, 
                                r2 * MathF.Sin(Main.orbits[i].angl) + Main.orbits[i].y);
                for(int j=0; j<Main.drawPoly; j++){
                    sita = 2 * MathF.PI * j/Main.drawPoly;

                    r1 = Main.orbits[i].l / 
                        (1+ (Main.orbits[i].e * MathF.Cos(sita)));
                    pos1.X = r1 * MathF.Cos(sita + Main.orbits[i].angl) + Main.orbits[i].x;
                    pos1.Y = r1 * MathF.Sin(sita + Main.orbits[i].angl) + Main.orbits[i].y;
                    //GD.Print(r1);
                    //GD.Print(r2);
                    DrawLine(pos2, pos1 , new Godot.Color(1,1,1));
                    r2 = Main.orbits[i].l / 
                        (1+ (Main.orbits[i].e * MathF.Cos(sita)));
                    pos2.X = r2 * MathF.Cos(sita + Main.orbits[i].angl) + Main.orbits[i].x;
                    pos2.Y = r2 * MathF.Sin(sita + Main.orbits[i].angl) + Main.orbits[i].y;
                    

                }
            }
        }
        */

        float s0 = MathF.Sin(Main.camAngl[1, 0]);
        float c0 = MathF.Cos(Main.camAngl[1, 0]);
        float s1 = MathF.Sin(Main.camAngl[1, 1]);
        float c1 = MathF.Cos(Main.camAngl[1, 1]);
        Godot.Vector2 pos;
        Godot.Vector2 del = new Godot.Vector2();





        // add points and orbits  0:probe 1:sun 2:mercury 3:...
        Main.points.Clear();

        pos = Main.paraPro(  //probe
                (float)Main.vpositions[0, 0] * Main.Inputs.scale[1],
                (float)Main.vpositions[0, 1] * Main.Inputs.scale[1],
                (float)Main.vpositions[0, 2] * Main.Inputs.scale[1],
                in s0,in s1,in c0,in c1);
        Main.points.Add(pos);

        pos = new Godot.Vector2(0,0); //sun
        Main.points.Add(pos);
        for(int ci=1; ci<Main.maxCI; ci++){
            Main.calOrbiterPosition(ci , Main.VSHT , true);
            pos = Main.paraPro(
                    (float)Main.vpositions[ci, 0] * Main.Inputs.scale[1],
                    (float)Main.vpositions[ci, 1] * Main.Inputs.scale[1],
                    (float)Main.vpositions[ci, 2] * Main.Inputs.scale[1],
                    in s0,in s1,in c0,in c1);
            pos = pos + del;
            Main.points.Add(pos);
        }
        Main.orbits.Clear();
        Main.orbits.Add(Main.solveOrbit(
                    new Vector3((float)Main.Probe.velo[0],(float)Main.Probe.velo[1],(float)Main.Probe.velo[2]) , 
                    0, 
                    new Vector3((float)Main.positions[0,0],(float)Main.positions[0,1],(float)Main.positions[0,2]),
                    Math.Sqrt(Math.Pow(Main.Probe.velo[0],2)+Math.Pow(Main.Probe.velo[1],2)+Math.Pow(Main.Probe.velo[2],2)),
                    Math.Sqrt(Math.Pow(Main.positions[0,0],2)+Math.Pow(Main.positions[0,1],2)+Math.Pow(Main.positions[0,2],2))));
        
        //del調整
        del = Main.Inputs.camDiff - Main.points[Main.viewCenter]; //delは太陽の画面上の位置

        //draw grid and axis
        float a = 15; //やじるしのながさ
        float b = 6; //やじるしのはばの半分
        for(int i=0; i<3; i++){
            Godot.Color col;
            if(i==0){
                pos = Main.paraPro(1000,0,0,in s0,in s1,in c0,in c1);
                col = new Godot.Color(1,0,0);
            }else if(i==1){
                pos = Main.paraPro(0,1000,0,in s0,in s1,in c0,in c1);
                col = new Godot.Color(0,1,0);
            }else{
                pos = Main.paraPro(0,0,1000,in s0,in s1,in c0,in c1);
                col = new Godot.Color(0,0,1);
            }
            float k1 = calMax(Main.Inputs.camDiff.X,Main.Inputs.camDiff.Y,pos.X,pos.Y);
            float k2 = calMax(Main.Inputs.camDiff.X,Main.Inputs.camDiff.Y,-pos.X,-pos.Y);
            DrawLine(pos*k1 + Main.Inputs.camDiff, -pos*k2 + Main.Inputs.camDiff , col);
            float len = pos.Length();
            Godot.Vector2[] arrow = new Godot.Vector2[]{
                new Godot.Vector2(pos.X*k1 + Main.Inputs.camDiff.X , pos.Y*k1 + Main.Inputs.camDiff.Y),
                new Godot.Vector2(pos.X*(k1 - a/len) - pos.Y*b/len + Main.Inputs.camDiff.X  ,  pos.Y*(k1 - a/len) + pos.X*b/len + Main.Inputs.camDiff.Y),
                new Godot.Vector2(pos.X*(k1 - a/len) + pos.Y*b/len + Main.Inputs.camDiff.X  ,  pos.Y*(k1 - a/len) - pos.X*b/len + Main.Inputs.camDiff.Y)
            };
            DrawPolygon( arrow , new Godot.Color[]{col});
        }

        //draw other option
        pos = new Godot.Vector2(8,8);
        DrawLine(Main.Inputs.camDiff+pos, Main.Inputs.camDiff-pos , new Godot.Color(1,1,1));
        pos = new Godot.Vector2(-8,8);
        DrawLine(Main.Inputs.camDiff+pos, Main.Inputs.camDiff-pos , new Godot.Color(1,1,1));

        // draw orbits
        for(int ci=0; ci<Main.maxCI; ci++){
            float l = (float)Main.CelE.o[ci].l * Main.Inputs.scale[1];
            float r = l / (1 + (Main.CelE.o[ci].e));
            Vector3 tmp = new Vector3(
                    r * MathF.Cos(Main.CelE.o[ci].s0),
                    r * MathF.Sin(Main.CelE.o[ci].s0),
                    0
                    );
            Godot.Vector2 pos1;
            pos1.X = (tmp.X * Main.CelE.o[ci].c2) - (tmp.Y * Main.CelE.o[ci].c1 * Main.CelE.o[ci].s2);
            pos1.Y = (tmp.X * Main.CelE.o[ci].s2) + (tmp.Y * Main.CelE.o[ci].c1 * Main.CelE.o[ci].c2);
            tmp.Z = tmp.Y * Main.CelE.o[ci].s1;
            pos1 = Main.paraPro(pos1.X, pos1.Y, tmp.Z, in s0, in s1, in c0, in c1);
            pos1.X += del.X; pos1.Y += del.Y + 10;
            Godot.Vector2 pos2;
            float sita;
            for(int j=0; j<=Main.drawPoly; j++){
                sita = 2 * MathF.PI * j/Main.drawPoly;

                r = l / (1 + (Main.CelE.o[ci].e * MathF.Cos(sita)));
                tmp = new Vector3(
                        r * MathF.Cos(sita + Main.CelE.o[ci].s0),
                        r * MathF.Sin(sita + Main.CelE.o[ci].s0),
                        0
                        );
                pos2.X = (tmp.X * Main.CelE.o[ci].c2) - (tmp.Y * Main.CelE.o[ci].c1 * Main.CelE.o[ci].s2);
                pos2.Y = (tmp.X * Main.CelE.o[ci].s2) + (tmp.Y * Main.CelE.o[ci].c1 * Main.CelE.o[ci].c2);
                tmp.Z = tmp.Y * Main.CelE.o[ci].s1;
                pos2 = Main.paraPro(pos2.X, pos2.Y, tmp.Z, in s0, in s1, in c0, in c1);
                pos2 = pos2 + del;
                DrawLine(pos1, pos2 , new Godot.Color(1,1,1));
                pos1 = pos2;
            }
        }
        for(int i=0; i<Main.orbits.Count; i++){
            float l = (float)Main.orbits[i].l * Main.Inputs.scale[1];
            float r = l / (1 + (Main.orbits[i].e));
            Vector3 tmp = new Vector3(
                    r * MathF.Cos(Main.orbits[i].s0),
                    r * MathF.Sin(Main.orbits[i].s0),
                    0
                    );
            Godot.Vector2 pos1;
            pos1.X = (tmp.X * Main.orbits[i].c2) - (tmp.Y * Main.orbits[i].c1 * Main.orbits[i].s2);
            pos1.Y = (tmp.X * Main.orbits[i].s2) + (tmp.Y * Main.orbits[i].c1 * Main.orbits[i].c2);
            tmp.Z = tmp.Y * Main.orbits[i].s1;
            pos1 = Main.paraPro(pos1.X, pos1.Y, tmp.Z, in s0, in s1, in c0, in c1);
            pos1.X += del.X; pos1.Y += del.Y + 10;
            Godot.Vector2 pos2;
            float sita;
            for(int j=0; j<=Main.drawPoly; j++){
                sita = 2 * MathF.PI * j/Main.drawPoly;
                r = l / (1 + (Main.orbits[i].e * MathF.Cos(sita)));
                tmp = new Vector3(
                        r * MathF.Cos(sita + Main.orbits[i].s0),
                        r * MathF.Sin(sita + Main.orbits[i].s0),
                        0
                        );
                pos2.X = (tmp.X * Main.orbits[i].c2) - (tmp.Y * Main.orbits[i].c1 * Main.orbits[i].s2);
                pos2.Y = (tmp.X * Main.orbits[i].s2) + (tmp.Y * Main.orbits[i].c1 * Main.orbits[i].c2);
                tmp.Z = tmp.Y * Main.orbits[i].s1;
                pos2 = Main.paraPro(pos2.X, pos2.Y, tmp.Z, in s0, in s1, in c0, in c1);
                pos2 += del;

                sita = 2 * MathF.PI * (j+1)/Main.drawPoly;
                r = l / (1 + (Main.orbits[i].e * MathF.Cos(sita)));
                if(r < 0){
                  //双曲線の描画時によけいな線をえがかないため
                    pos2 = pos1 + calMax(pos1.X,pos1.Y,pos2.X - pos1.X,pos2.Y - pos1.Y) * (pos2 - pos1);

                    DrawLine(pos1, pos2 , new Godot.Color(1,1,1)); //発散させる線1

                    j = Main.drawPoly - j;

                    sita = 2 * MathF.PI * (j+1)/Main.drawPoly;
                    r = l / (1 + (Main.orbits[i].e * MathF.Cos(sita)));
                    tmp = new Vector3(
                            r * MathF.Cos(sita + Main.orbits[i].s0),
                            r * MathF.Sin(sita + Main.orbits[i].s0),
                            0
                            );
                    pos1.X = (tmp.X * Main.orbits[i].c2) - (tmp.Y * Main.orbits[i].c1 * Main.orbits[i].s2);
                    pos1.Y = (tmp.X * Main.orbits[i].s2) + (tmp.Y * Main.orbits[i].c1 * Main.orbits[i].c2);
                    tmp.Z = tmp.Y * Main.orbits[i].s1;
                    pos1 = Main.paraPro(pos1.X, pos1.Y, tmp.Z, in s0, in s1, in c0, in c1);
                    pos1 += del;
                    
                    j++;

                    sita = 2 * MathF.PI * (j+1)/Main.drawPoly;
                    r = l / (1 + (Main.orbits[i].e * MathF.Cos(sita)));
                    tmp = new Vector3(
                            r * MathF.Cos(sita + Main.orbits[i].s0),
                            r * MathF.Sin(sita + Main.orbits[i].s0),
                            0
                            );
                    pos2.X = (tmp.X * Main.orbits[i].c2) - (tmp.Y * Main.orbits[i].c1 * Main.orbits[i].s2);
                    pos2.Y = (tmp.X * Main.orbits[i].s2) + (tmp.Y * Main.orbits[i].c1 * Main.orbits[i].c2);
                    tmp.Z = tmp.Y * Main.orbits[i].s1;
                    pos2 = Main.paraPro(pos2.X, pos2.Y, tmp.Z, in s0, in s1, in c0, in c1);
                    pos2 += del;
                    pos1 = pos2 + calMax(pos2.X,pos2.Y,pos2.X - pos2.X,pos1.Y - pos2.Y) * (pos1 - pos2);

                }else{
                }
                DrawLine(pos1, pos2 , new Godot.Color(1,1,1));
                pos1 = pos2;
            }
        }

        //draw directions(vector)
        float transScale = Main.Probe.node.Transform.Basis.Z.Length()*0.06F;
        Godot.Vector2 pos3 = Main.paraPro(Main.Probe.node.Transform.Basis.Z.X, Main.Probe.node.Transform.Basis.Z.Y, Main.Probe.node.Transform.Basis.Z.Z, in s0, in s1, in c0, in c1);
        DrawLine(Main.points[0]+del, Main.points[0] + del + pos3/transScale , new Godot.Color(1,1,1));
        float length = 2F;
        pos3 = Main.paraPro((float)Main.Probe.velo[0], (float)Main.Probe.velo[1], (float)Main.Probe.velo[2], in s0, in s1, in c0, in c1);
        DrawLine(Main.points[0]+del, Main.points[0] + del + pos3*length , new Godot.Color(0,1,1));

        //draw points
        Godot.Color color;
        float rad = 1;
        for(int i=0; i<Main.points.Count; i++){
            if(i==0)
                color = new Godot.Color(1,1,0);
            else if(i==1)
                color = new Godot.Color(1,0,0);
            else if(i == Main.selecting)
                color = new Godot.Color(0,1,0);
            else
                color = new Godot.Color(1,1,1);
            if(i == Main.hovering)
                color = new Godot.Color(0,0,1);

            if( i==0 ){
                rad = Main.pointMinRad;
            }else{
                rad = Main.CelE.radius[i-1] * Main.Inputs.scale[1];
                if( rad < Main.pointMinRad )
                    rad = Main.pointMinRad;
            }
            DrawCircle(Main.points[i]+del, rad, color);
        }
        
        /*
        for(int i=0; i<Main.bSelecting.Count; i++){
            DrawCircle(Main.points[Main.bSelecting[i]], Main.pointMinRad, new Godot.Color(0.3F,0.3F,1));
        }
        */


        //
    }

}
