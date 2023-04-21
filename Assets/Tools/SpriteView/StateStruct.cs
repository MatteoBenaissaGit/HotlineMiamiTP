using System;
using System.Collections.Generic;
using UnityEngine;

namespace MatteoBenaissaLibrary.SpriteView
{
    [Serializable]
    public struct Animation
    {
        public string Name;
    
        [Range(0, 1)] 
        public float TimeBetweenFrames;
    
        public List<Sprite> SpriteSheet;
    }
}

