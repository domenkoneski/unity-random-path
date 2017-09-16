using System.Collections.Generic;
using UnityEngine;

public class RandomPath : MonoBehaviour
{
    public enum PathQuality
    {
        LOW = 2,
        MEDIUM = 10,
        HIGH = 50
    }

    #region Variables
    [Header("Path Settings")]
    [Tooltip("Acts as the length of the path generator")]
    public int numberOfDecisions = 100;
    public int minDecisionSize = 10;
    public int maxDecisionSize = 20;

    public float minAngleSpeed = 1;
    public float maxAngleSpeed = 2;

    [Tooltip("Change this to create a more precise path. Or add more enum types and increment their value.")]
    public PathQuality quality = PathQuality.MEDIUM;
    #endregion

    /// <summary>
    /// An example on how to create the random path by inputing start value, number of decisions = length and starting angle.
    /// </summary>
    void TestGenerate ()
    {
        List<Vector3> path = CreatePath(new Vector2(transform.position.x, transform.position.z), numberOfDecisions, 0);
        float smallStep = 0.001f;
        for (float t = 0; t < 1; t+= smallStep)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = GetVectorFromPath(t, path);
        }
    }

    /// <summary>
    /// Creates a path using a "decision" logics. It decides what is the next angle and angle speed.
    /// </summary>
    /// <param name="start">Starting vector, change the algorithm if you don't want to create a path in X/Y direction</param>
    /// <param name="numberOfDecisions">Length of the path or how many times algorithm decides to provide with new angle. The overall length of the path is numOfDecisions * decisionLenght * step</param>
    /// <param name="startAngle">Start angle (see unit circle and its angles)</param>
    /// <returns></returns>
    public List<Vector3> CreatePath(Vector2 start, int numberOfDecisions, float startAngle = 0)
    {
        List<Vector3> pathPoints = new List<Vector3>();

        for (int i = 0; i < numberOfDecisions; i++)
        {
            int nextDecision = Random.Range(minDecisionSize, maxDecisionSize);

            float nextAngle = Random.Range(-30, 30);

            float minNextAngle = 10;
            while (nextAngle < minNextAngle && nextAngle > -minNextAngle)
                nextAngle = Random.Range(-30, 30);

            float endAngle = startAngle + nextAngle;
            float angleSpeed = Random.Range(minAngleSpeed, maxAngleSpeed);

            while (nextDecision-- > 0)
            {
                if (startAngle >= endAngle)
                    startAngle -= Mathf.Abs(angleSpeed);
                else if (startAngle <= endAngle)
                    startAngle += Mathf.Abs(angleSpeed);

                Quaternion qAngle = Quaternion.AngleAxis(startAngle, Vector3.forward);
                Vector3 nextPos = qAngle * new Vector3(1f / (int)quality, 0, 0);

                start += new Vector2(nextPos.x, nextPos.y);

                pathPoints.Add(start);
            }
        }

        return pathPoints;
    }

    /// <summary>
    /// Static method to provide a point from the path points list. It interpolates aswell for smoothing experience.
    /// </summary>
    /// <param name="t">Value from 0 - 1</param>
    /// <param name="list">List of generated points</param>
    /// <returns></returns>
    public static Vector3 GetVectorFromPath(float t, List<Vector3> list)
    {
        if (list == null || list.Count == 0)
            throw new UnityException("Path list is null or empty.");

        if (t == 0) return list[0];
        if (t == 1) return list[list.Count - 1];

        int startIndex = (int)(list.Count * t);
        int endIndex = startIndex + 1;

        if (endIndex >= list.Count)
            return list[startIndex];

        float excess = (list.Count * t) - (int)(list.Count * t);        

        return Vector3.Lerp(list[startIndex], list[endIndex], excess);
    }
}