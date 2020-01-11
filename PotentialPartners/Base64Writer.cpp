#include <iostream>

#include "Base64Writer.h"

using namespace std;

Base64Writer::Base64Writer(ostream& oStr) : out(oStr) { }

void Base64Writer::write(int num)
{
    // Convert number to bytes and add them to buffer.
    for (int i = 0; i < 4; i++)
    {
        buffer[bufferCount] = (num >> (i * 8)) & 255;
        bufferCount++;
    }

    if (bufferCount == bufferSize)
    {
        flush();
    }
}


void Base64Writer::flush()
{
    // Convert buffer to base64 string
    int i = 0;
    for (; i < bufferCount; i += 3)
    {
        int ind[4];

        if (bufferCount - i == 1)
        {
            ind[0] = buffer[i] >> 2;
            ind[1] = ((buffer[i] & 0x03) << 4) | 0x00;
            ind[2] = 64; // =
            ind[3] = 64; // =
        }
        else if (bufferCount - i == 2)
        {
            ind[0] = buffer[i] >> 2;
            ind[1] = ((buffer[i + 0] & 0x03) << 4) | (buffer[i + 1] >> 4);
            ind[2] = ((buffer[i + 1] & 0x0f) << 2) | 0x00;
            ind[3] = 64; // =
        }
        else
        {
            ind[0] = buffer[i] >> 2;
            ind[1] = ((buffer[i + 0] & 0x03) << 4) | (buffer[i + 1] >> 4);
            ind[2] = ((buffer[i + 1] & 0x0f) << 2) | (buffer[i + 2] >> 6);
            ind[3] = buffer[i + 2] & 0x3f;
        }

        for (int j = 0; j < 4; j++)
        {
            out << base64Chars[ind[j]];
        }
    }

    out << endl;
    bufferCount = 0;
}
