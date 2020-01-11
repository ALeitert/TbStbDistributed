//
// Created by Holly Strauch on 12/5/2019.
//

#include "Base64Writer.h"
#include "PotPart.h"

PotPart::PotPart()
{
    numCC = 0;
    nC = nullptr;
}

PotPart::PotPart(int numCC)
{
    nC = new h_set[numCC - 1];
    this->numCC = numCC - 1;
}

/**
 * \brief Reads in data formatted as in printPotPart() and creates a new PotPart.
 * @param in The stream to the file the data is stored in
 * @return
 */
PotPart::PotPart(istream& in)
{
    int numCC;
    in >> numCC;

    this->numCC = numCC;
    this->nC = new h_set[numCC];

    for (int i = 0; i < numCC; i++)
    {
        int numPP;
        in >> numPP;

        for (int p = 0; p < numPP; p++)
        {
            int partner;
            in >> partner;
            this->nC[i].insert(partner);
        }
    }
}

PotPart::~PotPart()
{
    if (nC != nullptr)
    {
        delete[] nC;
    }
}

int PotPart::getNumCC()
{
    return this->numCC;
}

int PotPart::getCCSize(int ccNo)
{
    return this->nC[ccNo].size();
}

h_set* PotPart::getSet(int ccNo)
{
    return &nC[ccNo];
}

void PotPart::addToSet(int ccNo, int value)
{
    nC[ccNo].insert(value);
}

/**
 * \brief Prints out potential partners in the format:
 * <number of cc for u> <number of pp in cc1> <p1> <p2> ... <pn> <number of pp in ccn>...
 * @param out ostream specifies what stream to send the data to
 */
void PotPart::print(ostream& out)
{
    out << this->numCC;

    for (int i = 0; i < this->numCC; i++)
    {
        out << " " << nC[i].size();

        for (const int part : nC[i])
        {
            out << " " << part;
        }
    }

    out << endl;
}

void writeInt(ostream& out, int num)
{
    unsigned char buffer[4];
    for (int i = 0; i < 4; i++)
    {
        buffer[i] = (num >> (i * 8)) & 255;
    }
    out.write((char*)buffer, 4);
}

void PotPart::printBinary(ostream& out)
{
    writeInt(out, this->numCC);

    for (int i = 0; i < this->numCC; i++)
    {
        writeInt(out, nC[i].size());

        for (const int part : nC[i])
        {
            writeInt(out, part);
        }
    }

    out.flush();
}

void PotPart::printBase64(ostream& out)
{
    Base64Writer writer(out);

    writer.write(this->numCC);

    for (int i = 0; i < this->numCC; i++)
    {
        writer.write(nC[i].size());

        for (const int part : nC[i])
        {
            writer.write(part);
        }
    }

    writer.flush();
}
