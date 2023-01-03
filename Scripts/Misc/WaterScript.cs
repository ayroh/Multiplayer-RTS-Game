using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour {
    
    // Flows water under the classic map with material offset

    private Renderer waterMaterial;
    void Start() {
        waterMaterial = GetComponent<Renderer>();
    }

    void FixedUpdate() {
        waterMaterial.material.mainTextureOffset = new Vector2(waterMaterial.material.mainTextureOffset.x + 0.001f, 0f);
        if (waterMaterial.material.mainTextureOffset.x > 10f)
            waterMaterial.material.mainTextureOffset = new Vector2(0f, 0f);
    }
}
