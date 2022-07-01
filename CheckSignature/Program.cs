using System;
using System.Numerics;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace RSAENCRYPT
{
    class Program
    {
        private static int p = 1423;
        private static int q = 991;
        private static int x = 71;
        private static int m = 800;
        private static float k = 449f;


        private static BigInteger GetY()
        {

            BigInteger pub_key_y = q;

            for (int i = 2; i <= x; i++)
            {
                pub_key_y = pub_key_y * q;
            }
            pub_key_y = pub_key_y % p;

            Console.WriteLine("The y key = " + pub_key_y.ToString());
            return pub_key_y;
        }

        private static int GetA()
        {
            BigInteger a = q;
            for (int i = 2; i <= k; i++)
            {
                a = a * q;
            }
            a = a % p;

            int a_int = (int)a;

            Console.WriteLine("The a = " + a_int.ToString());
            return a_int;

        }

        private static int GetB(int a_int)
        {

            int pow_k = 0;
            int step1 = 0;
            for (int l = 0; l < (p - 1); l++)
            {
                step1 = (int)((k * l) - 1);
                if (step1 % (p - 1) == 0)
                {
                    pow_k = l;
                }
            }

            int b = pow_k * (m - (a_int * x));
            int b_value = 0;
            if (b < 0)
            {
                b = Math.Abs(b);
                int mod_d = b % (p - 1);
                b_value = (p - 1) - mod_d;
                return b_value;

            }

            else
            {
                b_value = b % (p - 1);
                return b_value;
            }

            Console.WriteLine("The b = " + b_value.ToString());
        }

        private static BigInteger GetV(BigInteger pub_key_y, int a_int, int b_value)
        {
            BigInteger y_pow_a = pub_key_y;
            BigInteger a_pow_b = a_int;
            BigInteger V = 0;

            for (int i = 2; i <= a_int; i++)
            {
                y_pow_a = y_pow_a * pub_key_y;
            }

            for (int i = 2; i <= b_value; i++)
            {
                a_pow_b = a_pow_b * a_int;
            }

            V = BigInteger.Multiply(y_pow_a, a_pow_b);
            V = V % p;

            Console.WriteLine("The V = " + V.ToString());
            return V;
        }

        private static BigInteger GetW()
        {
            BigInteger q_pow_m = q;
            BigInteger W = 0;
            for (int i = 2; i <= m; i++)
            {
                q_pow_m = q_pow_m * q;
            }

            W = q_pow_m % p;
            Console.WriteLine("The W = " + W.ToString());
            return W;
        }

        static void Main(string[] args)
        {
            BigInteger pub_key_y = GetY();
            int a_int = GetA();
            int b_value = GetB(a_int);
            BigInteger V = GetV(pub_key_y, a_int, b_value);
            BigInteger W = GetW();
            if (V == W)
            {
                Console.WriteLine("The signature is genuine");
            }

        }
    }
}
