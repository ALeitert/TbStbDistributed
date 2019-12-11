//
// Created by Holly Strauch on 12/5/2019.
//

#include "PotPart.h"
#include <iostream>
#include <fstream>

PotPart::~PotPart() {
    if (nC != nullptr) delete[] nC;
}

PotPart::PotPart(){
    nC = nullptr;
    this->numCC = 0;
}

PotPart::PotPart(int numCC) {
    nC = new h_set[numCC - 1];
    this->numCC = numCC - 1;
}

/**
 * \brief Reads in data formatted as in printPotPart() and creates a new PotPart
 * @param file The ifstream to the file the data is stored in
 * @return
 */
PotPart::PotPart(ifstream & file) {
    int numCC;
    file >> numCC;
    this->numCC = numCC;
    this->nC = new h_set[numCC];

    for (int i = 0; i < numCC; i++) {
        int numPP;
        file >> numPP;

        for (int p = 0; p < numPP; p++) {
            int partner;
            file >> partner;
            this->nC[i].insert(partner);
        }
    }
}

h_set* PotPart::getSet(int ccNo) {
    return &nC[ccNo];
}

int PotPart::getNumCC() {
    return this->numCC;
}

int PotPart::getCCSize(int ccNo){
    return this->nC[ccNo].size();
}

void PotPart::addToSet(int ccNo, int value){
    nC[ccNo].insert(value);
}

/**
 * \brief prints out potential partners in the format:
 * <number of cc for u> <number of pp in cc1> <p1> <p2> ... <pn> <number of pp in ccn>...
 * @param out ostream specifies what stream to send the data to
 */
void PotPart::print(ostream &out) {
    out << this->numCC << " " << flush;

    for (int i = 0; i < this->numCC; i++) {

        out << this->getCCSize(i) << " " << flush;
        for (const int part: nC[i]) {
            out << part << " ";
        }
    }
    out << endl;
}