//
// Created by Holly Strauch on 10/1/2019.
//

#include <istream>
#include <unordered_set>
#include <vector>

using namespace std;

#ifndef STBRESEARCH_GRAPH_H
#define STBRESEARCH_GRAPH_H

class Graph
{
public:
    Graph();
    Graph(const Graph& g);
    explicit Graph(istream& in);

    const int* operator[] (int vId) const;

    int getVerts();

    vector<int>* bfs(int startId);
    vector<int>* limitedBFS(int startId, int maxDis);

    int findConnComp(int *partition, int pLength);

private:
    int noOfVertices;
    int** edges;
};

#endif
