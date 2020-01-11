#include <iostream>

using namespace std;

#ifndef _BASE64WRITER_H_
#define _BASE64WRITER_H_

class Base64Writer
{
public:
    Base64Writer(ostream& oStr);

    void write(int num);
    void flush();

private:
    ostream& out;

    int bufferCount = 0;
    static const int bufferSize = 48;
    unsigned char buffer[bufferSize];

    // -- Why 48? --
    // We want a multiple of 3, because bytes coverted in blocks of 3.
    // We want a multiple of 4, because we store 32-bit integers.
    // Hence, we want 12 * k bytes for some k.
    // 12 * k bytes =  12 * k * 4 / 3 chars = 16 * k chars.
    // We decided to use k = 4 (resulting in 64 chars per line written).
    // Hence, we need a buffer of 12 * 4 = 48 bytes.

    const string base64Chars =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        "abcdefghijklmnopqrstuvwxyz"
        "0123456789+/=";
};

#endif
