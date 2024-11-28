using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class HaltonSequence
{
    public List<int> HaltonSeq(int seq_size, int b, int m_size)
    {
        List<int> seq = new List<int>();
        int startPoint = UnityEngine.Random.Range(0, int.MaxValue - seq_size);
        
        for (int i = startPoint; i < startPoint + seq_size; i ++)
        {
            seq.Add(Mathf.RoundToInt((float)(2+(vdc(i, b) * (m_size-4)))));
        }
        Debug.Log("Halton Sequence Size: " + seq.Count);
        return seq;
    }

    double vdc(int n, int b)
    {
        double q = 0, bk = (double)1 / b;

        while (n > 0)
        {
            q += (n % b) * bk;
            n /= b;
            bk /= b;
        }

        return q;
    }
}
