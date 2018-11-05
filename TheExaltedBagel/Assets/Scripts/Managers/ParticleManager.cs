using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    [SerializeField] float checkAliveInterval = 0.5f;

    private List<GameObject> aliveParticles = new List<GameObject>();
    private Coroutine checkAliveRoutine;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        ParticleManager.instance = this;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void PlayParticleSystem(GameObject particleObject, Vector3 position)
    {
        if (particleObject != null)
        {
            PlayParticleSystem(particleObject, position, particleObject.transform.localEulerAngles);
        }
        else
        {
            print("Warning: Missing particle system.");
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void PlayParticleSystem(GameObject particleObject, Vector3 position, Vector3 eulerAngles)
    {
        if (particleObject != null)
        {
            GameObject newObject = Instantiate(particleObject, this.transform);
            newObject.transform.localPosition = position;
            newObject.transform.localEulerAngles = eulerAngles;

            ParticleSystem particleSystem = newObject.GetComponent<ParticleSystem>();
            particleSystem.Play();

            this.aliveParticles.Add(newObject);

            if (this.checkAliveRoutine == null)
            {
                this.checkAliveRoutine = StartCoroutine(CheckAlive());
            }
        }
        else
        {
            print("Warning: Missing particle system.");
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator CheckAlive()
    {
        while (this.aliveParticles.Count > 0)
        {
            yield return new WaitForSeconds(this.checkAliveInterval);
            for (int i = this.aliveParticles.Count - 1; i >= 0; --i)
            {
                ParticleSystem particleSystem = this.aliveParticles[i].GetComponent<ParticleSystem>();
                if (!particleSystem.IsAlive(true))
                {
                    Destroy(this.aliveParticles[i]);
                    this.aliveParticles.RemoveAt(i);
                }
            }
        }

        this.checkAliveRoutine = null;
    }
}
