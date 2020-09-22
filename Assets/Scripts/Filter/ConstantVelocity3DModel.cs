using Accord.Extensions.Math;
using Accord.Extensions.Statistics.Filters;
using Accord.Math;
using System;
using UnityEngine;

//referenced from ConstantVelocity2DModel.cs
//https://github.com/dajuric/accord-net-extensions/blob/master/Source/Math/Statistics/Filters/MotionModels/ConstantVelocity2DModel.cs
public class ConstantVelocity3DModel : ICloneable
{
    /// <summary>
    /// Gets the dimension of the model.
    /// </summary>
    public const int Dimension = 6;

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// Gets or sets the velocity.
    /// </summary>
    public Vector3 Velocity;

    /// <summary>
    /// Constructs an empty model.
    /// </summary>
    public ConstantVelocity3DModel()
    {
        Position = Vector3.zero;
        Velocity = Vector3.zero;
    }

    /// <summary>
    /// Evaluates the model by using the provided transition matrix.
    /// </summary>
    /// <param name="transitionMat">Transition matrix.</param>
    /// <returns>New model state.</returns>
    public ConstantVelocity3DModel Evaluate(double[,] transitionMat)
    {
        return FromArray(Accord.Math.Matrix.Multiply(transitionMat, ToArray(this)));
    }

    /// <summary>
    /// Gets the state transition matrix [4 x 4].
    /// </summary>
    /// <param name="timeInterval">Time interval.</param>
    /// <returns>State transition matrix.</returns>
    public static double[,] GetTransitionMatrix(double timeInterval = 1.0)
    {
        var t = timeInterval;

        return new double[,]
            {
                    {1, t, 0, 0, 0, 0},
                    {0, 1, 0, 0, 0, 0},
                    {0, 0, 1, t, 0, 0},
                    {0, 0, 0, 1, 0, 0},
                    {0, 0, 0, 0, 1, t},
                    {0, 0, 0, 0, 0, 1}
            };
    }

    /// <summary>
    /// Gets the position measurement matrix [3 x 6] used in Kalman filtering.
    /// </summary>
    /// <returns>Position measurement matrix.</returns>
    public static double[,] GetPositionMeasurementMatrix()
    {
        return new double[,]
        {
                {1, 0, 0, 0, 0, 0},
                {0, 0, 1, 0, 0, 0},
                {0, 0, 0, 0, 1, 0},
        };
    }

    /// <summary>
    /// Gets process-noise matrix [6 x 3] where the location is affected by (dt * dt) / 2 and velocity with the factor of dt - integrals of dt. 
    /// Factor 'dt' represents time interval.
    /// </summary>
    /// <param name="accelerationNoise">Acceleration noise.</param>
    /// <param name="timeInterval">Time interval.</param>
    /// <returns>Process noise matrix.</returns>
    public static double[,] GetProcessNoise(double accelerationNoise, double timeInterval = 1.0)
    {
        double[,] array = new double[6, 3];
        array[0, 0] = timeInterval * timeInterval / 2.0;
        array[1, 0] = timeInterval;
        array[2, 1] = timeInterval * timeInterval / 2.0;
        array[3, 1] = timeInterval;
        array[4, 2] = timeInterval * timeInterval / 2.0;
        array[5, 2] = timeInterval;
        double[,] array2 = array;
        double[,] array3 = Accord.Math.Matrix.Diagonal<double>(MatrixExtensions.ColumnCount<double>(array2), accelerationNoise);
        return Accord.Math.Matrix.Multiply(Accord.Math.Matrix.Multiply(array2, array3), Accord.Math.Matrix.Transpose<double>(array2));
    }

    /// <summary>
    /// Converts the array to the model.
    /// </summary>
    /// <param name="arr">Array to convert from.</param>
    /// <returns>Model.</returns>
    public static ConstantVelocity3DModel FromArray(double[] arr)
    {
        return new ConstantVelocity3DModel
        {
            Position = new Vector3((float)arr[0], (float)arr[2], (float)arr[4]),
            Velocity = new Vector3((float)arr[1], (float)arr[3], (float)arr[5])
        };
    }

    /// <summary>
    /// Converts the model to the array.
    /// </summary>
    /// <param name="modelState">Model to convert.</param>
    /// <returns>Array.</returns>
    public static double[] ToArray(ConstantVelocity3DModel modelState)
    {
        return new double[]
            {
                    modelState.Position.x,
                    modelState.Velocity.x,

                    modelState.Position.y,
                    modelState.Velocity.y,

                    modelState.Position.z,
                    modelState.Velocity.z,
            };
    }

    /// <summary>
    /// Clones the model.
    /// </summary>
    /// <returns>The copy of the model.</returns>
    public object Clone()
    {
        return new ConstantVelocity3DModel
        {
            Position = Position,
            Velocity = Velocity
        };
    }
}

