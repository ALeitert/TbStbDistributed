//
// Created by Holly Strauch on 10/16/2019.
//

#include "Graph.h"
#include "PotPart.h"

#include <iostream>
#include <unordered_set>

#ifndef STBRESEARCH_STBALG_H
#define STBRESEARCH_STBALG_H

using namespace std;
using h_set = std::unordered_set<int>;

class STBAlg
{
public:
    int findRho(Graph& graph);
    bool runAlg(Graph& graph, int rho);
    void findPotPart(Graph& graph, int uId, int rho, ostream& out);
    PotPart* findPotPart(Graph& graph, int u, int rho);

private:
    int* init_partition(int size);
    h_set* findCNeigh(int numCC, int* partition, Graph& graph, vector<int>* uNeigh, int rho);
    PotPart* uPartners(int* partition, Graph& graph, h_set* ccNeighbors, vector<int>& scope, int rho, int numCC);
    h_set* findDependents(PotPart** partners, int numVerts);
    int eliminate(PotPart** partners, h_set* dependents, int numVerts);
};

#endif
