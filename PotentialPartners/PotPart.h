//
// Created by Holly Strauch on 12/5/2019.
//

#include <iostream>
#include <unordered_set>

using namespace std;
using h_set = std::unordered_set<int>;

#ifndef STBRESEARCH_POTPART_H
#define STBRESEARCH_POTPART_H

class PotPart
{
public:
    PotPart();
    explicit PotPart(int numCC);
    explicit PotPart(istream& in);

    ~PotPart();

    int getNumCC();
    int getCCSize(int ccNo);

    h_set* getSet(int ccNo);
    void addToSet(int ccNo, int value);

    void print(ostream& out);
    void printBinary(ostream& out);
    void printBase64(ostream& out);

private:
    int numCC;
    h_set* nC;
};

#endif
