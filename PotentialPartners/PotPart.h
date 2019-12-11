//
// Created by Holly Strauch on 12/5/2019.
//

#ifndef STBRESEARCH_POTPART_H
#define STBRESEARCH_POTPART_H
#include <unordered_set>

using namespace std;
using h_set = std::unordered_set<int>;

class PotPart {

    h_set *nC;
    int numCC;

public:
    explicit PotPart(int numCC);
    PotPart();
    explicit PotPart(ifstream & file);
    ~PotPart();
    h_set* getSet(int ccNo);
    void addToSet(int ccNo, int value);
    int getNumCC();
    int getCCSize(int ccNo);
    void print(ostream &out);

};


#endif //STBRESEARCH_POTPART_H
