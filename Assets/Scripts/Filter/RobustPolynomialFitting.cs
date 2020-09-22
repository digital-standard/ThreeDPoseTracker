using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobustPolynomialFitting : FittingMethod
{
    /// <summary>コンストラクタ</summary>
    public RobustPolynomialFitting(FittingData[] data_list, int degree, bool is_enable_section)
        : base(data_list, degree + (is_enable_section ? 1 : 0))
    {

        this.Degree = degree;
        this.IsEnableSection = is_enable_section;
    }

    /// <summary>次数</summary>
    public int Degree
    {
        get; private set;
    }

    /// <summary>y切片を有効にするか</summary>
    public bool IsEnableSection { get; private set; }

    /// <summary>フィッティング値</summary>
    public override float FittingValue(float x, Vector coefficients)
    {
        if (IsEnableSection)
        {
            float y = coefficients[0], ploy_x = 1;

            for (int i = 1; i < coefficients.Dim; i++)
            {
                ploy_x *= x;
                y += ploy_x * coefficients[i];
            }

            return y;
        }
        else
        {
            float y = 0, ploy_x = 1;

            for (int i = 0; i < coefficients.Dim; i++)
            {
                ploy_x *= x;
                y += ploy_x * coefficients[i];
            }

            return y;
        }
    }

    /// <summary>フィッティング</summary>
    public Vector ExecureFitting(int converge_times = 8)
    {
        int n = data_list.Length;
        float err_threshold, inv_err;
        float[] weight_list = new float[n], err_list = new float[n], sort_err_list;
        Vector err, coef = null;
        WeightedPolynomialFitting fitting;

        for (int i = 0; i < n; i++)
        {
            weight_list[i] = 1;
        }

        while (converge_times > 0)
        {
            fitting = new WeightedPolynomialFitting(data_list, weight_list, Degree, IsEnableSection);

            coef = fitting.ExecureFitting();

            err = fitting.Error(coef);

            for (int i = 0; i < n; i++)
            {
                err_list[i] = Mathf.Abs(err[i]);
            }

            sort_err_list = (float[])err_list.Clone();

            Array.Sort(sort_err_list);

            err_threshold = sort_err_list[n / 2] * 1.25f;
            if (err_threshold <= 1e-14)
            {
                break;
            }

            inv_err = 1 / err_threshold;

            for (int i = 0; i < n; i++)
            {
                if (err_list[i] >= err_threshold)
                {
                    weight_list[i] = 0;
                }
                else
                {
                    float r = (1 - err_list[i] * inv_err);
                    weight_list[i] = r * r;
                }
            }

            converge_times--;
        }

        return coef;
    }
}
