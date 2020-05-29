//WorldRecenter.cs by Azuline Studios© All Rights Reserved
//Orients all game objects to scene origin if player travels 
//beyond threshold to correct floating point precision loss in large scenes 
//(unused in current version, but included for reference).
using UnityEngine;
using System.Collections;

public class WorldRecenter : MonoBehaviour {
	private Object[] objects;
	[Tooltip("Re-center objects if player moves further than this distance from scene origin (prevents floating point imprecision and model jitter in large scenes, but currently incompatible with navmesh and static objects).")]
	public float threshold = 700.0f;
	[Tooltip("Refresh terrain to update tree colliders (can cause momentary hiccup on large terrains).")]
	public bool refreshTerrain = true;
	[HideInInspector]
	public float worldRecenterTime = 0.0f;//most recent time of world recenter
	private ParticleSystem.Particle[] emitterParticles = new ParticleSystem.Particle[1];
	private int numParticlesAlive;
	
    void LateUpdate(){
        Vector3 cameraPosition = gameObject.transform.position;
        cameraPosition.y = 0.0f;
		//if we're beyond the recenter threshold, recenter objects to scene origin
        if (cameraPosition.magnitude > threshold){
			worldRecenterTime = Time.time;//update time of world recenter
			if(worldRecenterTime + (0.2f * Time.timeScale) > Time.time){
				//recenter objects
	            objects = FindObjectsOfType(typeof(Transform));
	            foreach(Object o in objects){
	                Transform t = (Transform)o;
	                if (t.parent == null && t.gameObject.layer != 14){//don't change position of GUI elements which need to stay at scene origin 0,0,0
	                    t.position -= cameraPosition;
	                }
	            }
				//recenter particles and particle emitters
				objects = FindObjectsOfType(typeof(ParticleSystem));
	            foreach (Object o in objects)
	            {
					ParticleSystem pe = (ParticleSystem)o;
					numParticlesAlive = pe.GetParticles(emitterParticles);
					for(int i = 0; i < numParticlesAlive; ++i)
	                {
	                    emitterParticles[i].position -= cameraPosition;
	                }
					pe.SetParticles(emitterParticles, numParticlesAlive);
	            }
				
				//Refresh terrain to update tree colliders (can cause momentary hiccup on large terrains)
				if(refreshTerrain){
					if(Terrain.activeTerrain){
						TerrainData terrain = Terrain.activeTerrain.terrainData;
			            float[,] heights = terrain.GetHeights(0, 0, 0, 0);
			            terrain.SetHeights(0, 0, heights);
					}
				}
			}
		}
	}
}