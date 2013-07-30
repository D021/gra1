using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanChopTree : MonoBehaviour
{
	public string ChoppedPrefabPostfix = "Chopped";
	
	private Terrain _terrain;
	private TerrainData _terrainData;
	private TreeInstance[] _initTreeInstances;
	private List<TreeInstance> _treeList;
	
	void Start ()
	{
		_terrain = Terrain.activeTerrain;
		_terrainData = _terrain.terrainData;
		_initTreeInstances = (TreeInstance[])_terrainData.treeInstances.Clone();
		_treeList = new List<TreeInstance>(_terrainData.treeInstances);
	}
	
	void OnDestroy()
	{
		_terrainData.treeInstances = _initTreeInstances;
	}
	
	void Update ()
	{
		if( Input.GetMouseButtonDown(0) ) // LEFT CLICK
		{
			TreeInstance[] trees = _terrainData.treeInstances;
			RaycastHit hit;
			
			Vector3 fwd = transform.TransformDirection(Vector3.forward);
			if( Physics.Raycast(transform.position, fwd, out hit, 10F) )
			{
				if( hit.collider.name != _terrain.name )
				{
					return;
				}
				
				
				if( _terrain.SampleHeight(hit.point) <= -0.01 )
				{
					return;
				}
				
				var maxDistance = float.MaxValue;
				var closestTreePos = Vector3.zero;
				var closestTreeIdx = 0;
				TreeInstance closestTree;
				
				for(var i=0; i<_treeList.Count; i++)
				{
					var currentTree = trees[i];
					var currentTreeWorldPos = Vector3.Scale(currentTree.position, _terrainData.size);
					var distance = Vector3.Distance(currentTreeWorldPos, hit.point);
					
					if( distance < maxDistance )
					{
						maxDistance = distance;
						closestTreeIdx = i;
						closestTreePos = currentTreeWorldPos;
						closestTree = currentTree;
					}
				}
				
				if( 0 == closestTreeIdx )
				{
					return;
				}
				
				var treePrefab = _terrainData.treePrototypes[closestTree.prototypeIndex].prefab;
				var treePrefabName = treePrefab.name;
				var treePrefabChoppedName = treePrefabName+ChoppedPrefabPostfix;
				var prefab = Resources.Load("Terrain/Tree/"+treePrefabChoppedName+"/"+treePrefabChoppedName);
				
				if( null == prefab )
				{
					return;
				}
				
				_treeList.RemoveAt(closestTreeIdx);
				_terrainData.treeInstances = _treeList.ToArray();
				
				float[,] heights = _terrainData.GetHeights(0, 0, 0, 0);
                _terrainData.SetHeights(0, 0, heights);
				
				Instantiate(prefab, closestTreePos, Quaternion.identity);
			}
		}
	}
}