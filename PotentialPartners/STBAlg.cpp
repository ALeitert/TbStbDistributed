//
// Created by Holly Strauch on 10/16/2019.
//

#include "STBAlg.h"

using namespace std;
using h_set = std::unordered_set<int>;

int STBAlg::findRho(Graph& graph)
{
    for (int rho = 1;; rho++)
    {
        if (runAlg(graph, rho))
        {
            return rho;
        }
    }
}

/**
 * \brief Runs the full algorithm on a graph with a set rho.
 * @param graph The current graph.
 * @param rho The current length being tested for STB.
 * @return
 */
bool STBAlg::runAlg(Graph& graph, int rho)
{
    PotPart** partners = new PotPart*[graph.getVerts()];

    for (int uId = 0; uId < graph.getVerts(); uId++)
    {
        partners[uId] = findPotPart(graph, uId, rho);
    }

    h_set* dependents = findDependents(partners, graph.getVerts());

    bool success = eliminate(partners, dependents, graph.getVerts()) > 0;


    for (int i = 0; i < graph.getVerts(); i++)
    {
        if (partners[i] == nullptr) continue;
        delete[] partners[i];
    }

    delete[] partners;
    delete[] dependents;

    return success;
}

PotPart* STBAlg::findPotPart(Graph& graph, int uId, int rho)
{
    vector<int>* scope = graph.limitedBFS(uId, 2 * rho);
    int* partition = init_partition(graph.getVerts());

    // Find neighbours up to distance rho.
    vector<int> uNeigh[2];
    for (int i = 0; scope[1][i] <= rho; i++)
    {
        int vId = scope[0][i];
        int vDis = scope[1][i];

        uNeigh[0].push_back(vId);
        uNeigh[1].push_back(vDis);

        partition[vId] = 0;
    }

    int numCC = graph.findConnComp(partition, graph.getVerts());
    h_set* ccNeighbors = findCNeigh(numCC, partition, graph, uNeigh, rho);

    PotPart* partners = uPartners(partition, graph, ccNeighbors, scope[0], rho, numCC);

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

    // Iterate over all potPart and add vect to q if there is an empty component.
    for (int i = 0; i < numVerts; i++)
    {
        for (int j = 0; j < partners[i]->getNumCC(); j++)
        {
            if (!partners[i]->getSet(j)->empty()) continue;

            q.push_back(i);
            break;
        }
    }

    int sets = numVerts;
    bool* remain = new bool[numVerts];
    memset(remain, true, numVerts);

    for (size_t qInd = 0; qInd < q.size(); qInd++, sets--)
    {
        for (const int vert : dependents[q[qInd]])
        {
            if (!remain[vert]) continue;

            for (int i = 0; i < partners[vert]->getNumCC(); i++)
            {
                if (partners[vert]->getSet(i)->count(q[qInd]) == 0) continue;

                partners[vert]->getSet(i)->erase(q[qInd]);

                if (partners[vert]->getSet(i)->empty())
                {
                    q.push_back(vert);
                    remain[vert] = 0;
                }
            }
        }
    }

    delete remain;
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
PotPart* STBAlg::uPartners(int* partition, Graph& graph, h_set* ccNeighbors, vector<int>& scope, int rho, int numCC)
{
    PotPart* uPartners = new PotPart(numCC);

    // Begin at 1 to exclude u itself.
    for (size_t i = 1; i < scope.size(); i++)
    {
        int vId = scope[i];

        // Vertex v (vId) is apotential partner of u (scope[0]) for some C in C[u] if
        //   (i) N^rho[v] contains N(C) (ccNeighbors) and
        //  (ii) N^rho[v] intersects C
        
        vector<int>* vVect = graph.limitedBFS(vId, rho);
        h_set vNeigh;

        bool* ncCheck = new bool[uPartners->getNumCC()];

        for (int c = 0; c < uPartners->getNumCC(); c++)
        {
            ncCheck[c] = false;
        }

        // Add v neighborhood into hash set.
        // Mark every partition that has a vertex contained in N[v] (condition (ii)).
        for (size_t j = 0; j < vVect[0].size(); j++)
        {
            int wId = vVect[0][j];
            vNeigh.insert(wId);

            if (partition[wId] == 0) continue;

            ncCheck[partition[wId] - 1] = true;
        }

        // Go through ccNeighbors and see which are in vNeigh (condition (i)).
        for (int c = 0; c < uPartners->getNumCC(); c++)
        {
            if (!ncCheck[c]) continue;

            // Check if all neighbours of C are in N[v] (condition (i)).
            for (const int vert : ccNeighbors[c])
            {
                if (vNeigh.count(vert) == 0)
                {
                    ncCheck[c] = false;
                    break;
                }
            }
        }

        for (int j = 0; j < uPartners->getNumCC(); j++)
        {
            if (ncCheck[j])
            {
                uPartners->addToSet(j, vId);
            }
        }

        delete[] ncCheck;
        delete[] vVect;
    }

    return uPartners;
}


/**
 * \brief Initializes an array to hold all values of -1
 * @param size The size of the array
 * @return A new array
 */
int* STBAlg::init_partition(int size)
{
    int* partition = new int[size];

    for (int i = 0; i < size; i++)
    {
        partition[i] = -1;
    }

    return partition;
}

/**
 * \brief Determines the sets N(C) for each connected component.
 * That is, the vertices in N^rho[u] which are adjacent to a vertex in C.
 * @param numCC - The number connected components
 * @param partition - An array that stores which connected component a vertex is in
 * @param graph - The original graph
 * @param uNeigh - Vectors with the neighbors and distances of those neighbors from u
 * @param rho - the current rho size
 * @return ccNeigh, neighbors of the connected components
 */
h_set* STBAlg::findCNeigh(int numCC, int* partition, Graph& graph, vector<int>* uNeigh, int rho)
{
    h_set* ccNeigh = new h_set[numCC - 1];

    // Search through all of the furthest vertices in u Neighborhood.
    for (int index = uNeigh[0].size() - 1; uNeigh[1][index] == rho; index--)
    {
        int vId = uNeigh[0][index];

        const int* neighs = graph[vId];
        int deg = neighs[0];

        for (int i = 1; i <= deg; i++)
        {
            // Find neighbor and its partition.
            int nId = neighs[i];

            int part = partition[nId];
            if (part == 0) continue;

            // Add to CC neighbor set.
            ccNeigh[part - 1].insert(vId);
        }
    }

    return ccNeigh;
}
