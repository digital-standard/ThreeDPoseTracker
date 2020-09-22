using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class FittingMethod 
{
    /// <summary>フィッティング対象のデータ</summary>
    protected readonly FittingData[] data_list;

    /// <summary>コンストラクタ</summary>
    public FittingMethod(FittingData[] data_list, int parameters)
    {
        if (data_list == null)
        {
            throw new ArgumentNullException(nameof(data_list));
        }
        if (data_list.Length < 1)
        {
            throw new ArgumentException(nameof(data_list));
        }
        if (parameters < 1)
        {
            throw new ArgumentException(nameof(parameters));
        }

        this.data_list = data_list;
        this.ParametersCount = parameters;
    }

    /// <summary>パラメータ数</summary>
    public int ParametersCount
    {
        get; private set;
    }

    /// <summary>誤差二乗和</summary>
    public float Cost(Vector parameters)
    {
        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }
        if (parameters.Dim != ParametersCount)
        {
            throw new ArgumentException(nameof(parameters));
        }

        Vector errors = Error(parameters);
        float cost = 0;
        for (int i = 0; i < errors.Dim; i++)
        {
            cost += errors[i] * errors[i];
        }

        return cost;
    }

    /// <summary>誤差</summary>
    public Vector Error(Vector parameters)
    {
        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }
        if (parameters.Dim != ParametersCount)
        {
            throw new ArgumentException(nameof(parameters));
        }

        Vector errors = Vector.Zero(data_list.Length);

        for (int i = 0; i < data_list.Length; i++)
        {
            errors[i] = FittingValue(data_list[i].X, parameters) - data_list[i].Y;
        }

        return errors;
    }

    /// <summary>フィッティング値</summary>
    public abstract float FittingValue(float x, Vector parameters);
}

/// <summary>フィッティング関数真値</summary>
public struct FittingData
{

    /// <summary>コンストラクタ</summary>
    public FittingData(float x, float y)
    {
        this.X = x;
        this.Y = y;
    }

    /// <summary>独立変数</summary>
    public float X { get; set; }

    /// <summary>従属変数</summary>
    public float Y { get; set; }

    /// <summary>文字列化</summary>
    public override string ToString()
    {
        return $"{X},{Y}";
    }
}

/// <summary>フィッティング関数</summary>
public class FittingFunction
{
    readonly Func<float, Vector, float> f;
    readonly Func<float, Vector, Vector> df;

    /// <summary>コンストラクタ</summary>
    public FittingFunction(int parameters_count, Func<float, Vector, float> f, Func<float, Vector, Vector> df)
    {
        this.ParametersCount = parameters_count;
        this.f = f;
        this.df = df;
    }

    /// <summary>パラメータ数</summary>
    public int ParametersCount
    {
        get; private set;
    }

    /// <summary>関数値</summary>
    public float F(float x, Vector v)
    {
        return f(x, v);
    }

    /// <summary>関数勾配</summary>
    public Vector DiffF(float x, Vector v)
    {
        return df(x, v);
    }
}
///<summary>ベクトルクラス</summary>
public class Vector : ICloneable
{
    protected readonly float[] v;

    /// <summary>コンストラクタ</summary>
    public Vector(params float[] v)
    {
        this.v = (float[])v.Clone();
    }

    /// <summary>インデクサ</summary>
    public float this[int index]
    {
        get
        {
            return v[index];
        }
        set
        {
            v[index] = value;
        }
    }

    /// <summary>X成分</summary>
    public float X
    {
        get
        {
            return v[0];
        }
        set
        {
            v[0] = value;
        }
    }

    /// <summary>Y成分</summary>
    public float Y
    {
        get
        {
            return v[1];
        }
        set
        {
            v[1] = value;
        }
    }

    /// <summary>Z成分</summary>
    public float Z
    {
        get
        {
            return v[2];
        }
        set
        {
            v[2] = value;
        }
    }

    /// <summary>W成分</summary>
    public float W
    {
        get
        {
            return v[3];
        }
        set
        {
            v[3] = value;
        }
    }

    /// <summary>次元数</summary>
    public int Dim => v.Length;

    /// <summary>ノルム</summary>
    public float Norm => Mathf.Sqrt(SquareNorm);

