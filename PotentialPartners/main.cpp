#include <iostream>

#include "Graph.h"
#include "PotPart.h"
#include "STBAlg.h"

int main()
{
    Graph g(cin);

    STBAlg algo;

    while (true)
    {
        int uId = -1;
        int rho = -1;

        cin >> uId;
        cin >> rho;

        if (uId < 0 || uId >= g.getVerts() || rho <= 0)
        {
            break;
        }

        algo.findPotPart(g, uId, rho, cout);
    }

#ifdef _DEBUG
    system("pause");
#endif

    return 0;
}
