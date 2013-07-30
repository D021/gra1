using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CanChopTree : MonoBehaviour
{
	public string ChoppedPrefabPostfix = "Chopped";
	public float MaximumDistance = 2F;
	public float ChopSeconds = 0.5F;
	public int ChopCount = 3;
	public bool Enabled = true;
	
	private Terrain _terrain;
	private TerrainData _terrainData;
	private TreeInstance[] _initTreeInstances;
	private List<TreeInstance> _treeList;
	private List<uint> _chopCounts;
	private float _lastChop = 0;
	
	void Start ()
	{
		_terrain = Terrain.activeTerrain;
		_terrainData = _terrain.terrainData;
		_initTreeInstances = (TreeInstance[])_terrainData.treeInstances.Clone();
		_treeList = new List<TreeInstance>(_terrainData.treeInstances);
		_chopCounts = new List<uint>();
		for( int i=0; i<_treeList.Count; i++ )
		{
			_chopCounts.Add(0);
		}
	}
	
	void OnDestroy()
	{
		_terrainData.treeInstances = _initTreeInstances;
	}
	
	void Update ()
	{
		if( CanChop && Input.GetButton("Fire1") )
		{
			if( TryChop() )
			{
				_lastChop = Time.time;
			}
		}
	}
	
	bool CanChop
	{
		get { return Enabled && (_lastChop + ChopSeconds < Time.time); }
	}
	
	/**
	 * @return bool chopped
	 */
	bool TryChop()
	{
		TreeInstance[] trees = _terrainData.treeInstances;
		RaycastHit hit;
		
		Vector3 fwd = transform.TransformDirection(Vector3.forward);
		if( Physics.Raycast(transform.position, fwd, out hit, MaximumDistance) )
		{
			if( !IsColliderGround(hit.collider) || PositionBelowGround(hit.point) )
			{
				return false;
			}
			
			Vector3 closestTreePos;
			int closestTreeIdx = -1;
			int closestTreePrototypeIdx = -1;
			int closestTreeTerrainPosX = 0,
				closestTreeTerrainPosY = 0;
			
			FindClosestTree(hit.point, trees, out closestTreePos, out closestTreeIdx, out closestTreePrototypeIdx, out closestTreeTerrainPosX, out closestTreeTerrainPosY);
			if( -1 == closestTreeIdx )
			{
				return false;
			}
			
			var prefab = GetChoppedPrefab(closestTreePrototypeIdx);
			if( null == prefab )
			{
				return false;
			}
			
			if( ShouldFall(closestTreeIdx) )
			{
				RemoveTreeAt(closestTreeIdx, closestTreeTerrainPosX, closestTreeTerrainPosY);
				Instantiate(prefab, closestTreePos, Quaternion.identity);
			}
			
			return true;
		}
		else
		{
			return false;
		}
	}
	
	bool ShouldFall(int treeIdx)
	{
		Debug.Log (_chopCounts[treeIdx] + " " + (_chopCounts[treeIdx] >= ChopCount));
		if( _chopCounts[treeIdx]++ >= ChopCount )
		{
			_chopCounts.RemoveAt (treeIdx);
			return true;
		}
		else
		{
			return false;
		}
	}
	
	bool IsColliderGround(Collider collider)
	{
		return collider.name == _terrain.name;
	}
	
	bool PositionBelowGround(Vector3 worldPosition)
	{
		return _terrain.SampleHeight(worldPosition) <= -0.01;
	}
	
	void FindClosestTree(Vector3 worldPos, TreeInstance[] trees, out Vector3 closestTreePos, out int closestTreeIdx,
		out int closestTreePrototypeIdx, out int closestTreeTerrainPosX, out int closestTreeTerrainPosY)
	{
		var maxDistance = float.MaxValue;
		closestTreePos = Vector3.zero;
		closestTreeIdx = closestTreePrototypeIdx = -1;
		closestTreeTerrainPosX = closestTreeTerrainPosY = 0;
		for(var i=0; i<_treeList.Count; i++)
		{
			var currentTree = trees[i];
			var currentTreeWorldPos = Vector3.Scale(currentTree.position, _terrainData.size) + _terrain.GetPosition();
			var distance = Vector3.Distance(currentTreeWorldPos, worldPos);
			
			if( distance < maxDistance )
			{
				maxDistance = distance;
				closestTreeIdx = i;
				closestTreePos = currentTreeWorldPos;
				closestTreePrototypeIdx = currentTree.prototypeIndex;
				closestTreeTerrainPosX = (int) currentTree.position.x;
				closestTreeTerrainPosY = (int) currentTree.position.y;
			}
		}
	}
	
	Object GetChoppedPrefab(int treePrototypeIdx)
	{
		if( treePrototypeIdx == -1 )
		{
			return null;
		}
		else
		{
			var treePrefab = _terrainData.treePrototypes[treePrototypeIdx].prefab;
			var treePrefabName = treePrefab.name;
			var treePrefabChoppedName = treePrefabName+ChoppedPrefabPostfix;
			return Resources.Load("Terrain/Tree/"+treePrefabChoppedName+"/"+treePrefabChoppedName);
		}
	}
	
	void RemoveTreeAt(int treeInstanceIdx, int treePosX, int treePosY)
	{
		_treeList.RemoveAt(treeInstanceIdx);
		_terrainData.treeInstances = _treeList.ToArray();
		
		float[,] heights = _terrainData.GetHeights(treePosX, treePosY, _terrainData.heightmapWidth, _terrainData.heightmapHeight);
		_terrainData.SetHeights(treePosX, treePosY, heights);
	}
}