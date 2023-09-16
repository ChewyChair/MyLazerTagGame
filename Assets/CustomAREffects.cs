using System.Collections;
using UnityEngine;
using Vuforia;

public class CustomAREffects : DefaultObserverEventHandler
{
    public bool isTargetVisible;
    public GameObject camera;
    public GameObject crosshair;
    public GameObject grenadePrefab;
    public GameObject shieldPrefab;
    public GameObject explosionPrefab;
    public GameObject spearPrefab;
    public ParticleSystem bloodSprayPs;
    private GameObject instantiatedGrenade;
    private GameObject instantiatedShield;
    private GameObject instantiatedExplosion;
    private GameObject instantiatedSpear;

    private GameObject instantiatedOpponentGrenade;
    private GameObject instantiatedOpponentExplosion;
    private GameObject instantiatedOpponentSpear;



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

    public void ShowBloodSpray() {
        bloodSprayPs.Play();
        StartCoroutine(EndSpray());
    }

    public IEnumerator EndSpray() {
        yield return new WaitForSeconds(0.1f);
        bloodSprayPs.Stop();
    }

    public void OnGrenadeButtonPressed()
    {
        RemoveGrenade(); // In case another grenade was already flying
        instantiatedGrenade = Instantiate(grenadePrefab, camera.transform.position, Quaternion.identity);
        instantiatedGrenade.transform.localScale = new Vector3(2f, 2f, 2f);
        StartCoroutine(MoveGrenadeTowardsTarget(instantiatedGrenade));
    }

    public void OnOpponentGrenadeButtonPressed()
    {
        RemoveOpponentGrenade(); // In case another grenade was already flying
        instantiatedOpponentGrenade = Instantiate(grenadePrefab, this.transform.position, Quaternion.identity);
        instantiatedOpponentGrenade.transform.localScale = new Vector3(2f, 2f, 2f);
        StartCoroutine(MoveGrenadeTowardsCamera(instantiatedOpponentGrenade));
    }

    public void OnShieldButtonPressed()
    {
        crosshair.SetActive(false);
        if (!instantiatedShield)
        {
            instantiatedShield = Instantiate(shieldPrefab, this.transform.position, Quaternion.identity);
            instantiatedShield.transform.position += new Vector3(0f, 0f, 1f);
            instantiatedShield.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            StartCoroutine(ShieldLookAtCamera(instantiatedShield));
            StartCoroutine(ShieldDuration());
        }
    }

    public void OnSpearButtonPressed()
    {
        RemoveSpear(); 
        Vector3 direction = this.transform.position - camera.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        instantiatedSpear = Instantiate(spearPrefab, camera.transform.position, rotation * Quaternion.Euler(70, 0, 0));
        instantiatedSpear.transform.localScale = new Vector3(2f, 2f, 2f);
        StartCoroutine(MoveSpearTowardsTarget(instantiatedSpear));
    }

    public void OnOpponentSpearButtonPressed()
    {
        RemoveOpponentSpear(); 
        Vector3 direction = camera.transform.position - this.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        instantiatedOpponentSpear = Instantiate(spearPrefab, this.transform.position, rotation * Quaternion.Euler(70, 0, 0));
        instantiatedOpponentSpear.transform.localScale = new Vector3(2f, 2f, 2f);
        StartCoroutine(MoveSpearTowardsCamera(instantiatedOpponentSpear));
    }

    private IEnumerator MoveGrenadeTowardsTarget(GameObject grenade)
    {
        Vector3 startPosition = grenade.transform.position;
        Vector3 endPosition = this.transform.position;
        endPosition.y -= 0f; // Adjust this value to set how much below the image we want the grenade to land.

        float journeyDuration = 2f;
        float startTime = Time.time;

        Quaternion startRotation = grenade.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(90, 0, 0);

        Vector3 hvel = Vector3.Lerp(startPosition, endPosition, 0.5f);
        float vvel = 2.0f;

        while ((Time.time - startTime) < journeyDuration)
        {
            float x = (Time.time - startTime) / journeyDuration;

            vvel *= 0.99f;
            vvel -= 0.05f;
      
            grenade.transform.position += hvel * Time.deltaTime;
            grenade.transform.position += new Vector3(0f, vvel * Time.deltaTime, 0f);
            grenade.transform.rotation = Quaternion.Lerp(startRotation, endRotation, x);

            yield return null;
        }

        instantiatedExplosion = Instantiate(explosionPrefab, grenade.transform.position, Quaternion.identity);
        instantiatedExplosion.transform.localScale = new Vector3(5f, 5f, 5f);
        StartCoroutine(explosionTimeout(instantiatedExplosion));
        Destroy(grenade);
    }

