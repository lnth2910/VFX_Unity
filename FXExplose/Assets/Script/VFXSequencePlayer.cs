using System.Collections;
using UnityEngine;

public class VFXSequencePlayer : MonoBehaviour
{
[Header("Cha chứa các vị trí spawn")]
    public Transform parentTransform;

[Header("Prefab VFX (Particle)")]
public GameObject vfxPrefab;

[Header("Thời gian delay giữa các spawn")]
public float delay = 1f;

[Header("Thời gian tồn tại FX trước khi hủy")]
public float lifeTime = 2f;

private bool isPlaying = false;

void Update()
{
    if (Input.GetKeyDown(KeyCode.Space) && !isPlaying)
    {
        StartCoroutine(PlaySequence());
    }
}

private IEnumerator PlaySequence()
{
    if (parentTransform == null || vfxPrefab == null) yield break;

    isPlaying = true;

    // Lặp qua tất cả child của parentTransform
    for (int i = 0; i < parentTransform.childCount; i++)
    {
        Transform child = parentTransform.GetChild(i);

        // Spawn VFX tại vị trí child
        GameObject vfx = Instantiate(vfxPrefab, child.position, child.rotation);

        // Hủy FX sau X giây (dù có particle hay không)
        Destroy(vfx, lifeTime);

        yield return new WaitForSeconds(delay);
    }

    isPlaying = false;
}
}