    /// <summary>ノルム2乗</summary>
    public float SquareNorm
    {
        get
        {
            float norm = 0;
            foreach (var vi in v)
            {
                norm += vi * vi;
            }

            return norm;
        }
    }

    /// <summary>行ベクトル</summary>
    public Matrix Horizontal
    {
        get
        {
            Matrix ret = new Matrix(1, Dim);
            for (int i = 0; i < Dim; i++)
            {
                ret[0, i] = v[i];
            }

            return ret;
        }
    }

    /// <summary>列ベクトル</summary>
    public Matrix Vertical
    {
        get
        {
            Matrix ret = new Matrix(Dim, 1);
            for (int i = 0; i < Dim; i++)
            {
                ret[i, 0] = v[i];
            }

            return ret;
        }
    }

    /// <summary>正規化</summary>
    public Vector Normal => this / Norm;

    /// <summary>単項プラス</summary>
    public static Vector operator +(Vector vector)
    {
        return (Vector)vector.Clone();
    }

    /// <summary>単項マイナス</summary>
    public static Vector operator -(Vector vector)
    {
        float[] v = new float[vector.Dim];

        for (int i = 0; i < vector.Dim; i++)
        {
            v[i] = -vector[i];
        }
        return new Vector(v);
    }

    /// <summary>ベクトル加算</summary>
    public static Vector operator +(Vector vector1, Vector vector2)
    {
        if (vector1.Dim != vector2.Dim)
        {
            throw new ArgumentException();
        }

        int size = vector1.Dim;
        float[] v = new float[size];

        for (int i = 0; i < size; i++)
        {
            v[i] = vector1[i] + vector2[i];
        }

        return new Vector(v);
    }

    /// <summary>ベクトル減算</summary>
    public static Vector operator -(Vector vector1, Vector vector2)
    {
        if (vector1.Dim != vector2.Dim)
        {
            throw new ArgumentException();
        }

        int size = vector1.Dim;
        float[] v = new float[size];

        for (int i = 0; i < size; i++)
        {
            v[i] = vector1[i] - vector2[i];
        }

        return new Vector(v);
    }

    /// <summary>スカラー倍</summary>
    public static Vector operator *(float r, Vector vector)
    {
        float[] v = new float[vector.Dim];

        for (int i = 0; i < vector.Dim; i++)
        {
            v[i] = vector[i] * r;
        }

        return new Vector(v);
    }

    /// <summary>スカラー倍</summary>
    public static Vector operator *(Vector vector, float r)
    {
        return r * vector;
    }

    /// <summary>スカラー逆数倍</summary>
    public static Vector operator /(Vector vector, float r)
    {
        return (1 / r) * vector;
    }

    /// <summary>ベクトル間距離</summary>
    public static float Distance(Vector vector1, Vector vector2)
    {
        return (vector1 - vector2).Norm;
    }

    /// <summary>ベクトル間距離2乗</summary>
    public static float SquareDistance(Vector vector1, Vector vector2)
    {
        return (vector1 - vector2).SquareNorm;
    }

    /// <summary>ベクトル内積</summary>
    public static float InnerProduct(Vector vector1, Vector vector2)
    {
        if (vector1.Dim != vector2.Dim)
        {
            throw new ArgumentException();
        }

        float sum = 0;
        for (int i = 0, dim = vector1.Dim; i < dim; i++)
        {
            sum += vector1[i] * vector2[i];
        }

        return sum;
    }

    /// <summary>ゼロベクトル</summary>
    public static Vector Zero(int size)
    {
        return new Vector(new float[size]);
    }

