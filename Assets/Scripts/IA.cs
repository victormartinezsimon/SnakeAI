using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IA : MonoBehaviour {

    public int m_worldSize;
    public Snake m_snake;

    public float weightObjective;
    public float weightFutureDeadNotSorted;
    public float weightFutureDeadSorted;

    public float m_percentDiferencesAreas = 0.3f; // we will use area 1 if area2 / area1 < 0.3f


    public Snake.Destinies getMovement()
    {
        float[] puntuaction = new float[4];//NORTH, WEST, SOUTH, EAST
        puntuaction = closeToObjective(puntuaction, weightObjective);//if we do this movement, we are more close to the objective
        puntuaction = futureDeadNotSort(puntuaction, weightFutureDeadNotSorted);
        puntuaction = futureDeadSort(puntuaction, weightFutureDeadSorted);

        puntuaction = closeToAutoEat(puntuaction);
        puntuaction = closeToDeadByWall(puntuaction);
        puntuaction = removeBadMovement(puntuaction);//remove imposible movements

        return calculateDestiny(puntuaction);
    }

    private float[] removeBadMovement(float[] puntuaction)
    {
        switch (m_snake.m_actualDestiny)
        {
            case Snake.Destinies.NORTH: puntuaction[2] = 0; break;
            case Snake.Destinies.SOUTH: puntuaction[0] = 0; break;
            case Snake.Destinies.EAST: puntuaction[3] = 0; break;
            case Snake.Destinies.WEST: puntuaction[1] = 0; break;
        }
        return puntuaction;
    }
    private float[] closeToObjective(float[] puntuaction, float weight)
    {
        Vector3 objective = m_snake.pricePosition;
        Vector3 size = m_snake.getSize();
        Vector3 head = m_snake.getBody()[0].transform.position;
        
        Vector3 north = head + new Vector3(0, 1, 0) * size.y;
        Vector3 east = head + new Vector3(1, 0, 0) * size.x;
        Vector3 south = head + new Vector3(0, -1, 0) * size.y;
        Vector3 west = head + new Vector3(-1, 0, 0) * size.x;

        float northDistance = (objective - north).magnitude;
        float southDistance = (objective - south).magnitude;
        float eastDistance = (objective - east).magnitude;
        float westDistance = (objective - west).magnitude;

        float max = Mathf.Max(Mathf.Max(northDistance, southDistance), Mathf.Max(eastDistance, westDistance));

        puntuaction[0] += (max - northDistance) * weight;
        puntuaction[1] += (max - eastDistance) * weight;
        puntuaction[2] += (max - southDistance) * weight;
        puntuaction[3] += (max - westDistance) * weight;

        return puntuaction;
    }
    private float[] closeToDeadByWall(float[] puntuaction)
    {
        Vector3 size = m_snake.getSize();
        Vector3 head = m_snake.getBody()[0].transform.position;

        Vector3 north = head + new Vector3(0, 1, 0) * size.y;
        Vector3 east = head + new Vector3(1, 0, 0) * size.x;
        Vector3 south = head + new Vector3(0, -1, 0) * size.y;
        Vector3 west = head + new Vector3(-1, 0, 0) * size.x;


        if(isBorder(north))
        {
            puntuaction[0] = 0;
        }
        if (isBorder(east))
        {
            puntuaction[1] = 0;
        }
        if (isBorder(south))
        {
            puntuaction[2] = 0;
        }
        if (isBorder(west))
        {
            puntuaction[3] = 0;
        }

        return puntuaction;
    }
    private float[] closeToAutoEat(float[] puntuaction)
    {
        Vector3 size = m_snake.getSize();
        Vector3 head = m_snake.getBody()[0].transform.position;

        Vector3 north = head + new Vector3(0, 1, 0) * size.y;
        Vector3 east = head + new Vector3(1, 0, 0) * size.x;
        Vector3 south = head + new Vector3(0, -1, 0) * size.y;
        Vector3 west = head + new Vector3(-1, 0, 0) * size.x;

        List<GameObject> list = m_snake.getBody();
        for(int i = 0; i < list.Count; i++)
        {
            if(m_snake.samePosition(north, list[i].transform.position))
            {
                puntuaction[0] = 0;
            }
            if (m_snake.samePosition(east, list[i].transform.position))
            {
                puntuaction[1] = 0;
            }
            if (m_snake.samePosition(south, list[i].transform.position))
            {
                puntuaction[2] = 0;
            }
            if (m_snake.samePosition(west, list[i].transform.position))
            {
                puntuaction[3] = 0;
            }
        }


        return puntuaction;
    }

    private float[] futureDeadNotSort(float[] puntuaction, float weight)
    {
        Vector3 size = m_snake.getSize();
        Vector3 head = m_snake.getBody()[0].transform.position;

        Vector3 north = head + new Vector3(0, 1, 0) * size.y;
        Vector3 east = head + new Vector3(1, 0, 0) * size.x;
        Vector3 south = head + new Vector3(0, -1, 0) * size.y;
        Vector3 west = head + new Vector3(-1, 0, 0) * size.x;

        List<Vector3> body = new List<Vector3>();
        List<GameObject> listGo = m_snake.getBody();

        for (int i = 0; i < listGo.Count - 1; i++)
        {
            body.Add(listGo[i].transform.position);
        }

        int puntNorth = getArea(north, body, size);
        int puntSouth = getArea(south, body, size);
        int puntEast = getArea(east, body, size);
        int puntWest = getArea(west, body, size);

        puntuaction[0] += puntNorth * weight;
        puntuaction[1] += puntEast * weight;
        puntuaction[2] += puntSouth * weight;
        puntuaction[3] += puntWest * weight;

        return puntuaction;
    }
    private float[] futureDeadSort(float[] puntuaction, float weight)
    {
        Vector3 size = m_snake.getSize();
        Vector3 head = m_snake.getBody()[0].transform.position;

        Vector3 north = head + new Vector3(0, 1, 0) * size.y;
        Vector3 east = head + new Vector3(1, 0, 0) * size.x;
        Vector3 south = head + new Vector3(0, -1, 0) * size.y;
        Vector3 west = head + new Vector3(-1, 0, 0) * size.x;

        List<Vector3> body = new List<Vector3>();
        List<GameObject> listGo = m_snake.getBody();

        for(int i = 0; i < listGo.Count - 1; i++)
        {
            body.Add(listGo[i].transform.position);
        }

        int puntNorth = getArea(north, body, size);
        int puntSouth = getArea(south, body, size);
        int puntEast = getArea(east, body, size);
        int puntWest = getArea(west, body, size);

        List<Vector2> list = new List<Vector2>();
        list.Add(new Vector2(0,puntNorth));
        list.Add(new Vector2(1, puntEast));
        list.Add(new Vector2(2, puntSouth));
        list.Add(new Vector2(3, puntWest));

        list.Sort(delegate (Vector2 p1, Vector2 p2) {
            if (p1.y == p2.y) return 0;
            if (p1.y < p2.y) return -1;
            return 1;
        });
        //here the list is sorted

        for(int i = 0; i < list.Count; i++)
        {
            puntuaction[(int)(list[i].x)] += i * weight;
        }

        return puntuaction;
    }

    private int getArea(Vector3 origin, List<Vector3> body, Vector3 size)
    {
        int areaBorder1 = getAreaWithBorderSize(origin, body, size, 1);
        int areaBorder2 = getAreaWithBorderSize(origin, body, size, 2);

        float percent = areaBorder1 * m_percentDiferencesAreas;

        if(areaBorder2 > (int)percent)
        {
            return areaBorder1;
        }
        else
        {
            return areaBorder2;
        }

    }

    private int getAreaWithBorderSize(Vector3 origin, List<Vector3> body, Vector3 size, int borderSize)
    {
        List<Vector3> listAnalyzed = new List<Vector3>();
        List<Vector3> listToAnalyze = new List<Vector3>() { origin };

        Vector3 bottomLeft = Vector3.zero;
        Vector3 topRight = new Vector3(size.x * m_worldSize, size.y * m_worldSize, 0);
        // take care for borders
        while (listToAnalyze.Count != 0 && listAnalyzed.Count < body.Count)
        {
            Vector3 actual = listToAnalyze[0];
            listToAnalyze.RemoveAt(0);
            listAnalyzed.Add(actual);

            if (!isBorder(actual) && !body.Contains(actual))
            {
                //calculate the next values
                Vector3 north = actual + new Vector3(0, 1, 0) * size.y;
                Vector3 east = actual + new Vector3(1, 0, 0) * size.x;
                Vector3 south = actual + new Vector3(0, -1, 0) * size.y;
                Vector3 west = actual + new Vector3(-1, 0, 0) * size.x;

                if(borderSize != 1 && isBorder(m_snake.pricePosition,2))
                {
                    borderSize = 1;//if the price is in the fake border, we put the border as 1
                }
                
                //north
                if (north.y < topRight.y && !listAnalyzed.Contains(north) && !listToAnalyze.Contains(north) && !body.Contains(north) && !isBorder(north, borderSize))
                {
                    listToAnalyze.Add(north);
                }
                //south
                if (south.y > bottomLeft.y && !listAnalyzed.Contains(south) && !listToAnalyze.Contains(south) && !body.Contains(south) && !isBorder(south, borderSize))
                {
                    listToAnalyze.Add(south);
                }
                //east
                if (east.x < topRight.x && !listAnalyzed.Contains(east) && !listToAnalyze.Contains(east) && !body.Contains(east) && !isBorder(east, borderSize))
                {
                    listToAnalyze.Add(east);
                }
                //west
                if (west.x > bottomLeft.x && !listAnalyzed.Contains(west) && !listToAnalyze.Contains(west) && !body.Contains(west) && !isBorder(west, borderSize))
                {
                    listToAnalyze.Add(west);
                }
            }

        }

        return listAnalyzed.Count - 1;//we add the first one, so we delete it
    }

    private bool isBorder(Vector3 pos, int borderSize = 1)
    {
        Vector3 size = m_snake.getSize();
        Vector3 bottomLeft = new Vector3(size.x * (borderSize - 1), size.y * (borderSize - 1), 0);
        Vector3 topRight = new Vector3(size.x * (m_worldSize - borderSize), size.y * (m_worldSize - borderSize), 0);

        if (pos.y >= topRight.y)
        {
            return true;
        }
        if (pos.x >= topRight.x)
        {
            return true;
        }
        if (pos.y <= bottomLeft.y)
        {
            return true;
        }
        if (pos.x <= bottomLeft.x)
        {
            return true;
        }

        return false;
    }

    private Snake.Destinies calculateDestinyRoulette(float[] puntuaction)
    {
        float acum = 0;
        for(int i = 0; i < puntuaction.Length; i++)
        {
            acum += puntuaction[i];
        }

        float random = Random.Range(0, acum);
        acum = -1;
        int selected = 0;
        for (int i = 0; i < puntuaction.Length; i++)
        {
            if(puntuaction[i] != 0)
            {
                float nextAcum = acum + puntuaction[i];
                if(nextAcum > random)
                {
                    selected = i;
                    break;
                }
                acum = nextAcum;
            }
        }

        switch (selected)
        {
            case 0: return Snake.Destinies.NORTH;
            case 1: return Snake.Destinies.EAST;
            case 2: return Snake.Destinies.SOUTH;
            case 3: return Snake.Destinies.WEST;
        }

        return m_snake.m_actualDestiny;
    }
    private Snake.Destinies calculateDestiny(float[] puntuaction)
    {
        int selected = 0;
        float max = puntuaction[0];

        for(int i = 1; i < puntuaction.Length; i++)
        {
            float val = puntuaction[i];
            if(val > max)
            {
                selected = i;
                max = val;
            }
            /*
            else
            {
                if(val == max)
                {
                    if(Random.value < 0.5f  )
                    {
                        selected = i;
                        max = val;
                    }
                }
            }
            */
        }

        switch (selected)
        {
            case 0: return Snake.Destinies.NORTH;
            case 1: return Snake.Destinies.EAST;
            case 2: return Snake.Destinies.SOUTH;
            case 3: return Snake.Destinies.WEST;
        }

        return m_snake.m_actualDestiny;

    }
}
