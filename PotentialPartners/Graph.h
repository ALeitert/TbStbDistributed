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
    Graph (const Graph & orig);
    vector<int>* limitedBFS(int startId, int maxDis);
    vector<int>* bfs(int startId);
    int getVerts();
    int findConnComp(int *partition, int pLength);
    //bfs

};

#endif //STBRESEARCH_GRAPH_H
