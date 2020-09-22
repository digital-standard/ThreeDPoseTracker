using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterWindow
{
    public List<float> Cnt = new List<float>(); //カウンタ - (N - 1) / 2 ~ (N - 1) / 2
    public List<float> Win = new List<float>(); //窓関数（ハミング）
    public List<float> H = new List<float>(); //設計したFIRフィルタ
    public int L; //N/2を0に置き換えたフィルタの領域

    // fc1: カットオフ周波数ここから
    // fc2: カットオフ周波数ここまで
    // dT: サンプリング時間
    // N: フィルタの次数
    private float fc1 = 0.01f;
    private float fc2 = 2f;
    //private float fc1 = 0.01f;
    //private float fc2 = 0.4f;
    //private float dT = 0.05f;
    private float fps = 0.05f;
    private float prevfps = 0.05f;
    private float N = 100f;

    public FilterWindow()
    {
    }

    public void Init(int n, float f1, float f2, float f)
    {
        N = n;
        fps = f;
        prevfps = f;
        fc1 = f1 / fps;
        fc2 = f2 / fps;

        CreateFilter();
    }

    public void SetFps(float f)
    {
        var _f = prevfps * 0.9f + f * 0.1f;
        if(Mathf.Abs((_f - fps) / fps) > 0.2f)
        {
            fps = _f;
            CreateFilter();
        }
    }

    public void CreateFilter()
    {
        Cnt.Clear();
        Win.Clear();
        H.Clear();

        // ハミング窓
        L = ((int)N - 1) / 2;
        for (var i = -L; i <= L; i++)
        {
            Cnt.Add(2f * Mathf.PI * i);
            Win.Add(0.54f + 0.46f * Mathf.Cos(2f * Mathf.PI * i / (N - 1f)));

            //Cnt[i + L] = i;
            //Win[i + L] = 0.54f + 0.46f * Mathf.Cos(2f * Mathf.PI * i / (N - 1f));
        }

        //フィルタ作成
        for (var i = 0; i < Cnt.Count; i++)
        {
            var h = 0f;
            if (Cnt[i] == 0)
            {
                h = 2f * fc2 - 2f * fc1;
            }
            else
            {
                h = (2f * fc2 * Mathf.Sin(Cnt[i] * fc2) / (Cnt[i] * fc2)) - (2f * fc1 * Mathf.Sin(Cnt[i] * fc1) / (Cnt[i] * fc1));
            }

            H.Add(h * Win[i]);
        }
    }

}

public class FIRFilter
{
    private List<float> X = new List<float>();
    private List<float> Y = new List<float>();
    private List<float> Z = new List<float>();
    private int bufferSize = 100;
    private FilterWindow filter;

    public FIRFilter(FilterWindow fw, int buffer)
    {
        filter = fw;
        bufferSize = buffer;
    }

    private float Add(List<float> list, float v)
    {
        list.Add(v);
        if (list.Count > 10)
        {
            if (list.Count > bufferSize)
            {
                list.RemoveAt(0);
            }

            //畳み込み処理
            float data = 0.0f;
            var i = list.Count - 1;
            for (var j = -filter.L; j <= filter.L; j++)
            {
                if (i + j < 0)
                {
                    data += list[0] * filter.H[j + filter.L];
                }
                else if (j > 0)
                {
                    data += list[i] * filter.H[j + filter.L];
                }
                else
                {
                    data += list[i + j] * filter.H[j + filter.L];
                }
            }
            return data;
            /*
             float[] data_list = new float[100];
             for (var i = 0; i < bufferSize; i++)
             {
                 data_list[i] = 0.0f;
                 for (var j = -filter.L; j <= filter.L; j++)
                 {
                     if (i + j < 0)
                     {
                         data_list[i] += list[0] * filter.H[j + filter.L];
                     }
                     else if (i + j >= bufferSize)
                     {
                         data_list[i] += list[bufferSize - 1] * filter.H[j + filter.L];
                     }
                     else
                     {
                         data_list[i] += list[i + j] * filter.H[j + filter.L];
                     }
                 }
             }
             return data_list[bufferSize - 1];
             */
        }
        else
        {
            return v;
        }
    }

    public (float, float, float) Add(float x, float y, float z, float samplingrate)
    {
 
        return (Add(X, x), Add(Y, y), Add(Z, z));
    }


}
