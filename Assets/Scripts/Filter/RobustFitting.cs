using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
public class RobustFitting
{
    private List<float> X = new List<float>();
    private List<float> Y = new List<float>();
    private List<float> Z = new List<float>();
    private List<float> mX = new List<float>();
    private List<float> mY = new List<float>();
    private List<float> mZ = new List<float>();
    private List<float> fX = new List<float>();
    private List<float> fY = new List<float>();
    private List<float> fZ = new List<float>();
    private int bufferSize = 100;
    private Vector3 prevVector = new Vector3();

    public RobustFitting(int bs)
    {
        Init(bs);
    }

    public void Init(int bs)
    {
        if (bufferSize % 2 == 0)
        {
            bufferSize = bs + 1;
        }
        else
        {
            bufferSize = bs;
        }
        bufferSize = 31;
        X.Clear();
        Y.Clear();
        Z.Clear();
        mX.Clear();
        mY.Clear();
        mZ.Clear();
        fX.Clear();
        fY.Clear();
        fZ.Clear();
    }

    private float Add(List<float> list, List<float> mlist, List<float> flist, float m, float f, float p)
    {
        var r = m - f;
        list.Add(r);
        mlist.Add(m);
        flist.Add(f);
        if (list.Count > bufferSize)
        {
            list.RemoveAt(0);
            mlist.RemoveAt(0);
            flist.RemoveAt(0);

            return calc(list, mlist, flist, m, f, p);
        }
        else
        {
            return f;
        }
    }

    private float calc(List<float> list, List<float> mlist, List<float> flist, float m, float f, float p)
    {
        for (var j = 0; j < 5; j++)
        {
            var sorted = list.OrderBy(a => a).ToList<float>();
            var mid = sorted[bufferSize / 2];
            for (var i = 0; i < bufferSize; i++)
            {
                sorted[i] = Mathf.Abs(sorted[i] - mid);
            }
            var sorted2 = sorted.OrderBy(a => a).ToList<float>();
            var MAD = sorted2[bufferSize / 2] * 6f;
            if(MAD == 0)
            {
                break;
            }
            for (var i = 1; i < bufferSize; i++)
            {
                if (MAD < list[i])
                {
                    if (i == 0)
                    {
                        flist[i] = flist[i + 1];
                    }
                    else if (i == bufferSize - 1)
                    {
                        flist[i] = flist[i - 1];
                    }
                    else
                    {
                        flist[i] = (flist[i - 1] + flist[i + 1]) / 2f;
                    }
                }
                else
                {
                    var k = Mathf.Pow(1f - Mathf.Pow(list[i] / MAD, 2f), 2f);
                    flist[i] = flist[i - 1] * (1 - k) + mlist[i] * k;
                }
                list[i] = mlist[i] - flist[i];
            }
        }

        return flist[bufferSize - 1];
    }

    public (float, float, float) Add(Vector3 measuredValue, Vector3 filterdValue)
    {
        var x = Add(X, mX, fX, measuredValue.x, filterdValue.x, prevVector.x);
        var y = Add(Y, mY, fY, measuredValue.y, filterdValue.y, prevVector.y);
        var z = Add(Z, mZ, fZ, measuredValue.z, filterdValue.z, prevVector.z);
        prevVector = filterdValue;

        return (x, y, z);
    }
}


*/
public class RobustFitting 
{
    private List<float> X = new List<float>();
    private List<float> Y = new List<float>();
    private List<float> Z = new List<float>();
    private List<float> mX = new List<float>();
    private List<float> mY = new List<float>();
    private List<float> mZ = new List<float>();
    private List<float> fX = new List<float>();
    private List<float> fY = new List<float>();
    private List<float> fZ = new List<float>();
    private int bufferSize = 100;
    private Vector3 prevVector = new Vector3();

    private List<float> V = new List<float>();
    private float prevMag = 0f;

    public RobustFitting(int bs)
    {
        Init(bs);
    }

    public void Init(int bs)
    {
        if (bufferSize % 2 == 0)
        {
            bufferSize = bs + 1;
        }
        else
        {
            bufferSize = bs;
        }
        X.Clear();
        Y.Clear();
        Z.Clear();
    }

    private float Add(List<float> list, float m, float f, float p)
    {
        var r = m - f;
        list.Add(r);
        if (list.Count > bufferSize)
        {
            list.RemoveAt(0);

            return calc( list,  m,  f,  p);
        }
        else
        {
            return f;
        }
    }

    private float calc(List<float> list, float m, float f, float p)
    {
        var v = f;
        //for(var j = 0; j < 5; j++)
        {
            var r = m - v;
            list[bufferSize - 1] = r;

            var sorted = list.OrderBy(a => a).ToList<float>();
            var mid = sorted[bufferSize / 2];
            for (var i = 0; i < bufferSize; i++)
            {
                sorted[i] = Mathf.Abs(sorted[i] - mid);
            }
            var sorted2 = sorted.OrderBy(a => a).ToList<float>();
            var MAD = sorted2[bufferSize / 2] * 6f;

            if (MAD < r || MAD == 0)
            {
                list[bufferSize - 1] = list[bufferSize - 2];
                return p;
            }
            else
            {
                //v = Mathf.Pow(1f - Mathf.Pow(r / MAD, 2f), 2f) * v;
                //list[bufferSize - 1] = m - v;
                return m;
            }
        }

        return (v + f) / 2f;
    }

    public (float, float, float) Add(Vector3 measuredValue, Vector3 filterdValue)
    {
        var x = Add(X, measuredValue.x, filterdValue.x, prevVector.x);
        var y = Add(Y, measuredValue.y, filterdValue.y, prevVector.y);
        var z = Add(Z, measuredValue.z, filterdValue.z, prevVector.z);
        prevVector = filterdValue;

        return (x, y, z);
    }

    private bool Add(List<float> list, float magnitude)
    {
        list.Add(magnitude);
        if (list.Count > bufferSize)
        {
            list.RemoveAt(0);

            var av = list.Average();
            var s2 = 0f;
            for (var i = 0; i < bufferSize; i++)
            {
                s2 += Mathf.Pow(list[i] - av, 2f);
            }
            var s = Mathf.Sqrt(s2 / bufferSize);
            if (av + 2f * s > magnitude)
            {
                prevMag = magnitude;
                return true;
            }
            else
            {
                list[bufferSize - 1] = magnitude / 2f;
                return false;
            }
        }
        else
        {
            prevMag = magnitude;
            return true;
        }
    }

    public bool Add(Vector3 vecNow3D)
    {
        return Add(V, vecNow3D.magnitude);
    }
}

