using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConnector : MonoBehaviour
{
    public LineRenderer line;
    public Transform[] connections;

    public void UpdateConnections()
    {
        Vector3[] positions = new Vector3[connections.Length];

        for(int it_index = 0; it_index < connections.Length; it_index += 1)
        {
            positions[it_index] = connections[it_index].position;
        }

        line.positionCount = positions.Length;
        line.SetPositions(positions);
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateConnections();
    }

    // Update is called once per frame
    void Update()
    {
        if(line.positionCount != connections.Length)
        {
            UpdateConnections();
        }

        for(int it_index = 0; it_index < connections.Length; it_index += 1)
        {
            line.SetPosition(it_index, connections[it_index].position);
        }
    }
}
