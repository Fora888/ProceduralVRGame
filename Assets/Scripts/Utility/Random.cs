using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//XorShift+ Algorithm http://codingha.us/2018/12/17/xorshift-fast-csharp-random-number-generator/
public struct XORSHIFT
{
    private ulong x_;
    private ulong y_;
    private const double DOUBLE_UNIT = 1.0 / (int.MaxValue + 1.0);

    public XORSHIFT(int seed1, int seed2)
    {
        x_ = (uint)seed1;
        y_ = (uint)seed2;
    }
    public float NextFloat()
    {
        double _;
        ulong temp_x, temp_y, temp_z;

        temp_x = y_;
        x_ ^= x_ << 23; temp_y = x_ ^ y_ ^ (x_ >> 17) ^ (y_ >> 26);

        temp_z = temp_y + y_;
        _ = DOUBLE_UNIT * (0x7FFFFFFF & temp_z);

        x_ = temp_x;
        y_ = temp_y;

        return (float)_;
    }
    public float NextFloat(double min, double max)
    {
        double _;
        ulong temp_x, temp_y, temp_z;

        temp_x = y_;
        x_ ^= x_ << 23; temp_y = x_ ^ y_ ^ (x_ >> 17) ^ (y_ >> 26);

        temp_z = temp_y + y_;
        _ = DOUBLE_UNIT * (0x7FFFFFFF & temp_z);

        x_ = temp_x;
        y_ = temp_y;



        return (float)(_ * (max - min) + min);
    }
}
