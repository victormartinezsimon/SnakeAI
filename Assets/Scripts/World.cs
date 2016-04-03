using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour
{

    public int m_size;
    
    public Color worldColor;
    public Color priceColor;

    public GameObject m_snake;

    private GameObject parent;
    private GameObject worldInstance;
    private Snake m_snakeInstance;
    private GameObject priceInstance;

    public GameObject m_canvas;
    public Text m_text;

    // Use this for initialization
    void Start()
    {
        init();
        buildWorld();
        Destroy(worldInstance);
        colocateCamera();
        instantiateSnake();
        instantiatePrice();
    }

    private void init()
    {
        Random.seed = (int)System.DateTime.Now.Ticks;
        parent = new GameObject("World");

        worldInstance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        worldInstance.GetComponent<Renderer>().material.shader = Shader.Find("Sprites/Default");
        worldInstance.GetComponent<Renderer>().material.color = worldColor;
       
        //isntantiate the price
        priceInstance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        priceInstance.name = "Price";
        priceInstance.GetComponent<Renderer>().material.shader = Shader.Find("Sprites/Default");
        priceInstance.GetComponent<Renderer>().material.color = priceColor;
        priceInstance.transform.parent = parent.transform;
    }

    private void buildWorld()
    {
        float posX = 0;
        float posY = 0;
        float posZ = 0;

        Vector3 cubeSize = worldInstance.GetComponent<Renderer>().bounds.size;

        //bottom part
        posX = 0;
        posY = 0;
        for (int i = 0; i < m_size; i++)
        {
            GameObject go = Instantiate(worldInstance, new Vector3(posX, posY, posZ), Quaternion.identity) as GameObject;
            go.transform.parent = parent.transform;
            posX += cubeSize.x;
        }

        //left part
        posX = 0;
        posY = cubeSize.y;
        for (int i = 0; i < m_size - 2; i++)
        {
            GameObject go = Instantiate(worldInstance, new Vector3(posX, posY, posZ), Quaternion.identity) as GameObject;
            go.transform.parent = parent.transform;
            posY += cubeSize.y  ;
        }

        //top part
        posX = 0;
        posY = cubeSize.y * (m_size - 1);
        for (int i = 0; i < m_size; i++)
        {
            GameObject go = Instantiate(worldInstance, new Vector3(posX, posY, posZ), Quaternion.identity) as GameObject;
            go.transform.parent = parent.transform;
            posX += cubeSize.x;
        }

        //right part
        posX = cubeSize.x * (m_size - 1);
        posY = cubeSize.y;
        for (int i = 0; i < m_size - 2; i++)
        {
            GameObject go = Instantiate(worldInstance, new Vector3(posX, posY, posZ), Quaternion.identity) as GameObject;
            go.transform.parent = parent.transform;
            posY += cubeSize.y;
        }

    }

    private void colocateCamera()
    {
        Vector3 cubeSize = worldInstance.GetComponent<Renderer>().bounds.size;
        float posX = ((m_size/2f) * cubeSize.x) - cubeSize.x/2f;
        float posY = ((m_size / 2f) * cubeSize.y) - cubeSize.y/2f;

        Camera.main.transform.position = new Vector3(posX, posY, -10);
        Camera.main.orthographicSize = m_size / 2 + 1;
    }

    private void instantiateSnake()
    {
        GameObject go = Instantiate(m_snake) as GameObject;
        m_snakeInstance = go.GetComponent<Snake>();
        m_snakeInstance.worldSize = m_size;
        m_snakeInstance.m_event += instantiatePrice;
        m_snakeInstance.m_gameOver += showPuntuaction;
        m_snakeInstance.init();

        IA m_ia = go.GetComponent<IA>();
        m_ia.m_size = m_size;
    }

    public void instantiatePrice()
    {
        List<GameObject> snakeBody = m_snakeInstance.getBody();
        Vector3 cubeSize = priceInstance.GetComponent<Renderer>().bounds.size;

        bool valid = false;
        float posX = 0;
        float posY = 0;

        while (!valid)
        {
            posX = Random.Range(1, m_size - 1) * cubeSize.x;
            posY = Random.Range(1, m_size - 1) * cubeSize.y;
            valid = true;
            for(int i = 0; i < snakeBody.Count; i++)
            {
                if(m_snakeInstance.samePosition(snakeBody[i].transform.position,new Vector3(posX, posY, 0)))
                {
                    valid = false;
                    break;
                }
            }
        }


        priceInstance.transform.position = new Vector3(posX, posY, 1);
        m_snakeInstance.pricePosition = priceInstance.transform.position;


    }

    public void priceEated()
    {
        instantiatePrice();
    }

    private void showPuntuaction(int puntuaction)
    {
        string message = "Total puntuaction => " + puntuaction + "\n From a maximun puntuaction of => " + ((m_size - 1) * (m_size - 1)).ToString();
        m_text.text = message;
        m_canvas.SetActive(true);
    }

    public void restartLevel()
    {
        SceneManager.LoadScene("game");
    }

}
