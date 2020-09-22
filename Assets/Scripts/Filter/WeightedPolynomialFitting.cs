using System;
using System.Collections;
using System.Collections.Generic;

public class WeightedPolynomialFitting : FittingMethod
{
    readonly Matrix weight_matrix;

    /// <summary>コンストラクタ</summary>
    public WeightedPolynomialFitting(FittingData[] data_list, float[] weight_list, int degree, bool is_enable_section)
        : base(data_list, degree + (is_enable_section ? 1 : 0))
    {

        this.Degree = degree;
        this.IsEnableSection = is_enable_section;

        if (weight_list == null)
        {
            throw new ArgumentNullException(nameof(weight_list));
        }

        if (data_list.Length != weight_list.Length)
        {
            throw new ArgumentException($"{nameof(data_list)},{nameof(weight_list)}");
        }

        foreach (var weight in weight_list)
        {
            if (!(weight >= 0))
            {
                throw new ArgumentException(nameof(weight_list));
            }
        }

        this.weight_matrix = new Matrix(weight_list.Length, weight_list.Length);

        for (int i = 0; i < weight_list.Length; i++)
        {
            weight_matrix[i, i] = weight_list[i];
        }
    }

    /// <summary>次数</summary>
    public int Degree
    {
        get; private set;
    }

    /// <summary>y切片を有効にするか</summary>
    public bool IsEnableSection { get; set; }

    /// <summary>重み付き誤差二乗和</summary>
    public float WeightedCost(Vector coefficients)
    {
        if (coefficients == null)
        {
            throw new ArgumentNullException(nameof(coefficients));
        }
        if (coefficients.Dim != ParametersCount)
        {
            throw new ArgumentException(nameof(coefficients));
        }

        Vector errors = Error(coefficients);
        float cost = 0;
        for (int i = 0; i < errors.Dim; i++)
        {
            cost += weight_matrix[i, i] * errors[i] * errors[i];
        }

        return cost;
    }

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
    public Vector ExecureFitting()
    {
        Matrix m = new Matrix(data_list.Length, ParametersCount);
        Vector b = Vector.Zero(data_list.Length);

        if (IsEnableSection)
        {
            for (int i = 0; i < data_list.Length; i++)
            {
                float x = data_list[i].X;
                b[i] = data_list[i].Y;

                m[i, 0] = 1;

                for (int j = 1; j <= Degree; j++)
                {
                    m[i, j] = m[i, j - 1] * x;
                }
            }
        }
        else
        {
            for (int i = 0; i < data_list.Length; i++)
            {
                float x = data_list[i].X;
                b[i] = data_list[i].Y;

                m[i, 0] = x;

                for (int j = 1; j < Degree; j++)
                {
                    m[i, j] = m[i, j - 1] * x;
                }
            }
        }

        return (m.Transpose * weight_matrix * m).Inverse * m.Transpose * weight_matrix * b;
    }
}
