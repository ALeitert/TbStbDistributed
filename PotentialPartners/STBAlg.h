//
// Created by Holly Strauch on 10/16/2019.
//


#ifndef STBRESEARCH_STBALG_H
#define STBRESEARCH_STBALG_H

#include "Graph.h"
#include "STBAlg.h"
#include "PotPart.h"
#include <unordered_set>
#include <iostream>
#include <fstream>
using namespace std;
using h_set = std::unordered_set<int>;



class STBAlg{

public:
    int findRho(Graph* graph, ostream &out);
    bool runAlg(Graph* graph, int rho, ostream &out);
    void setUNeigh(vector<int> q, int* partition);
    int* init_partition(int size);
    h_set* findCNeigh(int numCC, int* partition, Graph* graph, vector<int>* uNeigh, int rho);
    PotPart* uPartners(int *partition, Graph *graph, h_set *ccNeighbors, vector<int> scope, int rho, int numCC);
    PotPart* findPotPart(Graph* graph, int rho, ostream &out, int u);
    h_set* findDependents(PotPart** partners, int numVerts);
    void findBiDirect(h_set* dependents, h_set** partners, int numVerts);
    int eliminate(PotPart** partners, h_set* dependents, int numVerts);
    void printPotPart(PotPart* partners, ostream &out);
    h_set** readPotPart(ifstream & file);
};

#endif //STBRESEARCH_STBALG_H
