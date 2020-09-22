using Accord.Extensions.Statistics.Filters;
using Accord.Math;
using System;
using UnityEngine;

//https://www.codeproject.com/Articles/865935/Object-Tracking-Kalman-Filter-with-Ease
using ModelState = ConstantVelocity3DModel;

public class KalmanFilter
{
    private DiscreteKalmanFilter<ModelState, VNectModel.JointPoint> kalmanFilter;

    public KalmanFilter(double timeInterval, double noise)
    {
        ModelState initialState = new ModelState
        {
            Position = new Vector3(0, 0, 0),
            Velocity = new Vector3(0, 0, 0),
            //Acceleration = new Vector3(0, 0, 0),
        };

        double[,] initialStateError = ModelState.GetProcessNoise(noise, timeInterval);
        int measurementVectorDimension = 3;
        int controlVectorDimension = 0;
        Func<ModelState, double[]> stateConvertFunc = ModelState.ToArray;
        Func<double[], ModelState> stateConvertBackFunc = ModelState.FromArray;
        Func<VNectModel.JointPoint, double[]> measurementConvertFunc = v => { return new double[3] { v.Now3D.x, v.Now3D.y, v.Now3D.z }; };

        kalmanFilter = new DiscreteKalmanFilter<ModelState, VNectModel.JointPoint>(
            initialState,
            initialStateError,
            measurementVectorDimension,
            controlVectorDimension,
            stateConvertFunc,
            stateConvertBackFunc,
            measurementConvertFunc);

        kalmanFilter.ProcessNoise = ModelState.GetProcessNoise(noise, timeInterval);
        kalmanFilter.MeasurementNoise = Accord.Math.Matrix.Diagonal<double>(kalmanFilter.MeasurementVectorDimension, 1);
        kalmanFilter.MeasurementMatrix = ModelState.GetPositionMeasurementMatrix();
        kalmanFilter.TransitionMatrix = ModelState.GetTransitionMatrix(timeInterval);
        kalmanFilter.Predict();
    }

    public void UpdateFilterParameter(double timeInterval, double noise)
    {
        kalmanFilter.ProcessNoise = ModelState.GetProcessNoise(noise, timeInterval);
        kalmanFilter.TransitionMatrix = ModelState.GetTransitionMatrix(timeInterval);
    }

    public void Correct(VNectModel.JointPoint jp)
    {
        kalmanFilter.Correct(jp);
    }

    public void Predict()
    {
        kalmanFilter.Predict();
    }

    public Vector3 GetPosition()
    {
        return kalmanFilter.State.Position;
    }

    public Vector3 CorrectAndPredict(VNectModel.JointPoint jp)
    {
        kalmanFilter.Correct(jp);
        kalmanFilter.Predict();
        return GetPosition();
    }
}
