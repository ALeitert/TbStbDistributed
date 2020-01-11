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

        PotPart* potPar = algo.findPotPart(g, uId, rho);
        potPar->printBase64(cout);

        delete potPar;
    }

#ifdef _DEBUG
    system("pause");
#endif

    return 0;
}
