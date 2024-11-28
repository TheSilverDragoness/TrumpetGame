using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    [SerializeField]
    private GameObject iconPrefab;
    List<marker> markers = new List<marker>();
    [SerializeField]
    private RawImage compassImg;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private float maxDistance = 50f;

    float compassUnit;

    private void Start()
    {
        compassUnit = compassImg.rectTransform.rect.width / 360;
    }

    public void AssignPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    private void Update()
    {
        compassImg.uvRect = new Rect(player.localEulerAngles.y / 360f, 0f, 1f, 1f);


        foreach (var marker in markers)
        {
            //if (marker == null)
            //{
            //    markers.Remove(marker);
            //    continue;
            //}

            marker.image.rectTransform.anchoredPosition = GetPosOnCompass(marker);

            //float dst = Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), marker.position);
            //float scale = 0f;

            //if (dst > maxDistance) 
            //{
            //    scale = 1f - (dst / maxDistance);
            //}

            //marker.image.rectTransform.localScale = Vector3.one * scale;
        }
    }

    public void AddQuestMarker(marker m) 
    {
        Debug.Log("Adding Marker");
        GameObject newMarker = Instantiate(iconPrefab, compassImg.transform);
        m.image = newMarker.GetComponent<Image>();
        m.image.sprite = m.icon;
        markers.Add(m);
    }

    public void RemoveMarker(marker m)
    {
        markers.Remove(m);
        Destroy(m.image.gameObject);
    }

    Vector2 GetPosOnCompass (marker m)
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 playerFwd = new Vector2(player.transform.forward.x, player.transform.forward.z);

        float angle = Vector2.SignedAngle(m.position - playerPos, playerFwd);

        return new Vector2(compassUnit * angle, 0f);
    }
}
