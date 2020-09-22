using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Filter
{
    private List<float> X = new List<float>();
    private List<float> Y = new List<float>();
    private List<float> Z = new List<float>();
    private int bufferSize = 300;
    private float CutOff = 2f;
    private int ErrorCnt = 0;

    public Filter(int bs, float cutOff)
    {
        Init(bs, cutOff);
    }

    public void Init(int bs, float cutOff)
    {
        bufferSize = bs;
        CutOff = cutOff;
        X.Clear();
        Y.Clear();
        Z.Clear();
        ErrorCnt = 0;
    }

    private float Add(List<float> list, float v, float samplingrate)
    {
        list.Add(v);
        if (list.Count > bufferSize)
        {
            list.RemoveAt(0);
            FittingData[] data_list = new FittingData[100];
            for(var i = 0; i < 100; i++)
            {
                data_list[i] = new FittingData(i, list[i]);

            }
            var rpf = new RobustPolynomialFitting(data_list, 3, true);
            var rpfv = rpf.ExecureFitting();
            return data_list[99].Y;
            //return Butterworth(list, samplingrate);
        }
        else
        {
            return v;
        }
    }
    public (float, float, float) Add(float x, float y, float z, float samplingrate)
    {
        if(CutOff == 0)
        {
            return (x, y, z);
        }
        if(ErrorCnt == 10)
        {
            return (x, y, z);
        }

        if (samplingrate < 15f)
        {
            ErrorCnt++;

            X.Add(x);
            Y.Add(y);
            Z.Add(z);
            if (X.Count > bufferSize)
            {
                X.RemoveAt(0);
                Y.RemoveAt(0);
                Z.RemoveAt(0);
            }
            return (x, y, z);
        }

        if (200 / samplingrate < CutOff)
        {
            CutOff = 200 / samplingrate;
        }

        return (Add(X, x, samplingrate), Add(Y, y, samplingrate), Add(Z, z, samplingrate));
    }

    public float AddX(float x, float samplingrate)
    {
        return Add(X, x, samplingrate);
    }

    public float AddY(float y, float samplingrate)
    {
        return Add(Y, y, samplingrate);
    }

    public float AddZ(float z, float samplingrate)
    {
        return Add(Z, z, samplingrate);
    }

    //--------------------------------------------------------------------------
    // This function returns the data filtered. Converted to C# 2 July 2014.
    // Original source written in VBA for Microsoft Excel, 2000 by Sam Van
    // Wassenbergh (University of Antwerp), 6 june 2007.
    //--------------------------------------------------------------------------
    /// <summary>
    /// フィルタ
    /// </summary>
    /// <param name="indata">入力データ</param>
    /// <param name="deltaTimeinsec">フレーム間の時間差</param>
    /// <returns></returns>
    public float Butterworth(List<float> indata, float samplingrate)
    {
        var dF2 = indata.Count - 1;        // The data range is set with dF2
        var Dat2 = new float[dF2 + 4]; // Array with 4 extra points front and back

        // Copy indata to Dat2
        for (var r = 0; r < dF2; r++)
        {
            Dat2[2 + r] = indata[r];
        }
        Dat2[1] = Dat2[0] = indata[0];
        Dat2[dF2 + 3] = Dat2[dF2 + 2] = indata[dF2];

        float wc = Mathf.Tan(CutOff * Mathf.PI / samplingrate);
        float k1 = 1.414213562f * wc; // Sqrt(2) * wc
        float k2 = wc * wc;
        float a = k2 / (1 + k1 + k2);
        float b = 2 * a;
        float c = a;
        float k3 = b / k2;
        float d = -2 * a + k3;
        float e = 1 - (2 * a) - k3;

        // RECURSIVE TRIGGERS - ENABLE filter is performed (first, last points constant)
        float[] DatYt = new float[dF2 + 4];
        DatYt[1] = DatYt[0] = indata[0];
        for (long s = 2; s < dF2 + 2; s++)
        {
            DatYt[s] = a * Dat2[s] + b * Dat2[s - 1] + c * Dat2[s - 2] + d * DatYt[s - 1] + e * DatYt[s - 2];
        }
        DatYt[dF2 + 3] = DatYt[dF2 + 2] = DatYt[dF2 + 1];

        // FORWARD filter
        float[] DatZt = new float[dF2 + 2];
        DatZt[dF2] = DatYt[dF2 + 2];
        DatZt[dF2 + 1] = DatYt[dF2 + 3];
        for (long t = -dF2 + 1; t <= 0; t++)
        {
            DatZt[-t] = a * DatYt[-t + 2] + b * DatYt[-t + 3] + c * DatYt[-t + 4] + d * DatZt[-t + 1] + e * DatZt[-t + 2];
            if (DatZt[-t] > 1.0E+5f)
            {
                ErrorCnt++;
                return indata[indata.Count - 1];
            }
            if (float.IsInfinity(DatZt[-t]) || float.IsNaN(DatZt[-t]))
            {
                ErrorCnt++;
                return indata[indata.Count - 1];
            }
        }

        ErrorCnt = 0;

        // Calculated points copied for return
        for (var p = 0; p < dF2; p++)
        {
            indata[p] = DatZt[p];
        }
        return DatZt[indata.Count - 2];
    }
}