    private IEnumerator MoveGrenadeTowardsCamera(GameObject grenade)
    {
        Vector3 startPosition = grenade.transform.position;
        Vector3 endPosition = camera.transform.position;
        endPosition.y -= 0f; // Adjust this value to set how much below the image we want the grenade to land.

        float journeyDuration = 2f;
        float startTime = Time.time;

        Quaternion startRotation = grenade.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(90, 0, 0);

        Vector3 hvel = Vector3.Lerp(startPosition, endPosition, 0.5f);
        float vvel = 2.0f;

        while ((Time.time - startTime) < journeyDuration)
        {
            float x = (Time.time - startTime) / journeyDuration;

            vvel *= 0.99f;
            vvel -= 0.05f;
      
            grenade.transform.position -= hvel * Time.deltaTime;
            grenade.transform.position += new Vector3(0f, vvel * Time.deltaTime, 0f);
            grenade.transform.rotation = Quaternion.Lerp(startRotation, endRotation, x);

            yield return null;
        }

        instantiatedOpponentExplosion = Instantiate(explosionPrefab, grenade.transform.position, Quaternion.identity);
        instantiatedOpponentExplosion.transform.localScale = new Vector3(5f, 5f, 5f);
        StartCoroutine(explosionTimeout(instantiatedOpponentExplosion));
        Destroy(grenade);
    }
    
    private IEnumerator MoveSpearTowardsTarget(GameObject spear)
    {
        Vector3 startPosition = spear.transform.position;
        Vector3 endPosition = this.transform.position;
        endPosition.y += 1f; // Adjust this value to set how much below the image we want the grenade to land.

        float journeyDuration = 1f;
        float startTime = Time.time;

        Quaternion startRotation = spear.transform.rotation;
        Quaternion endRotation = spear.transform.rotation * Quaternion.Euler(40, 0, 0);

        Vector3 hvel = Vector3.Lerp(startPosition, endPosition, 1f);
        float vvel = 2.0f;

        while ((Time.time - startTime) < journeyDuration)
        {
            float x = (Time.time - startTime) / journeyDuration;

            vvel *= 0.99f;
            vvel -= 0.08f;
      
            spear.transform.position += hvel * Time.deltaTime;
            spear.transform.position += new Vector3(0f, vvel * Time.deltaTime, 0f);
            spear.transform.rotation = Quaternion.Lerp(startRotation, endRotation, x);
            spear.transform.GetChild(0).transform.rotation *= Quaternion.Euler(0, 180f * Time.deltaTime, 0); // SPIN

            yield return null;
        }

        Destroy(spear);
    }

    private IEnumerator MoveSpearTowardsCamera(GameObject spear)
    {
        Vector3 startPosition = spear.transform.position;
        Vector3 endPosition = camera.transform.position;
        endPosition.y += 1f; // Adjust this value to set how much below the image we want the grenade to land.

        float journeyDuration = 1f;
        float startTime = Time.time;

        Quaternion startRotation = spear.transform.rotation;
        Quaternion endRotation = spear.transform.rotation * Quaternion.Euler(40, 0, 0);

        Vector3 hvel = Vector3.Lerp(startPosition, endPosition, 1f);
        float vvel = 2.0f;

        while ((Time.time - startTime) < journeyDuration)
        {
            float x = (Time.time - startTime) / journeyDuration;

            vvel *= 0.99f;
            vvel -= 0.08f;
      
            spear.transform.position += hvel * Time.deltaTime;
            spear.transform.position += new Vector3(0f, vvel * Time.deltaTime, 0f);
            spear.transform.rotation = Quaternion.Lerp(startRotation, endRotation, x);
            spear.transform.GetChild(0).transform.rotation *= Quaternion.Euler(0, 180f * Time.deltaTime, 0); // SPIN

            yield return null;
        }

        Destroy(spear);
    }

    private IEnumerator explosionTimeout(GameObject explosion) {
        yield return new WaitForSeconds(1);
        RemoveExplosion(explosion);
    }

    private IEnumerator ShieldLookAtCamera(GameObject shield) {
        while (shield) {
            shield.transform.position = this.transform.position;
            shield.transform.LookAt(camera.transform.position);
            yield return null;
        }
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

    private void RemoveOpponentGrenade()
    {
        if (instantiatedOpponentGrenade)
        {
            Destroy(instantiatedOpponentGrenade);
        }
    }

    private void RemoveShield()
    {
        if (instantiatedShield)
        {
            Destroy(instantiatedShield);
        }
    }

    private void RemoveExplosion(GameObject explosion) {
        if (explosion) {
            Destroy(explosion);
        }
    }

    private void RemoveSpear()
    {
        if (instantiatedSpear)
        {
            Destroy(instantiatedSpear);
        }
    }

    private void RemoveOpponentSpear()
    {
        if (instantiatedOpponentSpear)
        {
            Destroy(instantiatedOpponentSpear);
        }
    }

}

