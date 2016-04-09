using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Snake : MonoBehaviour {

    public int worldSize;

    public Texture2D bodyTexture;
    public Texture2D headTexture;
    public enum Destinies { NORTH, SOUTH, WEST, EAST};
    public Destinies m_actualDestiny = Destinies.NORTH;
    private List<GameObject> m_body;
   
    public Vector3 pricePosition;

    private IA m_ia;

    int puntuacion = 0;

    #region callback to create another price
    public delegate void createPrice();
    public event createPrice m_event;
    #endregion

    #region callback to gameOver
    public delegate void gameOver(int puntuaction);
    public event gameOver m_gameOver;
    #endregion

    private bool dead = false;

    public bool automatic = false;

    public void init()
    {
        m_body = new List<GameObject>();
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //colocate in world
        Vector3 size = cube.GetComponent<Renderer>().bounds.size;
        float posX = worldSize / 2 * size.x;
        float posY = worldSize / 2 * size.y;

        increaseBody(posX, posY, cube);

        m_ia = GetComponent<IA>();
    }

    void Update()
    {
        Tick();
    }
    
    public void Tick()
    {
        if (dead) { return; }
        m_actualDestiny = m_ia.getMovement();
        moveSnake();
        dead = isDead();
        if(dead)
        {
           if(m_gameOver != null)
            {
                m_gameOver(puntuacion);
            }
        }
    }
    
    private void moveSnake()
    {
        for (int i = m_body.Count - 1; i > 0; i--)
        {
            m_body[i].transform.position = m_body[i - 1].transform.position;
        }
        moveHead();

        if (samePosition(m_body[0].transform.position, pricePosition))
        {
            increaseBody(-100, -100);//so he dead with this not happens
            //cal to have a new price
            if (m_event != null)
            {
                m_event();
            }
        }
    }

    public bool samePosition(Vector3 pos1, Vector3 pos2)
    {
        return (pos1.x == pos2.x && pos1.y == pos2.y);
    }

    private void increaseBody(float posX, float posY, GameObject go = null)
    {
        if(go == null)
        {
            go = Instantiate(m_body[0]);
            go.name = "body";
            go.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
            go.GetComponent<Renderer>().material.mainTexture = bodyTexture;
            puntuacion++;
        }
        else
        {
            go.name = "head";
            go.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
            go.GetComponent<Renderer>().material.mainTexture = headTexture;
        }
        //colocate in world
        go.transform.position = new Vector3(posX, posY, 0);
        go.transform.parent = this.gameObject.transform;
        //add to list
        m_body.Add(go);
    }

    private void moveHead()
    {
        Vector3 movement = Vector3.zero;
        switch(m_actualDestiny)
        {
            case Destinies.EAST: movement.x = 1; break;
            case Destinies.WEST: movement.x = -1; break;
            case Destinies.NORTH: movement.y = 1; break;
            case Destinies.SOUTH: movement.y = -1; break;
        }
        m_body[0].transform.position += movement * m_body[0].GetComponent<Renderer>().bounds.size.x;
    }

    public List<GameObject> getBody()
    {
        return m_body;
    }
  
    bool isDead()
    {
        //check if the head is in any limit
        Vector3 size = m_body[0].GetComponent<Renderer>().bounds.size;
        Vector3 bottomLeft = new Vector3(0, 0, 0);
        Vector3 topRight = new Vector3(size.x,size.y, 0) * (worldSize-1);

        float posX = m_body[0].transform.position.x;
        float posY = m_body[0].transform.position.y;

        if(posX == bottomLeft.x || posX == topRight.x || posY == bottomLeft.y || posY == topRight.y)
        {
            return true;
        }

        //check if the head hits any part of the body
        for(int i = 2; i < m_body.Count; i++) // 2 will avoid head and price collision
        {
            if(samePosition(m_body[0].transform.position, m_body[i].transform.position))
            {
                return true;
            }
        }

        return false;
    }

    public Vector3 getSize()
    {
        return m_body[0].GetComponent<Renderer>().bounds.size; 
    }
}
