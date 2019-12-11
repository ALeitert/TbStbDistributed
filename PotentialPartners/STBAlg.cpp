//
// Created by Holly Strauch on 10/16/2019.
//

#include "Graph.h"
#include "STBAlg.h"
#include "PotPart.h"
#include <unordered_set>
#include <iostream>
#include <fstream>
#include <vector>
#include <stdio.h>
#include <string.h>
using namespace std;
using h_set = std::unordered_set<int>;

int STBAlg::findRho(Graph* graph, ostream &out) {

    //todo set maximum limit for alg
    //todo time alg
    int rho = 0;
    do{
        rho++;
        cout << "rho " << rho << endl;
        if (runAlg(graph, rho, out)){
            return rho;
        }
    }
    while (true);
}

/**
 * \brief Runs the full algorithm on a graph with a set rho.
 * @param graph The current graph
 * @param rho The current length being tested for STB
 * @return
 */
bool STBAlg::runAlg(Graph* graph, int rho, ostream &out) {

    PotPart **partners = new PotPart *[graph->getVerts()];
    for( int u = 0; u < graph->getVerts(); u++) {
        cout << u << endl;
        partners[u] = findPotPart(graph, rho, out, u);
    }

    h_set* dependents = findDependents(partners, graph->getVerts());
    //findBiDirect(dependents, partners, graph->getVerts());

    bool success = false;
    if (eliminate(partners, dependents, graph->getVerts()) > 0) {
        success = true;
    }

    delete[] partners;
    delete[] dependents;

    return success;
}

PotPart* STBAlg::findPotPart(Graph* graph, int rho, ostream &out, int u){
    vector<int> *uNeigh = graph->limitedBFS(u, rho);
    int *partition = init_partition(graph->getVerts());
    setUNeigh(uNeigh[0], partition);

    int numCC = graph->findConnComp(partition, graph->getVerts());
    h_set *ccNeighbors = findCNeigh(numCC, partition, graph, uNeigh, rho);

    vector<int> *scope = graph->limitedBFS(u, 2 * rho);
    PotPart *partners = uPartners(partition, graph, ccNeighbors, scope[0], rho, numCC);
   /// partners->print(out);

    delete[] partition;
    delete[] ccNeighbors;
    delete[] scope;

    return partners;
}

/**
 * \brief Finds all potential partner sets which are empty, removes that vertex from all other sets, and decrements the
 *  number of valid remaining sets
 * @param partners, the potential partners for each CC for each N[u]
 * @param dependents, An array of sets tracking which vertices each vertex is dependent on
 * @param numVerts, the total number of vertices in the graph
 * @return the number of valid sets remaining.  0 sets indicates a higher rho value is need for STB
 */
int STBAlg::eliminate(PotPart** partners, h_set* dependents, int numVerts)
{

    vector<int> q = vector<int>();

    //iterate over all potPart and add vect to q if there is an empty component;
    for (int i = 0; i < numVerts; i++)
    {
        for (int j = 0; j < partners[i]->getNumCC(); j++) {
            if (!partners[i]->getSet(j)->empty()) continue;

            q.push_back(i);
            break;
        }
    }

    int sets = numVerts;
    bool* remain = new bool[numVerts];
    memset(remain, 1, sizeof(remain));

    for (int qInd = 0; qInd < q.size(); qInd++)
    {
        for (const int vert : dependents[q[qInd]])
        {
            if (!remain[vert]) continue;

            for (int i = 0; i < partners[vert]->getNumCC(); i++)
            {
                if (partners[vert]->getSet(i)->count(q[qInd]) == 0) continue;

                partners[vert]->getSet(i)->erase(q[qInd]);

                if (partners[vert]->getSet(i)->empty()){
                    q.push_back(vert);
                    remain[vert] = 0;
                }
            }
        }
        sets--;
    }
    delete remain;
    cout << "Sets: " << sets << endl;
    return sets;
}

/**
 * \brief For each vertex, iterate through the potential partners for each component and record it in a dependency array of
 *  hashsets
 * @param partners The potential parterns for each vertex's connected components
 * @param numVerts, The total number of vertices in the graph
 * @return An array of hashsets with the depencies for each vertex
 */
h_set* STBAlg::findDependents(PotPart** partners, int numVerts)
{
    h_set* dependencies = new h_set[numVerts];

    for (int i = 0; i < numVerts; i++)
    {

        for (int j = 0; j < partners[i]->getNumCC(); j++)
        {
            h_set* nC = partners[i]->getSet(j);
            for (const int vert : *nC)
            {
                dependencies[vert].insert(i);
            }
        }
    }
    return dependencies;
}

