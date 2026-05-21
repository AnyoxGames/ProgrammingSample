using System;
using System.Collections.Generic;
using AnyoxGames.Character;
using UnityEngine;

namespace AnyoxGames.Audio
{
    public class TerrainFoleyMaterial : MonoBehaviour, IFoleyCollider
    {
        [Serializable]
        public struct FoleyMaterialEntry
        {
            public TerrainLayer layer;
            public SoundBank walkingSoundBank;
            public SoundBank runningSoundBank;
        }
        
        [SerializeField] private Terrain terrain;
        [SerializeField] private FoleyMaterialEntry[] entries;
        [SerializeField] private float runSpeedThreshold = 10;

        private readonly Dictionary<TerrainLayer, FoleyMaterialEntry> cachedEntries = new();
        private readonly Dictionary<TerrainLayer, float> terrainMixture = new();
        
        private void Awake()
        {
            foreach (var entry in entries)
            {
                cachedEntries.Add(entry.layer, entry);
            }
        }

        public void RefreshTerrainMix(Vector3 position)
        {
            terrainMixture.Clear();
         
            float[,,] splatMapData = terrain.terrainData.GetAlphamaps(
                (int)((position.x - transform.position.x) / terrain.terrainData.size.x * terrain.terrainData.alphamapWidth), 
                (int)((position.z - transform.position.z) / terrain.terrainData.size.z * terrain.terrainData.alphamapHeight), 
                1, 1);

            for (int i = 0; i < splatMapData.Length; i++)
            {
                terrainMixture.Add(terrain.terrainData.terrainLayers[i], splatMapData[0, 0, i]);
            }
        }
        
        public void PlayFootstepSound(ACharacter character, float masterVolume)
        {
            RefreshTerrainMix(character.transform.position);
            
            var isRunning = character.TryGetCharacterBehaviour(out PlayerLocomotionBehaviour locomotionBehaviour) && locomotionBehaviour.Velocity.sqrMagnitude > runSpeedThreshold;
            
            foreach (var (layer, mix) in terrainMixture) if (cachedEntries.ContainsKey(layer) && mix > 0.1f)
            {
                Sound.PlayAtPosition(isRunning ? cachedEntries[layer].runningSoundBank.Next() : cachedEntries[layer].walkingSoundBank.Next(), character.transform.position, masterVolume * mix);
            }
        }
    }
}