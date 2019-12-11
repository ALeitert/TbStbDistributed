//
// Created by Holly Strauch on 10/1/2019.
//


/* structure of data (2d array for now, change to hashsets):
*		length: number of vertices
*		i: neighbors of vertex i
*
 * Check if pointers are all correct!
*/

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

//constructor for empty graph with set capacity
Graph::Graph (const int cap){
    noOfVertices = 0;
    capacity = cap;
    incomingEdges = new h_set*[cap];
    outgoingEdges = new h_set*[cap];
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

void Graph::removeVertex(int vInd){

    h_set::const_iterator got;
    for(const int x: *incomingEdges[vInd]){
        //makes sure it exists, then erase;
        got = outgoingEdges[x]->find (vInd);
        if(got != outgoingEdges[x]->end()){
            outgoingEdges[x]->erase(vInd);
        }
    }

    for(const int x: *outgoingEdges[vInd]){
        //makes sure it exists, then erase;
        got = incomingEdges[x]->find(vInd);
        if (got != incomingEdges[x]->end()){
            incomingEdges[x]->erase(vInd);
        }
    }

    noOfVertices--;
    delete incomingEdges[vInd];
    delete outgoingEdges[vInd];
    incomingEdges[vInd] = nullptr;
    outgoingEdges[vInd] = nullptr;
}

void Graph::addVertex(const int newVertex) {
    if(incomingEdges[newVertex] || outgoingEdges[newVertex]){
        cout << "Vertex already exists" << endl;
    }else if (noOfVertices == capacity) {
        cout << "Graph is at max capacity" << endl;
    }else{
        incomingEdges[newVertex] = new h_set;
        outgoingEdges[newVertex] = new h_set;
        noOfVertices++;
    }
}

void Graph::addEdge(int vert1, int vert2){
    addDirectedEdge(vert1, vert2);
    addDirectedEdge(vert2, vert1);
}

void Graph::addDirectedEdge(int vertOut, int vertIn) {
    if( outgoingEdges[vertOut]->count(vertIn) != 0 &&
        incomingEdges[vertIn]->count(vertOut) != 0){

        cout << "Edge " << vertOut << "->" << vertIn << " already exists" << endl;
    }else {
        incomingEdges[vertIn]->insert(vertOut);
        outgoingEdges[vertOut]->insert(vertIn);
    }
}

void Graph::printGraph(){
    int currVertex = 0;

    cout << "Incoming Edges" << endl;
    for(int i = 0; i < this->noOfVertices; i++){

        //required in-case vertices have been deleted (non-consecutive vertices)
        while (!incomingEdges[currVertex]) {
            currVertex++;
        }
        for (const int x: *incomingEdges[currVertex]) {
            cout << x << " ";
        }

        cout << endl;
        currVertex++;
    }

    currVertex = 0;

    cout << "Outgoing Edges" << endl;
    for(int i = 0; i < this->noOfVertices; i++){
        //required in case vertices have been deleted (non-consecutive vertices)
        while (!outgoingEdges[currVertex]) {
            currVertex++;
        }
        for (const int x: *outgoingEdges[currVertex]) {
            cout << x << " ";
        }

        cout << endl;
        currVertex++;
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

int Graph::getCapacity(){
    return this->capacity;
}