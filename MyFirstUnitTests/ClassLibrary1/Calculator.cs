using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Calculator
    {
        Rounder rounder;

        public Calculator()
        {
        }

        public Calculator(Rounder rounder)
        {
            this.rounder = rounder;
        }

        public int Add(int x, int y)
        {
            return x + y;
        }

        public int Substract(int x, int y)
        {
            return x - y;
        }

        public int DivideAndRound(int x, int y)
        {
            return rounder.Round(x / y);
        }
    }

    public interface Rounder
    {
        int Round(double amount);
    }
}
