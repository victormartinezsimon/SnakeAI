using UnityEngine;
using System.Collections;

public class Stats {

    #region singleton
    private static Stats m_instance;
    public static Stats getInstance()
    {
        if(m_instance == null)
        {
            m_instance = new Stats();
        }
        return m_instance;
    }
    #endregion

    #region variables
    private int m_actualSeed;
    private int m_bestSeed;
    private int m_actualScore;
    private int m_bestScore;
    #endregion

    #region setters
    public int Seed
    {
        set { m_actualSeed = value; }
    }

    public void increasePuntuaction()
    {
        m_actualScore++;
    }

    public void endGame()
    {
        if(m_actualScore > m_bestScore)
        {
            m_bestScore = m_actualScore;
            m_bestSeed = m_actualSeed;
        }
        m_actualScore = 0;
    }

    #endregion

    #region constructor
    public Stats()
    {
        m_bestScore = int.MinValue;
        m_actualScore = 0;
    }
    #endregion

    #region class methods
    public string getText()
    {
        return string.Concat(" Score=> ", m_actualScore.ToString(), 
                             ", Best => ",   m_bestScore.ToString(),
                             ", Best_Seed=> ",    m_bestSeed.ToString());
    }
    #endregion
}
