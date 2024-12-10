using Godot;
using System;

public partial class CelCon : MeshInstance3D
{
	double d;
	readonly float D = 4.096F;//原点からの最大距離km

	Transform3D transform = Transform3D.Identity;
	Vector3 pos = new Vector3(0,0,0);
	Node3D[] cel = new Node3D[9];
	String[] celName = {"Sun","Mercury","Venus","Earth","Mars","Jupitor","Saturn","Neptune","Uranus","Pluto"};
	public override void _Ready()
	{
		for(int i=1; i<9; i++){
			cel[i] = (Node3D)GetNode($"/root/Main/{celName[i]}");
		}//nodeをget*/
		GD.Print($"{Main.positions[1,0]},{Main.positions[1,1]},{Main.positions[1,2]}");
		transform.Origin  = new Vector3(0,0,0);
		
	}
	
	public override void _Process(double deltaT)
	{
		
	}

	private void fixCelPosition(int ci){
		if(Main.positions[ci , 0] > 4.096 || Main.positions[ci , 1] > 4.096 || Main.positions[ci , 2] > 4.096){
			
			d = Main.positions[ci , 0] * Main.positions[ci , 0] + Main.positions[ci , 1] * Main.positions[ci , 1] + Main.positions[ci , 2] * Main.positions[ci , 2]; //原点からの距離
			d = Mathf.Sqrt(d);
			
			pos.X = (float)(Main.positions[ci ,0] / d) * D;//各成分の割合 * D
			pos.Y = (float)(Main.positions[ci ,1] / d) * D;
			pos.Z = (float)(Main.positions[ci ,2] / d) * D;
		}else{
			pos.X = (float)Main.positions[ci ,0];
			pos.Y = (float)Main.positions[ci ,1];
			pos.Z = (float)Main.positions[ci ,2];
		}
		
		transform.Origin = pos;
		
	}
}
