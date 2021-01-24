﻿using System;
using MathNet.Numerics;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Integration;


namespace MathNetSnippet
{
    class Program
    {
        static void Special()
        {
            // Evaluate a special function
            const double x = 0.5;
            Console.WriteLine($"erf({x}) = {SpecialFunctions.Erf(x)}");
        }


        static void Integrate() 
        {
            // Approximate using a relative error of 1e-5.
            const double x0 = 0.0;
            const double x1 = 1.0;
            const double eps = 1e-6;
            double integral = 2 / Constants.SqrtPi * DoubleExponentialTransformation.Integrate(x => Math.Exp(-x * x), x0, x1, eps);
            double exact = SpecialFunctions.Erf(x1);
            Console.WriteLine($"erf({x1}) = {exact}");
            Console.WriteLine($"erf({x1}) = {integral} (num approx.)");
        }


        static void SolveLinSys()
        {
            // Solve a random linear equation system with 8 unknowns
            const int N = 8;
            var gen = new MersenneTwister();
            var A = Matrix<double>.Build.Dense(N, N);
            A.MapInplace(x => Normal.Sample(gen, 0.0, 1.0));
            var b = Vector<double>.Build.Dense(N);
            b.MapInplace(x => Normal.Sample(gen, 0.0, 1.0));
            var x = A.Solve(b);
            Console.WriteLine($"A = {A}");
            Console.WriteLine($"b = {b}");
            Console.WriteLine($"x = {x}");
            Console.WriteLine($"A * x - b = {A * x - b}");
        }


        static void Main(string[] args)
        {
            Special();
            Integrate();
            SolveLinSys();
        }
    }
}
