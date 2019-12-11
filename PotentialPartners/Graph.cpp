//
// Created by Holly Strauch on 10/1/2019.
//

#include "Graph.h"
#include <unordered_set>
#include <vector>
#include <cstdlib>
#include <iostream>
#include <fstream>

using namespace std;
using h_set = std::unordered_set<int>;

//create default constructor
Graph::Graph(){
    noOfVertices = 0;
    capacity = 0;
}

//constructor read from file
Graph::Graph (ifstream & infile) {
    int neighbors = 0, v = 0;


    infile >> v;
    noOfVertices = v;
    capacity = v;
    incomingEdges = new h_set*[noOfVertices];
    outgoingEdges = new h_set*[noOfVertices];

    for(int i = 0; i < noOfVertices; i++){
        incomingEdges[i] = new h_set;
        outgoingEdges[i] = new h_set;
        infile >> neighbors;

        for(int j = 0; j < neighbors; j++){
            infile >> v;
            incomingEdges[i]->insert(v);
            outgoingEdges[i]->insert(v);
        }
    }
    infile.close();
}

//copy constructor
Graph::Graph (const Graph & orig){
    this->noOfVertices = orig.noOfVertices;
    this->capacity = orig.capacity;

    incomingEdges = new h_set*[noOfVertices];
    outgoingEdges = new h_set*[noOfVertices];

    for(int i = 0; i < noOfVertices; i++){
        incomingEdges[i] = new h_set;
        outgoingEdges[i] = new h_set;

        for(const int vert: *orig.incomingEdges[i]){
            incomingEdges[i]->insert(vert);
            outgoingEdges[i]->insert(vert);
        }
    }
}

vector<int>* Graph::bfs(int startId){
    return limitedBFS(startId, INT_MAX);
}

////currently set up for all consecutive vertices
vector<int>* Graph::limitedBFS(int startId, int maxDis)
{
    vector<int> distances = vector<int>();
    vector<int> q = vector<int>();
    h_set visited;

    // Set start vertex
    distances.push_back(0);
    q.push_back(startId);
    visited.insert(startId);

    for (int qInd = 0; qInd < q.size() && distances[qInd] < maxDis; qInd++)
    {
        int vInd = q[qInd];
        int nDis = distances[qInd] + 1;

        for (const int uInd: *outgoingEdges[vInd])
        {
            if (visited.count(uInd) == 0)
            {
                distances.push_back(nDis);
                q.push_back(uInd);
                visited.insert(uInd);
            }
        }
    }

    vector<int>* bfsResult = new vector<int>[2];
    bfsResult[0] = q;
    bfsResult[1] = distances;
    return bfsResult;
}


///traverse up partition, run bfs from any point that = -1, assign int value to designate which partition, must not run
/// over any point that does not equal -1 in partition;  Future- hash table that assigns points to parition? Will vertices
/// ever be non-sequential?
int Graph::findConnComp(int *partition, int pLength){
    //tracks which partition a point belongs in
    int pNum = 1;

    //check if each point has been assigned
    for (int p = 0; p < pLength; p++) {

        if (partition[p] == - 1) {
            partition[p] = pNum;

            vector<int> q;

            // Set start vertex
            q.push_back(p);

            for (int qInd = 0; qInd < q.size(); qInd++)
            {
                int vInd = q[qInd];

                for (const int uInd: *outgoingEdges[vInd])
                {
                    if (partition[uInd] != -1)
                    {
                        continue;
                    }
                    q.push_back(uInd);
                    partition[uInd] = pNum;
                }
            }
            pNum++;
        }
    }
    return pNum;
}


int Graph::getVerts(){
    return this->noOfVertices;
}