    /// <summary>ゼロベクトルか判定</summary>
    public static bool IsZero(Vector vector)
    {
        for (int i = 0; i < vector.Dim; i++)
        {
            if (vector[i] != 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>不正なベクトル</summary>
    public static Vector Invalid(int size)
    {
        float[] v = new float[size];
        for (int i = 0; i < size; i++)
        {
            v[i] = float.NaN;
        }

        return new Vector(v);
    }

    /// <summary>有効なベクトルか判定</summary>
    public static bool IsValid(Vector vector)
    {
        for (int i = 0; i < vector.Dim; i++)
        {
            if (float.IsNaN(vector[i]) || float.IsInfinity(vector[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>ベクトルが等しいか</summary>
    public static bool operator ==(Vector vector1, Vector vector2)
    {
        if (ReferenceEquals(vector1, vector2))
        {
            return true;
        }
        if ((object)vector1 == null || (object)vector2 == null)
        {
            return false;
        }

        return vector1.v.SequenceEqual(vector2.v);
    }

    /// <summary>ベクトルが異なるか判定</summary>
    public static bool operator !=(Vector vector1, Vector vector2)
    {
        return !(vector1 == vector2);
    }

    /// <summary>等しいか判定</summary>
    public override bool Equals(object obj)
    {
        return obj is Vector ? (Vector)obj == this : false;
    }

    /// <summary>ハッシュ値</summary>
    public override int GetHashCode()
    {
        return Dim > 0 ? v[0].GetHashCode() : 0;
    }

    /// <summary>クローン</summary>
    public object Clone()
    {
        return new Vector(v);
    }

    /// <summary>コピー</summary>
    public Vector Copy()
    {
        return new Vector(v);
    }

    /// <summary>文字列化</summary>
    public override string ToString()
    {
        if (Dim <= 0)
        {
            return string.Empty;
        }

        string str = $"{v[0]}";

        for (int i = 1; i < Dim; i++)
        {
            str += $",{v[i]}";
        }

        return str;
    }
}

/// <summary>行列クラス</summary>
public partial class Matrix : ICloneable
{
    protected readonly float[,] e;

    /// <summary>コンストラクタ</summary>
    /// <param name="m">行列要素配列</param>
    public Matrix(float[,] m)
    {
        if (m == null)
        {
            throw new ArgumentNullException();
        }
        this.e = (float[,])m.Clone();
    }

    /// <summary>コンストラクタ </summary>
    /// <param name="rows">行数</param>
    /// <param name="columns">列数</param>
    public Matrix(int rows, int columns)
    {
        if (rows <= 0 || columns <= 0)
        {
            throw new ArgumentException();
        }

        this.e = new float[rows, columns];
    }

    /// <summary>インデクサ </summary>
    /// <param name="row_index">行</param>
    /// <param name="column_index">列</param>
    public float this[int row_index, int column_index]
    {
        get
        {
            return e[row_index, column_index];
        }
        set
        {
            e[row_index, column_index] = value;
        }
    }

    /// <summary>行数</summary>
    public int Rows => e.GetLength(0);

    /// <summary>列数</summary>
    public int Columns => e.GetLength(1);

    /// <summary>サイズ(正方行列のときのみ有効)</summary>
    public int Size
    {
        get
        {
            if (!IsSquare(this))
            {
                throw new InvalidOperationException();
            }

            return Rows;
        }
    }

    /// <summary>単項プラス</summary>
    public static Matrix operator +(Matrix matrix)
    {
        return matrix.Copy();
    }

    /// <summary>単項マイナス</summary>
    public static Matrix operator -(Matrix matrix)
    {
        Matrix ret = matrix.Copy();

        for (int i = 0, j; i < ret.Rows; i++)
        {
            for (j = 0; j < ret.Columns; j++)
            {
                ret[i, j] = -ret[i, j];
            }
        }

        return ret;
    }

    /// <summary>行列加算</summary>
    public static Matrix operator +(Matrix matrix1, Matrix matrix2)
    {
        if (!IsEqualSize(matrix1, matrix2))
        {
            throw new ArgumentException();
        }

        Matrix ret = new Matrix(matrix1.Rows, matrix1.Columns);

        for (int i = 0, j; i < ret.Rows; i++)
        {
            for (j = 0; j < ret.Columns; j++)
            {
                ret[i, j] = matrix1[i, j] + matrix2[i, j];
            }
        }

        return ret;
    }

    /// <summary>行列減算</summary>
    public static Matrix operator -(Matrix matrix1, Matrix matrix2)
    {
        if (!IsEqualSize(matrix1, matrix2))
        {
            throw new ArgumentException();
        }

        Matrix ret = new Matrix(matrix1.Rows, matrix1.Columns);

        for (int i = 0, j; i < ret.Rows; i++)
        {
            for (j = 0; j < ret.Columns; j++)
            {
                ret[i, j] = matrix1[i, j] - matrix2[i, j];
            }
        }

        return ret;
    }

    /// <summary>行列乗算</summary>
    public static Matrix operator *(Matrix matrix1, Matrix matrix2)
    {
        if (matrix1.Columns != matrix2.Rows)
        {
            throw new ArgumentException();
        }

        Matrix ret = new Matrix(matrix1.Rows, matrix2.Columns);
        int c = matrix1.Columns;

        for (int i = 0, j, k; i < ret.Rows; i++)
        {
            for (j = 0; j < ret.Columns; j++)
            {
                for (k = 0; k < c; k++)
                {
                    ret[i, j] += matrix1[i, k] * matrix2[k, j];
                }
            }
        }

        return ret;
    }

    /// <summary>行列・列ベクトル乗算</summary>
    public static Vector operator *(Matrix matrix, Vector vector)
    {
        if (matrix.Columns != vector.Dim)
        {
            throw new ArgumentException();
        }

        Vector ret = Vector.Zero(matrix.Rows);

        for (int i = 0, j; i < matrix.Rows; i++)
        {
            for (j = 0; j < matrix.Columns; j++)
            {
                ret[i] += matrix[i, j] * vector[j];
            }
        }

        return ret;
    }

    /// <summary>行列・行ベクトル乗算</summary>
    public static Vector operator *(Vector vector, Matrix matrix)
    {
        if (vector.Dim != matrix.Rows)
        {
            throw new ArgumentException();
        }

        Vector ret = Vector.Zero(matrix.Columns);

        for (int j = 0, i; j < matrix.Columns; j++)
        {
            for (i = 0; i < matrix.Rows; i++)
            {
                ret[j] += vector[i] * matrix[i, j];
            }
        }

        return ret;
    }

    /// <summary>行列スカラー倍</summary>
    public static Matrix operator *(float r, Matrix matrix)
    {
        Matrix ret = new Matrix(matrix.Rows, matrix.Columns);

        for (int i = 0, j; i < ret.Rows; i++)
        {
            for (j = 0; j < ret.Columns; j++)
            {
                ret[i, j] = matrix[i, j] * r;
            }
        }

        return ret;
    }

    /// <summary>行列スカラー倍</summary>
    public static Matrix operator *(Matrix matrix, float r)
    {
        return r * matrix;
    }

    /// <summary>行列スカラー逆数倍</summary>
    public static Matrix operator /(Matrix matrix, float r)
    {
        return (1 / r) * matrix;
    }

    /// <summary>行列が等しいか</summary>
    public static bool operator ==(Matrix matrix1, Matrix matrix2)
    {
        if (ReferenceEquals(matrix1, matrix2))
        {
            return true;
        }
        if ((object)matrix1 == null || (object)matrix2 == null)
        {
            return false;
        }

        if (!IsEqualSize(matrix1, matrix2))
        {
            return false;
        }

        for (int i = 0, j; i < matrix1.Rows; i++)
        {
            for (j = 0; j < matrix2.Columns; j++)
            {
                if (matrix1[i, j] != matrix2[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>行列が異なるか判定</summary>
    public static bool operator !=(Matrix matrix1, Matrix matrix2)
    {
        return !(matrix1 == matrix2);
    }

    /// <summary>等しいか判定</summary>
    public override bool Equals(object obj)
    {
        return obj is Matrix ? (Matrix)obj == this : false;
    }

    /// <summary>ハッシュ値</summary>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary>クローン</summary>
    public object Clone()
    {
        return new Matrix(e);
    }

    /// <summary>ディープコピー</summary>
    public Matrix Copy()
    {
        return new Matrix(e);
    }

    /// <summary>転置</summary>
    public Matrix Transpose
    {
        get
        {
            Matrix ret = new Matrix(Columns, Rows);

            for (int i = 0, j; i < Rows; i++)
            {
                for (j = 0; j < Columns; j++)
                {
                    ret.e[j, i] = e[i, j];
                }
            }

            return ret;
        }
    }

    /// <summary>逆行列</summary>
    public Matrix Inverse
    {
        get
        {
            if (IsZero(this) || !IsValid(this))
            {
                return Invalid(Columns, Rows);
            }
            if (Rows == Columns)
            {
                Matrix m = Copy(), d = Identity(Rows);
                GaussianEliminate(m, ref d);
                return d;
            }
            else if (Rows < Columns)
            {
                Matrix m = this * Transpose;
                return Transpose * m.Inverse;
            }
            else
            {
                Matrix m = Transpose * this;
                return m.Inverse * Transpose;
            }
        }
    }

    /// <summary>行列ノルム</summary>
    public float Norm
    {
        get
        {
            float sum_sq = 0;
            for (int i = 0, j; i < Rows; i++)
            {
                for (j = 0; j < Columns; j++)
                {
                    sum_sq += e[i, j] * e[i, j];
                }
            }

            return Mathf.Sqrt(sum_sq);
        }
    }

    /// <summary>行ベクトル</summary>
    /// <param name="row_index">行</param>
    public Vector Horizontal(int row_index)
    {
        Vector ret = Vector.Zero(Columns);
        for (int i = 0; i < Columns; i++)
        {
            ret[i] = e[row_index, i];
        }

        return ret;
    }

    /// <summary>列ベクトル</summary>
    /// <param name="column_index">列</param>
    public Vector Vertical(int column_index)
    {
        Vector ret = Vector.Zero(Rows);
        for (int i = 0; i < Rows; i++)
        {
            ret[i] = e[i, column_index];
        }

        return ret;
    }

    /// <summary>ゼロ行列</summary>
    /// <param name="rows">行数</param>
    /// <param name="columns">列数</param>
    public static Matrix Zero(int rows, int columns)
    {
        return new Matrix(rows, columns);
    }

    /// <summary>単位行列</summary>
    /// <param name="size">行列サイズ</param>
    public static Matrix Identity(int size)
    {
        Matrix ret = new Matrix(size, size);

        for (int i = 0, j; i < size; i++)
        {
            for (j = 0; j < size; j++)
            {
                ret.e[i, j] = (i == j) ? 1 : 0;
            }
        }

        return ret;
    }

    /// <summary>不正な行列</summary>
    /// <param name="rows">行数</param>
    /// <param name="columns">列数</param>
    public static Matrix Invalid(int rows, int columns)
    {
        Matrix ret = new Matrix(rows, columns);
        for (int i = 0, j; i < ret.Rows; i++)
        {
            for (j = 0; j < ret.Columns; j++)
            {
                ret.e[i, j] = float.NaN;
            }
        }

        return ret;
    }

    /// <summary>行列のサイズが等しいか判定</summary>
    public static bool IsEqualSize(Matrix matrix1, Matrix matrix2)
    {
        return (matrix1.Rows == matrix2.Rows) && (matrix1.Columns == matrix2.Columns);
    }

    /// <summary>正方行列か判定</summary>
    public static bool IsSquare(Matrix matrix)
    {
        return matrix.Rows == matrix.Columns;
    }

    /// <summary>対角行列か判定</summary>
    public static bool IsDiagonal(Matrix matrix)
    {
        if (!IsSquare(matrix))
        {
            return false;
        }

        for (int i = 0, j; i < matrix.Rows; i++)
        {
            for (j = 0; j < matrix.Columns; j++)
            {
                if (i != j && matrix.e[i, j] != 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>ゼロ行列か判定</summary>
    public static bool IsZero(Matrix matrix)
    {
        for (int i = 0, j; i < matrix.Rows; i++)
        {
            for (j = 0; j < matrix.Columns; j++)
            {
                if (matrix[i, j] != 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>単位行列か判定</summary>
    public static bool IsIdentity(Matrix matrix)
    {
        if (!IsSquare(matrix))
        {
            return false;
        }

        for (int i = 0, j; i < matrix.Rows; i++)
        {
            for (j = 0; j < matrix.Columns; j++)
            {
                if (i == j)
                {
                    if (matrix[i, j] != 1)
                    {
                        return false;
                    }
                }
                else
                {
                    if (matrix[i, j] != 0)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    /// <summary>対称行列か判定</summary>
    public static bool IsSymmetric(Matrix matrix)
    {
        if (!IsSquare(matrix))
        {
            return false;
        }

        for (int i = 0, j; i < matrix.Rows; i++)
        {
            for (j = i + 1; j < matrix.Columns; j++)
            {
                if (matrix[i, j] != matrix[j, i])
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>有効な行列か判定</summary>
    public static bool IsValid(Matrix matrix)
    {
        if (matrix.Rows < 1 || matrix.Columns < 1)
        {
            return false;
        }

        for (int i = 0, j; i < matrix.Rows; i++)
        {
            for (j = 0; j < matrix.Columns; j++)
            {
                if (float.IsNaN(matrix[i, j]) || float.IsInfinity(matrix[i, j]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>正則行列か判定</summary>
    public static bool IsRegular(Matrix matrix)
    {
        return IsValid(matrix.Inverse);
    }

    /// <summary>対角成分</summary>
    public float[] Diagonals
    {
        get
        {
            if (!IsSquare(this))
            {
                throw new InvalidOperationException();
            }

            float[] diagonals = new float[Size];

            for (int i = 0; i < Size; i++)
            {
                diagonals[i] = e[i, i];
            }

            return diagonals;
        }
    }

    /// <summary>文字列化</summary>
    public override string ToString()
    {
        if (!IsValid(this))
        {
            return "Invalid Matrix";
        }

        string str = "{ ";

        str += "{ ";
        str += $"{e[0, 0]}";
        for (int j = 1; j < Columns; j++)
        {
            str += $", {e[0, j]}";
        }
        str += " }";

        for (int i = 1, j; i < Rows; i++)
        {
            str += ", { ";
            str += $"{e[i, 0]}";
            for (j = 1; j < Columns; j++)
            {
                str += $", {e[i, j]}";
            }
            str += " }";
        }

        str += " }";

        return str;
    }

    /// <summary>ガウスの消去法</summary>
    public static void GaussianEliminate(Matrix m, ref Matrix v)
    {
        if (!IsSquare((Matrix)m) || m.Rows != v.Rows)
        {
            throw new ArgumentException();
        }

        int i, j, k, p, n = m.Rows;
        float pivot, inv_mii, mul, swap;

        for (i = 0; i < n; i++)
        {
            pivot = Math.Abs(m[i, i]);
            p = i;

            //ピボット選択
            for (j = i + 1; j < n; j++)
            {
                if (Math.Abs(m[i, j]) > pivot)
                {
                    pivot = Math.Abs(m[i, j]);
                    p = j;
                }
            }

            //ピボットが閾値以下ならばMは正則行列でないので逆行列は存在しない
            if (pivot < 1.0e-12)
            {
                v = Invalid(v.Rows, v.Columns);
                return;
            }

            //行入れ替え
            if (p != i)
            {
                for (j = 0; j < n; j++)
                {
                    swap = m[i, j];
                    m[i, j] = m[p, j];
                    m[p, j] = swap;
                }

                for (j = 0; j < v.Columns; j++)
                {
                    swap = v[i, j];
                    v[i, j] = v[p, j];
                    v[p, j] = swap;
                }
            }

            // 前進消去
            inv_mii = 1 / m[i, i];
            m[i, i] = 1;
            for (j = i + 1; j < n; j++)
            {
                m[i, j] *= inv_mii;
            }
            for (j = 0; j < v.Columns; j++)
            {
                v[i, j] *= inv_mii;
            }

            for (j = i + 1; j < n; j++)
            {
                mul = m[j, i];
                m[j, i] = 0;
                for (k = i + 1; k < n; k++)
                {
                    m[j, k] -= m[i, k] * mul;
                }
                for (k = 0; k < v.Columns; k++)
                {
                    v[j, k] -= v[i, k] * mul;
                }
            }
        }

        // 後退代入
        for (i = n - 1; i >= 0; i--)
        {
            for (j = i - 1; j >= 0; j--)
            {
                mul = m[j, i];
                for (k = i; k < n; k++)
                {
                    m[j, k] = 0;
                }
                for (k = 0; k < v.Columns; k++)
                {
                    v[j, k] -= v[i, k] * mul;
                }
            }
        }
    }
}
