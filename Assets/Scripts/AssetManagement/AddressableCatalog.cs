using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SwordHero.AssetManagement
{
    [CreateAssetMenu(fileName = "AddressableCatalog", menuName = "Asset Management/Addressable Catalog")]
    public class AddressableCatalog : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public string id;
            public AssetReference reference;
            public bool loadOnStartup;
        }
        
        [SerializeField] private List<Entry> entries = new List<Entry>();

        public IReadOnlyList<Entry> Entries => entries;

        public Entry[] GetStartupEntries()
        {
            return entries.Where(e => e.loadOnStartup && e.reference != null).ToArray();
        }
        
        public AssetReference GetReferenceById(string id)
        {
            return entries.FirstOrDefault(e => e.id == id).reference;
        }
    }
}