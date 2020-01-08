//
// Created by Holly Strauch on 10/1/2019.
//

#include "Graph.h"

using namespace std;
using h_set = std::unordered_set<int>;

// Default constructor.
Graph::Graph()
{
    noOfVertices = 0;
    edges = nullptr;
}

// Copy constructor.
Graph::Graph(const Graph& g)
{
    this->noOfVertices = g.noOfVertices;

    edges = new int*[noOfVertices];

    for (int i = 0; i < noOfVertices; i++)
    {
        int deg = g.edges[i][0];

        edges[i] = new int[deg + 1];
        edges[i][0] = deg;

        for (int j = 1; j <= deg; j++)
        {
            edges[i][j] = g.edges[i][j];
        }
    }
}

// Constructor read from file.
Graph::Graph(istream& in)
{
    in >> noOfVertices;

    edges = new int*[noOfVertices];

    for (int i = 0; i < noOfVertices; i++)
    {
        int deg = 0;
        in >> deg;

        edges[i] = new int[deg + 1];
        edges[i][0] = deg;

        for (int j = 1; j <= deg; j++)
        {
            int nId;
            in >> nId;
            edges[i][j] = nId;
        }
    }
}

const int* Graph::operator[](int vId) const
{
    if (edges == nullptr) return nullptr;
    if (vId < 0 || vId > noOfVertices) return nullptr;

    return edges[vId];
}

int Graph::getVerts()
{
    return this->noOfVertices;
}

vector<int>* Graph::bfs(int startId)
{
    return limitedBFS(startId, INT_MAX);
}

vector<int>* Graph::limitedBFS(int startId, int maxDis)
{
    vector<int> distances = vector<int>();
    vector<int> q = vector<int>();
    h_set visited;

    // Start vertex.
    distances.push_back(0);
    q.push_back(startId);
    visited.insert(startId);

    for (size_t qInd = 0; qInd < q.size() && distances[qInd] < maxDis; qInd++)
    {
        int vId = q[qInd];
        int vDeg = edges[vId][0];

        int nDis = distances[qInd] + 1;

        for (int j = 1; j <= vDeg; j++)
        {
            int uId = edges[vId][j];

            if (visited.count(uId) > 0) continue;

            distances.push_back(nDis);
            q.push_back(uId);
            visited.insert(uId);
        }
    }

    vector<int>* bfsResult = new vector<int>[2];
    bfsResult[0] = move(q);
    bfsResult[1] = move(distances);

    return bfsResult;
}

// Traverse up partition, run bfs from any point that = -1, assign int value to designate which partition,
// must not run over any point that does not equal -1 in partition;
// Future- hash table that assigns points to parition?
// Will vertices ever be non-sequential?
int Graph::findConnComp(int* partition, int pLength)
{
    // Tracks which partition a point belongs to.
    int pNum = 1;

    // Check if each point has been assigned.
    for (int p = 0; p < pLength; p++)
    {
        if (partition[p] != -1) continue;

        partition[p] = pNum;

        vector<int> q;
        q.push_back(p);

        for (size_t qInd = 0; qInd < q.size(); qInd++)
        {
            int vId = q[qInd];
            int vDeg = edges[vId][0];

            for (int j = 1; j <= vDeg; j++)
            {
                int uId = edges[vId][j];

                if (partition[uId] != -1) continue;

                q.push_back(uId);
                partition[uId] = pNum;
            }
        }

        pNum++;
    }

    return pNum;
}
