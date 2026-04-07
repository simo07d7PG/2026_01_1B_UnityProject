using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public CubeGenerator[] generatedCubes = new CubeGenerator[5];
    public float timer    = 0.0f;
    public float interval = 3.0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            RandomizeCubeActivition();
            timer = 0.0f;
        }
    }

    public void RandomizeCubeActivition()
    {
        for(int i = 0; i < generatedCubes.Length; i++)
        {
            int randomNum = Random.Range(0, 2);
            if(randomNum == 1)
            {
                generatedCubes[i].GenCube();
            }
        }
    }
}
