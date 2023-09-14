using System.Collections;
using UnityEngine;
using Vuforia;

public class CustomAREffects : DefaultObserverEventHandler
{
    public GameObject crosshair;
    public GameObject grenadePrefab;
    public GameObject shieldPrefab;
    private GameObject instantiatedGrenade;
    private GameObject instantiatedShield;

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        crosshair.SetActive(true);
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
        crosshair.SetActive(false);
        RemoveShield();
        RemoveGrenade();
    }

    public void OnGrenadeButtonPressed()
    {
        RemoveGrenade(); // In case another grenade was already flying
        instantiatedGrenade = Instantiate(grenadePrefab, grenadePrefab.transform.position, grenadePrefab.transform.rotation);
        StartCoroutine(MoveGrenadeTowardsTarget(instantiatedGrenade));
    }

    public void OnShieldButtonPressed()
    {
        crosshair.SetActive(false);
        if (!instantiatedShield)
        {
            instantiatedShield = Instantiate(shieldPrefab, shieldPrefab.transform.position, shieldPrefab.transform.rotation);
            StartCoroutine(ShieldDuration());
        }
    }

    private IEnumerator MoveGrenadeTowardsTarget(GameObject grenade)
    {
        Vector3 startPosition = grenade.transform.position;
        Vector3 endPosition = this.transform.position;
        endPosition.y -= 5f; // Adjust this value to set how much below the image we want the grenade to land.

        float journeyDuration = 2f;
        float startTime = Time.time;

        Quaternion startRotation = grenade.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(90, 0, 0);

        Vector3 startScale = grenade.transform.localScale;
        Vector3 endScale = startScale * 0.1f;

        while ((Time.time - startTime) < journeyDuration)
        {
            float x = (Time.time - startTime) / journeyDuration;
            float y = -4 * x * x + 4 * x; // y = -4x^2 + 4x, our parabolic equation

            Vector3 horizontalComponent = Vector3.Lerp(startPosition, endPosition, x);
            float verticalOffset = (endPosition.y - startPosition.y) * y;
            Vector3 verticalComponent = new Vector3(0, verticalOffset, 0);

            grenade.transform.position = horizontalComponent + verticalComponent;
            grenade.transform.rotation = Quaternion.Lerp(startRotation, endRotation, x);
            grenade.transform.localScale = Vector3.Lerp(startScale, endScale, x);

            yield return null;
        }

        Destroy(grenade);
    }


    private IEnumerator ShieldDuration()
    {
        yield return new WaitForSeconds(2);
        RemoveShield();
        crosshair.SetActive(true);
    }

    private void RemoveGrenade()
    {
        if (instantiatedGrenade)
        {
            Destroy(instantiatedGrenade);
        }
    }

    private void RemoveShield()
    {
        if (instantiatedShield)
        {
            Destroy(instantiatedShield);
        }
    }
}

