#pragma strict

public var FallingTreePrefab : Transform;

function Update ()
{
	if( Input.GetMouseButtonDown(0) ) // LEFT CLICK
	{
		//var terrain = Terrain.activeTerrain;
		//var trees = terrain.terrainData.treeInstances;
		var trees: TreeInstance[] = Terrain.activeTerrain.terrainData.treeInstances;
		
		var hit : RaycastHit;
		var fwd = transform.TransformDirection(Vector3.forward);
		if( Physics.Raycast (transform.position, fwd, hit, 10) )
		{
			if( hit.collider.name != Terrain.activeTerrain.name )
			{
				return;
			}
			
			/*
			var sampleHeight = Terrain.activeTerrain.SampleHeight(hit.point);
			if( hit.point.y <= sampleHeight + 0.01 )
			{
				return;
			}
			*/
			
			var terrain = Terrain.activeTerrain.terrainData;
			var treeInstances = terrain.treeInstances;
			
			var maxDistance = float.MaxValue;
			var closestTreePos = Vector3.zero;
			var closestTreeIdx = 0;
			
			for(var i=0; i<trees.length; i++)
			{
				var currentTree = trees[i];
				var currentTreeWorldPos = Vector3.Scale(currentTree.position, terrain.size);
				var distance = Vector3.Distance(currentTreeWorldPos, hit.point);
				
				if( distance < maxDistance )
				{
					maxDistance = distance;
					closestTreeIdx = i;
					closestTreePos = currentTreeWorldPos;
				}
			}
			
			if( 0 == closestTreeIdx )
			{
				return;
			}
			
			var tmp = new Array(trees);
			tmp.RemoveAt(closestTreeIdx);
			Terrain.activeTerrain.terrainData.treeInstances = tmp.ToBuiltin(TreeInstance) as TreeInstance[];
			
			var heights = terrain.GetHeights(0,0,0,0);
			terrain.SetHeights(0,0,heights);
			
			closestTreePos.y = Terrain.activeTerrain.SampleHeight(hit.point) -54.12927;
			
			GameObject.Instantiate(FallingTreePrefab, closestTreePos, Quaternion.identity);
		}
	}
}