//
// Created by Holly Strauch on 10/1/2019.
//
#include <unordered_set>
#include <fstream>
#include <vector>
using namespace std;
#ifndef STBRESEARCH_GRAPH_H
#define STBRESEARCH_GRAPH_H



class Graph {
    int noOfVertices;
    int capacity;
    std::unordered_set<int>** incomingEdges;
    std::unordered_set<int>** outgoingEdges;
public:
    Graph();
    explicit Graph(ifstream & infile);
    explicit Graph(const int cap);
    Graph (const Graph & orig);
    void removeVertex(int vInd);
    void printGraph();
    void addVertex(const int newVertex);
    void addEdge(int vert1, int vert2);
    void addDirectedEdge(int vertOut, int vertIn);
    vector<int>* limitedBFS(int startId, int maxDis);
    vector<int>* bfs(int startId);
    void printBFS(vector<int>** bfsResult);
    int getVerts();
    int getCapacity();
    int findConnComp(int *partition, int pLength);
    //bfs

};

#endif //STBRESEARCH_GRAPH_H