/**
 * \brief Finds the potential partners for the connected components created from partitioning the graph around N[u]
 * @param partition, an array that stores what connected component each vertex is in
 * @param graph, the original graph
 * @param ccNeighbors, the neighbors of each connected component.  Each connected component corresponds to an index.
 * @param scope, a vector of vertices 2*rho away from u.  Potential partners must occur within this distance.
 * @param rho, the current rho distance
 * @param numCC, the number of connected components
 * @return An potPart struct that contains the potential partners for each connected component. Each CC corresponds to an
 *  index.
 */
PotPart* STBAlg::uPartners(int *partition, Graph *graph, h_set *ccNeighbors, vector<int> scope, int rho, int numCC){

    PotPart* uPartners = new PotPart(numCC);

    //begin at 1 to exclude u itself
    for (int i = 1; i < scope.size(); i++) {
        int vId = scope[i];
        vector<int> *vVect = graph->limitedBFS(vId, rho);
        h_set vNeigh;

        bool* ncCheck = new bool[uPartners->getNumCC()];

        for (int c = 0; c < uPartners->getNumCC(); c++) {
            ncCheck[c] = false;
        }

        //get v neighborhood into hash set
        //mark every partition that has a vertex contained in N[v]
        for (int j = 0; j < vVect[0].size(); j++) {
            int wId = vVect[0].at(j);
            vNeigh.insert(wId);

            if (partition[wId] == 0) continue;

            ncCheck[partition[wId] - 1] = true;
        }

        //go through ccNeighbors and see which are in vNeigh
        for (int c = 0; c < uPartners->getNumCC(); c++) {
            if (!ncCheck[c]) continue;

            //check if all neighbors of C in u are in V
            for (const int vert : ccNeighbors[c]) {
                if (vNeigh.count(vert) == 0) {
                    ncCheck[c] = false;
                    break;
                }
            }
        }

        for (int j = 0; j < uPartners->getNumCC(); j++) {
            if (ncCheck[j]) {
                uPartners->addToSet(j, vId);
            }
        }

        delete[] ncCheck;
        delete[] vVect;
    }
    return uPartners;
}

/**
 * \brief Sets all vertices from a u neighborhood to 0 in a partition array
 * @param u, the vertices in N[u]
 * @param partition An array where the indices correspond to each vertex in the graph
 */
void STBAlg::setUNeigh(vector<int> u, int* partition) {

    for (int i = 0; i < u.size(); i++) {
        partition[u.at(i)] = 0;
    }
}


/**
 * \brief Initializes an array to hold all values of -1
 * @param size The size of the array
 * @return A new array
 */
int* STBAlg::init_partition(int size) {
    int* partition = new int[size];

    for (int i = 0; i < size; i++) {
        partition[i] = -1;
    }

    return partition;
}

/**
 * \brief Returns an array of sets of N[C], which are the neighbors of each connected component.
 * @param numCC - The number connected components
 * @param partition - An array that stores which connected component a vertex is in
 * @param graph - The original graph
 * @param uNeigh - Vectors with the neighbors and distances of those neighbors from u
 * @param rho - the current rho size
 * @return ccNeigh, neighbors of the connected components
 */
h_set* STBAlg::findCNeigh(int numCC, int* partition, Graph* graph, vector<int>* uNeigh, int rho) {
    h_set* ccNeigh = new h_set[numCC - 1] ;

    h_set uNeigh_set;
    for (int i = 0; i < uNeigh[0].size(); i++) {
        uNeigh_set.insert(uNeigh[0].at(i));
    }

    int index = uNeigh[0].size();
    index--;
    //search through all of the furthest vertices in u Neighborhood
    while (uNeigh[1].at(index) == rho) {

        int vert = uNeigh[0].at(index);

        vector<int>* result = graph->limitedBFS(vert, 1);

        for (int i = 1; i < result[0].size(); i++) {
            //find neighbor and its partition
            int neigh = result[0].at(i);

            //have to check that it is not in uNeigh
            if (uNeigh_set.count(neigh) == 0) {
                int part = partition[neigh];

                if (part == 0) continue;
                //add to CC neighbor set
                ccNeigh[part - 1].insert(vert);
            }
        }

        index--;
        delete [] result;
        result = nullptr;
    }
    return ccNeigh;
}




