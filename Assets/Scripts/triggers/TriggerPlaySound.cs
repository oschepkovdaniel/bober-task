using UnityEngine;

public class TriggerPlaySound : MonoBehaviour
{

    [Header("Params")]
    [SerializeField] private bool bFireOnce;

    [Header("Audio")]
    [SerializeField] private AudioSource TargetAudio;

    private bool bFired;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bFired = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (bFired && bFireOnce)
            return;

        if (other.gameObject.tag.Equals("Player"))
        {
            TargetAudio.Play();
            bFired = true;
        }
    }
}
