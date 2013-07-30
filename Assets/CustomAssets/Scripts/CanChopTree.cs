using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CanChopTree : MonoBehaviour
{
	public string ChoppedPrefabPostfix = "Chopped";
	public float MaximumDistance = 2F;
	
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
		if( Input.GetButtonDown("Fire1") )
		{
			TreeInstance[] trees = _terrainData.treeInstances;
			RaycastHit hit;
			
			Vector3 fwd = transform.TransformDirection(Vector3.forward);
			if( Physics.Raycast(transform.position, fwd, out hit, MaximumDistance) )
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
				int x = 0,
					y = 0;
				
				for(var i=0; i<_treeList.Count; i++)
				{
					var currentTree = trees[i];
					var currentTreeWorldPos = Vector3.Scale(currentTree.position, _terrainData.size) + _terrain.GetPosition();
					var distance = Vector3.Distance(currentTreeWorldPos, hit.point);
					
					if( distance < maxDistance )
					{
						maxDistance = distance;
						closestTreeIdx = i;
						closestTreePos = currentTreeWorldPos;
						closestTree = currentTree;
						x = (int) currentTree.position.x;
						y = (int) currentTree.position.y;
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
				
				float[,] heights = _terrainData.GetHeights(x, y, _terrainData.heightmapWidth, _terrainData.heightmapHeight);
				_terrainData.SetHeights(x, y, heights);
				
				Instantiate(prefab, closestTreePos, Quaternion.identity);
			}
		}
	}
}