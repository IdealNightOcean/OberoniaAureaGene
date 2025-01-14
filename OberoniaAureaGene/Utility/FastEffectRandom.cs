﻿using System;
using Verse;

namespace OberoniaAureaGene;

[StaticConstructorOnStartup]
public static class FastEffectRandom
{
    private const double REAL_UNIT_INT = 4.656612873077393E-10;

    private const double REAL_UNIT_UINT = 2.3283064365386963E-10;

    private const uint Y = 842502087u;

    private const uint Z = 3579807591u;

    private const uint W = 273326509u;

    private static uint x;

    private static uint y;

    private static uint z;

    private static uint w;

    static FastEffectRandom()
    {
        Reinitialise(Environment.TickCount);
    }

    public static void Reinitialise(int seed)
    {
        x = (uint)seed;
        y = Y;
        z = Z;
        w = W;
    }

    public static int Next(int lowerBound, int upperBound)
    {
        if (lowerBound > upperBound)
        {
            throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be equal to or large than lowerBound");
        }
        uint num = x ^ (x << 11);
        x = y;
        y = z;
        z = w;
        int num2 = upperBound - lowerBound;
        if (num2 < 0)
        {
            return lowerBound + (int)(REAL_UNIT_UINT * (double)(w = w ^ (w >> 19) ^ (num ^ (num >> 8))) * (double)((long)upperBound - (long)lowerBound));
        }
        return lowerBound + (int)(REAL_UNIT_INT * (double)(int)(0x7FFFFFFF & (w = w ^ (w >> 19) ^ (num ^ (num >> 8)))) * (double)num2);
    }
}